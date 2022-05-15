using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace NDayCycle
{
    public class NDayCycle : Mod
	{
        private static NDayCycle instance;
        private static UserInterface _ui;
        public static MenuBarUIState MenuBar;
        public static DawnDayUIState DayMessage;
        public static bool IsServer { get; private set; }
        public static bool IsSinglePlayer => Main.netMode != NetmodeID.MultiplayerClient;

        public static int[] StackableItems = new int[]
        {
            ItemID.Bomb,
            ItemID.BombFish,
            ItemID.BouncyBomb,
            ItemID.StickyBomb,

            ItemID.Torch,
            ItemID.BlueTorch,
            ItemID.BoneTorch,
            ItemID.CursedTorch,
            ItemID.DemonTorch,
            ItemID.GreenTorch,
            ItemID.IceTorch,
            ItemID.IchorTorch,
            ItemID.OrangeTorch,
            ItemID.PinkTorch,
            ItemID.PurpleTorch,
            ItemID.RainbowTorch,
            ItemID.RedTorch,
            ItemID.TikiTorch,
            ItemID.UltrabrightTorch,
            ItemID.WhiteTorch,
            ItemID.YellowTorch,

            ItemID.CopperCoin,
            ItemID.SilverCoin,
            ItemID.GoldCoin,
            ItemID.PlatinumCoin,

            ItemID.MusketBall,
            ItemID.MeteorShot,
            ItemID.ChlorophyteBullet,
            ItemID.CrystalBullet,
            ItemID.CursedBullet,
            ItemID.EmptyBullet,
            ItemID.ExplodingBullet,
            ItemID.GoldenBullet,
            ItemID.HighVelocityBullet,
            ItemID.IchorBullet,
            ItemID.MoonlordBullet,
            ItemID.NanoBullet,
            ItemID.PartyBullet,
            ItemID.SilverBullet,
            ItemID.VenomBullet,

            ItemID.Spike,

            ItemID.BoneArrow,
            ItemID.ChlorophyteArrow,
            ItemID.CursedArrow,
            ItemID.FlamingArrow,
            ItemID.FrostburnArrow,
            ItemID.HellfireArrow,
            ItemID.HolyArrow,
            ItemID.IchorArrow,
            ItemID.JestersArrow,
            ItemID.MoonlordArrow,
            ItemID.UnholyArrow,
            ItemID.VenomArrow,
            ItemID.WoodenArrow,
        };

        public static bool Pause { get; set; }

        public static bool StackableItemTypes(Item item)
        {
            return item.potion || item.consumable;
        }

        public List<int> ready;

        public override void MidUpdatePlayerNPC()
        {
            PreUpdateEntities();
        }

        public override void MidUpdateProjectileItem()
        {
            PreUpdateEntities();
        }

        public override void PreUpdateEntities()
        {
            if (Pause)
            {
                Main.gamePaused = true;
                Main.autoPause = true;
            }
            else
            {
                Main.gamePaused = false;
                Main.autoPause = false;
            }
        }

        public override void Load()
        {
            instance = this;
            ready = new List<int>();
            if (!Main.dedServ)
            {
                MenuBar = new MenuBarUIState();
                MenuBar.Activate();
                DayMessage = new DawnDayUIState();
                DayMessage.Activate();
                _ui = new UserInterface();
            }
            else
            {
                IsServer = true;
            }
        }

        public override void Unload()
        {
            instance = null;
            _ui = null;
            MenuBar = null;
        }

        public static void ShowMenu()
        {
            _ui.SetState(MenuBar);
        }

        public static void ShowDayMessage(string top, string mid, string bot)
        {
            DayMessage.SetMessage(top, mid, bot);
            _ui.SetState(DayMessage);
            Task.Delay(5000).ContinueWith(task => HideUI());
        }

        public static void HideUI()
        {
            _ui.SetState(null);
        }

        public override void UpdateUI(GameTime gameTime)
        {
            _ui?.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "NDayCycle: UI",
                    delegate
                    {
                        _ui?.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            byte messageType = reader.ReadByte();
            if (Main.dedServ) // is server
            {
                switch ((MessageTypes)messageType)
                {
                    case MessageTypes.GETDAY:
                        ModPacket packet = instance.GetPacket();
                        packet.Write((byte)MessageTypes.SETDAY);
                        packet.Write(WorldResetter.Day);
                        packet.Send(whoAmI);
                        break;
                    case MessageTypes.READYTORESET:
                        ready.Add(whoAmI);
                        IfAllAreReadyThenReset();
                        break;
                }
            }
            else // is client
            {
                switch ((MessageTypes)messageType)
                {
                    case MessageTypes.SETDAY:
                        WorldResetter.Day = reader.ReadInt32();
                        Main.NewText($"It is day {WorldResetter.Day + 1}.");
                        break;
                    case MessageTypes.RESETANDDISCONNECT:
                        MenuBar.ResetInventory();
                        break;
                }
            }
        }

        private void IfAllAreReadyThenReset()
        {
            bool allReady = true;
            foreach (Player player in Main.player)
            {
                Console.WriteLine($"{player.name} active? {player.active} & ready? {ready.Contains(player.whoAmI)}");
                allReady &= !player.active || ready.Contains(player.whoAmI);
            }

            if (allReady)
            {
                Console.WriteLine("All are ready. Reset the world.");
                // TODO: clear player inventory
                WorldResetter.ResetWorld();
            }
        }

        /// <summary>
        /// Client sends packet to server
        /// </summary>
        public static void GetDayFromServer()
        {
            ModPacket packet = instance.GetPacket();
            packet.Write((byte)MessageTypes.GETDAY);
            packet.Send();
        }

        /// <summary>
        /// Client sends packet to server
        /// </summary>
        public static void ReadyForReset()
        {
            ModPacket packet = instance.GetPacket();
            packet.Write((byte)MessageTypes.READYTORESET);
            packet.Send();
        }


        /// <summary>
        /// Server sends packet to client
        /// </summary>
        public static void SendDay()
        {
            ModPacket packet = instance.GetPacket();
            packet.Write((byte)MessageTypes.SETDAY);
            packet.Write(WorldResetter.Day);
            packet.Send();
        }

        /// <summary>
        /// Server sends packet to client
        /// </summary>
        public static void DisconnectPlayersForReset()
        {
            Main.autoShutdown = false;
            ModPacket packet = instance.GetPacket();
            packet.Write((byte)MessageTypes.RESETANDDISCONNECT);
            packet.Send();

            Task.Run(() =>
            {
                while (Main.player.Any(player => player.active))
                {
                    Thread.Sleep(100);
                }

                WorldResetter.RestoreOriginalWorld();
                Main.autoShutdown = true;
                Environment.Exit(0);
            });
        }

        enum MessageTypes { NONE = 0, GETDAY = 1, SETDAY = 2, READYTORESET = 3, RESETANDDISCONNECT = 4, }
    }
}
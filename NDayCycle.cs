using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace NDayCycle
{
    public class NDayCycle : Mod
	{
        public static NDayCycle instance;
        private static UserInterface _ui;
        public static KeepsUIState MenuBar;
        public static DawnDayUIState DayMessage;
        public static ResetSceneUIState ResetSceneUIState;
        private static bool finalHoursSound = false;
        public static bool IsServer { get; private set; }
        public static bool IsShowingOverlay { get; private set; }
        public static bool IsSinglePlayer => Main.netMode != NetmodeID.MultiplayerClient;
        public static double frozenTime = -1;

        public static SoundEffectInstance finalHoursSoundEffect;

        #region Stackables
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
        #endregion

        private static NPC[] npcsToThaw;
        private static bool _pause;
        public static bool Pause
        {
            get
            {
                return _pause;
            }
            set
            {
                _pause = value;
                frozenTime = Main.time;
                if (_pause == true)
                {
                    npcsToThaw = Main.npc.Where(npc => npc.active).ToArray();

                    foreach (NPC npc in npcsToThaw)
                    {
                        npc.active = false;
                    }
                }
                else
                {
                    foreach (NPC npc in npcsToThaw)
                    {
                        npc.active = true;
                        if (!Main.npc.Contains(npc))
                        {
                            for (int i = 0; i < Main.npc.Length; i++)
                            {
                                if (Main.npc[i] == null || !Main.npc[i].active)
                                {
                                    Main.npc[i] = npc;
                                    npc.active = true;
                                    break;
                                }
                            }
                        }
                    }
                    npcsToThaw = null;
                }
            }
        }

        public override void Load()
        {
            instance = this;
            ready = new List<int>();
            finalHoursSound = false;
            if (!Main.dedServ)
            {
                TextureManager.Load("Images/UI/ResetScene");
                TextureManager.Load("Images/UI/ResetScene_1");
                TextureManager.Load("Images/UI/ResetScene_2");
                TextureManager.Load("Images/UI/ResetScene_3");

                MenuBar = new KeepsUIState();
                MenuBar.Activate();
                DayMessage = new DawnDayUIState();
                DayMessage.Activate();
                ResetSceneUIState = new ResetSceneUIState();
                ResetSceneUIState.Activate();
                _ui = new UserInterface();
            }
            else
            {
                IsServer = true;
            }
        }

        public static bool StackableItemTypes(Item item)
        {
            return item.potion || item.consumable;
        }

        public override void PreSaveAndQuit()
        {
            base.PreSaveAndQuit();

            if (_ui.CurrentState == MenuBar)
            {
                MenuBar.MoveItemsBack();
            }
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
                Main.time = frozenTime;
            }
            else
            {
                Main.gamePaused = false;
                Main.autoPause = false;
            }

            if (!IsServer && _ui?.CurrentState == ResetSceneUIState)
            {
                ResetSceneUIState.Step();
            }

            if (!NDayCycle.IsServer && !Main.dayTime)
            {
                const int spacing = 200;
                if (Main.time > 3600 * 8 + (3600 - spacing * 5) && Main.GameUpdateCount % 200 == 0)
                {
                    if (!WorldResetter.IsLastDay())
                    {
                        Main.PlaySound(this.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/TickSound"));
                    }
                }
                if (WorldResetter.IsLastDay() && (Main.time > 3600 * 6 + 3600 * 1 / 4) && !finalHoursSound)
                {
                    Main.NewText("Final hours");
                    finalHoursSoundEffect = Main.PlaySound(this.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/FinalHoursSound"));
                    finalHoursSound = true;
                }
            }
        }

        public override void Unload()
        {
            instance = null;
            _ui = null;
            MenuBar = null;
            IsShowingOverlay = false;
            WorldResetter.Unload();
        }

        public static void ShowMenu()
        {
            _ui?.SetState(MenuBar);
            MenuBar?.AutoFillItems();
            IsShowingOverlay = true;
        }

        public static void ShowDayMessage(string top, string mid, string bot)
        {
            const int pauseTime = 5000;
            if (!IsServer)
            {
                DayMessage.SetMessage(top, mid, bot);
                Main.PlaySound(instance.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/DawnSound"));
                _ui.SetState(DayMessage);
                var invIsOpen = Main.playerInventory;
                NDayCycle.Pause = true;
                Main.playerInventory = true;
                IsShowingOverlay = true;
                Task.Delay(pauseTime).ContinueWith(task =>
                {
                    HideUI();
                    NDayCycle.Pause = false;
                    Main.playerInventory = invIsOpen;
                });
            }
            else
            {
                Console.WriteLine($"Pausing for {pauseTime}ms; game time {Main.time}");
                NDayCycle.frozenTime = Main.time;
                NDayCycle.Pause = true;
                Main.playerInventory = true;
                Task.Delay(pauseTime).ContinueWith(task =>
                {
                    Console.WriteLine($"unpaused; game time: {Main.time}");
                    NDayCycle.Pause = false;
                    SendTime();
                });
            }
        }

        public static void ShowResetUI()
        {
            _ui?.SetState(ResetSceneUIState);
            ResetSceneUIState?.Reset();
            IsShowingOverlay = true;
        }

        public static void HideUI()
        {
            _ui?.SetState(null);
            IsShowingOverlay = false;
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
                        SendDay(whoAmI);
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
                        Main.time = reader.ReadDouble();
                        Main.dayTime = reader.ReadBoolean();
                        Main.NewText($"It is day {WorldResetter.Day + 1}.");
                        if (!WorldResetter.IsEnd() && Main.time < 100 && Main.dayTime)
                        {
                            NDayCycle.ShowDayMessage("Dawn of", $"The {(WorldResetter.endDay - WorldResetter.Day == 1 ? "Final" : WorldResetter.NumberToPosition(WorldResetter.Day + 1))} Day", $"-{(WorldResetter.endDay - WorldResetter.Day) * 24} Hours Remain-");
                        }
                        break;
                    case MessageTypes.SETTIME:
                        WorldResetter.Day = reader.ReadInt32();
                        Main.time = reader.ReadDouble();
                        Main.dayTime = reader.ReadBoolean();
                        if (!WorldResetter.IsEnd())
                        {
                            Main.NewText($"It is day {WorldResetter.Day + 1}.");
                        }
                        break;
                    case MessageTypes.RESETANDDISCONNECT:
                        MenuBar.ResetInventory();
                        break;
                    case MessageTypes.REWIND:
                        MenuBar.ResetInventory();
                        NDayCycle.ShowResetUI();
                        break;
                    case MessageTypes.RESPAWNPLAYERS:
                        Main.NewText("Respawning");
                        foreach (Player player in Main.player)
                        {
                            player.ghost = false;
                            player.position = new Vector2(Main.spawnTileX, Main.spawnTileY - 3).ToWorldCoordinates();
                        }
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
                WorldResetter.ResetWorld();
            }
        }

        /// <summary>
        /// Client sends packet to server
        /// </summary>
        public static void GetDayFromServer()
        {
            if (!IsServer)
            {
                ModPacket packet = instance.GetPacket();
                packet.Write((byte)MessageTypes.GETDAY);
                packet.Send();
            }
        }

        /// <summary>
        /// Client sends packet to server
        /// </summary>
        public static void ReadyForReset()
        {
            if (!IsServer)
            {
                ModPacket packet = instance.GetPacket();
                packet.Write((byte)MessageTypes.READYTORESET);
                packet.Send();
            }
        }

        /// <summary>
        /// Server sends packet to client
        /// </summary>
        public static void SendDay(int whoAmi = -1)
        {
            if (IsServer)
            {
                ModPacket packet = instance.GetPacket();
                packet.Write((byte)MessageTypes.SETDAY);
                packet.Write(WorldResetter.Day);
                packet.Write(Main.time);
                packet.Write(Main.dayTime);
                if (whoAmi >= 0)
                {
                    packet.Send(whoAmi);
                }
                else
                {
                    packet.Send();
                }
            }
        }

        /// <summary>
        /// Server sends packet to client
        /// </summary>
        public static void SendTime(int whoAmi = -1)
        {
            if (IsServer)
            {
                ModPacket packet = instance.GetPacket();
                packet.Write((byte)MessageTypes.SETTIME);
                packet.Write(WorldResetter.Day);
                packet.Write(Main.time);
                packet.Write(Main.dayTime);
                if (whoAmi >= 0)
                {
                    packet.Send(whoAmi);
                }
                else
                {
                    packet.Send();
                }
            }
        }

        /// <summary>
        /// Server sends packet to client
        /// </summary>
        public static void DisconnectPlayersForReset()
        {
            if (IsServer)
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

                    WorldResetter.ResetWorld();
                    Main.autoShutdown = true;
                    Environment.Exit(0);
                });
            }
        }

        /// <summary>
        /// Server sends packet to client
        /// </summary>
        public static void RespawnPlayers()
        {
            if (IsServer)
            {
                foreach (Player player in Main.player)
                {
                    player.ghost = false;
                }

                ModPacket packet = instance.GetPacket();
                packet.Write((byte)MessageTypes.RESPAWNPLAYERS);
                packet.Send();
            }
        }

        /// <summary>
        /// Server sends packet to client
        /// </summary>
        public static void RewindTime()
        {
            if (IsServer)
            {
                ModPacket packet = instance.GetPacket();
                packet.Write((byte)MessageTypes.REWIND);
                packet.Send();
            }
        }

        enum MessageTypes { NONE, GETDAY, SETDAY, SETTIME, READYTORESET, RESETANDDISCONNECT, REWIND, RESPAWNPLAYERS }
    }
}
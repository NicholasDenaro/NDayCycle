using Terraria;
using Terraria.ModLoader;

namespace NDayCycle
{
    class PlayerRules : ModPlayer
    {
        static Item watch;

        public override void Initialize()
        {
            PlayerRules.watch = new Item();
            PlayerRules.watch.SetDefaults(Terraria.ID.ItemID.GoldWatch);
        }

        public override void OnEnterWorld(Player player)
        {
            Main.NewText($"It is day {WorldResetter.Day}.");
        }

        public override void PostUpdateEquips()
        {
            bool wallSpeedBuff = false;
            bool tileSpeedbuff = false;
            bool tileRangeBuff = false;
            player.VanillaUpdateEquip(PlayerRules.watch);
            player.VanillaUpdateAccessory(player.whoAmI, PlayerRules.watch, true, ref wallSpeedBuff, ref tileSpeedbuff, ref tileRangeBuff);
        }

        private bool showMenu = true;
        public override void PreUpdateMovement()
        {
            //player.ghost = false;
            if (WorldResetter.Freeze)
            {
                Main.dayRate = 0;
                Main.gamePaused = true;
                Main.playerInventory = true;
                player.ghost = true;
                player.velocity = Microsoft.Xna.Framework.Vector2.Zero;
                player.position = new Microsoft.Xna.Framework.Vector2(Main.spawnTileX, Main.spawnTileY).ToWorldCoordinates();
                Main.LocalPlayer.mouseInterface = true;
                if (showMenu)
                {
                    Main.NewText("Show menu");
                    NDayCycle.ShowMenu();

                    showMenu = false;
                }
            }
            else
            {
                player.ghost = false;
            }
        }
    }
}

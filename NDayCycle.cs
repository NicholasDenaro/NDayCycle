using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace NDayCycle
{
    public class NDayCycle : Mod
	{
        private static UserInterface _ui;
        public static MenuBar MenuBar;

        public override void Load()
        {
            MenuBar = new MenuBar();
            MenuBar.Activate();
            _ui = new UserInterface();
        }

        public static void ShowMenu()
        {
            _ui.SetState(MenuBar);
        }

        public static void HideMenu()
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
    }
}
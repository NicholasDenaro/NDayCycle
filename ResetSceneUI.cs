using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Graphics;
using Terraria.ModLoader;
using Terraria.UI;

namespace NDayCycle
{
    /// <summary>
    /// ResetScene is from https://makeagif.com/gif/the-legend-of-zelda-majoras-mask-3ds-part-15-song-of-storms-gameplay-walkthrough-h3nqGz and is owned by Nintendo
    /// </summary>
    public class ResetSceneUIState: UIState
    {
        private UIVideo resetUI;
        public override void OnInitialize()
        {
            this.Width.Set(Main.screenWidth, 0);
            this.Height.Set(Main.screenHeight, 0);
            this.Left.Set(0, 0);
            this.Top.Set(0, 0);

            var texture = NDayCycle.instance.GetTexture("Images/UI/ResetScene");
            resetUI = new UIVideo();
            resetUI.Left.Set(0, 0);
            resetUI.Top.Set(0, 0);
            resetUI.HAlign = 0.5f;
            resetUI.VAlign = 0.5f;

            var panel = new UIPanel();
            panel.Left.Set(0, 0);
            panel.Top.Set(0, 0);
            panel.Width.Set(Main.screenWidth, 0);
            panel.Height.Set(Main.screenHeight, 0);
            panel.BackgroundColor = new Microsoft.Xna.Framework.Color(255, 255, 255);

            this.Append(panel);
            panel.Append(resetUI);

            this.Recalculate();
        }

        public void Step()
        {
            resetUI.Step();
        }

        public void Reset()
        {
            resetUI.Reset();
        }
    }

    class UIVideo : UIElement
    {
        int index = 0;
        public UIVideo()
        {

        }

        public void Reset()
        {
            index = 0;
        }

        public void Step()
        {
            if (Main.GameUpdateCount % (130 /  (1000 / 60)) == 0 && index < 80)
            {
                index++;
                this.Recalculate();
            }
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            spriteBatch.Draw(
                NDayCycle.instance.GetTexture($"Images/UI/ResetScene_{1 + index / (8100 / 225)}"),
                new Rectangle(0, 0, Main.screenWidth - 0, Main.screenHeight - 0),
                new Rectangle(0, index % (8100 / 225) * 225, 400, 225),
                new Color(0, 0, 0));
            spriteBatch.Draw(
                NDayCycle.instance.GetTexture($"Images/UI/ResetScene_{1 + index / (8100 / 225)}"),
                new Rectangle(100, 100, Main.screenWidth - 200, Main.screenHeight - 200),
                new Rectangle(0, index % (8100 / 225) * 225, 400, 225),
                new Color(255, 255, 255));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace NDayCycle
{
    class DawnDayUI : UIElement
    {
        UIText topText;
        UIText midText;
        UIText botText;

        public DawnDayUI()
        {
            this.Width.Set(Main.screenWidth, 0);
            this.Height.Set(Main.screenHeight, 0);
            this.Left.Set(0, 0);
            this.Top.Set(0, 0);
            var panel = new UIPanel();
            panel.BackgroundColor = new Microsoft.Xna.Framework.Color(0, 0, 0);
            panel.Width.Set(Main.screenWidth, 0);
            panel.Height.Set(Main.screenHeight, 0);
            panel.Left.Set(0, 0);
            panel.Top.Set(0, 0);

            topText = new UIText("Dawn of", 2, true);
            topText.TextColor = new Microsoft.Xna.Framework.Color(255, 255, 255);
            topText.HAlign = 0.5f;
            topText.VAlign = 0.33f;
            midText = new UIText("The First Day", 3, true);
            midText.TextColor = new Microsoft.Xna.Framework.Color(255, 255, 255);
            midText.HAlign = 0.5f;
            midText.VAlign = 0.50f;
            botText = new UIText("-72 Hours Remain-", 2, false);
            botText.TextColor = new Microsoft.Xna.Framework.Color(255, 255, 255);
            botText.HAlign = 0.5f;
            botText.VAlign = 0.67f;

            panel.Append(topText);
            panel.Append(midText);
            panel.Append(botText);

            this.Append(panel);

            this.Recalculate();
        }

        public void SetMessage(string top, string mid, string bot)
        {
            topText.SetText(top);
            midText.SetText(mid);
            botText.SetText(bot);
        }
    }

    public class DawnDayUIState: UIState
    {
        private DawnDayUI dayUI;

        public override void OnInitialize()
        {
            this.Width.Set(Main.screenWidth, 0);
            this.Height.Set(Main.screenHeight, 0);
            this.Left.Set(0, 0);
            this.Top.Set(0, 0);

            this.Append(dayUI = new DawnDayUI());
            dayUI.Activate();

            this.Recalculate();
        }

        public void SetMessage(string top, string mid, string bot)
        {
            dayUI.SetMessage(top, mid, bot);
        }
    }
}

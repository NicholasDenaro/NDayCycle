using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace NDayCycle
{
    public class KeepsUI : UIElement
    {
        private MenuBar menu;

        private Item pickaxeItem;
        private UIImage pickaxeItemImage;
        private UIText pickaxeItemCount;

        private Item axeItem;
        private UIImage axeItemImage;
        private UIText axeItemCount;

        private Item weaponItem;
        private UIImage weaponItemImage;
        private UIText weaponItemCount;

        private Item item1;
        private UIImage item1Image;
        private UIText item1Count;

        private Item item2;
        private UIImage item2Image;
        private UIText item2Count;

        private Item item3;
        private UIImage item3Image;
        private UIText item3Count;

        public KeepsUI(MenuBar menu)
        {
            this.menu = menu;
        }

        public override void OnInitialize()
        {
            base.OnInitialize();

            const int imgOffset = 12;
            const float imgScale = 1.25f;
            const int txtOffset = 30;

            pickaxeItem = new Item();
            axeItem = new Item();
            weaponItem = new Item();
            item1 = new Item();
            item2 = new Item();
            item3 = new Item();

            this.Width.Set(150, 0);
            this.Height.Set(100, 0);

            var img = new UIImage(ModContent.GetTexture("Terraria/Inventory_Back2"));
            img.Left.Set(0, 0);
            img.OnClick += (e, l) => Img_OnClick(e, l, ref pickaxeItem, ref pickaxeItemImage, ref pickaxeItemCount);
            this.Append(img);
            var tool = new UIImage(Main.itemTexture[ItemID.CopperPickaxe]);
            tool.Left.Set(0 - 4, 0);
            tool.Top.Set(0 - 4, 0);
            tool.ImageScale = 0.5f;
            this.Append(tool);
            pickaxeItemImage = new UIImage(Main.itemTexture[0]);
            pickaxeItemImage.Left.Set(0 + imgOffset, 0);
            pickaxeItemImage.Top.Set(0 + imgOffset, 0);
            pickaxeItemImage.ImageScale = imgScale;
            this.Append(pickaxeItemImage);
            pickaxeItemCount = new UIText("");
            pickaxeItemCount.Left.Set(0 + imgOffset, 0);
            pickaxeItemCount.Top.Set(0 + txtOffset, 0);
            this.Append(pickaxeItemCount);

            img = new UIImage(ModContent.GetTexture("Terraria/Inventory_Back2"));
            img.Left.Set(50, 0);
            img.OnClick += (e, l) => Img_OnClick(e, l, ref axeItem, ref axeItemImage, ref axeItemCount);
            this.Append(img);
            tool = new UIImage(Main.itemTexture[ItemID.CopperAxe]);
            tool.Left.Set(50 - 4, 0);
            tool.Top.Set(0 - 4, 0);
            tool.ImageScale = 0.5f;
            this.Append(tool);
            axeItemImage = new UIImage(Main.itemTexture[0]);
            axeItemImage.Left.Set(50 + imgOffset, 0);
            axeItemImage.Top.Set(0 + imgOffset, 0);
            axeItemImage.ImageScale = imgScale;
            this.Append(axeItemImage);
            axeItemCount = new UIText("");
            axeItemCount.Left.Set(50 + imgOffset, 0);
            axeItemCount.Top.Set(0 + txtOffset, 0);
            this.Append(axeItemCount);

            img = new UIImage(ModContent.GetTexture("Terraria/Inventory_Back2"));
            img.Left.Set(100, 0);
            img.OnClick += (e, l) => Img_OnClick(e, l, ref weaponItem, ref weaponItemImage, ref weaponItemCount);
            this.Append(img);
            tool = new UIImage(Main.itemTexture[ItemID.CopperShortsword]);
            tool.Left.Set(100 - 4, 0);
            tool.Top.Set(0 - 4, 0);
            tool.ImageScale = 0.5f;
            this.Append(tool);
            weaponItemImage = new UIImage(Main.itemTexture[0]);
            weaponItemImage.Left.Set(100 + imgOffset, 0);
            weaponItemImage.Top.Set(0 + imgOffset, 0);
            weaponItemImage.ImageScale = imgScale;
            this.Append(weaponItemImage);
            weaponItemCount = new UIText("");
            weaponItemCount.Left.Set(100 + imgOffset, 0);
            weaponItemCount.Top.Set(0 + txtOffset, 0);
            this.Append(weaponItemCount);

            img = new UIImage(ModContent.GetTexture("Terraria/Inventory_Back2"));
            img.Left.Set(0, 0);
            img.Top.Set(50, 0);
            img.OnClick += (e, l) => Img_OnClick(e, l, ref item1, ref item1Image, ref item1Count);
            this.Append(img);
            item1Image = new UIImage(Main.itemTexture[0]);
            item1Image.Left.Set(0 + imgOffset, 0);
            item1Image.Top.Set(50 + imgOffset, 0);
            item1Image.ImageScale = imgScale;
            this.Append(item1Image);
            item1Count = new UIText("");
            item1Count.Left.Set(0 + imgOffset, 0);
            item1Count.Top.Set(50 + txtOffset, 0);
            this.Append(item1Count);

            img = new UIImage(ModContent.GetTexture("Terraria/Inventory_Back2"));
            img.Left.Set(50, 0);
            img.Top.Set(50, 0);
            img.OnClick += (e, l) => Img_OnClick(e, l, ref item2, ref item2Image, ref item2Count);
            this.Append(img);
            item2Image = new UIImage(Main.itemTexture[0]);
            item2Image.Left.Set(50 + imgOffset, 0);
            item2Image.Top.Set(50 + imgOffset, 0);
            item2Image.ImageScale = imgScale;
            this.Append(item2Image);
            item2Count = new UIText("");
            item2Count.Left.Set(50 + imgOffset, 0);
            item2Count.Top.Set(50 + txtOffset, 0);
            this.Append(item2Count);

            img = new UIImage(ModContent.GetTexture("Terraria/Inventory_Back2"));
            img.Left.Set(100, 0);
            img.Top.Set(50, 0);
            img.OnClick += (e, l) => Img_OnClick(e, l, ref item3, ref item3Image, ref item3Count);
            this.Append(img);
            item3Image = new UIImage(Main.itemTexture[0]);
            item3Image.Left.Set(100 + imgOffset, 0);
            item3Image.Top.Set(50 + imgOffset, 0);
            item3Image.ImageScale = imgScale;
            this.Append(item3Image);
            item3Count = new UIText("");
            item3Count.Left.Set(100 + imgOffset, 0);
            item3Count.Top.Set(50 + txtOffset, 0);
            this.Append(item3Count);

            this.Recalculate();
        }

        private void Img_OnClick(UIMouseEvent evt, UIElement listeningElement, ref Item item, ref UIImage img, ref UIText txt)
        {
            if (menu.IsReady)
            {
                return;
            }

            //Main.NewText($"keepsui {Main.mouseItem.ToString()}");

            if (item == pickaxeItem && !Main.mouseItem.IsAir && Main.mouseItem.pick == 0)
            {
                return;
            }
            else if (item == axeItem && !Main.mouseItem.IsAir && Main.mouseItem.axe == 0)
            {
                return;
            }
            else if (item == weaponItem && !Main.mouseItem.IsAir && Main.mouseItem.damage <= 0)
            {
                return;
            }

            var temp = Main.mouseItem;
            Main.mouseItem = item;
            item = temp;
            Main.isMouseLeftConsumedByUI = true;

            img.SetImage(Main.itemTexture[item.type]);
            txt.SetText(item.stack > 1 ? "" + item.stack : "");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Main.LocalPlayer.mouseInterface = true;
        }

        public void DeepCloneItems()
        {
            pickaxeItem = pickaxeItem.DeepClone();
            axeItem = axeItem.DeepClone();
            weaponItem = weaponItem.DeepClone();
            item1 = item1.DeepClone();
            item2 = item2.DeepClone();
            item3 = item3.DeepClone();
        }

        public void SetItems()
        {
            Main.player[Main.myPlayer].inventory[0] = pickaxeItem.DeepClone();
            Main.player[Main.myPlayer].inventory[1] = axeItem.DeepClone();
            Main.player[Main.myPlayer].inventory[2] = weaponItem.DeepClone();
            Main.player[Main.myPlayer].inventory[3] = item1.DeepClone();
            Main.player[Main.myPlayer].inventory[4] = item2.DeepClone();
            Main.player[Main.myPlayer].inventory[5] = item3.DeepClone();

            pickaxeItem.TurnToAir();
            axeItem.TurnToAir();
            weaponItem.TurnToAir();
            item1.TurnToAir();
            item2.TurnToAir();
            item3.TurnToAir();

            pickaxeItemImage.SetImage(Main.itemTexture[0]);
            axeItemImage.SetImage(Main.itemTexture[0]);
            weaponItemImage.SetImage(Main.itemTexture[0]);
            item1Image.SetImage(Main.itemTexture[0]);
            item2Image.SetImage(Main.itemTexture[0]);
            item3Image.SetImage(Main.itemTexture[0]);

            pickaxeItemCount.SetText("");
            axeItemCount.SetText("");
            weaponItemCount.SetText("");
            item1Count.SetText("");
            item2Count.SetText("");
            item3Count.SetText("");
        }
    }

    public class MenuBar: UIState
    {
        KeepsUI keeps;

        public bool IsReady { get; private set; } = false;

        public override void OnInitialize()
        {
            UIPanel panel = new UIPanel();
            panel.Width.Set(300, 0);
            panel.Height.Set(200, 0);
            panel.HAlign = panel.VAlign = 0.5f;

            panel.Append(keeps = new KeepsUI(this));

            Append(panel);

            var resetButton = new UIImageButton(ModContent.GetTexture("Terraria/UI/ButtonPlay"));
            resetButton.HAlign = resetButton.VAlign = 1.0f;
            //resetButton.Left.Set(150 - 30, 0);
            //resetButton.Top.Set(100 - 30, 0);
            resetButton.OnClick += ResetButton_OnClick;
            resetButton.Recalculate();
            panel.Append(resetButton);

            this.Recalculate();
        }

        private void ResetButton_OnClick(UIMouseEvent evt, UIElement listeningElement)
        {
            if (NDayCycle.IsSinglePlayer)
            {
                Main.NewText("Rewinding time");

                ResetInventory();
                WorldResetter.ResetWorld();
            }
            else
            {
                this.IsReady = true;
                Main.NewText("Sent ready message to server");
                NDayCycle.ReadyForReset();
            }
        }

        public void ResetInventory()
        {
            this.IsReady = false;
            keeps.DeepCloneItems();

            foreach (Item item in Main.player[Main.myPlayer].inventory)
            {
                item.TurnToAir();
            }

            foreach (Item item in Main.player[Main.myPlayer].armor)
            {
                item.TurnToAir();
            }

            keeps.SetItems();

            this.Deactivate();
            NDayCycle.HideMenu();
            WorldGen.SaveAndQuit();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //Main.LocalPlayer.mouseInterface = true;
        }

        public override void Click(UIMouseEvent evt)
        {
            //Main.NewText($"menubar {Main.mouseItem.ToString()}");
        }
    }
}

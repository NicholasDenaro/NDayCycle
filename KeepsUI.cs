using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace NDayCycle
{
    public class KeepsUI : UIElement
    {
        private MenuBarUIState menu;

        private List<ItemSlot> slots = new List<ItemSlot>();

        public int SlotsCount => slots.Count;

        public KeepsUI(MenuBarUIState menu)
        {
            this.menu = menu;
        }

        public override void OnInitialize()
        {
            base.OnInitialize();

            slots.Add(new ItemSlot(0, 0, this, ItemSlot.SlotType.pickaxe));
            slots.Add(new ItemSlot(50, 0, this, ItemSlot.SlotType.axe));
            slots.Add(new ItemSlot(100, 0, this, ItemSlot.SlotType.hammer));
            slots.Add(new ItemSlot(150, 0, this, ItemSlot.SlotType.weapon));

            for (int i = 0; i < ModContent.GetInstance<NDayCycleConfig>().MiscItemsToKeep; i++)
            {
                slots.Add(new ItemSlot(50 * (i % 3), 50 * (1 + i / 3), this, ItemSlot.SlotType.any));
            }

            this.Width.Set(200, 0);
            this.Height.Set(50 + 50 * (1 + ModContent.GetInstance<NDayCycleConfig>().MiscItemsToKeep / 3), 0);

            this.Recalculate();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Main.LocalPlayer.mouseInterface = true;
        }

        public void SetItems()
        {
            for (int i = 0; i < slots.Count; i++)
            {
                Main.player[Main.myPlayer].inventory[i] = slots[i].ItemClone;
                slots[i].TurnToAir();
            }
        }
    }

    public class ItemSlot
    {
        public enum SlotType { pickaxe = ItemID.CopperPickaxe, axe = ItemID.CopperAxe, hammer = ItemID.CopperHammer, weapon = ItemID.CopperShortsword, any = 0 }

        private Item item;
        private UIImage image;
        private UIText count;
        private SlotType type;

        public Item Item => item;
        public Item ItemClone => item.DeepClone();

        private bool enabled;

        const int imgOffset = 12;
        const float imgScale = 1.25f;
        const int txtOffset = 30;
        public ItemSlot(int x, int y, UIElement parent, SlotType slot)
        {
            this.item = new Item();
            this.image = new UIImage(Main.itemTexture[0]);
            this.count = new UIText("");
            this.enabled = true;
            this.type = slot;

            var img = new UIImage(ModContent.GetTexture("Terraria/Inventory_Back2"));
            img.Left.Set(x, 0);
            img.Top.Set(y, 0);
            img.OnClick += Img_OnClick;
            parent.Append(img);

            if (type != SlotType.any)
            {
                var tool = new UIImage(Main.itemTexture[(int)type]);
                tool.Left.Set(x - 4, 0);
                tool.Top.Set(y - 4, 0);
                tool.ImageScale = 0.5f;
                parent.Append(tool);
            }

            image = new UIImage(Main.itemTexture[0]);
            image.Left.Set(x + imgOffset, 0);
            image.Top.Set(y + imgOffset, 0);
            image.ImageScale = imgScale;
            parent.Append(image);

            count = new UIText("");
            count.Left.Set(x + imgOffset, 0);
            count.Top.Set(y + txtOffset, 0);
            parent.Append(count);
        }

        public void Disable()
        {
            this.enabled = false;
        }

        public void Enable()
        {
            this.enabled = true;
        }

        public void TurnToAir()
        {
            this.item.TurnToAir();
            image.SetImage(Main.itemTexture[0]);
            count.SetText("");
        }

        private void Img_OnClick(UIMouseEvent evt, UIElement listeningElement)
        {
            if (!this.enabled)
            {
                return;
            }

            if (ModContent.GetInstance<NDayCycleConfig>().DisableStackableItems && (NDayCycle.StackableItems.Contains(Main.mouseItem.type) || NDayCycle.StackableItemTypes(Main.mouseItem)))
            {
                return;
            }

            if (type == SlotType.pickaxe && !Main.mouseItem.IsAir && Main.mouseItem.pick <= 0)
            {
                return;
            }
            else if (type == SlotType.axe && !Main.mouseItem.IsAir && Main.mouseItem.axe <= 0)
            {
                return;
            }
            else if (type == SlotType.hammer && !Main.mouseItem.IsAir && Main.mouseItem.hammer <= 0)
            {
                return;
            }
            else if (type == SlotType.weapon && !Main.mouseItem.IsAir && Main.mouseItem.damage <= 0)
            {
                return;
            }

            var temp = Main.mouseItem;
            Main.mouseItem = item;
            item = temp;
            Main.isMouseLeftConsumedByUI = true;

            image.SetImage(Main.itemTexture[item.type]);
            count.SetText(item.stack > 1 ? "" + item.stack : "");
        }
    }

    public class MenuBarUIState: UIState
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

            keeps.SetItems();

            foreach (Item item in Main.player[Main.myPlayer].inventory.Skip(keeps.SlotsCount))
            {
                if ((item.type == ItemID.CopperCoin || item.type == ItemID.SilverCoin || item.type == ItemID.GoldCoin || item.type == ItemID.PlatinumCoin) && ModContent.GetInstance<NDayCycleConfig>().KeepMoney)
                {
                    continue;
                }

                item.TurnToAir();
            }

            foreach (Item item in Main.player[Main.myPlayer].armor.Skip(ModContent.GetInstance<NDayCycleConfig>().KeepEquips ? 3 : 0))
            {
                if (item.accessory && ModContent.GetInstance<NDayCycleConfig>().KeepAccessories)
                {
                    continue;
                }
                item.TurnToAir();
            }

            NDayCycle.HideUI();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}

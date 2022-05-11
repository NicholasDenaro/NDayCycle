using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Config;

namespace NDayCycle
{
    class NDayCycleConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [Label("Keep equipment after reset")]
        [Tooltip("When enabled, players will keep their equipment after the world is reset")]
        [DefaultValue(false)]
        public bool KeepEquips { get; set; }

        [Label("Keep accessories after reset")]
        [Tooltip("When enabled, players will keep their accessories after the world is reset")]
        [DefaultValue(false)]
        public bool KeepAccessories { get; set; }

        [Label("Keep money after reset")]
        [Tooltip("When enabled, players will keep their money after the world is reset")]
        [DefaultValue(false)]
        public bool KeepMoney { get; set; }

        [Label("Disallow items that are stackable")]
        [Tooltip("When enabled, players will not be allowed to keep stackable items after the world is reset")]
        [DefaultValue(false)]
        public bool DisableStackableItems { get; set; }

        [ReloadRequired]
        [Label("Specifies the number additional items to keep")]
        [Tooltip("Allows to customize additional items slots to keep after the world is reset (between 0 and 6)")]
        [DefaultValue(3)]
        public int MiscItemsToKeep { get; set; }


    }
}

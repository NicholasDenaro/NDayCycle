using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace NDayCycle.Commands
{
    class PauceCommand : ModCommand
    {
        public override string Command => "pause";

        public override CommandType Type => CommandType.Chat | CommandType.Console;

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            NDayCycle.Pause = !NDayCycle.Pause;
            Main.NewText($"Paused: {NDayCycle.Pause}");
        }
    }
}

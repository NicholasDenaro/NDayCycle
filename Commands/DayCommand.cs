using Terraria;
using Terraria.ModLoader;

namespace NDayCycle
{
    class DayCommand : ModCommand
    {
        public override string Command => "day";

        public override CommandType Type => CommandType.Chat | CommandType.Console;

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            Main.time = 3600 * 8 + 1800;
            Main.dayTime = false;
            NDayCycle.SendTime();
        }
    }
}

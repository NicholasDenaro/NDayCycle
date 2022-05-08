using Terraria.ModLoader;

namespace NDayCycle
{
    class DayCommand : ModCommand
    {
        public override string Command => "day";

        public override CommandType Type => CommandType.Chat;

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            WorldResetter.NextDay();
        }
    }
}

using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace NDayCycle
{
    class WorldResetter : ModWorld
    {
        static IWorldResetStrategy wrs = new CopyTileStateWorldResetStrategy();

        private static bool dayProgress = false;
        public static int Day { get; set; } = 0;
        public static bool Freeze => IsEndDay();

        public override void PostWorldGen()
        {
            wrs.CopyBaseState();
        }

        public override void Initialize()
        {
            Day = 0;
            dayProgress = true;
        }

        public override TagCompound Save()
        {
            try
            {
                var state = wrs.State();
                var tag = new TagCompound
                {
                    [nameof(Day)] = Day,
                    [nameof(dayProgress)] = dayProgress,
                };

                tag.Add("world", state);

                return tag;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"{DateTime.Now} {ex.Message}\n{ex.StackTrace}");
            }

            return null;
        }

        public override void Load(TagCompound tag)
        {
            Day = tag.GetInt(nameof(Day));
            dayProgress = tag.GetBool(nameof(dayProgress));
            wrs.LoadState(tag.GetCompound("world"));
        }

        private static double frozenTime;
        public override void PostUpdate()
        {
            base.PostUpdate();

            if (NDayCycle.IsSinglePlayer || NDayCycle.IsServer)
            {
                if (!dayProgress && Main.dayTime && Main.time < 3600)
                {
                    NextDay();
                    if (IsEndDay())
                    {
                        frozenTime = Main.time;
                    }
                    dayProgress = true;
                }
                else if (dayProgress && !Main.dayTime)
                {
                    dayProgress = false;
                }

                if (IsEndDay())
                {
                    Main.time = frozenTime;
                }
            }
        }

        public static void NextDay()
        {
            Day++;

            if (NDayCycle.IsServer)
            {
                NDayCycle.SendDay();
                Console.WriteLine($"day={Day} (Dawn of the {Day + 1} day)");
            }

            if (IsEndDay())
            {
                Main.gamePaused = true;
                Main.dayRate = 0;
                Main.NewText("The moon crashes into the world.", Color.Red);
            }
            else
            {
                Main.NewText($"Dawn of the {(endDay - Day == 1 ? "final" : "" + (Day + 1))} day, {(endDay - Day) * 24} hours remaining");
            }
        }

        public static void ResetWorld()
        {
            Day = 0;
            Main.dayRate = 1;
            dayProgress = true;
            wrs.ResetToBaseState(NDayCycle.IsServer);
            Main.NewText($"Day is {Day}");
        }

        public static void RestoreOriginalWorld()
        {
            string world = Path.Combine(Main.WorldPath, Main.worldName + ".wld");
            File.Copy(world + ".ncyclebak", world, true);
        }


        const int endDay = 3;
        public static bool IsEndDay()
        {
            return Day >= endDay;
        }
    }
}

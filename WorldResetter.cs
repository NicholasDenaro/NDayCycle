using Microsoft.Xna.Framework;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace NDayCycle
{
    class WorldResetter : ModWorld
    {
        static WorldResetter instance;

        private static bool dayProgress = false;
        public static int Day { get; private set; } = 0;
        public static bool Freeze => IsEndDay();

        public override void PostWorldGen()
        {
            string world = Path.Combine(Main.WorldPath, Main.worldName + ".wld");
            File.WriteAllText("c:\\modinfo\\log.txt", $"{File.Exists(world)}");
            Task.Run(() =>
            {
                while (!File.Exists(world))
                {
                    Thread.Sleep(100);
                }
                File.Copy(world, world + ".ncyclebak");
            });
        }

        public override void Initialize()
        {
            instance = this;
            Day = 0;
            dayProgress = true;
        }

        public override TagCompound Save()
        {
            return new TagCompound
            {
                [nameof(Day)] = Day,
                [nameof(dayProgress)] = dayProgress,
            };
        }

        public override void Load(TagCompound tag)
        {
            Day = tag.GetInt(nameof(Day));
            dayProgress = tag.GetBool(nameof(dayProgress));
        }

        public override void PostUpdate()
        {
            base.PostUpdate();

            if (!dayProgress && Main.dayTime && Main.time < 3600)
            {
                Day++;
                dayProgress = true;

                if (IsEndDay())
                {
                    Main.gamePaused = true;
                    Main.dayRate = 0;
                    Main.NewText("The moon crashes into the world.", Color.Red);
                }
                else
                {
                    Main.NewText($"Dawn of the ${(endDay - Day == 1 ? "final" : "" + (Day + 1))} day, ${(endDay - Day) * 24} hours remaining");
                }
            }
            else if (dayProgress && !Main.dayTime)
            {
                dayProgress = false;
            }
        }

        public static void NextDay()
        {
            Day++;
        }

        public static void ResetWorld()
        {
            Day = 0;
            Main.dayRate = 1;
            instance.Save();
            WorldGen.SaveAndQuit(WorldResetter.RestoreOriginalWorld);
        }

        public static void RestoreOriginalWorld()
        {
            string world = Path.Combine(Main.WorldPath, Main.worldName + ".wld");
            File.Copy(world + ".ncyclebak", world, true);
        }


        const int endDay = 3;
        private static bool IsEndDay()
        {
            return Day >= endDay;
        }
    }
}

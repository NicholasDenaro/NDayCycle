using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace NDayCycle
{
    class WorldResetter : ModWorld
    {
        private static TagCompound baseState;

        static IWorldResetStrategy wrs = new CopyTileStateWorldResetStrategy();

        private static bool dayProgress = false;
        public static int Day { get; set; } = 0;
        public static bool Freeze => IsEnd();

        public override void PostWorldGen()
        {
            wrs.CopyBaseState();
        }

        public override void Initialize()
        {
            Day = 0;
            dayProgress = true;
        }

        public static void Unload()
        {
            wrs.Unload();
            wrs = null;
        }

        class BackupData
        {
            public List<List<int>> Tiles { get; set; }

            public List<List<List<int>>> Chests { get; set; }
        }


        public override TagCompound Save()
        {
            try
            {
                var tag = new TagCompound
                {
                    [nameof(Day)] = Day,
                    [nameof(dayProgress)] = dayProgress,
                };

                if (baseState == null)
                {
                    var state = wrs.State();
                    File.WriteAllText(Path.Combine(Main.WorldPath, Main.worldName) + ".ndcbak", JsonConvert.SerializeObject(new BackupData
                    {
                        Tiles = state.Get<List<List<int>>>("tiles"),
                        Chests = state.Get<List<List<List<int>>>>("chests"),
                    }));
                }

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

            var state = JsonConvert.DeserializeObject<BackupData>(File.ReadAllText(Path.Combine(Main.WorldPath, Main.worldName) + ".ndcbak"));
            baseState = new TagCompound
            {
                ["tiles"] = state.Tiles,
                ["chests"] = state.Chests
            };
            wrs.LoadState(baseState);
        }

        private static bool doingReset = false;
        private static double frozenTime;
        public override void PostUpdate()
        {
            base.PostUpdate();

            if (doingReset && (NDayCycle.IsSinglePlayer || NDayCycle.IsServer))
            {
                doingReset = !wrs.ResetStep();
                return;
            }

            if (NDayCycle.IsSinglePlayer || NDayCycle.IsServer)
            {
                if (!dayProgress && Main.dayTime && Main.time < 1000)
                {
                    NextDay();
                    if (IsEnd())
                    {
                        frozenTime = Main.time;
                    }
                    dayProgress = true;
                }
                else if (dayProgress && !Main.dayTime)
                {
                    dayProgress = false;
                }

                if (IsEnd())
                {
                    Main.time = frozenTime;
                }
            }
        }

        public void NextDay()
        {
            Day++;

            if (NDayCycle.IsServer)
            {
                NDayCycle.SendDay();
                Console.WriteLine($"day={Day} (Dawn of the {Day + 1} day)");
            }

            if (IsEnd())
            {
                Main.gamePaused = true;
                Main.dayRate = 0;
                Main.NewText("The moon crashes into the world.", Color.Red);
            }
            else
            {
                Main.NewText($"Dawn of the {(endDay - Day == 1 ? "final" : "" + (Day + 1))} day, {(endDay - Day) * 24} hours remain");
                NDayCycle.ShowDayMessage("Dawn of", $"The {(endDay - Day == 1 ? "Final" : NumberToPosition(Day + 1))} Day", $"-{(endDay - Day) * 24} Hours Remain-");
            }
        }
        
        public static string NumberToPosition(int num)
        {
            switch(num)
            {
                case 1:
                    return "First";
                case 2:
                    return "Second";
                case 3:
                    return "Third";
                case 4:
                    return "Fourth";
                case 5:
                    return "Fifth";
                default:
                    return "Unknown";
            }
        }

        public static void ResetWorld()
        {
            Day = 0;
            Main.dayRate = 1;
            dayProgress = true;
            Main.StopTrackedSounds();
            NDayCycle.finalHoursSoundEffect.Stop();
            NDayCycle.ShowResetUI();
            wrs.ResetToBaseState(NDayCycle.IsServer);
            doingReset = true;
        }

        public const int endDay = 3;
        public static bool IsEnd()
        {
            return Day >= endDay;
        }
        public static bool IsLastDay()
        {
            return Day == endDay - 1;
        }
    }
}

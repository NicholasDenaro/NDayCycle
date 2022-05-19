using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace NDayCycle
{
    class CopyTileStateWorldResetStrategy : IWorldResetStrategy
    {
        private Tile[,] tileStates;
        private List<List<List<int>>> chests;
        private int size;

        public void CopyBaseState()
        {
            size = Main.maxTilesX * Main.maxTilesY;
            tileStates = new Tile[Main.maxTilesX, Main.maxTilesY];
            for (int y = 0; y < Main.maxTilesY; y++)
            {
                for (int x = 0; x < Main.maxTilesX; x++)
                {
                    tileStates[x, y] = new Tile(Main.tile[x, y]);
                }
            }

            chests = new List<List<List<int>>>(Main.chest.Length);
            for (int i = 0; i < Main.chest.Length; i++)
            {
                List<List<int>> items = Main.chest[i]?.item.Select(item => new List<int> {
                    item?.type ?? ItemID.None,
                    item?.stack ?? 0,
                }).ToList() ?? new List<List<int>>();

                chests.Insert(i, items);
            }
        }

        public void ResetToBaseState(bool isServer)
        {
            startReset = DateTime.Now;
            step = 0;
            stepState = ResetState.RESETTILES;

            NDayCycle.RewindTime();
        }

        private static DateTime startReset;
        enum ResetState { RESETTILES, RESETFRAMES, RESETPROJECTILES, RESETCHESTS, RESETNPCS, RESETPLAYERS };
        private int step = 0;
        private ResetState stepState = ResetState.RESETTILES;
        public bool ResetStep()
        {
            try
            {
                var start = DateTime.Now;
                Main.playerInventory = false;
                while ((DateTime.Now - start).TotalMilliseconds < 16.66667 * 1)
                {
                    int x = step % Main.maxTilesX;
                    int y = step / Main.maxTilesX;
                    switch (stepState)
                    {
                        case ResetState.RESETTILES:
                            Main.tile[x, y] = new Tile(tileStates[x, y]);
                            if (++step >= Main.maxTilesX * Main.maxTilesY)
                            {
                                Main.NewText("Finished resetting tiles");
                                Console.WriteLine("Finished resetting tiles");
                                step = 0;
                                stepState = ResetState.RESETFRAMES;
                            }
                            break;
                        case ResetState.RESETFRAMES:
                            if (Main.tile[x, y].active())
                            {
                                WorldGen.SquareTileFrame(x, y, true);
                            }
                            if (Main.tile[x, y].wall != WallID.None)
                            {
                                WorldGen.SquareWallFrame(x, y, true);
                            }
                            if (++step >= Main.maxTilesX * Main.maxTilesY)
                            {
                                Main.NewText("Finished resetting tile walls");
                                Console.WriteLine("Finished resetting tile walls");
                                step = 0;
                                stepState = ResetState.RESETPROJECTILES;
                            }
                            break;
                        case ResetState.RESETPROJECTILES:
                            if (step < Main.projectile.Length)
                            {
                                Main.projectile[step].active = false;
                            }

                            if (++step >= Main.projectile.Length)
                            {
                                Main.NewText("Finished resetting projectiles");
                                Console.WriteLine("Finished resetting projectiles");
                                step = 0;
                                stepState = ResetState.RESETCHESTS;
                            }

                            break;
                        case ResetState.RESETCHESTS:

                            for (int it = 0; it < (Main.chest[step]?.item.Length ?? 0); it++)
                            {
                                Main.chest[step]?.item[it]?.SetDefaults(chests[step][it][0]);
                                Main.chest[step].item[it].stack = chests[step][it][1];
                            }

                            if (++step >= Main.chest.Length)
                            {
                                Main.NewText("Finished resetting chests");
                                Console.WriteLine("Finished resetting chests");
                                step = 0;
                                stepState = ResetState.RESETNPCS;
                            }
                            break;
                        case ResetState.RESETNPCS:
                            if (step < Main.npc.Length)
                            {
                                Main.npc[step].life = 0;
                            }

                            if (++step >= Main.npc.Length)
                            {
                                Main.NewText("Finished resetting npcs");
                                Console.WriteLine("Finished resetting npcs");
                                step = 0;
                                stepState = ResetState.RESETPLAYERS;
                            }
                            break;
                        case ResetState.RESETPLAYERS:
                            if (step < Main.player.Length)
                            {
                                Main.player[step].ghost = false;
                                Main.player[step].position = new Microsoft.Xna.Framework.Vector2(Main.spawnTileX, Main.spawnTileY - 3).ToWorldCoordinates();
                            }

                            if (++step >= Main.player.Length)
                            {
                                step = 0;
                                stepState = ResetState.RESETTILES;
                                Main.NewText("Finished resetting players");
                                Console.WriteLine("Finished resetting players");

                                Main.updateMap = true;
                                Main.time = 0;
                                Main.dayTime = true;

                                NPC.NewNPC(Main.spawnTileX * 16, (Main.spawnTileY - 3) * 16, NPCID.Guide);

                                if (NDayCycle.IsSinglePlayer)
                                {
                                    Main.NewText($"Reset time: {DateTime.Now - startReset}");
                                    Console.WriteLine($"Reset time: {DateTime.Now - startReset}");
                                    NDayCycle.HideUI();
                                    NDayCycle.ShowDayMessage("Dawn of", $"The First Day", $"-72 Hours Remain-");
                                }
                                if (NDayCycle.IsServer)
                                {
                                    NDayCycle.SendDay();
                                    NDayCycle.RespawnPlayers();
                                }

                                return true;
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Main.NewText($"Exception: {ex.Message} {ex.StackTrace}");
                Console.Error.WriteLine($"Exception: {ex.Message} {ex.StackTrace}");
            }

            return false;
        }

        public void LoadState(TagCompound tag)
        {
            List<List<int>> tags = tag.Get<List<List<int>>>("tiles");
            tileStates = new Tile[Main.maxTilesX, Main.maxTilesY];
            for (int y = 0; y < Main.maxTilesY; y++)
            {
                for (int x = 0; x < Main.maxTilesX; x++)
                {
                    tileStates[x, y] = new Tile();
                    tileStates[x, y].type = (ushort)tags[y * Main.maxTilesX + x][0];
                    tileStates[x, y].wall = (ushort)tags[y * Main.maxTilesX + x][1];
                    tileStates[x, y].liquid = (byte)tags[y * Main.maxTilesX + x][2];
                    tileStates[x, y].sTileHeader = (ushort)tags[y * Main.maxTilesX + x][3];
                    tileStates[x, y].bTileHeader = (byte)tags[y * Main.maxTilesX + x][4];
                    tileStates[x, y].bTileHeader2 = (byte)tags[y * Main.maxTilesX + x][5];
                    tileStates[x, y].bTileHeader3 = (byte)tags[y * Main.maxTilesX + x][6];
                    tileStates[x, y].frameX = (short)tags[y * Main.maxTilesX + x][7];
                    tileStates[x, y].frameY = (short)tags[y * Main.maxTilesX + x][8];
                }
            }

            chests = tag.Get<List<List<List<int>>>>("chests");
        }

        public TagCompound State()
        {
            var tiles = new List<List<int>>(size);

            for (int y = 0; y < Main.maxTilesY; y++)
            {
                for (int x = 0; x < Main.maxTilesX; x++)
                {
                    try
                    {
                        var arr = new List<int> {
                            tileStates[x, y].type,
                            tileStates[x, y].wall,
                            tileStates[x, y].liquid,
                            tileStates[x, y].sTileHeader,
                            tileStates[x, y].bTileHeader,
                            tileStates[x, y].bTileHeader2,
                            tileStates[x, y].bTileHeader3,
                            tileStates[x, y].frameX,
                            tileStates[x, y].frameY,
                        };

                        tiles.Insert(y * Main.maxTilesX + x, arr);
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine($"{DateTime.Now} maxX,maxY {Main.maxTilesX},{Main.maxTilesY}\n");
                        Console.Error.WriteLine($"{DateTime.Now} x,y {x},{y}\n");
                        Console.Error.WriteLine($"{DateTime.Now} {ex.Message}\n{ex.StackTrace}\n");
                        throw;
                    }
                }
            }

            return new TagCompound
            {
                ["tiles"] = tiles,
                ["chests"] = chests,
            };
        }
    }
}

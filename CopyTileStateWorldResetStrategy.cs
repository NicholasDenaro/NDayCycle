using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
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
            try
            {
                Main.NewText("Resetting tiles");
                for (int y = 0; y < Main.maxTilesY; y++)
                {
                    for (int x = 0; x < Main.maxTilesX; x++)
                    {
                        Main.tile[x, y] = new Tile(tileStates[x, y]);
                    }
                }

                Main.NewText("Resetting tile frames");
                for (int y = 0; y < Main.maxTilesY; y++)
                {
                    for (int x = 0; x < Main.maxTilesX; x++)
                    {
                        WorldGen.SquareTileFrame(x, y, true);
                        WorldGen.SquareWallFrame(x, y, true);
                    }
                }

                Main.NewText("Resetting chests");
                for (int i = 0; i < Main.chest.Length; i++)
                {
                    for (int it = 0; it < (Main.chest[i]?.item.Length ?? 0); it++)
                    {
                        Main.chest[i]?.item[it]?.SetDefaults(chests[i][it][0]);
                        Main.chest[i].item[it].stack = chests[i][it][1];
                    }
                }

                Main.updateMap = true;
                Main.time = 3600;
                Main.dayTime = true;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"{DateTime.Now} {ex.Message}\n{ex.StackTrace}");
                Main.NewText($"Error: {ex.Message}");
            }
            finally
            {
                Main.NewText("Resetting player ghosts");
                foreach (Player player in Main.player)
                {
                    player.ghost = false;
                    player.Teleport(new Microsoft.Xna.Framework.Vector2(Main.spawnTileX, Main.spawnTileY - 3).ToWorldCoordinates());
                }

                Main.time = 3600;
                Main.dayTime = true;
                Main.NewText("Finished resetting");
            }
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

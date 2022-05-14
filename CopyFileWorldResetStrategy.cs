using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Terraria;
using Terraria.IO;
using Terraria.ModLoader.IO;

namespace NDayCycle
{
    class CopyFileWorldResetStrategy : IWorldResetStrategy
    {
        public void CopyBaseState()
        {
            string world = Path.Combine(Main.WorldPath, Main.worldName + ".wld");
            Task.Run(() =>
            {
                while (!File.Exists(world))
                {
                    Thread.Sleep(100);
                }
                File.Copy(world, world + ".ncyclebak");
            });
        }

        public void LoadState(TagCompound tags)
        {
        }

        public void ResetToBaseState(bool isServer)
        {
            if (!isServer)
            {
                WorldGen.SaveAndQuit(WorldResetter.RestoreOriginalWorld);
            }
            else
            {
                WorldFile.saveWorld(WorldFile.IsWorldOnCloud);
                NDayCycle.DisconnectPlayersForReset();
            }
        }

        public TagCompound State()
        {
            return new TagCompound();
        }
    }
}

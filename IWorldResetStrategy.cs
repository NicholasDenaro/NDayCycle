using Terraria.ModLoader.IO;

namespace NDayCycle
{
    interface IWorldResetStrategy
    {
        void CopyBaseState();

        void ResetToBaseState(bool isServer);

        void LoadState(TagCompound tags);

        TagCompound State();
    }
}

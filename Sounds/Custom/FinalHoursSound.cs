using Microsoft.Xna.Framework.Audio;
using Terraria;
using Terraria.ModLoader;

namespace NDayCycle.Sounds.Custom
{
	/// <summary>
	/// FinalHoursSound.mp3 is from https://www.youtube.com/watch?v=Lcpd0lhYMQI and is owned by Nintendo
	/// </summary>
	class FinalHoursSound : ModSound
    {
		public override SoundEffectInstance PlaySound(ref SoundEffectInstance soundInstance, float volume, float pan, SoundType type)
		{
			// By creating a new instance, this ModSound allows for overlapping sounds. Non-ModSound behavior is to restart the sound, only permitting 1 instance.
			soundInstance = sound.CreateInstance();
			soundInstance.Volume = volume * Main.ambientVolume;
			soundInstance.Pan = pan;
			//soundInstance.Pitch = -1.0f;
			return soundInstance;
		}
	}
}

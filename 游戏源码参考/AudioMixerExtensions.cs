using UnityEngine.Audio;

public static class AudioMixerExtensions
{
	public static void TransitionToSafe(this AudioMixerSnapshot snapshot, float timeToReach)
	{
		if (!(snapshot == null))
		{
			snapshot.TransitionTo(timeToReach);
		}
	}
}

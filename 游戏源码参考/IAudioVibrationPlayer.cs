using UnityEngine;

public interface IAudioVibrationPlayer
{
	VibrationEmission PlayAudioClip(AudioClip audioClip, string tag = null);

	VibrationEmission PlayAudioClip(AudioClip audioClip, AudioSource referenceSource, string tag = null);

	void StopEmissionsWithClip(AudioClip audioClip);
}

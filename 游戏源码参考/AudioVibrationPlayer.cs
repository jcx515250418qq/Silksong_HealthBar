using UnityEngine;

public class AudioVibrationPlayer : IAudioVibrationPlayer
{
	public VibrationEmission PlayAudioClip(AudioClip audioClip, string tag = null)
	{
		Debug.LogError("Play Audio clip vibration not implemented");
		return null;
	}

	public VibrationEmission PlayAudioClip(AudioClip audioClip, AudioSource referenceSource, string tag = null)
	{
		Debug.LogError("Play Audio clip vibration not implemented");
		return null;
	}

	public void StopEmissionsWithClip(AudioClip audioClip)
	{
		Debug.LogError("Stop Emissions with clip not implemented");
	}
}

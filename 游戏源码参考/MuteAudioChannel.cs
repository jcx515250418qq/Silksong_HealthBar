using UnityEngine;
using UnityEngine.Audio;

public class MuteAudioChannel : MonoBehaviour
{
	public AudioMixer mixer;

	public string exposedProperty = "volActors";

	private float initialVolume;

	public float Volume
	{
		get
		{
			float value = 0f;
			if ((bool)mixer)
			{
				mixer.GetFloat(exposedProperty, out value);
			}
			return DecibelToLinear(value);
		}
		set
		{
			if ((bool)mixer)
			{
				mixer.SetFloat(exposedProperty, LinearToDecibel(value));
			}
		}
	}

	private void OnEnable()
	{
		if ((bool)mixer)
		{
			initialVolume = Volume;
			Volume = 0f;
		}
	}

	private void OnDisable()
	{
		if ((bool)mixer)
		{
			Volume = initialVolume;
		}
	}

	private float LinearToDecibel(float linear)
	{
		if (linear != 0f)
		{
			return 20f * Mathf.Log10(linear);
		}
		return -144f;
	}

	private float DecibelToLinear(float dB)
	{
		return Mathf.Pow(10f, dB / 20f);
	}
}

using UnityEngine;

public class VibrationPlayer : MonoBehaviour
{
	[SerializeField]
	private VibrationData data;

	[SerializeField]
	private VibrationDataAsset vibrationDataAsset;

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private VibrationTarget target;

	[SerializeField]
	private bool playAutomatically;

	[SerializeField]
	private bool isLooping;

	[SerializeField]
	private string vibrationTag;

	[SerializeField]
	private bool isRealtime;

	private VibrationEmission emission;

	private bool tryPlayFromAudioSource;

	public VibrationData VibrationData
	{
		get
		{
			return data;
		}
		set
		{
			if (data.LowFidelityVibration != value.LowFidelityVibration || data.HighFidelityVibration != value.HighFidelityVibration || data.GamepadVibration != value.GamepadVibration)
			{
				data = value;
				Stop();
			}
		}
	}

	public VibrationTarget Target
	{
		get
		{
			return target;
		}
		set
		{
			if (target.Motors != value.Motors)
			{
				target = value;
				if (emission != null)
				{
					emission.Target = target;
				}
			}
		}
	}

	public bool PlayAutomatically
	{
		get
		{
			return playAutomatically;
		}
		set
		{
			playAutomatically = value;
		}
	}

	public bool IsLooping
	{
		get
		{
			return isLooping;
		}
		set
		{
			isLooping = value;
			if (emission != null)
			{
				emission.IsLooping = isLooping;
			}
		}
	}

	public string VibrationTag
	{
		get
		{
			return vibrationTag;
		}
		set
		{
			vibrationTag = value;
			if (emission != null)
			{
				emission.Tag = vibrationTag;
			}
		}
	}

	protected void OnEnable()
	{
		if ((bool)audioSource)
		{
			tryPlayFromAudioSource = true;
		}
		if ((bool)vibrationDataAsset)
		{
			data = vibrationDataAsset;
		}
		if (playAutomatically && !ObjectPool.IsCreatingPool)
		{
			Play();
		}
	}

	protected void OnDisable()
	{
		Stop();
	}

	public void Play()
	{
		if (emission != null)
		{
			emission.Stop();
		}
		emission = VibrationManager.PlayVibrationClipOneShot(data, target, isLooping, vibrationTag, isRealtime);
	}

	public void Stop()
	{
		if (emission != null)
		{
			emission.Stop();
			emission = null;
		}
	}
}

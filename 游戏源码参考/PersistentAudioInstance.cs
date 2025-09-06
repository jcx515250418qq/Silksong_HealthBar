using UnityEngine;

public sealed class PersistentAudioInstance : MonoBehaviour
{
	[SerializeField]
	private string key;

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private float fadeInRate = 1f;

	[SerializeField]
	private float fadeOutRate = 1f;

	[Tooltip("If true, other instances will fade out at same rate that is is fading in.")]
	[SerializeField]
	private bool alsoSetOtherChangeRate;

	[Tooltip("If true, will attach self to camera in new scene so that sound source moves with camera during positioning.")]
	[SerializeField]
	private bool keepRelativePositionInNewScene;

	[Tooltip("If true, relocate any existing instance(s) to this instance's position upon registration.")]
	[SerializeField]
	private bool adoptNewInstancePosition;

	[Tooltip("If true, adopt the previous instance's playing state (including its clip and play position) when registered.")]
	[SerializeField]
	private bool adoptPreviousPlayingState;

	private bool hasVolumeBlendController;

	private VolumeBlendController volumeBlendController;

	private VolumeModifier modifier;

	private bool destroyedQueued;

	private bool shouldUpdate;

	private float targetVolume = 1f;

	private float instanceVolume = 1f;

	private bool hasAudioSource;

	private bool trySync;

	private AudioSource syncTarget;

	public string Key => key;

	public float FadeInRate => fadeInRate;

	public float FadeOutRate => fadeOutRate;

	public bool AlsoSetOtherChangeRate => alsoSetOtherChangeRate;

	public AudioSource AudioSource => audioSource;

	public bool KeepRelativePositionInNewScene => keepRelativePositionInNewScene;

	public bool AdoptNewInstancePosition => adoptNewInstancePosition;

	public bool AdoptPreviousPlayingState => adoptPreviousPlayingState;

	public bool IsFromPreviousScene { get; set; }

	private void Awake()
	{
		hasAudioSource = audioSource != null;
		if (!hasAudioSource)
		{
			audioSource = base.gameObject.GetComponent<AudioSource>();
			hasAudioSource = audioSource != null;
			if (!hasAudioSource)
			{
				return;
			}
		}
		if (string.IsNullOrEmpty(key))
		{
			key = "PersistentAudioInstance";
		}
		volumeBlendController = audioSource.GetComponent<VolumeBlendController>();
		hasVolumeBlendController = volumeBlendController != null;
		if (hasVolumeBlendController)
		{
			modifier = volumeBlendController.GetModifier("PersistentAudioInstance");
		}
		else
		{
			targetVolume = audioSource.volume;
		}
		PersistentAudioManager.AddInstance(this);
	}

	private void Start()
	{
		base.transform.SetParent(null);
		Object.DontDestroyOnLoad(base.gameObject);
		shouldUpdate = true;
	}

	private void OnValidate()
	{
		if (audioSource == null)
		{
			audioSource = base.gameObject.GetComponent<AudioSource>();
		}
	}

	private void OnDestroy()
	{
		PersistentAudioManager.RemoveInstance(this);
	}

	public void UpdateVolume()
	{
		if (!shouldUpdate && !destroyedQueued)
		{
			return;
		}
		if (!hasAudioSource)
		{
			if (destroyedQueued)
			{
				Object.Destroy(base.gameObject);
			}
			return;
		}
		if (trySync)
		{
			trySync = false;
			if (syncTarget != null)
			{
				if (audioSource.clip == syncTarget.clip)
				{
					audioSource.timeSamples = syncTarget.timeSamples;
				}
				syncTarget = null;
			}
		}
		float num = (destroyedQueued ? fadeOutRate : fadeInRate);
		instanceVolume = Mathf.MoveTowards(instanceVolume, targetVolume, Time.deltaTime * num);
		SetVolume(instanceVolume);
		if (instanceVolume == targetVolume)
		{
			if (destroyedQueued)
			{
				Object.Destroy(base.gameObject);
			}
			shouldUpdate = false;
		}
	}

	private void SetVolume(float volume)
	{
		if (hasVolumeBlendController)
		{
			modifier.Volume = volume;
		}
		else
		{
			audioSource.volume = volume;
		}
	}

	private float GetCurrentVolume()
	{
		if (hasVolumeBlendController)
		{
			return modifier.Volume;
		}
		return audioSource.volume;
	}

	public void SetChangeRate(float changeRate)
	{
		fadeInRate = changeRate;
	}

	public void SetTargetVolume(float targetVolume)
	{
		targetVolume = Mathf.Clamp01(targetVolume);
		if (this.targetVolume != targetVolume)
		{
			this.targetVolume = targetVolume;
			shouldUpdate = true;
		}
	}

	public void MarkForDestroy()
	{
		if (IsFromPreviousScene && !destroyedQueued)
		{
			destroyedQueued = true;
			targetVolume = 0f;
			shouldUpdate = true;
		}
	}

	public void QueueFadeUp()
	{
		instanceVolume = 0f;
		SetVolume(0f);
		shouldUpdate = true;
	}

	public void SetSyncTarget(AudioSource audioSource)
	{
		syncTarget = audioSource;
		trySync = syncTarget != null;
	}
}

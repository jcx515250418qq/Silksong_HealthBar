using UnityEngine;

public sealed class TransitionAudioFader : MonoBehaviour
{
	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private bool ignoreAudioSource;

	[SerializeField]
	private bool ignoreFadeOutOnLevelUnload;

	[Space]
	[SerializeField]
	private float transitionRate = 4f;

	[Range(0f, 1f)]
	[SerializeField]
	private float targetVolume = 1f;

	[Range(0f, 1f)]
	[SerializeField]
	private float minVolume;

	[Range(0f, 1f)]
	[SerializeField]
	private float maxVolume = 1f;

	private static UniqueList<TransitionAudioFader> uniqueList = new UniqueList<TransitionAudioFader>();

	private VolumeModifier volumeModifier;

	private bool hasAudioSource;

	private bool registeredEvents;

	private GameManager gm;

	public float Volume { get; private set; }

	private void Awake()
	{
		if (!ignoreAudioSource)
		{
			hasAudioSource = audioSource != null;
			if (audioSource == null)
			{
				audioSource = base.gameObject.GetComponent<AudioSource>();
				hasAudioSource = audioSource != null;
				_ = (bool)audioSource;
			}
		}
		gm = GameManager.instance;
		VolumeBlendController component = base.gameObject.GetComponent<VolumeBlendController>();
		if ((bool)component)
		{
			volumeModifier = component.GetModifier("TransitionAudioFader");
		}
		if (gm != null && (gm.IsInSceneTransition || gm.IsLoadingSceneTransition))
		{
			float num2 = (Volume = minVolume);
			targetVolume = num2;
			SetVolume(minVolume);
		}
		RegisterEvents();
		uniqueList.Add(this);
	}

	private void OnValidate()
	{
		if (audioSource == null)
		{
			audioSource = base.gameObject.GetComponent<AudioSource>();
		}
		if (audioSource != null && !Application.isPlaying)
		{
			targetVolume = audioSource.volume;
		}
	}

	private void OnDestroy()
	{
		UnregisterEvents();
		uniqueList.Remove(this);
	}

	private void Update()
	{
		Volume = Mathf.MoveTowards(Volume, targetVolume, transitionRate * Time.deltaTime);
		SetVolume(Volume);
		base.enabled = Volume != targetVolume;
	}

	private void RegisterEvents()
	{
		if (!registeredEvents)
		{
			registeredEvents = true;
			AudioManager.OnAppliedActorSnapshot += OnAppliedActorSnapshot;
			if (!ignoreFadeOutOnLevelUnload && gm != null)
			{
				gm.UnloadingLevel += OnUnloadingLevel;
			}
		}
	}

	private void UnregisterEvents()
	{
		if (registeredEvents)
		{
			registeredEvents = false;
			AudioManager.OnAppliedActorSnapshot -= OnAppliedActorSnapshot;
			if (!ignoreFadeOutOnLevelUnload && gm != null)
			{
				gm.UnloadingLevel -= OnUnloadingLevel;
			}
		}
	}

	private void OnAppliedActorSnapshot()
	{
		targetVolume = Mathf.Clamp01(maxVolume);
		base.enabled = Volume != targetVolume;
	}

	private void OnUnloadingLevel()
	{
		targetVolume = (targetVolume = Mathf.Clamp01(minVolume));
		base.enabled = Volume != targetVolume;
	}

	public static void FadeOutAllFaders()
	{
		uniqueList.ReserveListUsage();
		foreach (TransitionAudioFader item in uniqueList.List)
		{
			item.OnUnloadingLevel();
		}
		uniqueList.ReleaseListUsage();
	}

	private void SetVolume(float volume)
	{
		if (volumeModifier != null)
		{
			volumeModifier.Volume = volume;
		}
		else if (hasAudioSource)
		{
			audioSource.volume = volume;
		}
	}
}

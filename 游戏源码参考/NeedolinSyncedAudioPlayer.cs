using System;
using UnityEngine;

public sealed class NeedolinSyncedAudioPlayer : MonoBehaviour
{
	[Flags]
	private enum StateFlags
	{
		None = 0,
		Fading = 1,
		Stopping = 2
	}

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private AudioSource syncTargetAudioSource;

	[Header("On Needolin Start")]
	[SerializeField]
	private bool syncPlayTimeOnStart;

	[SerializeField]
	private bool copyClipOnStart;

	[SerializeField]
	private bool playOnStart;

	[Space]
	[SerializeField]
	private bool fadeOnStart;

	[SerializeField]
	private float startFadeVolume;

	[SerializeField]
	private float startFadeRate = 1f;

	[Header("On Needolin End")]
	[SerializeField]
	private bool stopOnEnd;

	[SerializeField]
	private float stopDelay;

	[Space]
	[SerializeField]
	private bool fadeOnEnd;

	[SerializeField]
	private float endFadeVolume;

	[SerializeField]
	private float endFadeRate = 1f;

	private VolumeModifier volumeModifier;

	private bool hasAudioSource;

	private bool hasTarget;

	private float targetVolume;

	private float fadeRate;

	private float stopTimer;

	private StateFlags state;

	private void Awake()
	{
		HeroPerformanceRegion.StartedPerforming += OnStartedPerforming;
		HeroPerformanceRegion.StoppedPerforming += OnStoppedPerforming;
		hasAudioSource = audioSource != null;
		if (!hasAudioSource)
		{
			audioSource = base.gameObject.GetComponent<AudioSource>();
			hasAudioSource = audioSource != null;
		}
		VolumeBlendController component = GetComponent<VolumeBlendController>();
		if ((bool)component)
		{
			volumeModifier = component.GetModifier("NeedolinSyncedAudioPlayer");
			volumeModifier.Volume = component.InitialVolume;
		}
		hasTarget = syncTargetAudioSource != null;
		base.enabled = false;
	}

	private void OnDestroy()
	{
		HeroPerformanceRegion.StartedPerforming -= OnStartedPerforming;
		HeroPerformanceRegion.StoppedPerforming -= OnStoppedPerforming;
	}

	private void OnValidate()
	{
		if (audioSource == null)
		{
			audioSource = GetComponent<AudioSource>();
		}
	}

	private void LateUpdate()
	{
		if (state.HasFlag(StateFlags.Fading))
		{
			float volume = GetVolume();
			float num = Mathf.MoveTowards(volume, targetVolume, fadeRate * Time.deltaTime);
			SetVolume(num);
			if (Mathf.Approximately(volume, num))
			{
				state &= ~StateFlags.Fading;
			}
		}
		if (!state.HasFlag(StateFlags.Stopping))
		{
			return;
		}
		stopTimer -= Time.deltaTime;
		if (stopTimer <= 0f && !state.HasFlag(StateFlags.Fading))
		{
			if (hasAudioSource)
			{
				audioSource.Stop();
			}
			state &= ~StateFlags.Stopping;
		}
	}

	private void UpdateEnabledState()
	{
		base.enabled = hasAudioSource && state != StateFlags.None;
	}

	public void Play()
	{
		state &= ~StateFlags.Stopping;
		if (copyClipOnStart)
		{
			CopyClip();
		}
		if (syncPlayTimeOnStart)
		{
			SyncTime();
		}
		if (playOnStart && hasAudioSource && !audioSource.isPlaying && (!hasTarget || syncTargetAudioSource.isPlaying))
		{
			audioSource.Play();
		}
	}

	public void Stop()
	{
		if (hasAudioSource)
		{
			audioSource.Stop();
			audioSource.clip = null;
		}
	}

	private void OnStartedPerforming()
	{
		state &= ~StateFlags.Stopping;
		if (copyClipOnStart)
		{
			CopyClip();
		}
		if (syncPlayTimeOnStart)
		{
			SyncTime();
		}
		if (playOnStart && hasAudioSource && !audioSource.isPlaying && (!hasTarget || syncTargetAudioSource.isPlaying))
		{
			audioSource.Play();
		}
		if (fadeOnStart)
		{
			targetVolume = Mathf.Clamp01(startFadeVolume);
			fadeRate = startFadeRate;
			state |= StateFlags.Fading;
		}
		UpdateEnabledState();
	}

	private void OnStoppedPerforming()
	{
		if (stopOnEnd)
		{
			if (stopDelay <= 0f && !fadeOnEnd)
			{
				state = StateFlags.None;
				if (hasAudioSource)
				{
					audioSource.Stop();
				}
				base.enabled = false;
				return;
			}
			state |= StateFlags.Stopping;
			stopTimer = stopDelay;
		}
		if (fadeOnEnd)
		{
			state |= StateFlags.Fading;
			targetVolume = Mathf.Clamp01(endFadeVolume);
			fadeRate = endFadeRate;
		}
		UpdateEnabledState();
	}

	private void CopyClip()
	{
		if (hasAudioSource && hasTarget && audioSource.clip != syncTargetAudioSource.clip)
		{
			audioSource.clip = syncTargetAudioSource.clip;
		}
	}

	private void SyncTime()
	{
		if (hasAudioSource && hasTarget && audioSource.clip == syncTargetAudioSource.clip)
		{
			audioSource.timeSamples = syncTargetAudioSource.timeSamples;
		}
	}

	public void SetTarget(AudioSource target)
	{
		syncTargetAudioSource = target;
		hasTarget = target != null;
	}

	private float GetVolume()
	{
		if (volumeModifier != null)
		{
			return volumeModifier.Volume;
		}
		if (hasAudioSource)
		{
			return audioSource.volume;
		}
		return 1f;
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

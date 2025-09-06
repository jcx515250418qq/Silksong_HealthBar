using System;
using UnityEngine;

public class FadeAudioOnAwake : MonoBehaviour
{
	[Serializable]
	private enum FadeType
	{
		FadeOut = 0,
		FadeIn = 1
	}

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private FadeType fadeType;

	[SerializeField]
	private float fadeRate = 2f;

	[SerializeField]
	private bool setVolume;

	[SerializeField]
	private float startVolume;

	[SerializeField]
	private float targetVolume = 1f;

	[SerializeField]
	private bool useInitialVolumeAsTarget;

	[SerializeField]
	private bool waitForHeroInPosition;

	private bool hasBlend;

	private VolumeBlendController volumeBlendController;

	private VolumeModifier modifier;

	private bool waitingForHero;

	private HeroController hc;

	private void Awake()
	{
		if (audioSource == null)
		{
			audioSource = base.gameObject.GetComponent<AudioSource>();
			if (audioSource == null)
			{
				base.enabled = false;
				return;
			}
		}
		volumeBlendController = base.gameObject.GetComponent<VolumeBlendController>();
		if (volumeBlendController != null)
		{
			hasBlend = true;
			modifier = volumeBlendController.GetModifier("FadeAudioOnAwake");
		}
		if (useInitialVolumeAsTarget)
		{
			targetVolume = audioSource.volume;
		}
		if (setVolume)
		{
			SetVolume(startVolume);
		}
		targetVolume = Mathf.Clamp01(targetVolume);
		if (waitForHeroInPosition)
		{
			waitingForHero = true;
			hc = HeroController.instance;
		}
	}

	private void OnValidate()
	{
		if (audioSource != null)
		{
			audioSource = audioSource.GetComponent<AudioSource>();
		}
	}

	private void Update()
	{
		if (waitingForHero)
		{
			if (!hc.isHeroInPosition)
			{
				return;
			}
			waitingForHero = false;
		}
		float num = Mathf.MoveTowards(audioSource.volume, targetVolume, fadeRate * Time.deltaTime);
		SetVolume(num);
		if (num == targetVolume)
		{
			base.enabled = false;
		}
	}

	[ContextMenu("Record Target Volume")]
	private void RecordTargetVolume()
	{
		useInitialVolumeAsTarget = false;
		if (audioSource != null)
		{
			audioSource = audioSource.GetComponent<AudioSource>();
		}
		if (audioSource != null)
		{
			targetVolume = audioSource.volume;
		}
	}

	private void SetVolume(float volume)
	{
		if (hasBlend && modifier.IsValid)
		{
			modifier.Volume = volume;
		}
		else
		{
			audioSource.volume = volume;
		}
	}

	private float GetVolume()
	{
		if (hasBlend && modifier.IsValid)
		{
			return modifier.Volume;
		}
		return audioSource.volume;
	}
}

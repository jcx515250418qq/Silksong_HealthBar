using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AtmosRegion : TrackTriggerObjects
{
	[Space]
	[SerializeField]
	private int delay;

	[SerializeField]
	private AtmosCue enterAtmosCue;

	[SerializeField]
	private AudioMixerSnapshot enterAtmosSnapshot;

	[SerializeField]
	private float enterTransitionTime;

	[Space]
	[SerializeField]
	private AtmosCue exitAtmosCue;

	[SerializeField]
	private AudioMixerSnapshot exitAtmosSnapshot;

	[SerializeField]
	private float exitTransitionTime;

	private Coroutine fadeInRoutine;

	protected override void Awake()
	{
		base.Awake();
		if (base.gameObject.layer != 13)
		{
			base.gameObject.layer = 13;
		}
	}

	protected override void OnInsideStateChanged(bool isInside)
	{
		if (isInside)
		{
			if (fadeInRoutine != null)
			{
				StopCoroutine(fadeInRoutine);
			}
			fadeInRoutine = StartCoroutine(FadeIn());
		}
		else
		{
			if (fadeInRoutine != null)
			{
				StopCoroutine(fadeInRoutine);
			}
			FadeOut();
		}
	}

	private IEnumerator FadeIn()
	{
		if (delay > 0)
		{
			yield return new WaitForSeconds(delay);
		}
		GameManager instance = GameManager.instance;
		if (enterAtmosSnapshot != null)
		{
			AudioManager.TransitionToAtmosOverride(enterAtmosSnapshot, enterTransitionTime);
		}
		if ((bool)enterAtmosCue)
		{
			instance.AudioManager.ApplyAtmosCue(enterAtmosCue, enterTransitionTime);
		}
		fadeInRoutine = null;
	}

	private void FadeOut()
	{
		GameManager silentInstance = GameManager.SilentInstance;
		if ((bool)silentInstance)
		{
			if (exitAtmosSnapshot != null)
			{
				AudioManager.TransitionToAtmosOverride(enterAtmosSnapshot, exitTransitionTime);
			}
			if ((bool)exitAtmosCue)
			{
				silentInstance.AudioManager.ApplyAtmosCue(exitAtmosCue, exitTransitionTime);
			}
		}
	}
}

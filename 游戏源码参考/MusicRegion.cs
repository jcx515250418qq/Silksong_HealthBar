using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class MusicRegion : TrackTriggerObjects
{
	[Space]
	public int delay;

	public MusicCue enterMusicCue;

	public AudioMixerSnapshot enterMusicSnapshot;

	public float enterTransitionTime;

	[Space]
	public MusicCue exitMusicCue;

	public AudioMixerSnapshot exitMusicSnapshot;

	public float exitTransitionTime;

	private Coroutine fadeInRoutine;

	protected override void Awake()
	{
		base.Awake();
		if (base.gameObject.layer != 13)
		{
			base.gameObject.layer = 13;
		}
		if ((bool)enterMusicCue)
		{
			enterMusicCue.Preload(base.gameObject);
		}
		if ((bool)exitMusicCue)
		{
			exitMusicCue.Preload(base.gameObject);
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
		float transitionTime = enterTransitionTime;
		if (enterMusicSnapshot != null)
		{
			instance.AudioManager.ApplyMusicSnapshot(enterMusicSnapshot, 0f, transitionTime);
		}
		if ((bool)enterMusicCue)
		{
			instance.AudioManager.ApplyMusicCue(enterMusicCue, 0f, 0f, applySnapshot: false);
		}
		fadeInRoutine = null;
	}

	private void FadeOut()
	{
		GameManager silentInstance = GameManager.SilentInstance;
		if ((bool)silentInstance)
		{
			if (exitMusicSnapshot != null)
			{
				silentInstance.AudioManager.ApplyMusicSnapshot(exitMusicSnapshot, 0f, exitTransitionTime);
			}
			if ((bool)exitMusicCue)
			{
				silentInstance.AudioManager.ApplyMusicCue(exitMusicCue, 0f, 0f, applySnapshot: false);
			}
		}
	}
}

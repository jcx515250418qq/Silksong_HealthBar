using System.Collections;
using UnityEngine;

public class SyncNeedolinLoop : MonoBehaviour
{
	[SerializeField]
	private AudioSource source;

	private Coroutine playRoutine;

	private bool isPlaying;

	private void Reset()
	{
		source = GetComponent<AudioSource>();
	}

	private void OnEnable()
	{
		HeroPerformanceRegion.StartedPerforming += OnNeedolinStarted;
		HeroPerformanceRegion.StoppedPerforming += OnNeedolinStopped;
		if (HeroPerformanceRegion.IsPerforming)
		{
			OnNeedolinStarted();
		}
	}

	private void OnDisable()
	{
		HeroPerformanceRegion.StartedPerforming -= OnNeedolinStarted;
		HeroPerformanceRegion.StoppedPerforming -= OnNeedolinStopped;
		if (playRoutine != null)
		{
			StopCoroutine(playRoutine);
			playRoutine = null;
		}
	}

	private void OnNeedolinStarted()
	{
		isPlaying = true;
		if (playRoutine == null)
		{
			playRoutine = StartCoroutine(PlayAudioInSync());
		}
	}

	private void OnNeedolinStopped()
	{
		isPlaying = false;
	}

	private IEnumerator PlayAudioInSync()
	{
		HeroController instance = HeroController.instance;
		AudioSource needolinSource = instance.transform.Find("Sounds").Find("Needolin").GetComponent<AudioSource>();
		while (!needolinSource.isPlaying && isPlaying)
		{
			yield return null;
		}
		source.Play();
		source.timeSamples = needolinSource.timeSamples;
		while (isPlaying)
		{
			yield return null;
		}
		source.Stop();
		playRoutine = null;
	}
}

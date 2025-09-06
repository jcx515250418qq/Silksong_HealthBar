using System.Collections;
using TeamCherry.SharedUtils;
using UnityEngine;

public class InventoryPaneFollowAudio : MonoBehaviour
{
	[SerializeField]
	private InventoryPaneList paneList;

	[SerializeField]
	private InventoryPane mainPane;

	[SerializeField]
	private SavedItem itemCondition;

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private AudioLowPassFilter lowPassFilter;

	[SerializeField]
	private MinMaxFloat lowPassMapToRange;

	[SerializeField]
	private AudioClip mainPaneClip;

	[SerializeField]
	private AudioClip offPaneClip;

	[SerializeField]
	private float backgroundVolume;

	[SerializeField]
	private float paneFadeTime;

	[SerializeField]
	private float openCloseFadeTime;

	[SerializeField]
	private EventRegister stopAudioEvent;

	[SerializeField]
	private EventRegister startAudioEvent;

	private bool isActive;

	private int currentPaneIndex;

	private bool isStopped;

	private Coroutine fadeRoutine;

	private void Awake()
	{
		paneList.OpeningInventory += delegate
		{
			if (!itemCondition || itemCondition.GetSavedAmount() > 0)
			{
				isActive = true;
				OnMovedPaneIndex(currentPaneIndex, isOpening: true);
			}
		};
		paneList.MovedPaneIndex += delegate(int index)
		{
			OnMovedPaneIndex(index, isOpening: false);
		};
		paneList.ClosingInventory += delegate
		{
			isActive = false;
			isStopped = false;
			if (fadeRoutine != null)
			{
				StopCoroutine(fadeRoutine);
			}
			fadeRoutine = StartCoroutine(FadeOut());
		};
		if ((bool)stopAudioEvent)
		{
			stopAudioEvent.ReceivedEvent += delegate
			{
				isStopped = true;
				if (fadeRoutine != null)
				{
					StopCoroutine(fadeRoutine);
				}
				audioSource.Stop();
			};
		}
		if ((bool)startAudioEvent)
		{
			startAudioEvent.ReceivedEvent += delegate
			{
				isStopped = false;
				OnMovedPaneIndex(currentPaneIndex, isOpening: true);
			};
		}
	}

	private void OnMovedPaneIndex(int currentIndex, bool isOpening)
	{
		if (isStopped || (!isOpening && currentPaneIndex == currentIndex))
		{
			return;
		}
		currentPaneIndex = currentIndex;
		if (!isActive)
		{
			return;
		}
		int paneIndex = paneList.GetPaneIndex(mainPane);
		int totalPaneCount = paneList.TotalPaneCount;
		AudioClip newClip;
		float lowPassT;
		if (currentIndex == paneIndex)
		{
			newClip = mainPaneClip;
			lowPassT = 0f;
		}
		else
		{
			newClip = offPaneClip;
			lowPassT = 1f;
		}
		int closestOffsetToIndex = Helper.GetClosestOffsetToIndex(paneIndex, currentIndex, totalPaneCount);
		float panStereo;
		float volume;
		if (closestOffsetToIndex <= 1 && closestOffsetToIndex >= -1)
		{
			if (closestOffsetToIndex == 0)
			{
				panStereo = 0f;
				volume = 1f;
			}
			else
			{
				panStereo = ((closestOffsetToIndex > 0) ? 1 : (-1));
				volume = 1f;
			}
		}
		else
		{
			panStereo = 0f;
			volume = backgroundVolume;
		}
		if (fadeRoutine != null)
		{
			StopCoroutine(fadeRoutine);
		}
		fadeRoutine = StartCoroutine(FadeTo(panStereo, newClip, lowPassT, volume, isOpening));
	}

	private IEnumerator FadeTo(float panStereo, AudioClip newClip, float lowPassT, float volume, bool isOpening)
	{
		bool clipChanged = isOpening || audioSource.clip != newClip;
		float initialVolume = audioSource.volume;
		float initialPanStereo = audioSource.panStereo;
		float initialCutoffFreq = lowPassFilter.cutoffFrequency;
		float targetCutoffFreq = GetCutoffFrequency(lowPassT);
		float fadeUpTime;
		if (isOpening)
		{
			audioSource.volume = 0f;
			fadeUpTime = openCloseFadeTime;
		}
		else
		{
			for (float elapsed = 0f; elapsed < paneFadeTime; elapsed += Time.unscaledDeltaTime)
			{
				float t = elapsed / paneFadeTime;
				audioSource.panStereo = Mathf.Lerp(initialPanStereo, panStereo, t);
				lowPassFilter.cutoffFrequency = Mathf.Lerp(initialCutoffFreq, targetCutoffFreq, t);
				audioSource.volume = (clipChanged ? Mathf.Lerp(initialVolume, 0f, t) : Mathf.Lerp(initialVolume, volume, t));
				yield return null;
			}
			fadeUpTime = paneFadeTime;
		}
		audioSource.panStereo = panStereo;
		lowPassFilter.cutoffFrequency = targetCutoffFreq;
		if (clipChanged)
		{
			audioSource.clip = newClip;
			audioSource.Play();
			audioSource.timeSamples = Random.Range(0, newClip.samples);
			for (float elapsed = 0f; elapsed < fadeUpTime; elapsed += Time.unscaledDeltaTime)
			{
				float t2 = elapsed / fadeUpTime;
				audioSource.volume = Mathf.Lerp(0f, volume, t2);
				yield return null;
			}
		}
	}

	private IEnumerator FadeOut()
	{
		float initialVolume = audioSource.volume;
		for (float elapsed = 0f; elapsed < openCloseFadeTime; elapsed += Time.unscaledDeltaTime)
		{
			float t = elapsed / openCloseFadeTime;
			audioSource.volume = Mathf.Lerp(initialVolume, 0f, t);
			yield return null;
		}
		audioSource.volume = 0f;
		audioSource.Stop();
	}

	private float GetCutoffFrequency(float t)
	{
		float t2 = audioSource.GetCustomCurve(AudioSourceCurveType.CustomRolloff).Evaluate(t);
		return lowPassMapToRange.GetLerpedValue(t2);
	}
}

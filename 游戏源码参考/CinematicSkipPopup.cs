using System;
using System.Collections;
using UnityEngine;

public class CinematicSkipPopup : MonoBehaviour
{
	public enum Texts
	{
		Skip = 0,
		Loading = 1
	}

	[SerializeField]
	[ArrayForEnum(typeof(Texts))]
	private GameObject[] textGroups;

	[SerializeField]
	private float fadeInDuration;

	[SerializeField]
	private float fadeOutDuration;

	private float showTimer;

	private Coroutine fadeRoutine;

	private CanvasGroup canvasGroup;

	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize(ref textGroups, typeof(Texts));
	}

	private void Awake()
	{
		canvasGroup = GetComponent<CanvasGroup>();
	}

	public void Show(Texts text)
	{
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: true);
			canvasGroup.alpha = 0f;
		}
		for (int i = 0; i < textGroups.Length; i++)
		{
			textGroups[i].SetActive(i == (int)text);
		}
		if (fadeRoutine != null)
		{
			StopCoroutine(fadeRoutine);
		}
		fadeRoutine = StartCoroutine(FadeToAlpha(1f, fadeInDuration, disableOnEnd: false, null));
	}

	public void Hide(bool isInstant, Action onEnd)
	{
		if (fadeRoutine != null)
		{
			StopCoroutine(fadeRoutine);
		}
		if (isInstant)
		{
			onEnd?.Invoke();
			base.gameObject.SetActive(value: false);
		}
		else
		{
			fadeRoutine = StartCoroutine(FadeToAlpha(0f, fadeOutDuration, disableOnEnd: true, onEnd));
		}
	}

	private IEnumerator FadeToAlpha(float targetAlpha, float duration, bool disableOnEnd, Action onEnd)
	{
		float elapsed = 0f;
		float startAlpha = canvasGroup.alpha;
		for (; elapsed < duration; elapsed += Time.unscaledDeltaTime)
		{
			canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
			yield return null;
		}
		onEnd?.Invoke();
		if (disableOnEnd)
		{
			base.gameObject.SetActive(value: false);
		}
		else
		{
			canvasGroup.alpha = targetAlpha;
		}
	}
}

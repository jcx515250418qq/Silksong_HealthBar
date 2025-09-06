using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskerBlackout : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer spriteRenderer;

	private Coroutine fadeRoutine;

	private float lastValue;

	private static readonly HashSet<MaskerBlackout> _activeBlackouts = new HashSet<MaskerBlackout>();

	private static readonly HashSet<Object> _insideMasks = new HashSet<Object>();

	public static bool IsAnyFading
	{
		get
		{
			foreach (MaskerBlackout activeBlackout in _activeBlackouts)
			{
				if (activeBlackout.fadeRoutine != null)
				{
					return true;
				}
			}
			return false;
		}
	}

	private void OnEnable()
	{
		spriteRenderer.enabled = true;
		_activeBlackouts.Add(this);
	}

	private void OnDisable()
	{
		_activeBlackouts.Remove(this);
	}

	public static bool AddInside(Object blackoutMask, float fadeInTime)
	{
		if (_insideMasks.Add(blackoutMask) && _insideMasks.Count == 1)
		{
			StartMaskFade(1f, fadeInTime);
			return true;
		}
		return false;
	}

	public static bool RemoveInside(Object blackoutMask, float fadeOutTime)
	{
		if (_insideMasks.Remove(blackoutMask) && _insideMasks.Count == 0)
		{
			StartMaskFade(0f, fadeOutTime);
			return true;
		}
		return false;
	}

	public static void SetMaskFade(float value)
	{
		foreach (MaskerBlackout activeBlackout in _activeBlackouts)
		{
			if ((bool)activeBlackout)
			{
				if (activeBlackout.fadeRoutine != null)
				{
					activeBlackout.StopCoroutine(activeBlackout.fadeRoutine);
				}
				activeBlackout.fadeRoutine = null;
				activeBlackout.SetMaskValue(value);
			}
		}
	}

	private static void StartMaskFade(float value, float time)
	{
		foreach (MaskerBlackout activeBlackout in _activeBlackouts)
		{
			if ((bool)activeBlackout)
			{
				if (activeBlackout.fadeRoutine != null)
				{
					activeBlackout.StopCoroutine(activeBlackout.fadeRoutine);
				}
				if (time > 0f)
				{
					activeBlackout.fadeRoutine = activeBlackout.StartCoroutine(activeBlackout.MaskFade(value, time));
					continue;
				}
				activeBlackout.fadeRoutine = null;
				activeBlackout.SetMaskValue(value);
			}
		}
	}

	private IEnumerator MaskFade(float value, float time)
	{
		float elapsed = 0f;
		float startValue = lastValue;
		for (; elapsed < time; elapsed += Time.deltaTime)
		{
			float t = elapsed / time;
			SetMaskValue(Mathf.Lerp(startValue, value, t));
			yield return null;
		}
		SetMaskValue(value);
		fadeRoutine = null;
	}

	private void SetMaskValue(float value)
	{
		lastValue = value;
		Color color = spriteRenderer.color;
		color.a = value;
		spriteRenderer.color = color;
	}
}

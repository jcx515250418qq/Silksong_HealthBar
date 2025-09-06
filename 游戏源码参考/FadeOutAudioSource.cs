using System;
using UnityEngine;

public class FadeOutAudioSource : MonoBehaviour
{
	public enum EndBehaviours
	{
		None = 0,
		Disable = 1,
		Recycle = 2
	}

	[SerializeField]
	private bool resetVolumeOnEnable;

	[SerializeField]
	private EndBehaviours endBehaviour;

	private Coroutine fadeRoutine;

	private float resetVolume;

	private bool hasStarted;

	private float startVolume;

	private AudioSource source;

	private void Awake()
	{
		source = GetComponent<AudioSource>();
	}

	private void Start()
	{
		hasStarted = true;
		resetVolume = (source ? source.volume : 0f);
	}

	private void OnEnable()
	{
		if (hasStarted && resetVolumeOnEnable && (bool)source)
		{
			source.volume = resetVolume;
		}
	}

	private void OnDisable()
	{
		Cancel();
	}

	public void Cancel()
	{
		if (fadeRoutine != null)
		{
			StopCoroutine(fadeRoutine);
			OnFadeEnd();
		}
	}

	public void StartFade(float duration)
	{
		StartFade(duration, endBehaviour);
	}

	public void StartFade(float duration, EndBehaviours endBehaviourOverride)
	{
		if ((bool)source && base.isActiveAndEnabled)
		{
			if (fadeRoutine != null)
			{
				StopCoroutine(fadeRoutine);
			}
			startVolume = source.volume;
			endBehaviour = endBehaviourOverride;
			fadeRoutine = this.StartTimerRoutine(0f, duration, OnFade, null, OnFadeEnd);
		}
	}

	private void OnFade(float t)
	{
		if ((bool)source)
		{
			source.volume = Mathf.Lerp(startVolume, 0f, t);
		}
	}

	private void OnFadeEnd()
	{
		if ((bool)source)
		{
			source.Stop();
		}
		fadeRoutine = null;
		switch (endBehaviour)
		{
		case EndBehaviours.Disable:
			base.gameObject.SetActive(value: false);
			break;
		case EndBehaviours.Recycle:
			base.gameObject.Recycle();
			break;
		default:
			throw new ArgumentOutOfRangeException();
		case EndBehaviours.None:
			break;
		}
	}
}

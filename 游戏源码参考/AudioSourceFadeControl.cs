using System;
using UnityEngine;

public class AudioSourceFadeControl : MonoBehaviour
{
	private enum State
	{
		Idle = 0,
		FadingDown = 1,
		FadingUp = 2
	}

	public float fadeSpeed = 1f;

	public float volumeUp;

	public bool useStartVolume = true;

	[SerializeField]
	private bool disableAfterFadeDown;

	[SerializeField]
	private bool restartTimeOnEnd;

	[SerializeField]
	private bool finishFadeOutOnDisable;

	private AudioSource audioSource;

	[NonSerialized]
	private State currentState;

	private void Awake()
	{
		audioSource = GetComponent<AudioSource>();
		if (useStartVolume)
		{
			volumeUp = audioSource.volume;
		}
	}

	private void OnDisable()
	{
		if (restartTimeOnEnd)
		{
			audioSource.Stop();
			audioSource.time = 0f;
		}
		if (finishFadeOutOnDisable && currentState == State.FadingDown)
		{
			audioSource.volume = 0f;
		}
	}

	private void Update()
	{
		switch (currentState)
		{
		case State.FadingUp:
			audioSource.volume += Time.deltaTime * fadeSpeed;
			if (audioSource.volume >= volumeUp)
			{
				audioSource.volume = volumeUp;
				EndFade();
			}
			break;
		case State.FadingDown:
			audioSource.volume -= Time.deltaTime * fadeSpeed;
			if (audioSource.volume <= 0f)
			{
				audioSource.volume = 0f;
				EndFade();
				if (disableAfterFadeDown)
				{
					base.gameObject.SetActive(value: false);
				}
				if (restartTimeOnEnd)
				{
					audioSource.time = 0f;
				}
			}
			break;
		}
	}

	private void EndFade()
	{
		currentState = State.Idle;
	}

	public void FadeUp()
	{
		currentState = State.FadingUp;
	}

	public void FadeDown()
	{
		currentState = State.FadingDown;
	}
}

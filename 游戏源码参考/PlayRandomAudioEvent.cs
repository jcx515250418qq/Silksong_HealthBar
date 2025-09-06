using System;
using UnityEngine;
using UnityEngine.Events;

public class PlayRandomAudioEvent : MonoBehaviour
{
	private enum PlayOnSourceMethods
	{
		Play = 0,
		PlayOneShot = 1
	}

	[EnsurePrefab]
	[SerializeField]
	private AudioSource audioSourcePrefab;

	[Space]
	[SerializeField]
	private RandomAudioClipTable table;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("table", true, false, false)]
	private bool forcePlay;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("table", false, false, false)]
	private AudioEventRandom audioEvent;

	[Space]
	[SerializeField]
	private bool playOnEnable;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("playOnEnable", true, false, false)]
	private bool onStartForFirst;

	[SerializeField]
	private bool stopOnDisable;

	[SerializeField]
	private bool stopLastAudioOnPlay;

	[SerializeField]
	private bool useOwnAudio;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("useOwnAudio", true, false, false)]
	private PlayOnSourceMethods playOnSourceMethod;

	[SerializeField]
	private float limitFrequency;

	[SerializeField]
	private bool keepPlayingInNextScene;

	public UnityEvent OnPlayAudio;

	private AudioSource myAudioSource;

	private double nextPlayTime;

	private bool hasStarted;

	private AudioSource spawnedAudioSource;

	private void OnEnable()
	{
		if (useOwnAudio)
		{
			myAudioSource = GetComponent<AudioSource>();
		}
		if (playOnEnable && (!onStartForFirst || hasStarted))
		{
			Play();
		}
	}

	private void Start()
	{
		hasStarted = true;
		if (onStartForFirst && playOnEnable)
		{
			Play();
		}
	}

	private void OnDisable()
	{
		if (stopOnDisable && (bool)spawnedAudioSource)
		{
			spawnedAudioSource.Stop();
			spawnedAudioSource = null;
		}
		if (useOwnAudio)
		{
			nextPlayTime = 0.0;
		}
	}

	public void Play()
	{
		if (ObjectPool.IsCreatingPool || Time.timeAsDouble < nextPlayTime)
		{
			return;
		}
		nextPlayTime = Time.timeAsDouble + (double)limitFrequency;
		if (useOwnAudio)
		{
			if ((bool)myAudioSource)
			{
				if (stopLastAudioOnPlay)
				{
					myAudioSource.Stop();
				}
				if ((bool)table)
				{
					table.PlayOneShot(myAudioSource, forcePlay);
				}
				else
				{
					myAudioSource.pitch = audioEvent.SelectPitch();
					switch (playOnSourceMethod)
					{
					case PlayOnSourceMethods.Play:
						myAudioSource.clip = audioEvent.GetClip();
						myAudioSource.Play();
						break;
					case PlayOnSourceMethods.PlayOneShot:
						myAudioSource.PlayOneShot(audioEvent.GetClip());
						break;
					default:
						throw new ArgumentOutOfRangeException();
					}
					audioEvent.PlayVibrationRandom();
				}
			}
		}
		else
		{
			Action onRecycled = null;
			if (stopOnDisable)
			{
				onRecycled = delegate
				{
					spawnedAudioSource = null;
				};
			}
			if (stopLastAudioOnPlay && spawnedAudioSource != null)
			{
				spawnedAudioSource.Stop();
			}
			if ((bool)table)
			{
				spawnedAudioSource = table.SpawnAndPlayOneShot(audioSourcePrefab, base.transform.position, forcePlay, 1f, onRecycled);
			}
			else
			{
				spawnedAudioSource = audioEvent.SpawnAndPlayOneShot(audioSourcePrefab, base.transform.position, onRecycled);
			}
			if (keepPlayingInNextScene)
			{
				PlayAudioAndRecycle component = spawnedAudioSource.GetComponent<PlayAudioAndRecycle>();
				if ((bool)component)
				{
					component.KeepAliveThroughNextScene = true;
				}
			}
		}
		OnPlayAudio?.Invoke();
	}

	public void StopLast()
	{
		if (!stopOnDisable)
		{
			Debug.LogError("Can't stop event when \"stopOnDisable\" is false");
		}
		else if ((bool)spawnedAudioSource)
		{
			spawnedAudioSource.Stop();
			spawnedAudioSource = null;
		}
	}
}

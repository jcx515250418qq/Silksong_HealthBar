using System;
using UnityEngine;

public class AudioSourceMovingClips : MonoBehaviour
{
	[Serializable]
	private class Clip
	{
		public float SpeedThreshold;

		public AudioClip AudioClip;
	}

	[SerializeField]
	private Transform target;

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private float lerpSpeed;

	[SerializeField]
	private Clip[] clips;

	private float smoothedSpeed;

	private Vector2 previousLocalPos;

	private void OnEnable()
	{
		previousLocalPos = target.localPosition;
		audioSource.volume = 0f;
	}

	private void LateUpdate()
	{
		if (Time.deltaTime <= Mathf.Epsilon)
		{
			return;
		}
		Vector2 vector = target.localPosition;
		float b = (vector - previousLocalPos).magnitude / Time.deltaTime;
		smoothedSpeed = Mathf.Lerp(smoothedSpeed, b, lerpSpeed * Time.deltaTime);
		AudioClip audioClip = null;
		Clip[] array = clips;
		foreach (Clip clip in array)
		{
			if (!(smoothedSpeed < clip.SpeedThreshold))
			{
				audioClip = clip.AudioClip;
			}
		}
		if (audioSource.clip != audioClip)
		{
			bool isPlaying = audioSource.isPlaying;
			audioSource.clip = audioClip;
			if (isPlaying && audioSource.clip != null)
			{
				audioSource.Play();
			}
		}
		previousLocalPos = vector;
	}
}

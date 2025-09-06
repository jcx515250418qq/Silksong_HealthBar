using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioLoopRandom : MonoBehaviour
{
	[Serializable]
	private class ProbabilityAudioClip : Probability.ProbabilityBase<AudioClip>
	{
		[SerializeField]
		private AudioClip clip;

		public override AudioClip Item => clip;
	}

	[SerializeField]
	private bool dontAutoPlay;

	[SerializeField]
	private ProbabilityAudioClip[] setRandomClips;

	private AudioSource source;

	private void Awake()
	{
		source = GetComponent<AudioSource>();
	}

	private void Start()
	{
		PlayRandom(!dontAutoPlay);
	}

	private void PlayRandom(bool doPlay)
	{
		source.loop = true;
		ProbabilityAudioClip[] array = setRandomClips;
		if (array != null && array.Length > 0)
		{
			AudioClip randomItemByProbability = Probability.GetRandomItemByProbability<ProbabilityAudioClip, AudioClip>(setRandomClips);
			source.clip = randomItemByProbability;
		}
		if ((bool)source.clip)
		{
			source.time = UnityEngine.Random.Range(0f, source.clip.length);
			if (!source.isPlaying && doPlay)
			{
				source.Play();
			}
		}
	}

	public void PlayRandom()
	{
		PlayRandom(doPlay: true);
	}
}

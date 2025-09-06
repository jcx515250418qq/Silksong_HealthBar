using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Audio)]
	public class AudioPlayRandom : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		[Tooltip("The GameObject with an AudioSource component.")]
		public FsmGameObject gameObject;

		[CompoundArray("Audio Clips", "Audio Clip", "Weight")]
		public AudioClip[] audioClips;

		[HasFloatSlider(0f, 1f)]
		public FsmFloat[] weights;

		public FsmFloat pitchMin = 1f;

		public FsmFloat pitchMax = 2f;

		private AudioSource audio;

		public override void Reset()
		{
			gameObject = null;
			audioClips = new AudioClip[3];
			weights = new FsmFloat[3] { 1f, 1f, 1f };
			pitchMin = 1f;
			pitchMax = 1f;
		}

		public override void OnEnter()
		{
			DoPlayRandomClip();
			Finish();
		}

		private void DoPlayRandomClip()
		{
			if (audioClips.Length == 0)
			{
				return;
			}
			audio = gameObject.GetSafe<AudioSource>();
			if (!(audio != null))
			{
				return;
			}
			int randomWeightedIndex = ActionHelpers.GetRandomWeightedIndex(weights);
			if (randomWeightedIndex != -1)
			{
				AudioClip audioClip = audioClips[randomWeightedIndex];
				if (audioClip != null)
				{
					float pitch = Random.Range(pitchMin.Value, pitchMax.Value);
					audio.pitch = pitch;
					audio.PlayOneShot(audioClip);
				}
			}
		}
	}
}

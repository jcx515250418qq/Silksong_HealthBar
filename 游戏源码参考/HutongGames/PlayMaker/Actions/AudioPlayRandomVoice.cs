using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Audio)]
	public class AudioPlayRandomVoice : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		[Tooltip("The GameObject with an AudioSource component.")]
		public FsmOwnerDefault gameObject;

		[CompoundArray("Audio Clips", "Audio Clip", "Weight")]
		public AudioClip[] audioClips;

		[HasFloatSlider(0f, 1f)]
		public FsmFloat[] weights;

		public FsmFloat pitchMin = 1f;

		public FsmFloat pitchMax = 2f;

		public bool stopPreviousSound;

		private GameObject self;

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
			self = base.Fsm.GetOwnerDefaultTarget(gameObject);
			DoPlayRandomClip();
			Finish();
		}

		private void DoPlayRandomClip()
		{
			if (audioClips == null || audioClips.Length == 0 || self == null)
			{
				return;
			}
			audio = self.GetComponent<AudioSource>();
			if (stopPreviousSound)
			{
				audio.Stop();
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

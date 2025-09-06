using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Audio)]
	public class PlayOneShotOnAudioSource : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		[Tooltip("The GameObject with an AudioSource component.")]
		public FsmOwnerDefault gameObject;

		[ObjectType(typeof(AudioClip))]
		public FsmObject audioClip;

		[ObjectType(typeof(RandomAudioClipTable))]
		public FsmObject audioClipTable;

		public FsmFloat pitchOffset;

		public FsmFloat volumeScale;

		public bool stopPreviousSound;

		public bool forcePlay;

		private GameObject self;

		private AudioSource audio;

		public override void Reset()
		{
			gameObject = null;
			audioClipTable = null;
			pitchOffset = 0f;
			stopPreviousSound = true;
			forcePlay = false;
			audio = null;
			volumeScale = 1f;
		}

		public override void OnEnter()
		{
			self = base.Fsm.GetOwnerDefaultTarget(gameObject);
			DoPlayRandomClip();
			Finish();
		}

		private void DoPlayRandomClip()
		{
			if (!(self != null))
			{
				return;
			}
			audio = self.GetComponent<AudioSource>();
			if (!audio)
			{
				return;
			}
			if (stopPreviousSound)
			{
				audio.Stop();
			}
			RandomAudioClipTable randomAudioClipTable = audioClipTable.Value as RandomAudioClipTable;
			if (randomAudioClipTable != null)
			{
				randomAudioClipTable.PlayOneShotUnsafe(audio, pitchOffset.Value, volumeScale.Value, forcePlay);
				return;
			}
			AudioClip audioClip = this.audioClip.Value as AudioClip;
			if (audioClip != null)
			{
				audio.PlayOneShot(audioClip, volumeScale.Value);
			}
		}
	}
}

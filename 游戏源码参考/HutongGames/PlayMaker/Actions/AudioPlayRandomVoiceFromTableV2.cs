using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Audio)]
	public class AudioPlayRandomVoiceFromTableV2 : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		[Tooltip("The GameObject with an AudioSource component.")]
		public FsmOwnerDefault gameObject;

		[ObjectType(typeof(RandomAudioClipTable))]
		public FsmObject audioClipTable;

		public FsmFloat pitchOffset;

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
			if ((bool)audio)
			{
				if (stopPreviousSound)
				{
					audio.Stop();
				}
				RandomAudioClipTable randomAudioClipTable = audioClipTable.Value as RandomAudioClipTable;
				if (randomAudioClipTable != null)
				{
					randomAudioClipTable.PlayOneShot(audio, pitchOffset.Value, forcePlay);
				}
			}
		}
	}
}

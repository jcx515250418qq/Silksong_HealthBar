using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Audio)]
	public class AudioPlayRandomVoiceFromTableBool : FsmStateAction
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

		public FsmBool activeBool;

		private FsmGameObject self;

		private AudioSource audio;

		public override void Reset()
		{
			gameObject = null;
			audioClipTable = null;
			pitchOffset = 0f;
			stopPreviousSound = true;
			forcePlay = false;
			activeBool = null;
		}

		public override void OnEnter()
		{
			if (activeBool.Value)
			{
				self = base.Fsm.GetOwnerDefaultTarget(gameObject);
				DoPlayRandomClip();
				Finish();
			}
		}

		private void DoPlayRandomClip()
		{
			audio = self.Value.GetComponent<AudioSource>();
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

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Audio)]
	public class AudioPlayRandomVoiceFromTable : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		[Tooltip("The GameObject with an AudioSource component.")]
		public FsmOwnerDefault gameObject;

		public RandomAudioClipTable audioClipTable;

		public FsmFloat pitchOffset;

		public bool stopPreviousSound;

		public bool forcePlay;

		private FsmGameObject self;

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
			audio = self.Value.GetComponent<AudioSource>();
			if ((bool)audio)
			{
				if (stopPreviousSound)
				{
					audio.Stop();
				}
				audioClipTable.PlayOneShot(audio, pitchOffset.Value, forcePlay);
			}
		}
	}
}

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Audio)]
	[Tooltip("Sets the Pitch of the Audio Clip played by the AudioSource component on a Game Object.")]
	public class SetAudioPitch : ComponentAction<AudioSource>
	{
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		[Tooltip("A GameObject with an AudioSource component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("Set the pitch.")]
		public FsmFloat pitch;

		[Tooltip("Repeat every frame. Useful if you're driving pitch with a float variable.")]
		public bool everyFrame;

		public override void Reset()
		{
			gameObject = null;
			pitch = 1f;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoSetAudioPitch();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoSetAudioPitch();
		}

		private void DoSetAudioPitch()
		{
			if (UpdateCache(base.Fsm.GetOwnerDefaultTarget(gameObject)) && !pitch.IsNone)
			{
				base.audio.pitch = pitch.Value;
			}
		}
	}
}

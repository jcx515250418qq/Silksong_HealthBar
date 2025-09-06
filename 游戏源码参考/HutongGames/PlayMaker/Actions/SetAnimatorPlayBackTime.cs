using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Sets the playback position in the recording buffer. When in playback mode (use AnimatorStartPlayback), this value is used for controlling the current playback position in the buffer (in seconds). The value can range between recordingStartTime and recordingStopTime ")]
	public class SetAnimatorPlayBackTime : ComponentAction<Animator>
	{
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The playback time")]
		public FsmFloat playbackTime;

		[Tooltip("Repeat every frame. Useful for changing over time.")]
		public bool everyFrame;

		public override void Reset()
		{
			gameObject = null;
			playbackTime = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoPlaybackTime();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoPlaybackTime();
		}

		private void DoPlaybackTime()
		{
			if (UpdateCache(base.Fsm.GetOwnerDefaultTarget(gameObject)))
			{
				cachedComponent.playbackTime = playbackTime.Value;
			}
		}
	}
}

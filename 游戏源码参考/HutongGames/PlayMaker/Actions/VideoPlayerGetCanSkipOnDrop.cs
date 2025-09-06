using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Check whether it's possible to set if the player can skips frames to catch up with current time. (Read Only)")]
	public class VideoPlayerGetCanSkipOnDrop : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with a VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The Value")]
		[UIHint(UIHint.Variable)]
		public FsmBool canSetSkipOnDrop;

		[Tooltip("Event sent if SkipOnDrop can be set")]
		public FsmEvent canSetSkipOnDropEvent;

		[Tooltip("Event sent if SkipOnDrop can not be set")]
		public FsmEvent canNotSetSkipOnDropEvent;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		private int _canSetSkipOnDrop = -1;

		private GameObject go;

		private VideoPlayer _vp;

		public override void Reset()
		{
			gameObject = null;
			canSetSkipOnDrop = null;
			canSetSkipOnDropEvent = null;
			canNotSetSkipOnDropEvent = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			GetVideoPlayer();
			ExecuteAction();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			ExecuteAction();
		}

		private void ExecuteAction()
		{
			if (_vp.canSetSkipOnDrop)
			{
				canSetSkipOnDrop.Value = true;
				if (_canSetSkipOnDrop != 1)
				{
					base.Fsm.Event(canSetSkipOnDropEvent);
				}
				_canSetSkipOnDrop = 1;
			}
			else
			{
				canSetSkipOnDrop.Value = false;
				if (_canSetSkipOnDrop != 0)
				{
					base.Fsm.Event(canNotSetSkipOnDropEvent);
				}
				_canSetSkipOnDrop = 0;
			}
		}

		private void GetVideoPlayer()
		{
			go = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (go != null)
			{
				_vp = go.GetComponent<VideoPlayer>();
			}
		}
	}
}

using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Check whether the player is allowed to skips frames to catch up with current time.")]
	public class VideoPlayerGetSkipOnDrop : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with a VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The Value")]
		[UIHint(UIHint.Variable)]
		public FsmBool skipOnDrop;

		[Tooltip("Event sent if SkipOnDrop is true")]
		public FsmEvent doesSkipOnDropEvent;

		[Tooltip("Event sent if SkipOnDrop is false")]
		public FsmEvent DoNotSkipOnDropEvent;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		private int _canSetSkipOnDrop = -1;

		private GameObject go;

		private VideoPlayer _vp;

		public override void Reset()
		{
			gameObject = null;
			skipOnDrop = null;
			doesSkipOnDropEvent = null;
			DoNotSkipOnDropEvent = null;
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
			if (_vp == null)
			{
				return;
			}
			if (_vp.skipOnDrop)
			{
				skipOnDrop.Value = true;
				if (_canSetSkipOnDrop != 1)
				{
					base.Fsm.Event(doesSkipOnDropEvent);
				}
				_canSetSkipOnDrop = 1;
			}
			else
			{
				skipOnDrop.Value = false;
				if (_canSetSkipOnDrop != 0)
				{
					base.Fsm.Event(DoNotSkipOnDropEvent);
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

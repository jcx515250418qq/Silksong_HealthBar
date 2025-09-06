using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Check whether current time can be changed using the time or timeFrames property on a VideoPlayer. (Read Only)")]
	public class VideoPlayerGetCanSetTime : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with a VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The Value")]
		[UIHint(UIHint.Variable)]
		public FsmBool canSetTime;

		[Tooltip("Event sent if time can be set")]
		public FsmEvent canSetTimeEvent;

		[Tooltip("Event sent if time can not be set")]
		public FsmEvent canNotSetTimeEvent;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		private GameObject go;

		private VideoPlayer _vp;

		public override void Reset()
		{
			gameObject = null;
			canSetTime = null;
			canSetTimeEvent = null;
			canNotSetTimeEvent = null;
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
			if (_vp != null)
			{
				canSetTime.Value = _vp.canSetTime;
				base.Fsm.Event(_vp.canSetTime ? canSetTimeEvent : canNotSetTimeEvent);
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

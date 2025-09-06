using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Sets the time value of a VideoPlayer.")]
	public class VideoPlayerSetTime : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with a VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[Tooltip("The time Value")]
		public FsmFloat time;

		[Tooltip("Event sent if time can not be set")]
		public FsmEvent canNotSetTime;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		private GameObject go;

		private VideoPlayer _vp;

		public override void Reset()
		{
			gameObject = null;
			time = null;
			canNotSetTime = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			GetVideoPlayer();
			if (_vp != null && !_vp.canSetTime)
			{
				base.Fsm.Event(canNotSetTime);
			}
			else
			{
				ExecuteAction();
			}
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
			if (_vp != null && _vp.canSetTime)
			{
				_vp.time = time.Value;
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

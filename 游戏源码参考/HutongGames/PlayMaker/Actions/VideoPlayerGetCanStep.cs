using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Check if the VideoPlayer can step forwards into the video content. (Read Only)")]
	public class VideoPlayerGetCanStep : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with a VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The Value")]
		[UIHint(UIHint.Variable)]
		public FsmBool canStep;

		[Tooltip("Event sent if time can be set")]
		public FsmEvent canStepEvent;

		[Tooltip("Event sent if time can not be set")]
		public FsmEvent canNotStepEvent;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		private GameObject go;

		private VideoPlayer _vp;

		public override void Reset()
		{
			gameObject = null;
			canStep = null;
			canStepEvent = null;
			canNotStepEvent = null;
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
				canStep.Value = _vp.canStep;
				base.Fsm.Event(_vp.canSetTime ? canStepEvent : canNotStepEvent);
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

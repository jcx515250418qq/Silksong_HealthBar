using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Check Whether the player restarts from the beginning without when it reaches the end of the clip.")]
	public class VideoPlayerGetIsLooping : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The Value")]
		[UIHint(UIHint.Variable)]
		public FsmBool isLooping;

		[Tooltip("Event sent if content is looping")]
		public FsmEvent isLoopingEvent;

		[Tooltip("Event sent if content is not looping")]
		public FsmEvent isNotLoopingEvent;

		[Tooltip("Execute action everyframe. Events are however sent discretly, only when changes occurs")]
		public bool everyframe;

		private GameObject go;

		private VideoPlayer _vp;

		private int _isLooping = -1;

		public override void Reset()
		{
			gameObject = null;
			isLooping = null;
			isLoopingEvent = null;
			isNotLoopingEvent = null;
		}

		public override void OnEnter()
		{
			GetVideoPlayer();
			ExecuteAction();
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
			if (_vp.isLooping)
			{
				isLooping.Value = true;
				if (_isLooping != 1)
				{
					base.Fsm.Event(isLoopingEvent);
				}
				_isLooping = 1;
			}
			else
			{
				isLooping.Value = false;
				if (_isLooping != 0)
				{
					base.Fsm.Event(isNotLoopingEvent);
				}
				_isLooping = 0;
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

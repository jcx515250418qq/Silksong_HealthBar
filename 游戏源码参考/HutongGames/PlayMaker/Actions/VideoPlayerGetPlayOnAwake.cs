using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Check Whether the content will start playing back as soon as the component awakes.")]
	public class VideoPlayerGetPlayOnAwake : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The Value")]
		[UIHint(UIHint.Variable)]
		public FsmBool isPlayingOnAwake;

		[Tooltip("Event sent if content content will start playing back as soon as the component awakes")]
		public FsmEvent isPlayingOnAwakeEvent;

		[Tooltip("Event sent if content will not start playing back as soon as the component awakes")]
		public FsmEvent isNotPlayingOnAwakeEvent;

		[Tooltip("Execute action everyframe. Events are however sent discretly, only when changes occurs")]
		public bool everyframe;

		private GameObject go;

		private VideoPlayer _vp;

		private int _isPlayingOnAwake = -1;

		public override void Reset()
		{
			gameObject = null;
			isPlayingOnAwake = null;
			isPlayingOnAwakeEvent = null;
			isNotPlayingOnAwakeEvent = null;
			everyframe = false;
		}

		public override void OnEnter()
		{
			GetVideoPlayer();
			ExecuteAction();
			if (!everyframe)
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
			if (_vp.playOnAwake)
			{
				isPlayingOnAwake.Value = true;
				if (_isPlayingOnAwake != 1)
				{
					base.Fsm.Event(isPlayingOnAwakeEvent);
				}
				_isPlayingOnAwake = 1;
			}
			else
			{
				isPlayingOnAwake.Value = false;
				if (_isPlayingOnAwake != 0)
				{
					base.Fsm.Event(isNotPlayingOnAwakeEvent);
				}
				_isPlayingOnAwake = 0;
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

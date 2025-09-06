using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Check Whether frameReady events are enabled")]
	public class VideoPlayerGetSendFrameReadyEvents : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The Value")]
		[UIHint(UIHint.Variable)]
		public FsmBool isSendingFrameReadyEvents;

		[Tooltip("Event sent if frameReady events are sent")]
		public FsmEvent isSendingFrameReadyEventsEvent;

		[Tooltip("Event sent if frameReady events are not sent")]
		public FsmEvent isNotSendingFrameReadyEventsEvent;

		[Tooltip("Execute action everyframe. Events are however sent discretly, only when changes occurs")]
		public bool everyframe;

		private GameObject go;

		private VideoPlayer _vp;

		private int _isSendingFrameReadyEvents = -1;

		public override void Reset()
		{
			gameObject = null;
			isSendingFrameReadyEvents = null;
			isSendingFrameReadyEventsEvent = null;
			isNotSendingFrameReadyEventsEvent = null;
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
			if (_vp.sendFrameReadyEvents)
			{
				isSendingFrameReadyEvents.Value = true;
				if (_isSendingFrameReadyEvents != 1)
				{
					base.Fsm.Event(isSendingFrameReadyEventsEvent);
				}
				_isSendingFrameReadyEvents = 1;
			}
			else
			{
				isSendingFrameReadyEvents.Value = false;
				if (_isSendingFrameReadyEvents != 0)
				{
					base.Fsm.Event(isNotSendingFrameReadyEventsEvent);
				}
				_isSendingFrameReadyEvents = 0;
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

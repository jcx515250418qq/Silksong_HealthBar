using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Send the frameReady event from a VideoPlayer when a new frame is ready.")]
	public class VideoPlayerFrameReadyEvent : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("event sent when a new frame is ready.")]
		public FsmEvent onFrameReadyEvent;

		private GameObject go;

		private VideoPlayer _vp;

		public override void Reset()
		{
			gameObject = null;
			onFrameReadyEvent = null;
		}

		public override void OnEnter()
		{
			GetVideoPlayer();
			if (_vp != null)
			{
				_vp.frameReady += OnFrameReady;
			}
		}

		public override void OnExit()
		{
			if (_vp != null)
			{
				_vp.frameReady -= OnFrameReady;
			}
		}

		private void OnFrameReady(VideoPlayer source, long frameIndex)
		{
			Fsm.EventData.GameObjectData = source.gameObject;
			Fsm.EventData.IntData = (int)frameIndex;
			base.Fsm.Event(onFrameReadyEvent);
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

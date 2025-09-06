using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Send the framedropped event from a VideoPlayer when playback detects it does not keep up with the time source..")]
	public class VideoPlayerFrameDroppedEvent : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("event sent when playback detects it does not keep up with the time source.")]
		public FsmEvent onFrameDroppedEvent;

		private GameObject go;

		private VideoPlayer _vp;

		public override void Reset()
		{
			gameObject = null;
			onFrameDroppedEvent = null;
		}

		public override void OnEnter()
		{
			GetVideoPlayer();
			if (_vp != null)
			{
				_vp.frameDropped += OnFrameDropped;
			}
		}

		public override void OnExit()
		{
			if (_vp != null)
			{
				_vp.frameDropped -= OnFrameDropped;
			}
		}

		private void OnFrameDropped(VideoPlayer source)
		{
			Fsm.EventData.GameObjectData = source.gameObject;
			base.Fsm.Event(onFrameDroppedEvent);
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

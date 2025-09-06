using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Send error event from a VideoPlayer.")]
	public class VideoPlayerErrorEvent : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("event sent when VideoPlayer throws an error")]
		public FsmEvent onErrorEvent;

		private GameObject go;

		private VideoPlayer _vp;

		public override void Reset()
		{
			gameObject = null;
			onErrorEvent = null;
		}

		public override void OnEnter()
		{
			GetVideoPlayer();
			if (_vp != null)
			{
				_vp.errorReceived += OnErrorReceived;
			}
		}

		public override void OnExit()
		{
			if (_vp != null)
			{
				_vp.errorReceived -= OnErrorReceived;
			}
		}

		private void OnErrorReceived(VideoPlayer source, string errorMessage)
		{
			Fsm.EventData.GameObjectData = source.gameObject;
			Fsm.EventData.StringData = errorMessage;
			base.Fsm.Event(onErrorEvent);
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

using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Get the Destination for the audio embedded in the video.")]
	public class VideoPlayerGetAudioOutputMode : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with a VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The AudioOutputMode type")]
		[ObjectType(typeof(VideoAudioOutputMode))]
		public FsmEnum audioOutputMode;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		private GameObject go;

		private VideoPlayer _vp;

		public override void Reset()
		{
			gameObject = null;
			audioOutputMode = VideoAudioOutputMode.AudioSource;
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
				audioOutputMode.Value = _vp.audioOutputMode;
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

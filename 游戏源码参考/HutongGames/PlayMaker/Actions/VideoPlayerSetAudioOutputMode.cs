using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Defines Destination for the audio embedded in the video.")]
	public class VideoPlayerSetAudioOutputMode : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with a VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[Tooltip("The AudioOutputMode type")]
		[ObjectType(typeof(VideoAudioOutputMode))]
		public FsmEnum audioOutputMode;

		private GameObject go;

		private VideoPlayer _vp;

		public override void Reset()
		{
			gameObject = null;
			audioOutputMode = VideoAudioOutputMode.AudioSource;
		}

		public override void OnEnter()
		{
			GetVideoPlayer();
			ExecuteAction();
			Finish();
		}

		private void ExecuteAction()
		{
			if (_vp != null)
			{
				_vp.audioOutputMode = (VideoAudioOutputMode)(object)audioOutputMode.Value;
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

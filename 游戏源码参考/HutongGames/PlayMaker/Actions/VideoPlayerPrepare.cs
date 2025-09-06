using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Initiates playback engine preparation of a VideoPlayer. The preparation consists of reserving the resources needed for playback, and preloading some or all of the content to be played. After this is done, frames can be received immediately.")]
	public class VideoPlayerPrepare : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		private GameObject go;

		private VideoPlayer _vp;

		public override void Reset()
		{
			gameObject = null;
		}

		public override void OnEnter()
		{
			GetVideoPlayer();
			if (_vp != null)
			{
				_vp.Prepare();
			}
			Finish();
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

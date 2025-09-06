using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Starts playback of a VideoPlayer. Initiates the preparation if not already done, in which case the playback will not start instantly.")]
	public class VideoPlayerPlay : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with a VideoPlayer component.")]
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
				_vp.Play();
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

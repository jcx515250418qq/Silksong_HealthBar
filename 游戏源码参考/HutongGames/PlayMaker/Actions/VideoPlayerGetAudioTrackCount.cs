using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Number of audio tracks found in the data source currently configured on a videoPlayer. For URL sources, this will only be set once the source preparation is completed. See VideoPlayer.Prepare.")]
	public class VideoPlayerGetAudioTrackCount : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Number of audio tracks found in the data source currently configured")]
		public FsmInt audioTrackCount;

		[Tooltip("Event sent if source is not prepared")]
		public FsmEvent isNotPrepared;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		private GameObject go;

		private VideoPlayer _vp;

		public override void Reset()
		{
			gameObject = null;
			audioTrackCount = null;
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
				if (_vp.isPrepared)
				{
					base.Fsm.Event(isNotPrepared);
					audioTrackCount.Value = 0;
				}
				else
				{
					audioTrackCount.Value = _vp.audioTrackCount;
				}
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

using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Check whether direct-output volume controls are supported for the current platform and video format on a VideoPlayer. (Read Only)")]
	public class VideoPlayerGetCanSetDirectAudioVolume : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with a VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The Value")]
		[UIHint(UIHint.Variable)]
		public FsmBool canSetDirectAudioVolume;

		[Tooltip("Event sent if DirectAudioVolume can be set")]
		public FsmEvent canSetDirectAudioVolumeEvent;

		[Tooltip("Event sent if DirectAudioVolume can not be set")]
		public FsmEvent canNotSetDirectAudioVolumeEvent;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		private GameObject go;

		private VideoPlayer _vp;

		public override void Reset()
		{
			gameObject = null;
			canSetDirectAudioVolume = null;
			canSetDirectAudioVolumeEvent = null;
			canNotSetDirectAudioVolumeEvent = null;
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
				canSetDirectAudioVolume.Value = _vp.canSetDirectAudioVolume;
				base.Fsm.Event(_vp.canSetTime ? canSetDirectAudioVolumeEvent : canNotSetDirectAudioVolumeEvent);
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

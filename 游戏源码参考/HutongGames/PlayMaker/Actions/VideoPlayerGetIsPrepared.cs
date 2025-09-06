using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Check Whether the player has successfully prepared the content to be played. (Read Only)")]
	public class VideoPlayerGetIsPrepared : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The Value")]
		[UIHint(UIHint.Variable)]
		public FsmBool isPrepared;

		[Tooltip("Event sent if content is prepared")]
		public FsmEvent isPreparedEvent;

		[Tooltip("Event sent if content is not yet prepared")]
		public FsmEvent isNotPreparedEvent;

		[Tooltip("Execute action everyframe. Events are however sent discretly, only when changes occurs")]
		public bool everyframe;

		private GameObject go;

		private VideoPlayer _vp;

		private int _isPrepared = -1;

		public override void Reset()
		{
			gameObject = null;
			isPrepared = null;
			isPreparedEvent = null;
			isNotPreparedEvent = null;
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
			if (_vp.isPrepared)
			{
				isPrepared.Value = true;
				if (_isPrepared != 1)
				{
					base.Fsm.Event(isPreparedEvent);
				}
				_isPrepared = 1;
			}
			else
			{
				isPrepared.Value = false;
				if (_isPrepared != 0)
				{
					base.Fsm.Event(isNotPreparedEvent);
				}
				_isPrepared = 0;
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

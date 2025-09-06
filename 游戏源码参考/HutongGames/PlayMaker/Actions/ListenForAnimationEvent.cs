using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class ListenForAnimationEvent : FsmStateAction
	{
		public FsmOwnerDefault Target;

		public FsmEvent Response;

		private CaptureAnimationEvent eventSource;

		public override void Reset()
		{
			Target = null;
			Response = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe)
			{
				eventSource = safe.GetComponent<CaptureAnimationEvent>();
				if ((bool)eventSource)
				{
					eventSource.EventFired += OnEventFired;
				}
			}
		}

		public override void OnExit()
		{
			Unsubscribe();
		}

		private void OnEventFired()
		{
			Unsubscribe();
			base.Fsm.Event(Response);
			Finish();
		}

		private void Unsubscribe()
		{
			if ((bool)eventSource)
			{
				eventSource.EventFired -= OnEventFired;
				eventSource = null;
			}
		}
	}
}

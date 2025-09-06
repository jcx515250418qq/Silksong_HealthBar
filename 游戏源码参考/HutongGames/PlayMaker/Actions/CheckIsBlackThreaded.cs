using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class CheckIsBlackThreaded : FsmStateAction
	{
		public FsmOwnerDefault Target;

		public FsmEvent TrueEvent;

		public FsmEvent FalseEvent;

		[UIHint(UIHint.Variable)]
		public FsmBool StoreValue;

		public bool EveryFrame;

		private BlackThreadState blackThreadState;

		public override void Reset()
		{
			Target = null;
			TrueEvent = null;
			FalseEvent = null;
			StoreValue = null;
			EveryFrame = false;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe)
			{
				blackThreadState = safe.GetComponentInParent<BlackThreadState>(includeInactive: true);
			}
			DoAction();
			if (!EveryFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoAction();
		}

		private void DoAction()
		{
			bool flag = (bool)blackThreadState && blackThreadState.IsVisiblyThreaded;
			StoreValue.Value = flag;
			base.Fsm.Event(flag ? TrueEvent : FalseEvent);
		}
	}
}

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class BecomeBlackThreaded : FsmStateAction
	{
		public FsmOwnerDefault Target;

		public FsmBool WaitForSing;

		public override void Reset()
		{
			Target = null;
			WaitForSing = true;
		}

		public override void Awake()
		{
			BlackThreadState blackThreadState = GetBlackThreadState();
			if ((bool)blackThreadState)
			{
				blackThreadState.ReportWillBeThreaded();
			}
		}

		public override void OnEnter()
		{
			BlackThreadState blackThreadState = GetBlackThreadState();
			if ((bool)blackThreadState)
			{
				if (WaitForSing.Value)
				{
					blackThreadState.BecomeThreaded();
				}
				else
				{
					blackThreadState.BecomeThreadedNoSing();
				}
			}
			Finish();
		}

		private BlackThreadState GetBlackThreadState()
		{
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe)
			{
				BlackThreadState componentInParent = safe.GetComponentInParent<BlackThreadState>(includeInactive: true);
				if ((bool)componentInParent)
				{
					return componentInParent;
				}
				Debug.LogError("Object \"" + safe.name + "\" does not have a BlackThreadState component", base.Owner);
			}
			else
			{
				Debug.LogError("Target is null", base.Owner);
			}
			return null;
		}
	}
}

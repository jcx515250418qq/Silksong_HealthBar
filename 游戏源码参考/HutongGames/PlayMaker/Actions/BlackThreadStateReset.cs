using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class BlackThreadStateReset : FsmStateAction
	{
		public FsmOwnerDefault Target;

		public override void Reset()
		{
			Target = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe)
			{
				BlackThreadState componentInParent = safe.GetComponentInParent<BlackThreadState>(includeInactive: true);
				if ((bool)componentInParent)
				{
					componentInParent.ResetThreaded();
				}
				else
				{
					Debug.LogError("Object \"" + safe.name + "\" does not have a BlackThreadState component", base.Owner);
				}
			}
			else
			{
				Debug.LogError("Target is null", base.Owner);
			}
			Finish();
		}
	}
}

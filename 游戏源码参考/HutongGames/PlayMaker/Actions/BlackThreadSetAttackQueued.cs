using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class BlackThreadSetAttackQueued : FsmStateAction
	{
		public FsmOwnerDefault Target;

		public FsmBool SetAttackQueued;

		public override void Reset()
		{
			Target = null;
			SetAttackQueued = true;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe)
			{
				BlackThreadState componentInParent = safe.GetComponentInParent<BlackThreadState>(includeInactive: true);
				if ((bool)componentInParent)
				{
					componentInParent.SetAttackQueued(SetAttackQueued.Value);
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

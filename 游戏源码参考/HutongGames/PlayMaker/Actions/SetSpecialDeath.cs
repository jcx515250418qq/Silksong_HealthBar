using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class SetSpecialDeath : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		public FsmBool hasSpecialDeath;

		public override void Reset()
		{
			target = new FsmOwnerDefault();
			hasSpecialDeath = new FsmBool();
		}

		public override void OnEnter()
		{
			GameObject safe = target.GetSafe(this);
			if (safe != null)
			{
				HealthManager component = safe.GetComponent<HealthManager>();
				if (component != null && !hasSpecialDeath.IsNone)
				{
					component.hasSpecialDeath = hasSpecialDeath.Value;
				}
			}
			Finish();
		}
	}
}

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class SetIsDead : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		public FsmBool setValue;

		public override void Reset()
		{
			target = new FsmOwnerDefault();
			setValue = new FsmBool();
		}

		public override void OnEnter()
		{
			GameObject safe = target.GetSafe(this);
			if (safe != null)
			{
				HealthManager component = safe.GetComponent<HealthManager>();
				if (component != null)
				{
					component.SetIsDead(setValue.Value);
				}
			}
			Finish();
		}
	}
}

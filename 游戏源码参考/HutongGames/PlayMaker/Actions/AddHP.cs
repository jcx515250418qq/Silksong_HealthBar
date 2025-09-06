using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class AddHP : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		public FsmInt amount;

		public FsmBool healToMax;

		public override void Reset()
		{
			target = new FsmOwnerDefault();
			amount = new FsmInt();
		}

		public override void OnEnter()
		{
			GameObject safe = target.GetSafe(this);
			if (safe != null)
			{
				HealthManager component = safe.GetComponent<HealthManager>();
				if (component != null)
				{
					if (healToMax.Value)
					{
						component.HealToMax();
					}
					else if (!amount.IsNone)
					{
						component.hp += amount.Value;
					}
				}
			}
			Finish();
		}
	}
}

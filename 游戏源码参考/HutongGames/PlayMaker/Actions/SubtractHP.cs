using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class SubtractHP : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		public FsmInt amount;

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
				if (component != null && !amount.IsNone)
				{
					component.hp -= amount.Value;
				}
			}
			Finish();
		}
	}
}

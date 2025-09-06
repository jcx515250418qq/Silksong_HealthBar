using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class SetHP : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		public FsmInt hp;

		public override void Reset()
		{
			target = new FsmOwnerDefault();
			hp = new FsmInt();
		}

		public override void OnEnter()
		{
			GameObject safe = target.GetSafe(this);
			if (safe != null)
			{
				HealthManager component = safe.GetComponent<HealthManager>();
				if (component != null && !hp.IsNone)
				{
					component.hp = hp.Value;
					if (hp.Value > 0)
					{
						component.isDead = false;
					}
				}
			}
			Finish();
		}
	}
}

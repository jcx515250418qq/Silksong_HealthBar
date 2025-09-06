using UnityEngine;

namespace HutongGames.PlayMaker
{
	[ActionCategory("Hollow Knight")]
	public class SetDamageEnemyDirection : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		public FsmFloat damageDirection;

		public override void Reset()
		{
			target = new FsmOwnerDefault();
			damageDirection = null;
		}

		public override void OnEnter()
		{
			GameObject safe = target.GetSafe(this);
			if (safe != null)
			{
				DamageEnemies component = safe.GetComponent<DamageEnemies>();
				if (component != null && !damageDirection.IsNone)
				{
					component.direction = damageDirection.Value;
				}
			}
			Finish();
		}
	}
}

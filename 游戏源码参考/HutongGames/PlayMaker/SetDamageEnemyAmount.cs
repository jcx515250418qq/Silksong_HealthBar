using UnityEngine;

namespace HutongGames.PlayMaker
{
	[ActionCategory("Hollow Knight")]
	public class SetDamageEnemyAmount : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		public FsmInt damageDealt;

		public override void Reset()
		{
			target = new FsmOwnerDefault();
			damageDealt = null;
		}

		public override void OnEnter()
		{
			GameObject safe = target.GetSafe(this);
			if (safe != null)
			{
				DamageEnemies component = safe.GetComponent<DamageEnemies>();
				if (component != null && !damageDealt.IsNone)
				{
					component.damageDealt = damageDealt.Value;
				}
			}
			Finish();
		}
	}
}

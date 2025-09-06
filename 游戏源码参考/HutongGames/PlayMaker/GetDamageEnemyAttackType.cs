using UnityEngine;

namespace HutongGames.PlayMaker
{
	public class GetDamageEnemyAttackType : FsmStateAction
	{
		public FsmOwnerDefault Target;

		[UIHint(UIHint.Variable)]
		[ObjectType(typeof(AttackTypes))]
		public FsmEnum StoreAttackType;

		public override void Reset()
		{
			Target = null;
			StoreAttackType = null;
		}

		public override void OnEnter()
		{
			StoreAttackType.Value = AttackTypes.Generic;
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe)
			{
				DamageEnemies component = safe.GetComponent<DamageEnemies>();
				if ((bool)component)
				{
					StoreAttackType.Value = component.attackType;
				}
			}
			Finish();
		}
	}
}

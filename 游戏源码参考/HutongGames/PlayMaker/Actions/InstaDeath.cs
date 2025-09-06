using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class InstaDeath : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		public FsmFloat direction;

		public override void Reset()
		{
			target = new FsmOwnerDefault();
		}

		public override void OnEnter()
		{
			GameObject safe = target.GetSafe(this);
			if (safe != null)
			{
				HealthManager component = safe.GetComponent<HealthManager>();
				if (component != null)
				{
					if (!component.isDead)
					{
						float value = (direction.IsNone ? DirectionUtils.GetAngle(component.GetAttackDirection()) : direction.Value);
						component.Die(value, AttackTypes.Generic, NailElements.None, null, ignoreEvasion: false, 1f, overrideSpecialDeath: true);
					}
				}
				else
				{
					safe.GetComponent<EnemyDeathEffects>().ReceiveDeathEvent(DirectionUtils.GetAngle(1), AttackTypes.Generic, 0f);
				}
			}
			Finish();
		}
	}
}

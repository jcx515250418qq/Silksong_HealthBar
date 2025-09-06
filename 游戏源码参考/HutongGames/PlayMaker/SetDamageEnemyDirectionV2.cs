using UnityEngine;

namespace HutongGames.PlayMaker
{
	[ActionCategory("Hollow Knight")]
	public class SetDamageEnemyDirectionV2 : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault Target;

		public FsmFloat Direction;

		public FsmBool UseChildren;

		public override void Reset()
		{
			Target = new FsmOwnerDefault();
			Direction = null;
			UseChildren = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if (safe != null)
			{
				if (!UseChildren.Value)
				{
					DamageEnemies component = safe.GetComponent<DamageEnemies>();
					if (component != null)
					{
						SetDirection(component);
					}
				}
				else
				{
					DamageEnemies[] componentsInChildren = safe.GetComponentsInChildren<DamageEnemies>(includeInactive: true);
					foreach (DamageEnemies direction in componentsInChildren)
					{
						SetDirection(direction);
					}
				}
			}
			Finish();
		}

		private void SetDirection(DamageEnemies damager)
		{
			if (!Direction.IsNone)
			{
				damager.direction = Direction.Value;
			}
		}
	}
}

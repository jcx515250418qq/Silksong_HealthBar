using System;
using UnityEngine;

namespace HutongGames.PlayMaker
{
	public class ResetDamageEnemiesLists : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		[CheckForComponent(typeof(DamageEnemies))]
		public FsmOwnerDefault Target;

		public override void Reset()
		{
			Target = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if (safe != null)
			{
				DamageEnemies component = safe.GetComponent<DamageEnemies>();
				if (component != null)
				{
					try
					{
						component.EndDamage();
						component.StartDamage();
					}
					catch (Exception)
					{
					}
				}
			}
			Finish();
		}
	}
}

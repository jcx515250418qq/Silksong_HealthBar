using UnityEngine;

namespace HutongGames.PlayMaker
{
	public class SetDamageEnemies : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		[CheckForComponent(typeof(DamageEnemies))]
		public FsmOwnerDefault Target;

		[RequiredField]
		public new FsmBool Enabled;

		public override void Reset()
		{
			Target = null;
			Enabled = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if (safe != null)
			{
				DamageEnemies component = safe.GetComponent<DamageEnemies>();
				if (component != null)
				{
					component.enabled = Enabled.Value;
				}
			}
			Finish();
		}
	}
}

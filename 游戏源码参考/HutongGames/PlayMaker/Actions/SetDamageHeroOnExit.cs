using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class SetDamageHeroOnExit : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault Target;

		[RequiredField]
		public new FsmBool Enabled;

		public override void Reset()
		{
			Target = null;
			Enabled = null;
		}

		public override void OnExit()
		{
			GameObject safe = Target.GetSafe(this);
			if (safe != null)
			{
				DamageHero component = safe.GetComponent<DamageHero>();
				if (component != null)
				{
					component.enabled = Enabled.Value;
				}
			}
			Finish();
		}
	}
}

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class SetDamageHeroAmount : FsmStateAction
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
				DamageHero component = safe.GetComponent<DamageHero>();
				if (component != null && !damageDealt.IsNone)
				{
					component.damageDealt = damageDealt.Value;
				}
			}
			Finish();
		}
	}
}

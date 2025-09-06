using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class SetDamageOverride : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		public FsmBool damageOverride;

		public override void Reset()
		{
			target = new FsmOwnerDefault();
			damageOverride = new FsmBool();
		}

		public override void OnEnter()
		{
			GameObject gameObject = ((target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : target.GameObject.Value);
			if (gameObject != null)
			{
				HealthManager component = gameObject.GetComponent<HealthManager>();
				if (component != null)
				{
					component.SetDamageOverride(damageOverride.Value);
				}
			}
			Finish();
		}
	}
}

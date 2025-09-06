using UnityEngine;

namespace HutongGames.PlayMaker
{
	[ActionCategory("Hollow Knight")]
	public class DoDamage : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault damager;

		public FsmGameObject target;

		public override void Reset()
		{
			damager = new FsmOwnerDefault();
			target = new FsmGameObject();
		}

		public override void OnEnter()
		{
			GameObject gameObject = ((damager.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : damager.GameObject.Value);
			if (target.Value == null)
			{
				Finish();
				return;
			}
			if (gameObject != null)
			{
				DamageEnemies component = gameObject.GetComponent<DamageEnemies>();
				if (component != null)
				{
					component.DoDamage(target.Value);
					component.ForceUpdate();
				}
			}
			Finish();
		}
	}
}

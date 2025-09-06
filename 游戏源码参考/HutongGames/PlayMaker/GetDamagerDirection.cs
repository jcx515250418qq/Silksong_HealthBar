using UnityEngine;

namespace HutongGames.PlayMaker
{
	[ActionCategory("Hollow Knight")]
	public class GetDamagerDirection : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		public FsmFloat storeDirection;

		public override void Reset()
		{
			target = new FsmOwnerDefault();
			storeDirection = new FsmFloat();
		}

		public override void OnEnter()
		{
			GameObject gameObject = ((target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : target.GameObject.Value);
			if (gameObject != null)
			{
				DamageEnemies component = gameObject.GetComponent<DamageEnemies>();
				if (component != null)
				{
					storeDirection.Value = component.GetDirection();
				}
			}
			Finish();
		}
	}
}

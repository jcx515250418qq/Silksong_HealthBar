using HutongGames.PlayMaker;
using UnityEngine;

[ActionCategory("Hollow Knight")]
public class SendDeathEvent : FsmStateAction
{
	[UIHint(UIHint.Variable)]
	public FsmOwnerDefault target;

	public FsmFloat attackDirection;

	public override void Reset()
	{
		target = new FsmOwnerDefault();
		attackDirection = new FsmFloat
		{
			UseVariable = true
		};
	}

	public override void OnEnter()
	{
		GameObject gameObject = ((target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : target.GameObject.Value);
		if (gameObject != null)
		{
			EnemyDeathEffects component = gameObject.GetComponent<EnemyDeathEffects>();
			if (component != null)
			{
				component.ReceiveDeathEvent(attackDirection.Value, AttackTypes.Generic);
			}
		}
		Finish();
	}
}

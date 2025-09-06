using HutongGames.PlayMaker;
using UnityEngine;

[ActionCategory("Hollow Knight")]
public class SetCorpsePrefab : FsmStateAction
{
	[UIHint(UIHint.Variable)]
	public FsmOwnerDefault target;

	public FsmGameObject corpsePrefab;

	public override void Reset()
	{
		target = new FsmOwnerDefault();
		corpsePrefab = new FsmGameObject();
	}

	public override void OnEnter()
	{
		GameObject gameObject = ((target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : target.GameObject.Value);
		if (gameObject != null)
		{
			EnemyDeathEffects component = gameObject.GetComponent<EnemyDeathEffects>();
			if (component != null)
			{
				component.CorpsePrefab = corpsePrefab.Value;
			}
		}
		Finish();
	}
}

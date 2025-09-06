using HutongGames.PlayMaker;
using UnityEngine;

[ActionCategory("Hollow Knight")]
public class EnemyPusherIgnore : FsmStateAction
{
	public FsmOwnerDefault target;

	public FsmGameObject other;

	public override void Reset()
	{
		target = null;
		other = new FsmGameObject
		{
			UseVariable = true
		};
	}

	public override void OnEnter()
	{
		GameObject safe = target.GetSafe(this);
		if ((bool)safe && (bool)other.Value)
		{
			EnemyPusher componentInChildren = other.Value.GetComponentInChildren<EnemyPusher>();
			if ((bool)componentInChildren)
			{
				Collider2D component = safe.GetComponent<Collider2D>();
				Collider2D component2 = componentInChildren.GetComponent<Collider2D>();
				if ((bool)component && (bool)component2)
				{
					Physics2D.IgnoreCollision(component, component2);
				}
			}
		}
		Finish();
	}
}

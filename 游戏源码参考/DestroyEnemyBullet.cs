using HutongGames.PlayMaker;
using UnityEngine;

[ActionCategory("Hollow Knight")]
public class DestroyEnemyBullet : FsmStateAction
{
	public FsmOwnerDefault target;

	public override void Reset()
	{
		target = null;
	}

	public override void OnEnter()
	{
		GameObject safe = target.GetSafe(this);
		if ((bool)safe)
		{
			EnemyBullet component = safe.GetComponent<EnemyBullet>();
			if ((bool)component)
			{
				component.OrbitShieldHit(base.Owner.transform);
			}
		}
		Finish();
	}
}

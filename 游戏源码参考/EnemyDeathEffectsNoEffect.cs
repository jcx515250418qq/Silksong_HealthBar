using UnityEngine;

public class EnemyDeathEffectsNoEffect : EnemyDeathEffects
{
	protected override void EmitEffects(GameObject corpseObj)
	{
		if (corpseObj != null)
		{
			SpriteFlash component = corpseObj.GetComponent<SpriteFlash>();
			if (component != null)
			{
				component.FlashEnemyHit();
			}
		}
		ShakeCameraIfVisible();
	}
}

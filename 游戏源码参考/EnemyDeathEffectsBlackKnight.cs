using UnityEngine;

public class EnemyDeathEffectsBlackKnight : EnemyDeathEffects
{
	protected override void EmitEffects(GameObject corpseObj)
	{
		if (corpseObj != null)
		{
			SpriteFlash component = corpseObj.GetComponent<SpriteFlash>();
			if (component != null)
			{
				component.flashFocusHeal();
			}
		}
		ShakeCameraIfVisible("AverageShake");
	}
}

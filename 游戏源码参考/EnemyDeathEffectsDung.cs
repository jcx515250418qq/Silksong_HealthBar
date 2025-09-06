using UnityEngine;

public class EnemyDeathEffectsDung : EnemyDeathEffects
{
	[Header("Dung Variables")]
	public GameObject deathPuffDung;

	protected override void EmitEffects(GameObject corpseObj)
	{
		if (corpseObj != null)
		{
			SpriteFlash component = corpseObj.GetComponent<SpriteFlash>();
			if (component != null)
			{
				component.flashDung();
			}
		}
		deathPuffDung.Spawn(base.transform.position + effectOrigin);
		EmitSound();
		ShakeCameraIfVisible("AverageShake");
		GameManager.instance.FreezeMoment(1);
	}
}

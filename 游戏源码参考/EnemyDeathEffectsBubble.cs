using UnityEngine;

public class EnemyDeathEffectsBubble : EnemyDeathEffects
{
	[Header("Bubble Effects")]
	public GameObject bubblePopPrefab;

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
		ShakeCameraIfVisible("EnemyKillShake");
		GameManager.instance.FreezeMoment(1);
		Object.Instantiate(bubblePopPrefab, base.transform.position + effectOrigin, Quaternion.identity);
	}
}

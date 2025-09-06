using GlobalSettings;
using UnityEngine;

public class LavaCorpseSplash : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (Corpse.TryGetCorpse(collision.gameObject, out var corpse))
		{
			if ((bool)GlobalSettings.Corpse.EnemyLavaDeath)
			{
				GlobalSettings.Corpse.EnemyLavaDeath.Spawn().transform.SetPosition2D(corpse.transform.position);
			}
			corpse.gameObject.SetActive(value: false);
		}
		if (ActiveCorpse.TryGetCorpse(collision.gameObject, out var corpse2))
		{
			if ((bool)GlobalSettings.Corpse.EnemyLavaDeath)
			{
				GlobalSettings.Corpse.EnemyLavaDeath.Spawn().transform.SetPosition2D(corpse2.transform.position);
			}
			corpse2.gameObject.SetActive(value: false);
		}
	}
}

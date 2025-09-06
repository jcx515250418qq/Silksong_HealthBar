using UnityEngine;

public class SetNailDamage : MonoBehaviour
{
	[SerializeField]
	private DamageEnemies damager;

	[Space]
	[SerializeField]
	private float multiplier;

	private void OnEnable()
	{
		if ((bool)damager)
		{
			damager.damageDealt = Mathf.FloorToInt((float)PlayerData.instance.nailDamage * multiplier);
		}
	}
}

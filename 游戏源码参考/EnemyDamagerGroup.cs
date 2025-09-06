using UnityEngine;

public class EnemyDamagerGroup : MonoBehaviour
{
	[SerializeField]
	private DamageEnemies[] damagers;

	private void Awake()
	{
		DamageEnemies[] array = damagers;
		foreach (DamageEnemies damageEnemies in array)
		{
			DamageEnemies[] array2 = damagers;
			foreach (DamageEnemies toDamager in array2)
			{
				if (!(damageEnemies == null) && !(toDamager == null) && !(damageEnemies == toDamager))
				{
					damageEnemies.WillDamageEnemyCollider += delegate(Collider2D collider)
					{
						toDamager.PreventDamage(collider);
					};
				}
			}
		}
	}
}

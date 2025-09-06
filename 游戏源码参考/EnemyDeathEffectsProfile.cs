using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Profiles/Enemy Death Effects Profile")]
public class EnemyDeathEffectsProfile : ScriptableObject
{
	[SerializeField]
	private BloodSpawner.Config[] blood;

	[SerializeField]
	private GameObject[] spawnEffectPrefabs;

	[SerializeField]
	private FlingUtils.Config[] spawnFlings;

	[SerializeField]
	private AudioEvent[] deathSounds;

	public void SpawnEffects(Transform spawnPoint, Vector3 offset, Transform corpse, Color? bloodColorOverride = null, float blackThreadAmount = -1f)
	{
		BloodSpawner.Config[] array = blood;
		for (int i = 0; i < array.Length; i++)
		{
			BloodSpawner.Config config = array[i];
			config.Position += offset;
			GameObject gameObject = BloodSpawner.SpawnBlood(config, spawnPoint, bloodColorOverride);
			if ((bool)gameObject && (bool)corpse)
			{
				FollowTransform follow = gameObject.GetComponent<FollowTransform>() ?? gameObject.AddComponent<FollowTransform>();
				follow.Target = corpse;
				RecycleResetHandler.Add(gameObject, (Action)delegate
				{
					follow.Target = null;
				});
			}
		}
		Vector3 position = spawnPoint.TransformPoint(offset);
		GameObject[] array2 = spawnEffectPrefabs;
		for (int i = 0; i < array2.Length; i++)
		{
			GameObject gameObject2;
			BlackThreadDeathAltProxy component = (gameObject2 = array2[i]).GetComponent<BlackThreadDeathAltProxy>();
			if ((bool)component)
			{
				BlackThreadState component2 = spawnPoint.GetComponent<BlackThreadState>();
				if ((bool)component2 && component2.IsVisiblyThreaded)
				{
					gameObject2 = component.AltPrefab;
				}
			}
			if (!gameObject2)
			{
				continue;
			}
			GameObject gameObject3 = gameObject2.Spawn(position);
			if (blackThreadAmount > 0f)
			{
				BlackThreadEffectRendererGroup component3 = gameObject3.GetComponent<BlackThreadEffectRendererGroup>();
				if (component3 != null)
				{
					component3.SetBlackThreadAmount(blackThreadAmount);
				}
			}
			if ((bool)corpse)
			{
				FollowTransform component4 = gameObject3.GetComponent<FollowTransform>();
				if ((bool)component4)
				{
					component4.Target = corpse;
					component4.Offset = Vector3.zero;
				}
			}
		}
		FlingUtils.Config[] array3 = spawnFlings;
		for (int i = 0; i < array3.Length; i++)
		{
			FlingUtils.SpawnAndFling(array3[i], spawnPoint, offset, null, blackThreadAmount);
		}
		AudioEvent[] array4 = deathSounds;
		foreach (AudioEvent audioEvent in array4)
		{
			audioEvent.SpawnAndPlayOneShot(position);
		}
	}

	public void EnsurePersonalPool(GameObject gameObject)
	{
		for (int i = 0; i < spawnFlings.Length; i++)
		{
			FlingUtils.Config config = spawnFlings[i];
			if (!(config.Prefab == null))
			{
				PersonalObjectPool.EnsurePooledInScene(gameObject, config.Prefab, 3, finished: false);
			}
		}
		for (int j = 0; j < spawnEffectPrefabs.Length; j++)
		{
			GameObject prefab = spawnEffectPrefabs[j];
			PersonalObjectPool.EnsurePooledInScene(gameObject, prefab, 3, finished: false);
		}
		PersonalObjectPool.CreateIfRequired(gameObject);
	}
}

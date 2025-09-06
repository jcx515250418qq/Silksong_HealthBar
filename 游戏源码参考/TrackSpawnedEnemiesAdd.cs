using HutongGames.PlayMaker;
using UnityEngine;

[ActionCategory("Hollow Knight")]
public class TrackSpawnedEnemiesAdd : FsmStateAction
{
	public FsmOwnerDefault Target;

	public FsmGameObject SpawnedEnemy;

	public FsmBool UsesEnemySpawner;

	public override void Reset()
	{
		Target = null;
		SpawnedEnemy = null;
		UsesEnemySpawner = null;
	}

	public override void OnEnter()
	{
		GameObject safe = Target.GetSafe(this);
		if ((bool)safe && (bool)SpawnedEnemy.Value)
		{
			TrackSpawnedEnemies track = safe.GetComponent<TrackSpawnedEnemies>() ?? safe.AddComponent<TrackSpawnedEnemies>();
			if (UsesEnemySpawner.Value)
			{
				EnemySpawner component = SpawnedEnemy.Value.GetComponent<EnemySpawner>();
				if ((bool)component)
				{
					component.OnEnemySpawned += delegate(GameObject enemy)
					{
						AddTracked(track, enemy);
					};
				}
			}
			else
			{
				AddTracked(track, SpawnedEnemy.Value);
			}
		}
		Finish();
	}

	private void AddTracked(TrackSpawnedEnemies tracker, GameObject obj)
	{
		HealthManager component = obj.GetComponent<HealthManager>();
		if ((bool)component)
		{
			tracker.Add(component);
		}
	}
}

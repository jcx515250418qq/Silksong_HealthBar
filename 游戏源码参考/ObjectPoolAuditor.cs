using UnityEngine;

public static class ObjectPoolAuditor
{
	public static void RecordPoolCreated(GameObject prefab, int initialPoolSize)
	{
	}

	public static void RecordSpawned(GameObject prefab, bool didInstantiate)
	{
	}

	public static void RecordDespawned(GameObject instanceOrPrefab, bool willReuse)
	{
	}
}

using System;
using UnityEngine;

[Serializable]
public struct StartupPool
{
	public int size;

	public GameObject prefab;

	[NonSerialized]
	public int SpawnedCount;

	public bool initialiseSpawnedObjects;

	public bool shared;

	public StartupPool(int size, GameObject prefab, bool initialiseSpawnedObjects = false, bool shared = false)
	{
		this.size = size;
		this.prefab = prefab;
		SpawnedCount = 0;
		this.initialiseSpawnedObjects = initialiseSpawnedObjects;
		this.shared = shared;
	}
}

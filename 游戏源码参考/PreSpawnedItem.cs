using System;
using UnityEngine;

public class PreSpawnedItem : IDisposable
{
	private GameObject spawnedObject;

	private bool recycle;

	private bool disposed;

	private bool hasAwaken;

	private bool hasStarted;

	private IInitialisable[] children;

	public GameObject SpawnedObject => spawnedObject;

	public PreSpawnedItem(GameObject spawnedObject, bool recycle)
	{
		this.spawnedObject = spawnedObject;
		this.recycle = recycle;
	}

	~PreSpawnedItem()
	{
		Dispose();
	}

	public void Dispose()
	{
		if (disposed)
		{
			return;
		}
		disposed = true;
		if (spawnedObject != null)
		{
			if (recycle)
			{
				spawnedObject.Recycle();
			}
			else
			{
				spawnedObject.SetActive(value: false);
			}
		}
	}

	public bool OnAwake()
	{
		if (hasAwaken)
		{
			return false;
		}
		hasAwaken = true;
		if (spawnedObject != null)
		{
			children = spawnedObject.GetComponentsInChildren<IInitialisable>(includeInactive: true);
			IInitialisable[] array = children;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].OnAwake();
			}
		}
		return true;
	}

	public bool OnStart()
	{
		OnAwake();
		if (hasStarted)
		{
			return false;
		}
		hasStarted = true;
		if (children != null)
		{
			IInitialisable[] array = children;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].OnStart();
			}
		}
		return true;
	}
}

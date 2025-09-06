using TeamCherry.ObjectPool;
using UnityEngine;

public class PooledEffectTracker<T> where T : PooledEffect, IPoolable<T>
{
	private int spawnedCount;

	private PooledEffectProfile profile;

	private ObjectPool<T> effectPool;

	private UniqueList<T> recycleQueue;

	public PooledEffectTracker(PooledEffectProfile profile)
	{
		this.profile = profile;
		recycleQueue = new UniqueList<T>();
		effectPool = new ObjectPool<T>(CreateNewAction, OnGet, OnRelease, OnDestroy, profile.InitialSize);
	}

	private T CreateNewAction()
	{
		if (profile.EffectPrefab is T original)
		{
			T val = Object.Instantiate(original, PooledEffectManager.Instance.transform);
			val.SetReleaser(profile);
			return val;
		}
		return null;
	}

	private void OnGet(T element)
	{
		spawnedCount++;
	}

	private void OnRelease(T element)
	{
		ReduceSpawnCount();
		element.OnRelease();
		element.transform.SetParent(PooledEffectManager.Instance.transform, worldPositionStays: false);
	}

	public void OnDestroy(T element)
	{
		if (element != null)
		{
			recycleQueue.Remove(element);
			element.ClearReleaser();
			ReduceSpawnCount();
		}
	}

	public void ReduceSpawnCount()
	{
		spawnedCount = Mathf.Max(0, --spawnedCount);
	}

	public void Update()
	{
		recycleQueue.ReserveListUsage();
		foreach (T item in recycleQueue.List)
		{
			if (item.CanRelease())
			{
				ReleaseEffect(item);
			}
		}
		recycleQueue.ReleaseListUsage();
	}

	public void ReleaseEffect(T effect)
	{
		recycleQueue.Remove(effect);
		effectPool.Release(effect);
	}

	public bool EnqueueRelease(T effect)
	{
		return recycleQueue.Add(effect);
	}

	public bool TryGet(out T effect)
	{
		if (spawnedCount >= profile.PoolCapacity)
		{
			effect = null;
			return false;
		}
		effect = effectPool.Get();
		return true;
	}

	public void Clear()
	{
		effectPool?.Clear();
	}
}

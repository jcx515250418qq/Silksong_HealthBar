using TeamCherry.ObjectPool;
using UnityEngine;

public sealed class PooledEffectManager : MonoBehaviour
{
	private static UniqueList<PooledEffectProfile> profiles = new UniqueList<PooledEffectProfile>();

	private static bool hasInstance;

	private static PooledEffectManager instance;

	public static PooledEffectManager Instance
	{
		get
		{
			if (!hasInstance)
			{
				instance = Object.FindObjectOfType<PooledEffectManager>();
				if (instance == null)
				{
					instance = new GameObject("PooledEffectManager").AddComponent<PooledEffectManager>();
					Object.DontDestroyOnLoad(instance.gameObject);
				}
				hasInstance = true;
			}
			return instance;
		}
	}

	private void Awake()
	{
		if (instance != null && instance != this)
		{
			base.enabled = false;
			return;
		}
		hasInstance = true;
		instance = this;
	}

	private void OnDestroy()
	{
		if (!hasInstance || !(instance == this))
		{
			return;
		}
		profiles.ReserveListUsage();
		foreach (PooledEffectProfile item in profiles.List)
		{
			item.Clear();
		}
		profiles.Clear();
		profiles.ReleaseListUsage();
	}

	private void Update()
	{
		profiles.ReserveListUsage();
		foreach (PooledEffectProfile item in profiles.List)
		{
			item.Update();
		}
		profiles.ReleaseListUsage();
	}

	public static PooledEffectTracker<T> InitProfile<T>(PooledEffectProfile profile) where T : PooledEffect, IPoolable<T>
	{
		profiles.Add(profile);
		return new PooledEffectTracker<T>(profile);
	}
}

using TeamCherry.ObjectPool;
using UnityEngine;

public class PooledEffect : MonoBehaviour, IPoolable<PooledEffect>
{
	private PooledEffectProfile pooledEffectProfile;

	private bool spawned;

	private int releaseState;

	private void OnDestroy()
	{
		if (pooledEffectProfile != null)
		{
			pooledEffectProfile.NotifyDestroyed(this);
			pooledEffectProfile = null;
		}
	}

	private void OnDisable()
	{
		Release();
	}

	public void SetReleaser(IPoolReleaser<PooledEffect> releaser)
	{
		if (releaser is PooledEffectProfile pooledEffectProfile)
		{
			this.pooledEffectProfile = pooledEffectProfile;
		}
	}

	public void ClearReleaser()
	{
		pooledEffectProfile = null;
	}

	public void Release()
	{
		if (!spawned)
		{
			if (base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(value: false);
			}
		}
		else if ((bool)pooledEffectProfile)
		{
			pooledEffectProfile.Release(this);
		}
	}

	public virtual void OnSpawn()
	{
		spawned = true;
		releaseState = 0;
		base.gameObject.SetActive(value: true);
	}

	public virtual void OnRelease()
	{
		spawned = false;
		base.gameObject.SetActive(value: false);
	}

	public virtual bool CanRelease()
	{
		if (releaseState >= 2)
		{
			return true;
		}
		Transform parent = base.transform.parent;
		if (parent == null || parent.gameObject.activeInHierarchy)
		{
			return true;
		}
		releaseState++;
		return false;
	}
}

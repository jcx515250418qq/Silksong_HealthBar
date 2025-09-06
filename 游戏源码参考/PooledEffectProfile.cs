using TeamCherry.ObjectPool;
using UnityEngine;

[CreateAssetMenu(fileName = "Pooled Effect Profile", menuName = "Pooled Effect Profile")]
public sealed class PooledEffectProfile : ScriptableObject, IPoolReleaser<PooledEffect>
{
	[SerializeField]
	private PooledEffect effectPrefab;

	[SerializeField]
	private int initialSize = 10;

	[SerializeField]
	private int poolCapacity = 10;

	private PooledEffectTracker<PooledEffect> effectTracker;

	public PooledEffect EffectPrefab => effectPrefab;

	public int InitialSize => initialSize;

	public int PoolCapacity => poolCapacity;

	public void Init()
	{
		if (effectTracker == null)
		{
			effectTracker = PooledEffectManager.InitProfile<PooledEffect>(this);
		}
	}

	public void Update()
	{
		effectTracker?.Update();
	}

	public void Release(PooledEffect element)
	{
		if (element == null)
		{
			return;
		}
		if (effectTracker != null)
		{
			if (element.CanRelease())
			{
				effectTracker.ReleaseEffect(element);
			}
			else
			{
				effectTracker.EnqueueRelease(element);
			}
		}
		else
		{
			Object.Destroy(element.gameObject);
		}
	}

	public void NotifyDestroyed(PooledEffect element)
	{
		effectTracker?.OnDestroy(element);
	}

	public void Clear()
	{
		effectTracker?.Clear();
		effectTracker = null;
	}

	public void SpawnEffect(Transform parent)
	{
		SpawnEffect(Vector3.zero, Quaternion.identity, parent);
	}

	public void SpawnEffect(Vector3 position, Quaternion rotation, Transform parent)
	{
		Init();
		if (effectTracker != null && effectTracker.TryGet(out var effect))
		{
			effect.transform.SetParent(parent, worldPositionStays: false);
			effect.transform.localPosition = position;
			effect.transform.localRotation = rotation;
			effect.OnSpawn();
		}
	}
}

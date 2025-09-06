using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PersonalObjectPool : MonoBehaviour, IInitialisable
{
	public List<StartupPool> startupPool;

	[SerializeField]
	[Tooltip("Will steal and aggregate all child pools. Good for optimising pooling by reducing the amount of objects pooled, assuming each pool wont need all at once.")]
	private GameObject stealFromParent;

	private bool createdStartupPools;

	private bool startFinished;

	private bool poolsAdded;

	private string sceneName;

	public static readonly List<PersonalObjectPool> _activePoolManagers = new List<PersonalObjectPool>();

	private static readonly List<PersonalObjectPool> _inactivePoolManagers = new List<PersonalObjectPool>();

	private static readonly Dictionary<GameObject, int> _extraSpawnedCount = new Dictionary<GameObject, int>();

	private static readonly List<GameObject> _tempList = new List<GameObject>();

	private bool hasAwaken;

	private bool hasStarted;

	public GameObject StealFromParent => stealFromParent;

	GameObject IInitialisable.gameObject => base.gameObject;

	public PersonalObjectPool(bool createdStartupPools)
	{
		this.createdStartupPools = createdStartupPools;
	}

	public bool OnAwake()
	{
		if (hasAwaken)
		{
			return false;
		}
		hasAwaken = true;
		sceneName = base.gameObject.scene.name;
		_activePoolManagers.Add(this);
		if (startupPool == null)
		{
			startupPool = new List<StartupPool>();
		}
		if ((bool)stealFromParent)
		{
			List<StartupPool> list = new List<StartupPool>();
			PersonalObjectPool[] componentsInChildren = stealFromParent.GetComponentsInChildren<PersonalObjectPool>(includeInactive: true);
			for (int num = componentsInChildren.Length - 1; num >= 0; num--)
			{
				PersonalObjectPool personalObjectPool = componentsInChildren[num];
				if (!(personalObjectPool == this) && personalObjectPool.startupPool != null)
				{
					for (int num2 = personalObjectPool.startupPool.Count - 1; num2 >= 0; num2--)
					{
						StartupPool item = personalObjectPool.startupPool[num2];
						if (item.SpawnedCount <= 0)
						{
							list.Add(item);
							personalObjectPool.startupPool.RemoveAt(num2);
						}
					}
				}
			}
			IEnumerable<StartupPool> collection = (from pool in list
				group pool by pool.prefab).SelectMany((IGrouping<GameObject, StartupPool> group) => group.OrderByDescending((StartupPool pool) => pool.size).Take(1)).Select(delegate(StartupPool pool)
			{
				pool.size *= 2;
				return pool;
			});
			startupPool.AddRange(collection);
		}
		for (int num3 = startupPool.Count - 1; num3 >= 0; num3--)
		{
			if (!startupPool[num3].prefab)
			{
				startupPool.RemoveAt(num3);
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
		CreateStartupPools();
		startFinished = true;
		return true;
	}

	protected void Awake()
	{
		OnAwake();
	}

	private void Start()
	{
		OnStart();
	}

	private void OnDestroy()
	{
		if (!createdStartupPools)
		{
			_activePoolManagers.Remove(this);
		}
	}

	public static void PreUnloadingScene(string unloadingSceneName)
	{
		for (int num = _activePoolManagers.Count - 1; num >= 0; num--)
		{
			PersonalObjectPool personalObjectPool = _activePoolManagers[num];
			if ((bool)personalObjectPool)
			{
				string baseSceneName = GameManager.GetBaseSceneName(personalObjectPool.sceneName);
				if (unloadingSceneName != baseSceneName && unloadingSceneName != personalObjectPool.sceneName)
				{
					continue;
				}
			}
			_activePoolManagers.RemoveAt(num);
			_inactivePoolManagers.Add(personalObjectPool);
		}
	}

	public static void UnloadingScene(string unloadingSceneName)
	{
		foreach (PersonalObjectPool inactivePoolManager in _inactivePoolManagers)
		{
			inactivePoolManager.DestroyMyPooledObjects();
		}
		_inactivePoolManagers.Clear();
	}

	public static void ForceReleasePoolManagers()
	{
		foreach (PersonalObjectPool activePoolManager in _activePoolManagers)
		{
			activePoolManager.DestroyMyPooledObjects();
		}
		foreach (PersonalObjectPool inactivePoolManager in _inactivePoolManagers)
		{
			inactivePoolManager.DestroyMyPooledObjects();
		}
		_activePoolManagers.Clear();
		_inactivePoolManagers.Clear();
	}

	public void CreateStartupPools()
	{
		createdStartupPools = true;
		List<StartupPool> list = startupPool;
		if (list == null || list.Count <= 0)
		{
			return;
		}
		poolsAdded = false;
		for (int i = 0; i < startupPool.Count; i++)
		{
			StartupPool value = startupPool[i];
			GameObject prefab = value.prefab;
			if (!prefab)
			{
				continue;
			}
			int num = value.size - value.SpawnedCount;
			if (num <= 0)
			{
				continue;
			}
			bool flag = _extraSpawnedCount.ContainsKey(prefab);
			if (flag)
			{
				int num2 = _extraSpawnedCount[prefab];
				while (num > 0 && num2 > 0)
				{
					num--;
					num2--;
				}
			}
			if (num > 0)
			{
				ObjectPool.GetPooled(prefab, _tempList, appendList: true);
				int count = _tempList.Count;
				_tempList.Clear();
				CreatePool(prefab, num, value.initialiseSpawnedObjects, value.shared);
				ObjectPool.GetPooled(prefab, _tempList, appendList: true);
				int count2 = _tempList.Count;
				_tempList.Clear();
				int num3 = count2 - count;
				value.SpawnedCount += num3;
				startupPool[i] = value;
				if (!flag)
				{
					_extraSpawnedCount.Add(prefab, 0);
				}
			}
		}
	}

	public void CreatePool(GameObject prefab, int initialPoolSize, bool runInitialisation, bool shared)
	{
		int num = initialPoolSize;
		if (num > 0)
		{
			if (!shared)
			{
				ObjectPool.GetPooled(prefab, _tempList, appendList: true);
				num += _tempList.Count;
			}
			ObjectPool.CreatePool(prefab, num, runInitialisation);
			_tempList.Clear();
		}
	}

	public void DestroyMyPooledObjects()
	{
		if (!createdStartupPools)
		{
			return;
		}
		createdStartupPools = false;
		for (int i = 0; i < startupPool.Count; i++)
		{
			StartupPool value = startupPool[i];
			if (value.SpawnedCount <= 0)
			{
				continue;
			}
			for (int j = 0; j < 2; j++)
			{
				int num = 0;
				do
				{
					num = 0;
					bool flag = false;
					foreach (PersonalObjectPool activePoolManager in _activePoolManagers)
					{
						for (int k = 0; k < activePoolManager.startupPool.Count; k++)
						{
							StartupPool value2 = activePoolManager.startupPool[k];
							if (!(value2.prefab != value.prefab) && (j != 0 || value2.SpawnedCount < value2.size))
							{
								num++;
								value2.SpawnedCount++;
								value.SpawnedCount--;
								activePoolManager.startupPool[k] = value2;
								startupPool[i] = value;
								if (value.SpawnedCount == 0)
								{
									flag = true;
									break;
								}
							}
						}
						if (flag)
						{
							break;
						}
					}
				}
				while (value.SpawnedCount > 0 && num > 0);
			}
		}
		List<GameObject> list = new List<GameObject>();
		if (startupPool != null && startupPool.Count > 0)
		{
			foreach (StartupPool item in startupPool)
			{
				if (item.SpawnedCount > 0 && (bool)item.prefab)
				{
					ObjectPool.DestroyPooled(item.prefab, item.SpawnedCount);
					if (!list.Contains(item.prefab))
					{
						list.Add(item.prefab);
					}
				}
			}
		}
		foreach (GameObject item2 in list)
		{
			if (_extraSpawnedCount.ContainsKey(item2))
			{
				int num2 = _extraSpawnedCount[item2];
				if (num2 > 0)
				{
					ObjectPool.DestroyPooled(item2, num2);
				}
				_extraSpawnedCount.Remove(item2);
			}
		}
	}

	public static void RegisterNewSpawned(GameObject prefab, int amount)
	{
		if (_extraSpawnedCount.ContainsKey(prefab))
		{
			_extraSpawnedCount[prefab] += amount;
		}
	}

	public static void EnsurePooledInScene(GameObject ownerObj, GameObject prefab, int poolAmount, bool finished = true, bool initialiseSpawned = false, bool shared = false)
	{
		GameManager.instance.EnsureGlobalPool();
		int num = ObjectPool.CountPooled(prefab);
		int size = Mathf.Max(0, poolAmount - num);
		PersonalObjectPool personalObjectPool = ownerObj.AddComponentIfNotPresent<PersonalObjectPool>();
		personalObjectPool.OnAwake();
		if (personalObjectPool.startupPool.All((StartupPool pool) => pool.prefab != prefab))
		{
			personalObjectPool.startupPool.Add(new StartupPool(size, prefab, initialiseSpawned, shared));
			personalObjectPool.poolsAdded = true;
		}
		if (finished && personalObjectPool.startFinished)
		{
			personalObjectPool.CreateStartupPools();
		}
	}

	public static void EnsurePooledInSceneFinished(GameObject ownerObj)
	{
		PersonalObjectPool component = ownerObj.GetComponent<PersonalObjectPool>();
		if ((bool)component)
		{
			component.CreateStartupPools();
		}
	}

	public static void CreateIfRequired(GameObject gameObject, bool forced = false)
	{
		PersonalObjectPool component = gameObject.GetComponent<PersonalObjectPool>();
		if (component != null && component.poolsAdded)
		{
			if (component.startFinished)
			{
				component.CreateStartupPools();
			}
			else if (forced)
			{
				component.OnStart();
			}
		}
	}
}

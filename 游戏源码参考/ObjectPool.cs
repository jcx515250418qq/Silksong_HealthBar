using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class ObjectPool : MonoBehaviour
{
	[Serializable]
	public class StartupPool
	{
		public int size;

		public GameObject prefab;
	}

	private class PoolCounter
	{
		public int startupCount;

		public int personalPoolCount;

		public int sharedPoolCount;

		public int RequiredCount => Mathf.Max(startupCount, personalPoolCount, sharedPoolCount);
	}

	private static readonly List<GameObject> _tempList = new List<GameObject>();

	private static readonly Queue<List<GameObject>> _spareGameObjectPools = new Queue<List<GameObject>>();

	private readonly Dictionary<GameObject, List<GameObject>> pooledObjects = new Dictionary<GameObject, List<GameObject>>();

	private readonly Dictionary<GameObject, GameObject> spawnedObjects = new Dictionary<GameObject, GameObject>();

	private static readonly HashSet<GameObject> recentlyRecycled = new HashSet<GameObject>();

	public StartupPool[] startupPools;

	public bool isPrimary;

	private bool startupPoolsCreated;

	private static readonly Vector2 activeStashLocation = new Vector2(-20f, -20f);

	private static ObjectPool _instance;

	private static bool hasInstance;

	private readonly List<GameObject> noInstancePrefabs = new List<GameObject>();

	private static List<GameObject> reparentList = new List<GameObject>();

	public static bool IsCreatingPool { get; private set; }

	public static ObjectPool instance
	{
		get
		{
			if (!hasInstance)
			{
				_instance = UnityEngine.Object.FindObjectOfType<ObjectPool>();
				hasInstance = _instance;
				if (hasInstance && _instance.isPrimary)
				{
					UnityEngine.Object.DontDestroyOnLoad(_instance.gameObject);
				}
			}
			return _instance;
		}
	}

	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
			if (isPrimary)
			{
				UnityEngine.Object.DontDestroyOnLoad(this);
			}
		}
		else if (_instance != this)
		{
			if (isPrimary && !_instance.isPrimary)
			{
				UnityEngine.Object.Destroy(_instance.gameObject);
				_instance = this;
				UnityEngine.Object.DontDestroyOnLoad(this);
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}

	private void OnDestroy()
	{
		if (_instance == this)
		{
			hasInstance = false;
			_instance = null;
		}
	}

	private void LateUpdate()
	{
		if (reparentList.Count <= 0)
		{
			return;
		}
		for (int num = reparentList.Count - 1; num >= 0; num--)
		{
			GameObject gameObject = reparentList[num];
			if (gameObject == null)
			{
				reparentList.RemoveAt(num);
			}
			else
			{
				gameObject.transform.parent = base.transform;
				if (!(gameObject.transform.parent != base.transform))
				{
					reparentList.RemoveAt(num);
				}
			}
		}
	}

	public static void CreateStartupPools()
	{
		if (instance.startupPoolsCreated)
		{
			return;
		}
		instance.startupPoolsCreated = true;
		StartupPool[] array = instance.startupPools;
		if (array != null && array.Length > 0)
		{
			StartupPool[] array2 = array;
			foreach (StartupPool startupPool in array2)
			{
				CreatePool(startupPool.prefab, startupPool.size, runInitialisation: true);
			}
		}
	}

	public static void CreatePool<T>(T prefab, int initialPoolSize, bool runInitialisation = false) where T : Component
	{
		CreatePool(prefab, initialPoolSize, setPosition: false, Vector3.zero, Quaternion.identity, runInitialisation);
	}

	public static void CreatePool<T>(T prefab, int initialPoolSize, bool setPosition, Vector3 position, Quaternion rotation, bool runInitialisation = false) where T : Component
	{
		CreatePool(prefab.gameObject, initialPoolSize, setPosition, position, rotation, runInitialisation);
	}

	public static void CreatePool(GameObject prefab, int initialPoolSize, bool runInitialisation = false)
	{
		CreatePool(prefab, initialPoolSize, setPosition: false, Vector3.zero, Quaternion.identity, runInitialisation);
	}

	public static void CreatePool(GameObject prefab, int initialPoolSize, bool setPosition, Vector3 position, Quaternion rotation, bool runInitialisation = false)
	{
		ObjectPoolAuditor.RecordPoolCreated(prefab, initialPoolSize);
		if ((bool)prefab)
		{
			if (!instance.pooledObjects.TryGetValue(prefab, out var value))
			{
				value = ((_spareGameObjectPools.Count > 0) ? _spareGameObjectPools.Dequeue() : new List<GameObject>());
				instance.pooledObjects.Add(prefab, value);
			}
			CreatePooledObjects(prefab, initialPoolSize, value, setPosition, position, rotation);
			if (value == null)
			{
				return;
			}
			{
				foreach (GameObject item in value)
				{
					if ((bool)item)
					{
						tk2dSprite[] componentsInChildren = item.GetComponentsInChildren<tk2dSprite>(includeInactive: true);
						for (int i = 0; i < componentsInChildren.Length; i++)
						{
							componentsInChildren[i].ForceBuild();
						}
						if (runInitialisation)
						{
							IInitialisable.DoFullInitForcePool(item);
						}
					}
				}
				return;
			}
		}
		_ = (bool)prefab;
	}

	private static void CreatePooledObjects(GameObject prefab, int initialPoolSize, ICollection<GameObject> pooledList, bool setPosition, Vector3 position, Quaternion rotation)
	{
		if (initialPoolSize <= 0)
		{
			return;
		}
		bool flag = prefab.GetComponent<ActiveRecycler>();
		Transform parent = instance.transform;
		if (!setPosition)
		{
			IsCreatingPool = true;
		}
		try
		{
			while (pooledList.Count < initialPoolSize)
			{
				GameObject gameObject = (setPosition ? UnityEngine.Object.Instantiate(prefab, position, rotation, parent) : UnityEngine.Object.Instantiate(prefab, parent, worldPositionStays: true));
				if (flag)
				{
					gameObject.SetActive(value: true);
					SetActiveRecycled(gameObject);
				}
				else if (!setPosition)
				{
					gameObject.SetActive(value: false);
				}
				pooledList.Add(gameObject);
			}
		}
		finally
		{
			IsCreatingPool = false;
		}
	}

	public void RevertToStartState()
	{
		noInstancePrefabs.Clear();
		Dictionary<GameObject, PoolCounter> dictionary = new Dictionary<GameObject, PoolCounter>();
		for (int i = 0; i < startupPools.Length; i++)
		{
			StartupPool startupPool = startupPools[i];
			if (!(startupPool.prefab == null))
			{
				if (!dictionary.TryGetValue(startupPool.prefab, out var value))
				{
					value = (dictionary[startupPool.prefab] = new PoolCounter());
				}
				value.startupCount = Mathf.Max(value.startupCount, startupPool.size);
			}
		}
		try
		{
			foreach (PersonalObjectPool activePoolManager in PersonalObjectPool._activePoolManagers)
			{
				if (activePoolManager == null || activePoolManager.startupPool == null)
				{
					continue;
				}
				for (int j = 0; j < activePoolManager.startupPool.Count; j++)
				{
					global::StartupPool startupPool2 = activePoolManager.startupPool[j];
					if (!(startupPool2.prefab == null))
					{
						if (!dictionary.TryGetValue(startupPool2.prefab, out var value2))
						{
							value2 = (dictionary[startupPool2.prefab] = new PoolCounter());
						}
						if ((bool)activePoolManager.StealFromParent)
						{
							value2.sharedPoolCount = Mathf.Max(value2.sharedPoolCount, startupPool2.size);
						}
						else
						{
							value2.personalPoolCount += startupPool2.size;
						}
					}
				}
			}
		}
		catch (Exception message)
		{
			Debug.LogError(message);
		}
		foreach (KeyValuePair<GameObject, List<GameObject>> pooledObject in pooledObjects)
		{
			GameObject key = pooledObject.Key;
			List<GameObject> value3 = pooledObject.Value;
			int num = 0;
			if ((bool)key && dictionary.TryGetValue(key, out var value4))
			{
				num = value4.RequiredCount;
			}
			for (int num2 = value3.Count - 1; num2 >= 0; num2--)
			{
				if (!value3[num2])
				{
					value3.RemoveAt(num2);
				}
			}
			while (value3.Count > num)
			{
				UnityEngine.Object.Destroy(value3[0]);
				value3.RemoveAt(0);
			}
			if (num == 0)
			{
				noInstancePrefabs.Add(key);
			}
			else if (value3.Count < num)
			{
				CreatePool(key, num - value3.Count);
			}
		}
		foreach (GameObject noInstancePrefab in noInstancePrefabs)
		{
			pooledObjects.Remove(noInstancePrefab);
		}
		noInstancePrefabs.Clear();
		PurgeRecentRecycled();
		AuditSpawnedDictionary();
	}

	public static void PurgeRecentRecycled()
	{
		recentlyRecycled.Clear();
	}

	public static void AuditSpawnedDictionary()
	{
		ObjectPool objectPool = instance;
		if (objectPool == null)
		{
			return;
		}
		List<GameObject> list = new List<GameObject>(objectPool.spawnedObjects.Count);
		foreach (GameObject key in objectPool.spawnedObjects.Keys)
		{
			if (key == null)
			{
				list.Add(key);
			}
		}
		foreach (GameObject item in list)
		{
			objectPool.spawnedObjects.Remove(item);
		}
		bool flag = list.Count > 0;
		list.Clear();
		if (!flag)
		{
			return;
		}
		foreach (KeyValuePair<GameObject, List<GameObject>> pooledObject in objectPool.pooledObjects)
		{
			pooledObject.Value.RemoveAll((GameObject o) => o == null);
		}
	}

	public static T Spawn<T>(T prefab, Transform parent, Vector3 position, Quaternion rotation) where T : Component
	{
		return Spawn(prefab.gameObject, parent, position, rotation).GetComponent<T>();
	}

	public static T Spawn<T>(T prefab, Vector3 position, Quaternion rotation) where T : Component
	{
		return Spawn(prefab.gameObject, null, position, rotation).GetComponent<T>();
	}

	public static T Spawn<T>(T prefab, Transform parent, Vector3 position) where T : Component
	{
		return Spawn(prefab.gameObject, parent, position, Quaternion.identity).GetComponent<T>();
	}

	public static T Spawn<T>(T prefab, Vector3 position) where T : Component
	{
		return Spawn(prefab.gameObject, null, position, Quaternion.identity).GetComponent<T>();
	}

	public static T Spawn<T>(T prefab, Transform parent) where T : Component
	{
		return Spawn(prefab.gameObject, parent, Vector3.zero, Quaternion.identity).GetComponent<T>();
	}

	public static T Spawn<T>(T prefab) where T : Component
	{
		return Spawn(prefab.gameObject, null, Vector3.zero, Quaternion.identity).GetComponent<T>();
	}

	public static GameObject Spawn(GameObject prefab, Transform parent, Vector3 position, Quaternion rotation)
	{
		return Spawn(prefab, parent, position, rotation, stealActiveSpawned: false);
	}

	public static GameObject Spawn(GameObject prefab, Transform parent, Vector3 position, Quaternion rotation, bool stealActiveSpawned = false)
	{
		if (prefab == null)
		{
			return null;
		}
		bool flag = false;
		bool activeRecyclerType = prefab.GetComponent<ActiveRecycler>() != null;
		if (!stealActiveSpawned && (bool)prefab.GetComponent<CurrencyObjectBase>())
		{
			stealActiveSpawned = true;
		}
		ObjectPool objectPool = instance;
		GameObject gameObject;
		if (objectPool.pooledObjects.TryGetValue(prefab, out var value))
		{
			gameObject = null;
			bool flag2 = false;
			while (value.Count > 0 && !flag2)
			{
				int index = value.Count - 1;
				gameObject = value[index];
				flag2 = gameObject;
				value.RemoveAt(index);
			}
			if (stealActiveSpawned && (value.Count <= 1 || !flag2))
			{
				Vector2 vector = HeroController.instance.transform.position;
				float num = 0f;
				GameObject gameObject2 = null;
				foreach (KeyValuePair<GameObject, GameObject> spawnedObject in objectPool.spawnedObjects)
				{
					if (spawnedObject.Value != prefab)
					{
						continue;
					}
					GameObject key = spawnedObject.Key;
					if (!(key == null))
					{
						Vector3 position2 = key.transform.position;
						float num2 = Vector2.SqrMagnitude(vector - new Vector2(position2.x, position2.y));
						if (num2 > num || !(gameObject2 != null))
						{
							gameObject2 = key;
							num = num2;
						}
					}
				}
				if (gameObject2 != null)
				{
					if (!flag2)
					{
						RecycleProcess(gameObject2);
						gameObject = gameObject2;
						flag2 = true;
						flag = true;
					}
					else
					{
						DropRecycle component = gameObject2.GetComponent<DropRecycle>();
						if ((bool)component)
						{
							component.StartDrop();
						}
					}
				}
			}
			if (flag2)
			{
				Transform obj = gameObject.transform;
				obj.parent = parent;
				obj.localPosition = position;
				obj.localRotation = rotation;
				ActivateSpawningObject(gameObject, activeRecyclerType);
				if (!flag)
				{
					objectPool.AddSpawnedObject(gameObject, prefab);
					ObjectPoolAuditor.RecordSpawned(prefab, didInstantiate: false);
				}
				return gameObject;
			}
			gameObject = UnityEngine.Object.Instantiate(prefab, position, rotation, parent);
			Transform obj2 = gameObject.transform;
			obj2.localPosition = position;
			obj2.localRotation = rotation;
			ActivateSpawningObject(gameObject, activeRecyclerType);
			objectPool.AddSpawnedObject(gameObject, prefab);
			ObjectPoolAuditor.RecordSpawned(prefab, didInstantiate: true);
			PersonalObjectPool.RegisterNewSpawned(prefab, 1);
			return gameObject;
		}
		CreatePool(prefab.gameObject, 1, setPosition: true, position, rotation);
		gameObject = Spawn(prefab, parent, position, rotation);
		gameObject.SetActive(value: true);
		return gameObject;
	}

	public static bool ObjectWasSpawned(GameObject gameObject)
	{
		if (instance == null)
		{
			return false;
		}
		GameObject value;
		return instance.spawnedObjects.TryGetValue(gameObject, out value);
	}

	private static void ActivateSpawningObject(GameObject obj, bool activeRecyclerType)
	{
		if (activeRecyclerType)
		{
			FSMUtility.SendEventToGameObject(obj, "A SPAWN");
		}
		else
		{
			obj.SetActive(value: true);
		}
	}

	private void AddSpawnedObject(GameObject obj, GameObject prefab)
	{
		spawnedObjects.TryAdd(obj, prefab);
	}

	private bool RemoveSpawnedObject(GameObject obj)
	{
		return spawnedObjects.Remove(obj);
	}

	public static GameObject Spawn(GameObject prefab, Transform parent, Vector3 position)
	{
		return Spawn(prefab, parent, position, Quaternion.identity);
	}

	public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
	{
		return Spawn(prefab, null, position, rotation);
	}

	public static GameObject Spawn(GameObject prefab, Transform parent)
	{
		return Spawn(prefab, parent, Vector3.zero, Quaternion.identity);
	}

	public static GameObject Spawn(GameObject prefab, Vector3 position)
	{
		return Spawn(prefab, null, position, Quaternion.identity);
	}

	public static GameObject Spawn(GameObject prefab)
	{
		return Spawn(prefab, null, Vector3.zero, Quaternion.identity);
	}

	public static void Recycle<T>(T obj) where T : Component
	{
		Recycle(obj.gameObject);
	}

	public static void Recycle(GameObject obj)
	{
		if ((bool)obj)
		{
			RecycleResetHandler component = obj.GetComponent<RecycleResetHandler>();
			if ((bool)component)
			{
				component.OnPreRecycle();
			}
			if (instance != null && instance.spawnedObjects.TryGetValue(obj, out var value))
			{
				Recycle(obj, value);
			}
			else if (!recentlyRecycled.Contains(obj))
			{
				ObjectPoolAuditor.RecordDespawned(obj, willReuse: false);
				UnityEngine.Object.Destroy(obj);
			}
		}
	}

	private static void Recycle(GameObject obj, GameObject prefab)
	{
		if (obj == null || prefab == null)
		{
			return;
		}
		bool flag = instance.RemoveSpawnedObject(obj);
		if (instance.pooledObjects.TryGetValue(prefab, out var value))
		{
			if (flag)
			{
				recentlyRecycled.Add(obj);
				value.Add(obj);
			}
			RecycleProcess(obj);
			ObjectPoolAuditor.RecordDespawned(obj, willReuse: true);
		}
		else
		{
			ObjectPoolAuditor.RecordDespawned(obj, willReuse: false);
			UnityEngine.Object.Destroy(obj);
		}
	}

	private static void RecycleProcess(GameObject obj)
	{
		ResetDynamicHierarchy component = obj.GetComponent<ResetDynamicHierarchy>();
		if ((bool)component)
		{
			component.DoReset();
		}
		if (!obj.activeInHierarchy)
		{
			reparentList.Add(obj.gameObject);
		}
		else
		{
			obj.transform.parent = instance.transform;
		}
		if (obj.GetComponent<ActiveRecycler>() != null)
		{
			SetActiveRecycled(obj);
		}
		else
		{
			obj.SetActive(value: false);
		}
	}

	private static void SetActiveRecycled(GameObject obj)
	{
		obj.transform.SetPosition2D(activeStashLocation);
		FSMUtility.SendEventToGameObject(obj, "A RECYCLE");
	}

	public static void RecycleAll<T>(T prefab) where T : Component
	{
		RecycleAll(prefab.gameObject);
	}

	public static void RecycleAll(GameObject prefab)
	{
		foreach (KeyValuePair<GameObject, GameObject> spawnedObject in instance.spawnedObjects)
		{
			if (spawnedObject.Value == prefab)
			{
				_tempList.Add(spawnedObject.Key);
			}
		}
		for (int i = 0; i < _tempList.Count; i++)
		{
			Recycle(_tempList[i]);
		}
		_tempList.Clear();
	}

	public static void RecycleAll()
	{
		if ((bool)instance)
		{
			_tempList.AddRange(instance.spawnedObjects.Keys);
			for (int i = 0; i < _tempList.Count; i++)
			{
				Recycle(_tempList[i]);
			}
			_tempList.Clear();
		}
	}

	public static bool IsSpawned(GameObject obj)
	{
		return instance.spawnedObjects.ContainsKey(obj);
	}

	public static bool IsParentsPooledRecursive(GameObject obj)
	{
		if (instance.spawnedObjects.ContainsKey(obj))
		{
			return true;
		}
		foreach (KeyValuePair<GameObject, List<GameObject>> pooledObject in instance.pooledObjects)
		{
			foreach (GameObject item in pooledObject.Value)
			{
				if (item == obj)
				{
					return true;
				}
			}
		}
		Transform parent = obj.transform.parent;
		if ((bool)parent)
		{
			return IsParentsPooledRecursive(parent.gameObject);
		}
		return false;
	}

	public static int CountPooled<T>(T prefab) where T : Component
	{
		return CountPooled(prefab.gameObject);
	}

	public static int CountPooled(GameObject prefab)
	{
		if (instance.pooledObjects.TryGetValue(prefab, out var value))
		{
			return value.Count;
		}
		return 0;
	}

	public static int CountSpawned<T>(T prefab) where T : Component
	{
		return CountSpawned(prefab.gameObject);
	}

	public static int CountSpawned(GameObject prefab)
	{
		int num = 0;
		foreach (GameObject value in instance.spawnedObjects.Values)
		{
			if (prefab == value)
			{
				num++;
			}
		}
		return num;
	}

	public static int CountAllPooled()
	{
		int num = 0;
		foreach (List<GameObject> value in instance.pooledObjects.Values)
		{
			num += value.Count;
		}
		return num;
	}

	public static IEnumerable<(GameObject, int)> EnumerateAllPooledCounts()
	{
		foreach (KeyValuePair<GameObject, List<GameObject>> pooledObject in instance.pooledObjects)
		{
			yield return (pooledObject.Key, pooledObject.Value.Count);
		}
	}

	public static int GetStartupCount(GameObject prefab)
	{
		StartupPool[] array = instance.startupPools;
		foreach (StartupPool startupPool in array)
		{
			if (startupPool.prefab == prefab)
			{
				return startupPool.size;
			}
		}
		return 0;
	}

	public static List<GameObject> GetPooled(GameObject prefab, List<GameObject> list, bool appendList)
	{
		if (list == null)
		{
			list = new List<GameObject>();
		}
		if (!appendList)
		{
			list.Clear();
		}
		if (instance.pooledObjects.TryGetValue(prefab, out var value))
		{
			list.AddRange(value);
		}
		return list;
	}

	public static List<T> GetPooled<T>(T prefab, List<T> list, bool appendList) where T : Component
	{
		if (list == null)
		{
			list = new List<T>();
		}
		if (!appendList)
		{
			list.Clear();
		}
		if (instance.pooledObjects.TryGetValue(prefab.gameObject, out var value))
		{
			for (int i = 0; i < value.Count; i++)
			{
				list.Add(value[i].GetComponent<T>());
			}
		}
		return list;
	}

	public static List<GameObject> GetSpawned(GameObject prefab, List<GameObject> list, bool appendList)
	{
		if (list == null)
		{
			list = new List<GameObject>();
		}
		if (!appendList)
		{
			list.Clear();
		}
		foreach (KeyValuePair<GameObject, GameObject> spawnedObject in instance.spawnedObjects)
		{
			if (spawnedObject.Value == prefab)
			{
				list.Add(spawnedObject.Key);
			}
		}
		return list;
	}

	public static List<T> GetSpawned<T>(T prefab, List<T> list, bool appendList) where T : Component
	{
		if (list == null)
		{
			list = new List<T>();
		}
		if (!appendList)
		{
			list.Clear();
		}
		GameObject gameObject = prefab.gameObject;
		foreach (KeyValuePair<GameObject, GameObject> spawnedObject in instance.spawnedObjects)
		{
			if (spawnedObject.Value == gameObject && !(spawnedObject.Key == null))
			{
				list.Add(spawnedObject.Key.GetComponent<T>());
			}
		}
		return list;
	}

	public static void DestroyPooled(GameObject prefab)
	{
		if (!instance.pooledObjects.TryGetValue(prefab, out var value))
		{
			return;
		}
		foreach (GameObject item in value)
		{
			UnityEngine.Object.Destroy(item);
		}
		value.Clear();
		_spareGameObjectPools.Enqueue(value);
		instance.pooledObjects.Remove(prefab);
	}

	public static void DestroyPooled<T>(T prefab) where T : Component
	{
		DestroyPooled(prefab.gameObject);
	}

	public static void DestroyPooled(GameObject prefab, int amountToRemove)
	{
		RecycleAll(prefab);
		if (!instance.pooledObjects.TryGetValue(prefab, out var value))
		{
			return;
		}
		for (int i = 0; i < amountToRemove; i++)
		{
			if (value.Count <= 0)
			{
				break;
			}
			UnityEngine.Object.Destroy(value[0]);
			value.RemoveAt(0);
		}
		if (value.Count <= 0)
		{
			_spareGameObjectPools.Enqueue(value);
			instance.pooledObjects.Remove(prefab);
		}
	}

	public static void DestroyPooled<T>(T prefab, int amount) where T : Component
	{
		DestroyPooled(prefab.gameObject, amount);
	}

	public static void DestroyAll(GameObject prefab)
	{
		RecycleAll(prefab);
		DestroyPooled(prefab);
	}

	public static void DestroyAll<T>(T prefab) where T : Component
	{
		DestroyAll(prefab.gameObject);
	}
}

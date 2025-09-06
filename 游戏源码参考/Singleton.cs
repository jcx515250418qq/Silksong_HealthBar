using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T _instance;

	private static object _lock = new object();

	private static bool applicationIsQuitting = false;

	public static T instance
	{
		get
		{
			if (applicationIsQuitting)
			{
				Debug.LogWarning("[Singleton] Instance '" + typeof(T)?.ToString() + "' already destroyed on application quit. Won't create again - returning null.");
				return null;
			}
			lock (_lock)
			{
				if (_instance == null)
				{
					_instance = (T)Object.FindObjectOfType(typeof(T));
					if (Object.FindObjectsOfType(typeof(T)).Length > 1)
					{
						Debug.LogError("[Singleton] Something went really wrong  - there should never be more than one singleton! Reopening the scene might fix it.");
						return _instance;
					}
					if (_instance == null)
					{
						GameObject obj = new GameObject();
						_instance = obj.AddComponent<T>();
						obj.name = "(singleton) " + typeof(T).ToString();
						Object.DontDestroyOnLoad(obj);
					}
				}
				return _instance;
			}
		}
	}

	public void Awake()
	{
		if (_instance == null)
		{
			_instance = base.gameObject as T;
			Object.DontDestroyOnLoad(base.gameObject);
		}
		else if (this != _instance)
		{
			Object.DestroyImmediate(base.gameObject);
		}
	}

	public void OnDestroy()
	{
		if (_instance == this)
		{
			_instance = null;
		}
		applicationIsQuitting = true;
	}
}

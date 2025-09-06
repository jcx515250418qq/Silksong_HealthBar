using UnityEngine;

public class ManagerSingleton<T> : MonoBehaviour where T : ManagerSingleton<T>
{
	public static T Instance => SilentInstance;

	public static T SilentInstance
	{
		get
		{
			if (UnsafeInstance == null)
			{
				UnsafeInstance = Object.FindAnyObjectByType<T>();
			}
			return UnsafeInstance;
		}
	}

	public static T UnsafeInstance { get; private set; }

	protected virtual void Awake()
	{
		if (UnsafeInstance == null)
		{
			UnsafeInstance = (T)this;
		}
		else if (UnsafeInstance != this)
		{
			Object.Destroy(this);
		}
	}

	protected virtual void OnDestroy()
	{
		if (UnsafeInstance == this)
		{
			UnsafeInstance = null;
		}
	}
}

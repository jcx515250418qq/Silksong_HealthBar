using System;
using System.Linq;
using UnityEngine;

namespace InControl
{
	public abstract class SingletonMonoBehavior<TComponent> : MonoBehaviour where TComponent : MonoBehaviour
	{
		private static TComponent instance;

		private static bool hasInstance;

		private static int instanceId;

		private static readonly object lockObject = new object();

		public static TComponent Instance
		{
			get
			{
				lock (lockObject)
				{
					if (hasInstance)
					{
						return instance;
					}
					instance = FindFirstInstance();
					if (instance == null)
					{
						throw new Exception("The instance of singleton component " + typeof(TComponent)?.ToString() + " was requested, but it doesn't appear to exist in the scene.");
					}
					hasInstance = true;
					instanceId = instance.GetInstanceID();
					return instance;
				}
			}
		}

		protected bool EnforceSingleton
		{
			get
			{
				if (GetInstanceID() == Instance.GetInstanceID())
				{
					return false;
				}
				if (Application.isPlaying)
				{
					base.enabled = false;
				}
				return true;
			}
		}

		protected bool IsTheSingleton
		{
			get
			{
				lock (lockObject)
				{
					return GetInstanceID() == instanceId;
				}
			}
		}

		protected bool IsNotTheSingleton
		{
			get
			{
				lock (lockObject)
				{
					return GetInstanceID() != instanceId;
				}
			}
		}

		private static TComponent[] FindInstances()
		{
			TComponent[] array = UnityEngine.Object.FindObjectsByType<TComponent>(FindObjectsSortMode.None);
			Array.Sort(array, (TComponent a, TComponent b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));
			return array;
		}

		private static TComponent FindFirstInstance()
		{
			TComponent[] array = FindInstances();
			if (array.Length == 0)
			{
				return null;
			}
			return array[0];
		}

		protected virtual void Awake()
		{
			if (!Application.isPlaying || !Instance)
			{
				return;
			}
			if (GetInstanceID() != instanceId)
			{
				base.enabled = false;
			}
			foreach (TComponent item in from o in FindInstances()
				where o.GetInstanceID() != instanceId
				select o)
			{
				item.enabled = false;
			}
		}

		protected virtual void OnDestroy()
		{
			lock (lockObject)
			{
				if (GetInstanceID() == instanceId)
				{
					hasInstance = false;
				}
			}
		}
	}
}

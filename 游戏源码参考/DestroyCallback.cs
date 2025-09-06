using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class DestroyCallback : MonoBehaviour
{
	private bool hasSentCallback;

	private static Dictionary<GameObject, DestroyCallback> instances = new Dictionary<GameObject, DestroyCallback>();

	public event Action OnDestroyed;

	private void Awake()
	{
		instances[base.gameObject] = this;
	}

	private void OnDestroy()
	{
		instances.Remove(base.gameObject);
		DoCallback();
	}

	private void DoCallback()
	{
		if (!hasSentCallback)
		{
			hasSentCallback = true;
			this.OnDestroyed?.Invoke();
		}
	}

	public static void AddCallback(GameObject gameObject, Action callback)
	{
		if (!(gameObject == null))
		{
			if (!instances.TryGetValue(gameObject, out var value))
			{
				value = (instances[gameObject] = gameObject.AddComponentIfNotPresent<DestroyCallback>());
			}
			value.OnDestroyed += callback;
		}
	}
}

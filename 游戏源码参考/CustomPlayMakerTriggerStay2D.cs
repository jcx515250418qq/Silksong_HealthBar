using System.Collections.Generic;
using UnityEngine;

public sealed class CustomPlayMakerTriggerStay2D : CustomPlayMakerPhysicsEvent<Collider2D>
{
	private static Dictionary<GameObject, CustomPlayMakerTriggerStay2D> lookup = new Dictionary<GameObject, CustomPlayMakerTriggerStay2D>();

	private void Awake()
	{
		lookup[base.gameObject] = this;
	}

	private void OnDestroy()
	{
		lookup.Remove(base.gameObject);
	}

	public static CustomPlayMakerTriggerStay2D GetEventSender(GameObject gameObject)
	{
		if (gameObject == null)
		{
			return null;
		}
		if (!lookup.TryGetValue(gameObject, out var value))
		{
			return gameObject.AddComponentIfNotPresent<CustomPlayMakerTriggerStay2D>();
		}
		return value;
	}

	private void OnTriggerStay2D(Collider2D other)
	{
		SendEvent(other);
	}
}

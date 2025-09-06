using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class CustomPlayMakerCollisionStay2D : CustomPlayMakerPhysicsEvent<Collision2D>
{
	private static Dictionary<GameObject, CustomPlayMakerCollisionStay2D> lookup = new Dictionary<GameObject, CustomPlayMakerCollisionStay2D>();

	private void Awake()
	{
		lookup[base.gameObject] = this;
	}

	private void OnDestroy()
	{
		lookup.Remove(base.gameObject);
	}

	public static CustomPlayMakerCollisionStay2D GetEventSender(GameObject gameObject)
	{
		if (gameObject == null)
		{
			return null;
		}
		if (!lookup.TryGetValue(gameObject, out var value))
		{
			return gameObject.AddComponentIfNotPresent<CustomPlayMakerCollisionStay2D>();
		}
		return value;
	}

	private void OnCollisionStay2D(Collision2D other)
	{
		SendEvent(other);
	}
}

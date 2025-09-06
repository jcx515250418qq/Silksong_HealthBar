using System.Collections.Generic;
using UnityEngine;

public class SteepSlope : MonoBehaviour
{
	private static readonly HashSet<GameObject> STEEP_SLOPES = new HashSet<GameObject>();

	private void Awake()
	{
		GameObject gameObject = base.gameObject;
		gameObject.AddComponentIfNotPresent<NonSlider>();
		STEEP_SLOPES.Add(gameObject);
	}

	private void OnDestroy()
	{
		STEEP_SLOPES.Remove(base.gameObject);
	}

	public static bool IsSteepSlope(Collider2D collider2D)
	{
		return STEEP_SLOPES.Contains(collider2D.gameObject);
	}

	public static bool IsSteepSlope(GameObject gameObject)
	{
		return STEEP_SLOPES.Contains(gameObject);
	}
}

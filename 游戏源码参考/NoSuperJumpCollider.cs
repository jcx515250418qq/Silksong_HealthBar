using System.Collections.Generic;
using UnityEngine;

public class NoSuperJumpCollider : MonoBehaviour
{
	private static readonly List<NoSuperJumpCollider> _insideZones = new List<NoSuperJumpCollider>();

	private Collider2D collider;

	private void Awake()
	{
		collider = GetComponent<Collider2D>();
	}

	private void OnValidate()
	{
	}

	private void OnEnable()
	{
		if (collider == null)
		{
			base.enabled = false;
		}
		else
		{
			_insideZones.AddIfNotPresent(this);
		}
	}

	private void OnDisable()
	{
		_insideZones.Remove(this);
	}

	public static bool IsInside(Vector2 pos)
	{
		foreach (NoSuperJumpCollider insideZone in _insideZones)
		{
			if (insideZone.collider.OverlapPoint(pos))
			{
				return true;
			}
		}
		return false;
	}
}

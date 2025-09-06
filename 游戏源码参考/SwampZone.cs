using System.Collections.Generic;
using UnityEngine;

public class SwampZone : DebugDrawColliderRuntimeAdder
{
	private static readonly List<SwampZone> _activeZones = new List<SwampZone>();

	private Collider2D collider;

	protected override void Awake()
	{
		base.Awake();
		collider = GetComponent<Collider2D>();
	}

	private void OnEnable()
	{
		_activeZones.AddIfNotPresent(this);
	}

	private void OnDisable()
	{
		_activeZones.Remove(this);
	}

	public override void AddDebugDrawComponent()
	{
		DebugDrawColliderRuntime.AddOrUpdate(base.gameObject, DebugDrawColliderRuntime.ColorType.Region);
	}

	public static bool IsInside(Vector2 pos)
	{
		foreach (SwampZone activeZone in _activeZones)
		{
			if (activeZone.collider.OverlapPoint(pos))
			{
				return true;
			}
		}
		return false;
	}
}

using System;
using System.Collections.Generic;
using GlobalEnums;
using UnityEngine;

public class NoTeleportRegion : DebugDrawColliderRuntimeAdder
{
	public enum TeleportAllowState
	{
		Standard = 0,
		Blocked = 1,
		Allowed = 2
	}

	[Space]
	[SerializeField]
	private TeleportAllowState allowState = TeleportAllowState.Blocked;

	private static readonly List<NoTeleportRegion> _activeRegions = new List<NoTeleportRegion>();

	private Collider2D collider;

	private new void Awake()
	{
		collider = GetComponent<Collider2D>();
	}

	private void OnEnable()
	{
		_activeRegions.AddIfNotPresent(this);
	}

	private void OnDisable()
	{
		_activeRegions.Remove(this);
	}

	public override void AddDebugDrawComponent()
	{
		DebugDrawColliderRuntime.AddOrUpdate(base.gameObject, DebugDrawColliderRuntime.ColorType.Region);
	}

	public static TeleportAllowState GetTeleportBlockedState(Vector2 pos)
	{
		GameManager instance = GameManager.instance;
		CustomSceneManager customSceneManager = (instance ? instance.sm : null);
		if (customSceneManager == null)
		{
			return TeleportAllowState.Blocked;
		}
		foreach (NoTeleportRegion activeRegion in _activeRegions)
		{
			if (activeRegion.allowState == TeleportAllowState.Blocked && activeRegion.collider.OverlapPoint(pos))
			{
				return TeleportAllowState.Blocked;
			}
		}
		if (ToolItemManager.ActiveState != 0)
		{
			return TeleportAllowState.Blocked;
		}
		foreach (NoTeleportRegion activeRegion2 in _activeRegions)
		{
			if (activeRegion2.allowState == TeleportAllowState.Allowed && activeRegion2.collider.OverlapPoint(pos))
			{
				return TeleportAllowState.Allowed;
			}
		}
		MapZone mapZone = customSceneManager.mapZone;
		if (mapZone == MapZone.DUST_MAZE || mapZone == MapZone.ABYSS)
		{
			return TeleportAllowState.Blocked;
		}
		if (GameManager.IsMemoryScene(customSceneManager.mapZone))
		{
			return TeleportAllowState.Blocked;
		}
		switch (customSceneManager.TeleportAllowState)
		{
		case TeleportAllowState.Standard:
		{
			instance.gameMap.HasMapForScene(instance.GetSceneNameString(), out var sceneHasSprite);
			if (!sceneHasSprite)
			{
				return TeleportAllowState.Blocked;
			}
			return TeleportAllowState.Standard;
		}
		case TeleportAllowState.Blocked:
			return TeleportAllowState.Blocked;
		case TeleportAllowState.Allowed:
			return TeleportAllowState.Standard;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}
}

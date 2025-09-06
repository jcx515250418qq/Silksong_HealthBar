using System;
using System.Linq;
using GlobalEnums;
using GlobalSettings;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

public class InventoryWideMap : MonoBehaviour
{
	[Serializable]
	private class ConditionalPosition
	{
		public Vector2 Position;

		public InventoryItemWideMapZone[] ZoneConditions;
	}

	[SerializeField]
	private NestedFadeGroupBase fadeGroup;

	[SerializeField]
	private Transform compassIcon;

	[SerializeField]
	private Transform corpseIcon;

	[Space]
	[SerializeField]
	private ConditionalPosition[] positionsOrdered;

	[SerializeField]
	private Vector2 soloSlabPosition;

	[SerializeField]
	private Vector2 soloAbyssPosition;

	private InventoryItemWideMapZone[] selectables;

	public NestedFadeGroupBase FadeGroup => fadeGroup;

	public InventoryItemWideMapZone[] DefaultSelectables => selectables ?? (selectables = GetComponentsInChildren<InventoryItemWideMapZone>(includeInactive: true));

	public Vector2 PositionOffset
	{
		get
		{
			if (GameManager.instance.gameMap.IsLostInAbyssPostMap())
			{
				return soloAbyssPosition;
			}
			if (CollectableItemManager.IsInHiddenMode())
			{
				return soloSlabPosition;
			}
			ConditionalPosition[] array = positionsOrdered;
			foreach (ConditionalPosition conditionalPosition in array)
			{
				InventoryItemWideMapZone[] zoneConditions = conditionalPosition.ZoneConditions;
				bool flag = false;
				InventoryItemWideMapZone[] array2 = zoneConditions;
				foreach (InventoryItemWideMapZone inventoryItemWideMapZone in array2)
				{
					if ((bool)inventoryItemWideMapZone && inventoryItemWideMapZone.IsUnlocked)
					{
						flag = true;
						break;
					}
				}
				if (zoneConditions.Length == 0 || flag)
				{
					return conditionalPosition.Position;
				}
			}
			return Vector2.zero;
		}
	}

	public event Action PlacedCompassIcon;

	public void UpdatePositions()
	{
		if (!IsInvalid(PositionOffset))
		{
			base.transform.SetLocalPosition2D(PositionOffset);
		}
		this.PlacedCompassIcon?.Invoke();
		GameManager instance = GameManager.instance;
		if ((bool)instance.gameMap)
		{
			instance.gameMap.UpdateCurrentScene();
			MapZone zoneForBounds;
			Vector2 compassPositionLocalBounds = instance.gameMap.GetCompassPositionLocalBounds(out zoneForBounds);
			ToolItem compassTool = Gameplay.CompassTool;
			PositionIcon(compassIcon, compassPositionLocalBounds, (bool)compassTool && compassTool.IsEquipped && !instance.gameMap.IsLostInAbyssPreMap(), zoneForBounds);
			MapZone zoneForBounds2;
			Vector2 corpsePositionLocalBounds = instance.gameMap.GetCorpsePositionLocalBounds(out zoneForBounds2);
			PlayerData instance2 = PlayerData.instance;
			PositionIcon(corpseIcon, corpsePositionLocalBounds, !string.IsNullOrEmpty(instance2.HeroCorpseScene), zoneForBounds2);
		}
	}

	private static bool IsInvalid(Vector2 vector2)
	{
		if (!float.IsNaN(vector2.x))
		{
			return float.IsInfinity(vector2.y);
		}
		return true;
	}

	private void PositionIcon(Transform icon, Vector2 mapBoundsPos, bool isActive, MapZone currentMapZone)
	{
		if (!icon)
		{
			return;
		}
		if (!isActive || IsInvalid(mapBoundsPos))
		{
			icon.gameObject.SetActive(value: false);
			return;
		}
		icon.gameObject.SetActive(value: true);
		InventoryItemWideMapZone inventoryItemWideMapZone = null;
		InventoryItemWideMapZone[] defaultSelectables = DefaultSelectables;
		foreach (InventoryItemWideMapZone inventoryItemWideMapZone2 in defaultSelectables)
		{
			if (inventoryItemWideMapZone2.EnumerateMapZones().Contains(currentMapZone))
			{
				inventoryItemWideMapZone = inventoryItemWideMapZone2;
				break;
			}
		}
		if (!(inventoryItemWideMapZone == null))
		{
			Vector2 closestNodePosLocalBounds = inventoryItemWideMapZone.GetClosestNodePosLocalBounds(mapBoundsPos);
			icon.SetPosition2D(inventoryItemWideMapZone.transform.TransformPoint(closestNodePosLocalBounds));
		}
	}
}

using System.Collections.Generic;
using GlobalEnums;
using TMProOld;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Serialization;

public class InventoryItemWideMapZone : InventoryItemSelectableDirectional
{
	private static readonly Color _deselectedMultiplyColor = new Color(0.6f, 0.6f, 0.6f, 1f);

	[SerializeField]
	private SpriteRenderer sprite;

	[SerializeField]
	[EnumPickerBitmask(typeof(MapZone))]
	private long mapZones;

	[SerializeField]
	[FormerlySerializedAs("mapZone")]
	private MapZone zoomToZone;

	[SerializeField]
	private TMP_Text labelText;

	[SerializeField]
	private Vector2[] compassNodes;

	private Color initialColor;

	private Color initialLabelColor;

	private bool isSelected;

	private InventoryMapManager manager;

	private InventoryPane pane;

	public MapZone ZoomToZone => zoomToZone;

	public override bool ShowCursor => false;

	public bool IsUnlocked
	{
		get
		{
			GameMap gameMap = GameManager.instance.gameMap;
			if (CollectableItemManager.IsInHiddenMode())
			{
				if (!gameMap.HasAnyMapForZone(MapZone.THE_SLAB))
				{
					return false;
				}
				if (!mapZones.IsBitSet(7))
				{
					return false;
				}
			}
			if (gameMap.IsLostInAbyssPreMap())
			{
				return false;
			}
			if (gameMap.IsLostInAbyssPostMap() && !mapZones.IsBitSet(37))
			{
				return false;
			}
			foreach (MapZone item in EnumerateMapZones())
			{
				if (gameMap.HasAnyMapForZone(item))
				{
					return true;
				}
			}
			return false;
		}
	}

	protected override bool IsAutoNavSelectable => IsUnlocked;

	private void Reset()
	{
		sprite = GetComponent<SpriteRenderer>();
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireSphere(base.NavigationOffset, 0.2f);
	}

	protected override void Awake()
	{
		base.Awake();
		manager = GetComponentInParent<InventoryMapManager>();
		if ((bool)sprite)
		{
			initialColor = sprite.color;
			initialColor.a = 1f;
		}
		if ((bool)labelText)
		{
			initialLabelColor = labelText.color;
			initialLabelColor.a = 1f;
		}
		pane = GetComponentInParent<InventoryPane>();
		if ((bool)pane)
		{
			pane.OnPaneStart += EvaluateUnlocked;
		}
	}

	public override void Select(InventoryItemManager.SelectionDirection? direction)
	{
		base.Select(direction);
		isSelected = true;
		UpdateColor();
	}

	public override void Deselect()
	{
		base.Deselect();
		isSelected = false;
		UpdateColor();
	}

	public override bool Submit()
	{
		MapZone currentMapZone = manager.GetCurrentMapZone();
		MapZone mapZone = (mapZones.IsBitSet((int)currentMapZone) ? currentMapZone : zoomToZone);
		manager.ZoomIn(mapZone, animate: true);
		return true;
	}

	private void UpdateColor()
	{
		if ((bool)sprite)
		{
			sprite.color = (isSelected ? initialColor : initialColor.MultiplyElements(_deselectedMultiplyColor));
		}
		if ((bool)labelText)
		{
			labelText.color = (isSelected ? initialLabelColor : initialLabelColor.MultiplyElements(_deselectedMultiplyColor));
		}
	}

	private void EvaluateUnlocked()
	{
		if (IsUnlocked)
		{
			if (base.gameObject.activeSelf)
			{
				EvaluateAutoNav();
			}
			else
			{
				base.gameObject.SetActive(value: true);
			}
			UpdateColor();
		}
		else
		{
			base.gameObject.SetActive(value: false);
		}
	}

	public override InventoryItemSelectable GetNextSelectable(InventoryItemManager.SelectionDirection direction)
	{
		InventoryItemSelectable nextSelectable = GetNextSelectable(direction, allowAutoNavOnFirst: false);
		if ((bool)nextSelectable)
		{
			InventoryItemWideMapZone inventoryItemWideMapZone = nextSelectable as InventoryItemWideMapZone;
			if (inventoryItemWideMapZone == null || inventoryItemWideMapZone.gameObject.activeSelf)
			{
				return nextSelectable;
			}
			return GetSelectableFromAutoNavGroup(direction, (InventoryItemWideMapZone zone) => zone.gameObject.activeSelf);
		}
		return null;
	}

	public Vector2 GetClosestNodePosLocalBounds(Vector2 localBoundsPos)
	{
		Bounds bounds = sprite.bounds;
		Vector3 vector = base.transform.InverseTransformPoint(bounds.min);
		Vector3 vector2 = base.transform.InverseTransformPoint(bounds.max);
		Vector2 b = new Vector2(Mathf.Lerp(vector.x, vector2.x, localBoundsPos.x), Mathf.Lerp(vector.y, vector2.y, localBoundsPos.y));
		Vector2 result = Vector2.zero;
		float num = float.MaxValue;
		Vector2[] array = compassNodes;
		foreach (Vector2 vector3 in array)
		{
			float num2 = Vector2.Distance(vector3, b);
			if (!(num2 > num))
			{
				result = vector3;
				num = num2;
			}
		}
		return result;
	}

	public IEnumerable<MapZone> EnumerateMapZones()
	{
		for (int i = 0; i < 64; i++)
		{
			if (mapZones.IsBitSet(i))
			{
				yield return (MapZone)i;
			}
		}
	}
}

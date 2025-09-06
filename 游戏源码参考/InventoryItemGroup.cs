using System.Collections.Generic;
using UnityEngine;

public class InventoryItemGroup : InventoryItemSelectable
{
	private readonly List<InventoryItemSelectable> childItems = new List<InventoryItemSelectable>();

	private InventoryItemManager itemManager;

	public override string DisplayName => string.Empty;

	public override string Description => string.Empty;

	protected void Awake()
	{
		itemManager = GetComponentInParent<InventoryItemManager>();
	}

	private void UpdateChildItems()
	{
		childItems.Clear();
		foreach (Transform item in base.transform)
		{
			if (!item.gameObject.activeInHierarchy)
			{
				continue;
			}
			InventoryItemGroup component = item.GetComponent<InventoryItemGroup>();
			if ((bool)component)
			{
				component.UpdateChildItems();
				childItems.AddRange(component.childItems);
				continue;
			}
			InventoryItemSelectable component2 = item.GetComponent<InventoryItemSelectable>();
			if ((bool)component2)
			{
				childItems.Add(component2);
			}
		}
	}

	public override InventoryItemSelectable Get(InventoryItemManager.SelectionDirection? direction)
	{
		UpdateChildItems();
		if (childItems.Count <= 0)
		{
			return null;
		}
		if (direction.HasValue && (bool)itemManager && (bool)itemManager.CurrentSelected)
		{
			InventoryItemSelectable closestOnAxis = InventoryItemNavigationHelper.GetClosestOnAxis(direction.Value, itemManager.CurrentSelected, childItems);
			if ((bool)closestOnAxis)
			{
				return closestOnAxis.Get(direction);
			}
		}
		return childItems[0].Get(direction);
	}

	public override InventoryItemSelectable GetNextSelectable(InventoryItemManager.SelectionDirection direction)
	{
		return this;
	}
}

using System.Collections.Generic;

public class InventoryItemParent : InventoryItemSelectableDirectional, IInventorySelectionParent
{
	private InventoryItemManager manager;

	private List<InventoryItemSelectable> childSelectables;

	public override string DisplayName => string.Empty;

	public override string Description => string.Empty;

	protected override bool IsAutoNavSelectable => false;

	protected override void Awake()
	{
		manager = GetComponentInParent<InventoryItemManager>();
		childSelectables = new List<InventoryItemSelectable>(base.transform.childCount);
	}

	public override InventoryItemSelectable Get(InventoryItemManager.SelectionDirection? direction)
	{
		childSelectables.Clear();
		GetComponentsInChildren(childSelectables);
		childSelectables.Remove(this);
		if (childSelectables.Count <= 0)
		{
			return null;
		}
		if (direction.HasValue)
		{
			return InventoryItemNavigationHelper.GetClosestOnAxis(direction.Value, manager.CurrentSelected, childSelectables);
		}
		return childSelectables[0];
	}

	public InventoryItemSelectable GetNextSelectable(InventoryItemSelectable source, InventoryItemManager.SelectionDirection? direction)
	{
		InventoryItemSelectable inventoryItemSelectable = ((!direction.HasValue) ? null : GetNextSelectable(direction.Value));
		if (inventoryItemSelectable == source)
		{
			inventoryItemSelectable = null;
		}
		return inventoryItemSelectable;
	}
}

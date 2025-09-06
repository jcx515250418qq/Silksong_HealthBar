public interface IInventorySelectionParent
{
	InventoryItemSelectable GetNextSelectable(InventoryItemSelectable source, InventoryItemManager.SelectionDirection? direction);
}

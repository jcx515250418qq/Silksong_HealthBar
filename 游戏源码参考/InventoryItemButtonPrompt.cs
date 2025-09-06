public class InventoryItemButtonPrompt : InventoryItemButtonPromptBase<InventoryItemButtonPromptData>
{
	protected override void OnShow(InventoryItemButtonPromptDisplayList displayList, InventoryItemButtonPromptData data)
	{
		displayList.Append(data, forceDisabled: false, order);
	}
}

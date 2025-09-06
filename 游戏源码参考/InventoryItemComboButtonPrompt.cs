public class InventoryItemComboButtonPrompt : InventoryItemButtonPromptBase<InventoryItemComboButtonPromptDisplay.Display>
{
	protected override void OnShow(InventoryItemButtonPromptDisplayList displayList, InventoryItemComboButtonPromptDisplay.Display data)
	{
		displayList.Append(data, order);
	}
}

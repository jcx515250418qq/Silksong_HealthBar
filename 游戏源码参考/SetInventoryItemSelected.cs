using HutongGames.PlayMaker;

public class SetInventoryItemSelected : FSMUtility.GetComponentFsmStateAction<InventoryItemManager>
{
	[ObjectType(typeof(InventoryItemManager.SelectedActionType))]
	public FsmEnum SelectedAction;

	public FsmGameObject CustomObject;

	[HideIf("IsUsingCustomObject")]
	public FsmBool JustDisplay;

	public bool IsUsingCustomObject()
	{
		return CustomObject.Value;
	}

	public override void Reset()
	{
		base.Reset();
		SelectedAction = null;
		CustomObject = null;
		JustDisplay = null;
	}

	protected override void DoAction(InventoryItemManager itemManager)
	{
		if (IsUsingCustomObject())
		{
			itemManager.SetSelected(CustomObject.Value, JustDisplay.Value);
		}
		else if (!SelectedAction.IsNone)
		{
			InventoryItemManager.SelectedActionType selectedAction = (InventoryItemManager.SelectedActionType)(object)SelectedAction.Value;
			itemManager.SetSelected(selectedAction, JustDisplay.Value);
			itemManager.InstantScroll();
		}
	}
}

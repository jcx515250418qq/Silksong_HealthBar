using HutongGames.PlayMaker;

public class TrySetInventoryItemSelected : FSMUtility.GetComponentFsmStateAction<InventoryItemManager>
{
	[ObjectType(typeof(InventoryItemManager.SelectedActionType))]
	public FsmEnum SelectedAction;

	public FsmGameObject CustomObject;

	[HideIf("IsUsingCustomObject")]
	public FsmBool JustDisplay;

	public FsmEvent successEvent;

	public FsmEvent failedEvent;

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
		successEvent = null;
		failedEvent = null;
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
			if (itemManager.SetSelected(selectedAction, JustDisplay.Value))
			{
				itemManager.InstantScroll();
				base.Fsm.Event(successEvent);
			}
			else
			{
				base.Fsm.Event(failedEvent);
			}
		}
	}
}

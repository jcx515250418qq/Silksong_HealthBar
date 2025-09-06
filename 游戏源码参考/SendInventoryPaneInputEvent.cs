using HutongGames.PlayMaker;

public class SendInventoryPaneInputEvent : FSMUtility.GetComponentFsmStateAction<InventoryPaneBase>
{
	[ObjectType(typeof(InventoryPaneBase.InputEventType))]
	public FsmEnum InputEvent;

	public override void Reset()
	{
		base.Reset();
		InputEvent = null;
	}

	protected override void DoAction(InventoryPaneBase pane)
	{
		if (!InputEvent.IsNone)
		{
			InventoryPaneBase.InputEventType eventType = (InventoryPaneBase.InputEventType)(object)InputEvent.Value;
			pane.SendInputEvent(eventType);
		}
	}
}

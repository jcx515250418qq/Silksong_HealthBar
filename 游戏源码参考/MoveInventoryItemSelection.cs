using HutongGames.PlayMaker;

public class MoveInventoryItemSelection : FSMUtility.GetComponentFsmStateAction<InventoryItemManager>
{
	[ObjectType(typeof(InventoryItemManager.SelectionDirection))]
	public FsmEnum Direction;

	public FsmEvent CancelEvent;

	public override void Reset()
	{
		base.Reset();
		Direction = null;
		CancelEvent = null;
	}

	protected override void DoAction(InventoryItemManager itemManager)
	{
		if (!itemManager.MoveSelection((InventoryItemManager.SelectionDirection)(object)Direction.Value))
		{
			base.Fsm.Event(CancelEvent);
		}
	}
}

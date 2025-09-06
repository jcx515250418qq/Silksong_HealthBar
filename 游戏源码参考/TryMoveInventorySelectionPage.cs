using HutongGames.PlayMaker;

public class TryMoveInventorySelectionPage : FSMUtility.GetComponentFsmStateAction<InventoryItemManager>
{
	[ObjectType(typeof(InventoryItemManager.SelectionDirection))]
	public FsmEnum Direction;

	[UIHint(UIHint.Variable)]
	public FsmBool StoreValue;

	public FsmEvent trueEvent;

	public FsmEvent falseEvent;

	public override void Reset()
	{
		base.Reset();
		Direction = null;
		StoreValue = null;
		trueEvent = null;
		falseEvent = null;
	}

	protected override void DoAction(InventoryItemManager itemManager)
	{
		if (!Direction.IsNone)
		{
			bool flag = itemManager.MoveSelectionPage((InventoryItemManager.SelectionDirection)(object)Direction.Value);
			StoreValue.Value = flag;
			if (flag)
			{
				base.Fsm.Event(trueEvent);
			}
			else
			{
				base.Fsm.Event(falseEvent);
			}
		}
	}
}

using HutongGames.PlayMaker;

public class MoveInventorySelectionPage : FSMUtility.GetComponentFsmStateAction<InventoryItemManager>
{
	[ObjectType(typeof(InventoryItemManager.SelectionDirection))]
	public FsmEnum Direction;

	public override void Reset()
	{
		base.Reset();
		Direction = null;
	}

	protected override void DoAction(InventoryItemManager itemManager)
	{
		if (!Direction.IsNone)
		{
			itemManager.MoveSelectionPage((InventoryItemManager.SelectionDirection)(object)Direction.Value);
		}
	}
}

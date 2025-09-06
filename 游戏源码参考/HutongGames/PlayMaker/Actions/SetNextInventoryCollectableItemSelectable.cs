namespace HutongGames.PlayMaker.Actions
{
	public class SetNextInventoryCollectableItemSelectable : FsmStateAction
	{
		[ObjectType(typeof(InventoryCollectableItemSelectionHelper.SelectionType))]
		public FsmEnum selectableType;

		public override void Reset()
		{
			selectableType = null;
		}

		public override void OnEnter()
		{
			InventoryCollectableItemSelectionHelper.LastSelectionUpdate = (InventoryCollectableItemSelectionHelper.SelectionType)(object)selectableType.Value;
			Finish();
		}
	}
}

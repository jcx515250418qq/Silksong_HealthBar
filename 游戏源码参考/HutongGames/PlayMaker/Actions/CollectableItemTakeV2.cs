namespace HutongGames.PlayMaker.Actions
{
	public class CollectableItemTakeV2 : CollectableItemAction
	{
		public FsmInt Amount;

		public FsmBool ShowCounter;

		[ObjectType(typeof(TakeItemTypes))]
		public FsmEnum TakeDisplay;

		public override void Reset()
		{
			base.Reset();
			Amount = 1;
			ShowCounter = true;
			TakeDisplay = null;
		}

		protected override void DoAction(CollectableItem item)
		{
			item.Take(Amount.IsNone ? 1 : Amount.Value, ShowCounter.Value);
			TakeItemTypes takeItemType = (TakeItemTypes)(object)TakeDisplay.Value;
			CollectableUIMsg.ShowTakeMsg(item, takeItemType);
		}
	}
}

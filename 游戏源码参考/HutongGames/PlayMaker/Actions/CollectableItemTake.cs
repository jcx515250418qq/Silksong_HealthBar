namespace HutongGames.PlayMaker.Actions
{
	public class CollectableItemTake : CollectableItemAction
	{
		public FsmInt Amount;

		public FsmBool ShowCounter;

		public override void Reset()
		{
			base.Reset();
			Amount = 1;
			ShowCounter = true;
		}

		protected override void DoAction(CollectableItem item)
		{
			item.Take(Amount.IsNone ? 1 : Amount.Value, ShowCounter.Value);
		}
	}
}

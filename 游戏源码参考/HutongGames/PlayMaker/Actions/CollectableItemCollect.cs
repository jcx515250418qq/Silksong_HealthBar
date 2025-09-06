namespace HutongGames.PlayMaker.Actions
{
	public class CollectableItemCollect : CollectableItemAction
	{
		public FsmInt Amount;

		public override void Reset()
		{
			base.Reset();
			Amount = new FsmInt(1);
		}

		protected override void DoAction(CollectableItem item)
		{
			item.Collect(Amount.IsNone ? 1 : Amount.Value);
		}
	}
}

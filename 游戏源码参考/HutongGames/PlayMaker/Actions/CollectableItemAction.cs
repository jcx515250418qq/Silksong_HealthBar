namespace HutongGames.PlayMaker.Actions
{
	public abstract class CollectableItemAction : FsmStateAction
	{
		[ObjectType(typeof(CollectableItem))]
		public FsmObject Item;

		public override void Reset()
		{
			Item = null;
		}

		public override void OnEnter()
		{
			CollectableItem collectableItem = Item.Value as CollectableItem;
			if (collectableItem != null)
			{
				DoAction(collectableItem);
			}
			Finish();
		}

		protected abstract void DoAction(CollectableItem item);
	}
}

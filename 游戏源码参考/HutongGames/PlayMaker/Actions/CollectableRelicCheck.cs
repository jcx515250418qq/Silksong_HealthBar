namespace HutongGames.PlayMaker.Actions
{
	public class CollectableRelicCheck : FsmStateAction
	{
		[ObjectType(typeof(CollectableRelic))]
		[RequiredField]
		public FsmObject Relic;

		[UIHint(UIHint.Variable)]
		public FsmBool StoreIsCollected;

		[UIHint(UIHint.Variable)]
		public FsmBool StoreIsDeposited;

		[UIHint(UIHint.Variable)]
		public FsmBool StoreIsInInventory;

		public FsmEvent NotCollectedEvent;

		public FsmEvent IsCollectedEvent;

		public FsmEvent IsDepositedEvent;

		public FsmEvent IsInInventoryEvent;

		public override void Reset()
		{
			Relic = null;
			StoreIsCollected = null;
			StoreIsDeposited = null;
			StoreIsInInventory = null;
		}

		public override void OnEnter()
		{
			CollectableRelic collectableRelic = Relic.Value as CollectableRelic;
			if (collectableRelic != null)
			{
				CollectableRelicsData.Data savedData = collectableRelic.SavedData;
				bool isCollected = savedData.IsCollected;
				bool isDeposited = savedData.IsDeposited;
				bool isInInventory = collectableRelic.IsInInventory;
				StoreIsCollected.Value = isCollected;
				StoreIsDeposited.Value = isDeposited;
				StoreIsInInventory = isInInventory;
				if (isCollected)
				{
					base.Fsm.Event(IsCollectedEvent);
				}
				else
				{
					base.Fsm.Event(NotCollectedEvent);
				}
				if (isDeposited)
				{
					base.Fsm.Event(IsDepositedEvent);
				}
				if (isInInventory)
				{
					base.Fsm.Event(IsInInventoryEvent);
				}
			}
			Finish();
		}
	}
}

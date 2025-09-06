namespace HutongGames.PlayMaker.Actions
{
	public class GetToolEquipInfo : FsmStateAction
	{
		[ObjectType(typeof(ToolItem))]
		public FsmObject Tool;

		[UIHint(UIHint.Variable)]
		public FsmBool StoreIsEquipped;

		[UIHint(UIHint.Variable)]
		public FsmBool StoreIsUnlocked;

		[UIHint(UIHint.Variable)]
		public FsmInt StoreAmountLeft;

		[UIHint(UIHint.Variable)]
		public FsmInt StoreMaxAmount;

		public FsmEvent SomeLeftEquippedEvent;

		public FsmEvent NoneLeftEquippedEvent;

		public bool EveryFrame;

		public override void Reset()
		{
			Tool = null;
			StoreIsEquipped = null;
			StoreIsUnlocked = null;
			StoreAmountLeft = null;
			StoreMaxAmount = null;
			SomeLeftEquippedEvent = null;
			NoneLeftEquippedEvent = null;
			EveryFrame = false;
		}

		public override void OnEnter()
		{
			DoAction();
			if (!EveryFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoAction();
		}

		private void DoAction()
		{
			ToolItem toolItem = Tool.Value as ToolItem;
			if (toolItem != null)
			{
				ToolItemsData.Data savedData = toolItem.SavedData;
				bool isEquipped = toolItem.IsEquipped;
				StoreIsEquipped.Value = isEquipped;
				StoreIsUnlocked.Value = savedData.IsUnlocked;
				StoreAmountLeft.Value = savedData.AmountLeft;
				StoreMaxAmount.Value = ToolItemManager.GetToolStorageAmount(toolItem);
				base.Fsm.Event((isEquipped && savedData.AmountLeft > 0) ? SomeLeftEquippedEvent : NoneLeftEquippedEvent);
			}
			else
			{
				StoreIsEquipped.Value = false;
				StoreIsUnlocked.Value = false;
				StoreAmountLeft.Value = 0;
				base.Fsm.Event(NoneLeftEquippedEvent);
			}
		}
	}
}

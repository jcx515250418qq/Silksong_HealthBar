namespace HutongGames.PlayMaker.Actions
{
	public class CheckIfToolEquipped : FSMUtility.CheckFsmStateAction
	{
		[ObjectType(typeof(ToolItem))]
		public FsmObject Tool;

		public FsmInt RequiredAmountLeft;

		public override bool IsTrue
		{
			get
			{
				ToolItem toolItem = Tool.Value as ToolItem;
				if (toolItem == null)
				{
					return false;
				}
				if (!((base.Owner.layer == 5) ? toolItem.IsEquippedHud : toolItem.IsEquipped))
				{
					return false;
				}
				if (toolItem.BaseStorageAmount > 0)
				{
					return toolItem.SavedData.AmountLeft >= RequiredAmountLeft.Value;
				}
				return true;
			}
		}

		public override void Reset()
		{
			base.Reset();
			Tool = null;
			RequiredAmountLeft = new FsmInt
			{
				UseVariable = true
			};
		}
	}
}

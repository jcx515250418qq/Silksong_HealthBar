namespace HutongGames.PlayMaker.Actions
{
	public class SavedItemGetV2 : FsmStateAction
	{
		[ObjectType(typeof(SavedItem))]
		public FsmObject Item;

		public FsmInt Amount;

		public FsmBool ShowPopup;

		public override void Reset()
		{
			Item = null;
			Amount = 1;
			ShowPopup = true;
		}

		public override void OnEnter()
		{
			SavedItem savedItem = Item.Value as SavedItem;
			if (savedItem != null)
			{
				savedItem.Get(Amount.Value, ShowPopup.Value);
			}
			Finish();
		}
	}
}

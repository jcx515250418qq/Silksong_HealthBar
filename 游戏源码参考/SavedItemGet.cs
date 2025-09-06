namespace HutongGames.PlayMaker.Actions
{
	public class SavedItemGet : FsmStateAction
	{
		[ObjectType(typeof(SavedItem))]
		public FsmObject Item;

		public override void Reset()
		{
			Item = null;
		}

		public override void OnEnter()
		{
			SavedItem savedItem = Item.Value as SavedItem;
			if (savedItem != null)
			{
				savedItem.Get();
			}
			Finish();
		}
	}
}

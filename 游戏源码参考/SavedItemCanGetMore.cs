namespace HutongGames.PlayMaker.Actions
{
	public class SavedItemCanGetMore : FSMUtility.CheckFsmStateAction
	{
		[ObjectType(typeof(SavedItem))]
		public FsmObject Item;

		public override bool IsTrue
		{
			get
			{
				SavedItem savedItem = Item.Value as SavedItem;
				if (savedItem != null)
				{
					return savedItem.CanGetMore();
				}
				return false;
			}
		}

		public override void Reset()
		{
			base.Reset();
			Item = null;
		}
	}
}

namespace HutongGames.PlayMaker.Actions
{
	public class CountShopItemsFromStock : FsmStateAction
	{
		[ObjectType(typeof(ShopItemList))]
		public FsmObject StockList;

		[ObjectType(typeof(ShopItem.TypeFlags))]
		public FsmEnum MatchFlag;

		public FsmBool PurchasedOnly;

		[UIHint(UIHint.Variable)]
		public FsmInt StoreCount;

		public override void Reset()
		{
			StockList = null;
			MatchFlag = null;
			PurchasedOnly = null;
			StoreCount = null;
		}

		public override void OnEnter()
		{
			StoreCount.Value = 0;
			DoAction();
			Finish();
		}

		private void DoAction()
		{
			ShopItemList obj = StockList.Value as ShopItemList;
			int value = CountShopItems.CountShopItemsFromStock(typeFlags: (ShopItem.TypeFlags)(object)MatchFlag.Value, stock: obj?.ShopItems, purchasedOnly: PurchasedOnly.Value);
			StoreCount.Value = value;
		}
	}
}

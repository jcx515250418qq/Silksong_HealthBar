namespace HutongGames.PlayMaker.Actions
{
	public class ShopHasStockNotInfinite : ShopCheck
	{
		protected override bool CheckShop(ShopMenuStock shop)
		{
			return shop.StockLeftNotInfinite();
		}
	}
}

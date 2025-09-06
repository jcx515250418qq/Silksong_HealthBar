namespace HutongGames.PlayMaker.Actions
{
	public class ShopHasStock : ShopCheck
	{
		protected override bool CheckShop(ShopMenuStock shop)
		{
			return shop.StockLeft();
		}
	}
}

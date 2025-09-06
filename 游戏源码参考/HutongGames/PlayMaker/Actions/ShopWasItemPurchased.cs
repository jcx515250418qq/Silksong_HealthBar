namespace HutongGames.PlayMaker.Actions
{
	public class ShopWasItemPurchased : ShopCheck
	{
		protected override bool CheckShop(ShopMenuStock shop)
		{
			return shop.WasItemPurchased;
		}
	}
}

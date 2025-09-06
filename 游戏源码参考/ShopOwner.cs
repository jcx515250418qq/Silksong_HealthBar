using UnityEngine;

public class ShopOwner : ShopOwnerBase
{
	[SerializeField]
	private ShopItemList stockList;

	[SerializeField]
	private ShopItem[] stock;

	public override ShopItem[] Stock
	{
		get
		{
			if (!stockList)
			{
				return stock;
			}
			return stockList.ShopItems;
		}
	}

	private void OnValidate()
	{
		if ((bool)stockList && stock != null && stock.Length != 0)
		{
			stock = new ShopItem[0];
		}
	}
}

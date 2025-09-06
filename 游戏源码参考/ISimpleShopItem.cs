using UnityEngine;

public interface ISimpleShopItem
{
	string GetDisplayName();

	Sprite GetIcon();

	int GetCost();

	bool DelayPurchase();
}

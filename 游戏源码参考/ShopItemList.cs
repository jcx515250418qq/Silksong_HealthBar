using UnityEngine;

[CreateAssetMenu(menuName = "Hornet/Shop Item List")]
public class ShopItemList : ScriptableObject
{
	[SerializeField]
	private ShopItem[] shopItems;

	public ShopItem[] ShopItems => shopItems;
}

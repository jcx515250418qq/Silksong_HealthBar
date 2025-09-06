using UnityEngine;

public class ShopSubItemStats : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer icon;

	public void Setup(ShopItem.SubItem item)
	{
		icon.sprite = item.Icon;
	}
}

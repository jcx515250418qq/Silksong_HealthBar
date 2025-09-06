using TMProOld;
using UnityEngine;

public class SavedItemDisplay : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer icon;

	[SerializeField]
	private TMP_Text amountText;

	public void Setup(SavedItem item, int amount)
	{
		Sprite popupIcon;
		if (item is CollectableItem collectableItem)
		{
			popupIcon = collectableItem.GetIcon(CollectableItem.ReadSource.TakePopup);
			if (!collectableItem.DisplayAmount)
			{
				amount = 0;
			}
		}
		else
		{
			popupIcon = item.GetPopupIcon();
		}
		icon.sprite = popupIcon;
		amountText.text = ((amount > 1) ? amount.ToString() : string.Empty);
	}
}

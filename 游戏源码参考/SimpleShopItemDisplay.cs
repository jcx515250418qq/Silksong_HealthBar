using TMProOld;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

public class SimpleShopItemDisplay : MonoBehaviour
{
	[SerializeField]
	private TMP_Text titleText;

	[SerializeField]
	private SpriteRenderer itemIcon;

	[SerializeField]
	private Vector2 textOffsetWithIcon;

	[SerializeField]
	private GameObject costGroup;

	[SerializeField]
	private TMP_Text costText;

	[SerializeField]
	private NestedFadeGroupBase fadeGroup;

	[SerializeField]
	private float disabledOpacity = 0.5f;

	private bool isSetup;

	private Vector2 initialTextPos;

	private int cost;

	private void OnDisable()
	{
		if ((bool)titleText)
		{
			titleText.transform.localPosition = initialTextPos;
		}
	}

	public void SetItem(ISimpleShopItem item)
	{
		if (!isSetup)
		{
			if ((bool)titleText)
			{
				initialTextPos = titleText.transform.localPosition;
			}
			isSetup = true;
		}
		bool flag = false;
		if ((bool)itemIcon)
		{
			Sprite icon = item.GetIcon();
			itemIcon.sprite = icon;
			flag = icon;
		}
		if ((bool)titleText)
		{
			titleText.text = item.GetDisplayName();
			if (flag)
			{
				titleText.transform.SetLocalPosition2D(initialTextPos + textOffsetWithIcon);
			}
			else
			{
				titleText.transform.SetLocalPosition2D(initialTextPos);
			}
		}
		cost = item.GetCost();
		if (cost > 0)
		{
			if ((bool)costGroup)
			{
				costGroup.SetActive(value: true);
			}
			if ((bool)costText)
			{
				costText.text = cost.ToString();
			}
		}
		else if ((bool)costGroup)
		{
			costGroup.SetActive(value: false);
		}
		RefreshCostOpacity();
	}

	public void RefreshCostOpacity()
	{
		if ((bool)fadeGroup)
		{
			fadeGroup.AlphaSelf = ((PlayerData.instance.geo < cost) ? disabledOpacity : 1f);
		}
	}
}

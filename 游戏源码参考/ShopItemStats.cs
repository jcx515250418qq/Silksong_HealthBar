using System;
using TMProOld;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

public class ShopItemStats : MonoBehaviour
{
	[SerializeField]
	private ShopItem shopItem;

	[Space]
	[SerializeField]
	private Color activeColour;

	[SerializeField]
	private Color inactiveColour;

	[SerializeField]
	private SpriteRenderer costSprite;

	[SerializeField]
	private Sprite rosarySprite;

	[SerializeField]
	private Sprite shardSprite;

	[SerializeField]
	private float requiredItemCostScale = 0.5f;

	[SerializeField]
	private TextMeshPro itemCostText;

	[SerializeField]
	private SpriteRenderer itemSprite;

	[SerializeField]
	private float itemSpriteSize = 1f;

	[SerializeField]
	private SpriteRenderer materialCostIcon;

	[SerializeField]
	private SpriteRenderer upgradeIcon;

	[Space]
	[SerializeField]
	private NestedFadeGroupBase fadeGroup;

	[SerializeField]
	private float topY;

	[SerializeField]
	private float botY;

	[NonSerialized]
	public int ItemNumber;

	private bool isInitialised;

	private Vector3 initialCostSpriteScale;

	private bool hidden = true;

	private bool DisplayMoneyCost
	{
		get
		{
			if (shopItem.Cost <= 0)
			{
				return !shopItem.RequiredItem;
			}
			return true;
		}
	}

	public ShopItem Item
	{
		get
		{
			return shopItem;
		}
		set
		{
			SetItem(value);
		}
	}

	private void Awake()
	{
		Init();
	}

	private void OnEnable()
	{
		fadeGroup.AlphaSelf = 0f;
		hidden = true;
	}

	private void Update()
	{
		float y = base.transform.position.y;
		if (y > topY || y < botY)
		{
			if (!hidden)
			{
				fadeGroup.AlphaSelf = 0f;
				hidden = true;
			}
		}
		else if (hidden)
		{
			fadeGroup.AlphaSelf = 1f;
			hidden = false;
		}
	}

	private void Init()
	{
		if (!isInitialised)
		{
			initialCostSpriteScale = costSprite.transform.localScale;
			isInitialised = true;
		}
	}

	public int GetCost()
	{
		return shopItem.Cost;
	}

	public string GetName()
	{
		return shopItem.DisplayName;
	}

	public string GetDesc()
	{
		return shopItem.Description;
	}

	public int GetItemNumber()
	{
		return ItemNumber;
	}

	public Sprite GetCurrencySprite()
	{
		return Item.CurrencyType switch
		{
			CurrencyType.Money => rosarySprite, 
			CurrencyType.Shard => shardSprite, 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}

	private bool HasRequiredItems()
	{
		if (Item.RequiredTools != 0 && ToolItemManager.GetOwnedToolsCount(Item.RequiredTools) < Item.RequiredToolsAmount)
		{
			return false;
		}
		if ((bool)Item.UpgradeFromItem && Item.UpgradeFromItem.CollectedAmount <= 0)
		{
			return false;
		}
		if ((bool)Item.RequiredItem)
		{
			return Item.RequiredItem.CollectedAmount >= Item.RequiredItemAmount;
		}
		return true;
	}

	public bool CanBuy()
	{
		if (!HasRequiredItems())
		{
			return false;
		}
		return Item.CurrencyType switch
		{
			CurrencyType.Money => PlayerData.instance.geo >= shopItem.Cost, 
			CurrencyType.Shard => PlayerData.instance.ShellShards >= shopItem.Cost, 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}

	public void BuyFail()
	{
		PlayerData instance = PlayerData.instance;
		switch (Item.CurrencyType)
		{
		case CurrencyType.Money:
			if (instance.geo < shopItem.Cost)
			{
				CurrencyCounter.ReportFail(CurrencyType.Money);
			}
			break;
		case CurrencyType.Shard:
			if (instance.ShellShards < shopItem.Cost)
			{
				CurrencyCounter.ReportFail(CurrencyType.Shard);
			}
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	public bool IsAtMax()
	{
		return Item.IsAtMax();
	}

	public bool IsAvailable()
	{
		if ((bool)shopItem)
		{
			return shopItem.IsAvailable;
		}
		return false;
	}

	public void SetItem(ShopItem item)
	{
		Init();
		shopItem = item;
		if (!shopItem)
		{
			return;
		}
		if ((bool)itemSprite)
		{
			itemSprite.sprite = item.ItemSprite;
			itemSprite.transform.localScale = Vector3.one * item.ItemSpriteScale * itemSpriteSize;
		}
		if ((bool)itemCostText)
		{
			itemCostText.text = (DisplayMoneyCost ? item.Cost.ToString() : item.RequiredItemAmount.ToString());
		}
		bool flag = (bool)item.UpgradeFromItem || ((bool)item.Item && item.Item.HasUpgradeIcon());
		if ((bool)materialCostIcon)
		{
			materialCostIcon.gameObject.SetActive((bool)item.RequiredItem && !flag);
		}
		if ((bool)upgradeIcon)
		{
			upgradeIcon.gameObject.SetActive(flag);
		}
		if ((bool)costSprite)
		{
			if (DisplayMoneyCost)
			{
				costSprite.sprite = GetCurrencySprite();
				costSprite.transform.localScale = initialCostSpriteScale;
			}
			else
			{
				costSprite.sprite = item.RequiredItem.GetIcon(CollectableItem.ReadSource.Tiny);
				costSprite.transform.localScale = Vector3.one * requiredItemCostScale;
			}
		}
	}

	public void UpdateAppearance()
	{
		if (CanBuy())
		{
			if ((bool)costSprite)
			{
				costSprite.color = activeColour;
			}
			if ((bool)itemCostText)
			{
				itemCostText.color = activeColour;
			}
		}
		else
		{
			if ((bool)costSprite)
			{
				costSprite.color = inactiveColour;
			}
			if ((bool)itemCostText)
			{
				itemCostText.color = inactiveColour;
			}
		}
		if ((bool)materialCostIcon)
		{
			materialCostIcon.color = (HasRequiredItems() ? activeColour : inactiveColour);
		}
	}

	public void SetPurchased(Action onComplete, int subItemIndex)
	{
		shopItem.SetPurchased(onComplete, subItemIndex);
	}
}

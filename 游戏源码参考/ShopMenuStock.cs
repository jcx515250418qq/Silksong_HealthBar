using System;
using System.Collections.Generic;
using System.Linq;
using TMProOld;
using UnityEngine;
using UnityEngine.UI;

public class ShopMenuStock : MonoBehaviour
{
	[SerializeField]
	private float yDistance = -1.5f;

	[SerializeField]
	private TextMeshPro titleText;

	public ShopMenuStock MasterList;

	[SerializeField]
	[Conditional("MasterList", false, false, false)]
	private ShopItem[] stock;

	[SerializeField]
	[Conditional("MasterList", false, false, false)]
	private ShopItemStats templateItem;

	[SerializeField]
	[Conditional("MasterList", false, false, false)]
	private ShopSubItemStats templateSubItem;

	[SerializeField]
	[Conditional("MasterList", false, false, false)]
	private LayoutGroup subItemsLayout;

	private List<ShopItemStats> spawnedStock = new List<ShopItemStats>();

	private List<ShopSubItemStats> spawnedSubItems = new List<ShopSubItemStats>();

	private readonly List<ShopItemStats> availableStock = new List<ShopItemStats>();

	private Vector3 selfPos;

	private double lastSpawnTime;

	public string Title
	{
		get
		{
			if (!titleText)
			{
				return string.Empty;
			}
			return titleText.text;
		}
		set
		{
			if ((bool)titleText)
			{
				titleText.text = value;
			}
		}
	}

	public bool WasItemPurchased { get; private set; }

	private void Start()
	{
		SpawnStock();
	}

	public void SpawnStock()
	{
		if ((bool)MasterList)
		{
			spawnedStock = MasterList.spawnedStock;
			spawnedSubItems = MasterList.spawnedSubItems;
			subItemsLayout = MasterList.subItemsLayout;
			return;
		}
		lastSpawnTime = Time.time;
		int num = 0;
		int num2 = 0;
		ShopItem[] array = stock;
		foreach (ShopItem shopItem in array)
		{
			if (shopItem.IsAvailable)
			{
				num++;
			}
			if (shopItem.HasSubItems)
			{
				int subItemsCount = shopItem.SubItemsCount;
				if (subItemsCount > num2)
				{
					num2 = subItemsCount;
				}
			}
		}
		templateItem.gameObject.SetActive(value: false);
		if ((bool)templateSubItem)
		{
			templateSubItem.gameObject.SetActive(value: false);
		}
		int num3 = num - spawnedStock.Count;
		if (num3 > 0)
		{
			Transform parent = templateItem.transform.parent;
			bool flag = false;
			if (parent != null)
			{
				flag = parent.gameObject.activeSelf;
				parent.gameObject.SetActive(value: true);
			}
			while (num3 > 0)
			{
				ShopItemStats shopItemStats = UnityEngine.Object.Instantiate(templateItem, parent);
				shopItemStats.gameObject.SetActive(value: true);
				shopItemStats.gameObject.SetActive(value: false);
				spawnedStock.Add(shopItemStats);
				num3--;
			}
			if (!flag)
			{
				parent.gameObject.SetActive(value: false);
			}
		}
		int num4 = num2 - spawnedSubItems.Count;
		if (num4 > 0)
		{
			Transform parent2 = templateSubItem.transform.parent;
			bool flag2 = false;
			if (parent2 != null)
			{
				flag2 = parent2.gameObject.activeSelf;
				parent2.gameObject.SetActive(value: true);
			}
			while (num4 > 0)
			{
				ShopSubItemStats shopSubItemStats = UnityEngine.Object.Instantiate(templateSubItem, parent2);
				shopSubItemStats.gameObject.SetActive(value: true);
				shopSubItemStats.gameObject.SetActive(value: false);
				spawnedSubItems.Add(shopSubItemStats);
				num4--;
			}
			if (!flag2)
			{
				parent2.gameObject.SetActive(value: false);
			}
		}
		int num5 = 0;
		bool flag3 = false;
		GameObject ownerObj = base.gameObject;
		array = stock;
		foreach (ShopItem shopItem2 in array)
		{
			if (num5 >= spawnedStock.Count)
			{
				break;
			}
			if (shopItem2.IsAvailable)
			{
				spawnedStock[num5++].Item = shopItem2;
				if (shopItem2.EnsurePool(ownerObj))
				{
					flag3 = true;
				}
			}
		}
		if (flag3)
		{
			PersonalObjectPool.EnsurePooledInSceneFinished(ownerObj);
		}
		for (int j = num5; j < spawnedStock.Count; j++)
		{
			spawnedStock[j].Item = null;
		}
	}

	public void SetStock(ShopItem[] newStock)
	{
		if (stock != newStock || !(Time.timeAsDouble - lastSpawnTime < 0.5))
		{
			stock = newStock;
			bool activeSelf = base.gameObject.activeSelf;
			if (!activeSelf)
			{
				base.gameObject.SetActive(value: true);
			}
			SpawnStock();
			if (!activeSelf)
			{
				base.gameObject.SetActive(value: false);
			}
		}
	}

	public IEnumerable<ShopItem> EnumerateStock()
	{
		return stock.Where((ShopItem item) => item);
	}

	public void UpdateStock()
	{
	}

	public bool StockLeft()
	{
		ShopItem[] array = stock;
		foreach (ShopItem shopItem in array)
		{
			if ((bool)shopItem && shopItem.IsAvailable)
			{
				return true;
			}
		}
		return false;
	}

	public bool StockLeftNotInfinite()
	{
		ShopItem[] array = stock;
		foreach (ShopItem shopItem in array)
		{
			if ((bool)shopItem && shopItem.IsAvailableNotInfinite)
			{
				return true;
			}
		}
		return false;
	}

	public void BuildItemList()
	{
		SpawnStock();
		availableStock.Clear();
		float num = 0f;
		foreach (ShopItemStats item in spawnedStock)
		{
			if (item.IsAvailable())
			{
				item.transform.localPosition = new Vector3(0f, num, 0f);
				item.ItemNumber = availableStock.Count;
				availableStock.Add(item);
				num += yDistance;
				item.gameObject.SetActive(value: true);
				item.UpdateAppearance();
			}
		}
		foreach (ShopSubItemStats spawnedSubItem in spawnedSubItems)
		{
			spawnedSubItem.gameObject.SetActive(value: false);
		}
	}

	public int GetItemCount()
	{
		return availableStock.Count;
	}

	public int GetCost(int itemNum)
	{
		return availableStock[itemNum].GetCost();
	}

	public string GetName(int itemNum)
	{
		return availableStock[itemNum].GetName();
	}

	public string GetDesc(int itemNum)
	{
		return availableStock[itemNum].GetDesc();
	}

	public float GetYDistance()
	{
		return yDistance;
	}

	public Sprite GetItemSprite(int itemNum)
	{
		return availableStock[itemNum].Item.ItemSprite;
	}

	public Sprite GetSubItemPurchaseIcon(int itemNum, int subItem)
	{
		return availableStock[itemNum].Item.GetSubItem(subItem).PurchaseIcon;
	}

	public Vector3 GetItemSpriteScale(int itemNum)
	{
		return Vector3.one * availableStock[itemNum].Item.ItemSpriteScale;
	}

	public Sprite GetItemCurrencySprite(int itemNum)
	{
		return availableStock[itemNum].GetCurrencySprite();
	}

	public bool CanBuy(int itemNum)
	{
		return availableStock[itemNum].CanBuy();
	}

	public void BuyFail(int itemNum)
	{
		availableStock[itemNum].BuyFail();
	}

	public bool IsAtMax(int itemNum)
	{
		return availableStock[itemNum].IsAtMax();
	}

	public GameObject GetItemGameObject(int itemNum)
	{
		return availableStock[itemNum].gameObject;
	}

	public bool IsToolItem(int itemNum)
	{
		return availableStock[itemNum].Item.IsToolItem();
	}

	public ToolItemType GetToolType(int itemNum)
	{
		return availableStock[itemNum].Item.GetToolType();
	}

	public ShopItem.PurchaseTypes GetPurchaseType(int itemNum)
	{
		return availableStock[itemNum].Item.GetPurchaseType();
	}

	public CollectableItem GetRequiredItem(int itemNum)
	{
		return availableStock[itemNum].Item.RequiredItem;
	}

	public int GetRequiredItemAmount(int itemNum)
	{
		return availableStock[itemNum].Item.RequiredItemAmount;
	}

	public void SetWasItemPurchased(bool value)
	{
		WasItemPurchased = value;
	}

	public void DisplayCurrencyCounters()
	{
		bool flag = false;
		bool flag2 = false;
		ShopItem[] array = stock;
		foreach (ShopItem shopItem in array)
		{
			if ((bool)shopItem && shopItem.IsAvailable)
			{
				switch (shopItem.CurrencyType)
				{
				case CurrencyType.Money:
					flag = true;
					break;
				case CurrencyType.Shard:
					flag2 = true;
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
			}
		}
		if (flag)
		{
			CurrencyCounter.Show(CurrencyType.Money, setStackVisible: true);
		}
		if (flag2)
		{
			CurrencyCounter.Show(CurrencyType.Shard, setStackVisible: true);
		}
		array = stock;
		foreach (ShopItem shopItem2 in array)
		{
			if ((bool)shopItem2 && shopItem2.IsAvailable)
			{
				if ((bool)shopItem2.RequiredItem)
				{
					ItemCurrencyCounter.Show(shopItem2.RequiredItem);
				}
				if ((bool)shopItem2.UpgradeFromItem)
				{
					ItemCurrencyCounter.Show(shopItem2.UpgradeFromItem);
				}
			}
		}
	}

	public void HideCurrencyCounters()
	{
		HashSet<CurrencyType> hashSet = new HashSet<CurrencyType>(stock.Length);
		HashSet<CollectableItem> hashSet2 = new HashSet<CollectableItem>(stock.Length);
		ShopItem[] array = stock;
		foreach (ShopItem shopItem in array)
		{
			if ((bool)shopItem)
			{
				hashSet.Add(shopItem.CurrencyType);
				if ((bool)shopItem.RequiredItem)
				{
					hashSet2.Add(shopItem.RequiredItem);
				}
				if ((bool)shopItem.UpgradeFromItem)
				{
					hashSet2.Add(shopItem.UpgradeFromItem);
				}
			}
		}
		foreach (CurrencyType item in hashSet)
		{
			CurrencyCounter.HideForced(item);
		}
		foreach (CollectableItem item2 in hashSet2)
		{
			ItemCurrencyCounter.HideForced(item2);
		}
	}

	public string GetEventAfterPurchase(int itemNum)
	{
		return availableStock[itemNum].Item.EventAfterPurchase;
	}

	public bool GetHasSubItems(int itemNum)
	{
		return availableStock[itemNum].Item.HasSubItems;
	}

	public void SetupSubItems(int itemNum)
	{
		ShopItem item = availableStock[itemNum].Item;
		int subItemsCount = item.SubItemsCount;
		for (int i = 0; i < subItemsCount; i++)
		{
			ShopItem.SubItem subItem = item.GetSubItem(i);
			ShopSubItemStats shopSubItemStats = spawnedSubItems[i];
			shopSubItemStats.Setup(subItem);
			shopSubItemStats.gameObject.SetActive(value: true);
		}
		if ((bool)subItemsLayout)
		{
			subItemsLayout.ForceUpdateLayoutNoCanvas();
		}
	}

	public string GetSubItemSelectPrompt(int itemNum)
	{
		return availableStock[itemNum].Item.SubItemSelectPrompt;
	}
}

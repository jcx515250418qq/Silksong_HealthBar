using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemCurrencyCounter : CurrencyCounterTyped<CollectableItem>
{
	[SerializeField]
	private SpriteRenderer iconSprite;

	private CollectableItem item;

	private static readonly List<ItemCurrencyCounter> ItemCounters = new List<ItemCurrencyCounter>();

	private static ItemCurrencyCounter _templateItem;

	protected override int Count
	{
		get
		{
			if (!item)
			{
				return 0;
			}
			return item.CollectedAmount;
		}
	}

	protected override CollectableItem CounterType => item;

	protected override void Awake()
	{
		base.Awake();
		if (_templateItem == null)
		{
			_templateItem = this;
			return;
		}
		ItemCounters.Add(this);
		CurrencyCounterStack currencyCounterStack = (base.transform.parent ? base.transform.parent.GetComponent<CurrencyCounterStack>() : null);
		if ((bool)currencyCounterStack)
		{
			currencyCounterStack.AddNewCounter(this);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (_templateItem == this)
		{
			_templateItem = null;
		}
		else
		{
			ItemCounters.Remove(this);
		}
	}

	public void SetItem(CollectableItem newItem)
	{
		item = newItem;
		if ((bool)iconSprite)
		{
			iconSprite.sprite = newItem.GetIcon(CollectableItem.ReadSource.Tiny);
		}
		UpdateCounterStart();
	}

	private static ItemCurrencyCounter GetCurrencyCounter(CollectableItem item, bool getNew)
	{
		if (item.HideInShopCounters)
		{
			return null;
		}
		ItemCurrencyCounter itemCurrencyCounter = ItemCounters.FirstOrDefault((ItemCurrencyCounter c) => c.item == item && c.IsActive);
		if (itemCurrencyCounter != null || !getNew)
		{
			return itemCurrencyCounter;
		}
		itemCurrencyCounter = ItemCounters.FirstOrDefault((ItemCurrencyCounter c) => !c.IsActive);
		if (itemCurrencyCounter != null)
		{
			itemCurrencyCounter.SetItem(item);
			return itemCurrencyCounter;
		}
		if (_templateItem == null)
		{
			return null;
		}
		itemCurrencyCounter = Object.Instantiate(_templateItem, _templateItem.transform.parent);
		itemCurrencyCounter.SetItem(item);
		return itemCurrencyCounter;
	}

	public static void Show(CollectableItem item)
	{
		ItemCurrencyCounter currencyCounter = GetCurrencyCounter(item, getNew: true);
		if (!(currencyCounter == null))
		{
			currencyCounter.InternalShow();
		}
	}

	public static void Hide(CollectableItem item)
	{
		ItemCurrencyCounter currencyCounter = GetCurrencyCounter(item, getNew: false);
		if (!(currencyCounter == null))
		{
			currencyCounter.InternalHide();
		}
	}

	public static void HideForced(CollectableItem item)
	{
		ItemCurrencyCounter currencyCounter = GetCurrencyCounter(item, getNew: false);
		if (!(currencyCounter == null))
		{
			currencyCounter.InternalHide(forced: true);
		}
	}

	public static void Take(CollectableItem item, int amount)
	{
		ItemCurrencyCounter currencyCounter = GetCurrencyCounter(item, getNew: true);
		if (!(currencyCounter == null))
		{
			currencyCounter.InternalTake(amount);
		}
	}

	public static void UpdateValue(CollectableItem item)
	{
		ItemCurrencyCounter itemCurrencyCounter = ItemCounters.FirstOrDefault((ItemCurrencyCounter c) => c.item == item && c.IsActive);
		if (!(itemCurrencyCounter == null))
		{
			itemCurrencyCounter.UpdateValue();
		}
	}
}

using System.Collections.Generic;
using System.Linq;
using TeamCherry.NestedFadeGroup;
using TeamCherry.SharedUtils;
using UnityEngine;

public class LiquidReserveCounter : CurrencyCounterTyped<ToolItemStatesLiquid>
{
	[Space]
	[SerializeField]
	private NestedFadeGroupSpriteRenderer fill;

	[SerializeField]
	private MinMaxFloat fillPosRange;

	[Space]
	[SerializeField]
	private CurrencyCounterIcon infiniteIcon;

	private ToolItemStatesLiquid item;

	private static readonly List<LiquidReserveCounter> _itemCounters = new List<LiquidReserveCounter>();

	private static LiquidReserveCounter _templateItem;

	protected override int Count
	{
		get
		{
			if (!item)
			{
				return 0;
			}
			return item.LiquidSavedData.RefillsLeft;
		}
	}

	protected override ToolItemStatesLiquid CounterType => item;

	protected override void Awake()
	{
		base.Awake();
		if (_templateItem == null)
		{
			_templateItem = this;
			return;
		}
		_itemCounters.Add(this);
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
			_itemCounters.Remove(this);
		}
	}

	public void SetItem(ToolItemStatesLiquid newItem)
	{
		item = newItem;
		fill.Color = item.LiquidColor;
		UpdateCounterStart();
	}

	protected override void RefreshText(bool isCountingUp)
	{
		if ((bool)item)
		{
			int refillsMax = item.RefillsMax;
			float t = (float)item.LiquidSavedData.RefillsLeft / (float)refillsMax;
			float lerpedValue = fillPosRange.GetLerpedValue(t);
			fill.transform.SetLocalPositionY(lerpedValue);
		}
	}

	private static LiquidReserveCounter GetCurrencyCounter(ToolItemStatesLiquid item, bool getNew)
	{
		LiquidReserveCounter liquidReserveCounter = _itemCounters.FirstOrDefault((LiquidReserveCounter c) => c.item == item && c.IsActiveOrQueued);
		if (liquidReserveCounter != null || !getNew)
		{
			return liquidReserveCounter;
		}
		liquidReserveCounter = _itemCounters.FirstOrDefault((LiquidReserveCounter c) => !c.IsActiveOrQueued);
		if (liquidReserveCounter != null)
		{
			liquidReserveCounter.SetItem(item);
			return liquidReserveCounter;
		}
		if (_templateItem == null)
		{
			return null;
		}
		liquidReserveCounter = Object.Instantiate(_templateItem, _templateItem.transform.parent);
		liquidReserveCounter.SetItem(item);
		return liquidReserveCounter;
	}

	public static void Take(ToolItemStatesLiquid item, int amount)
	{
		LiquidReserveCounter currencyCounter = GetCurrencyCounter(item, getNew: true);
		if (!(currencyCounter == null))
		{
			currencyCounter.IconOverride = null;
			if ((bool)currencyCounter.infiniteIcon)
			{
				currencyCounter.infiniteIcon.gameObject.SetActive(value: false);
			}
			currencyCounter.QueueTake(amount);
		}
	}

	public static void InfiniteRefillPopup(ToolItemStatesLiquid item)
	{
		LiquidReserveCounter currencyCounter = GetCurrencyCounter(item, getNew: true);
		if (!(currencyCounter == null))
		{
			if ((bool)currencyCounter.infiniteIcon)
			{
				currencyCounter.infiniteIcon.gameObject.SetActive(value: true);
				currencyCounter.IconOverride = currencyCounter.infiniteIcon;
			}
			currencyCounter.QueuePopup();
		}
	}
}

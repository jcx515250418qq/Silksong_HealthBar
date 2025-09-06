using System;
using UnityEngine;

public class CurrencyCounterAppearRegion : TrackTriggerObjects
{
	private enum CounterType
	{
		Currency = 0,
		CollectableItem = 1
	}

	[Serializable]
	private struct CounterInfo
	{
		public CounterType CounterType;

		[ModifiableProperty]
		[Conditional("IsCurrencyType", true, true, true)]
		public CurrencyType CurrencyType;

		[ModifiableProperty]
		[Conditional("IsCurrencyType", false, true, true)]
		public CollectableItem CollectableItem;

		private bool IsCurrencyType()
		{
			return CounterType == CounterType.Currency;
		}
	}

	[SerializeField]
	private CounterInfo[] showCounters;

	protected override bool RequireEnabled => true;

	protected override void OnInsideStateChanged(bool isInside)
	{
		CounterInfo[] array = showCounters;
		for (int i = 0; i < array.Length; i++)
		{
			CounterInfo counterInfo = array[i];
			switch (counterInfo.CounterType)
			{
			case CounterType.Currency:
				if (isInside)
				{
					CurrencyCounter.Show(counterInfo.CurrencyType);
				}
				else
				{
					CurrencyCounter.Hide(counterInfo.CurrencyType);
				}
				break;
			case CounterType.CollectableItem:
				if (isInside)
				{
					ItemCurrencyCounter.Show(counterInfo.CollectableItem);
				}
				else
				{
					ItemCurrencyCounter.Hide(counterInfo.CollectableItem);
				}
				break;
			}
		}
	}
}

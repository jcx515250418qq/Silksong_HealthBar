using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class CurrencyCounterStack : MonoBehaviour
{
	[SerializeField]
	private float yOffset;

	[Space]
	[SerializeField]
	private Transform outAppearParent;

	[SerializeField]
	private Vector2 outAppearPosition;

	[Space]
	[SerializeField]
	private PositionRelativeTo inPositioner;

	private Transform initialParent;

	private Vector3 initialPosition;

	private Vector3 initialScale;

	private bool isVisibleTargetNull;

	private bool wasVisible = true;

	private readonly HashSet<CurrencyCounterBase> activeCounters = new HashSet<CurrencyCounterBase>();

	private readonly List<CurrencyCounterBase> currentCounters = new List<CurrencyCounterBase>();

	private void Awake()
	{
		Transform transform = base.transform;
		initialParent = transform.parent;
		initialPosition = transform.localPosition;
		initialScale = transform.localScale;
	}

	[UsedImplicitly]
	public void ReportVisibleChange(bool isVisible)
	{
		if (wasVisible == isVisible)
		{
			return;
		}
		Transform transform = base.transform;
		if (isVisible)
		{
			if ((bool)inPositioner)
			{
				inPositioner.enabled = true;
			}
			transform.SetParent(initialParent);
			transform.localPosition = initialPosition;
			transform.localScale = initialScale;
			CurrencyCounterBase.FadeInIfActive();
		}
		else
		{
			if ((bool)inPositioner)
			{
				inPositioner.enabled = false;
			}
			transform.SetParent(outAppearParent);
			transform.SetLocalPosition2D(outAppearPosition);
			transform.localScale = initialScale;
			CurrencyCounterBase.HideAllInstant();
		}
		wasVisible = isVisible;
	}

	public void AddNewCounter(CurrencyCounterBase counter)
	{
		CurrencyCounterBase currencyCounterBase = counter;
		currencyCounterBase.Appeared = (Action)Delegate.Combine(currencyCounterBase.Appeared, (Action)delegate
		{
			if (activeCounters.Add(counter))
			{
				int num = -1;
				for (int num2 = currentCounters.Count - 1; num2 >= 0; num2--)
				{
					if (currentCounters[num2].StackOrder <= counter.StackOrder)
					{
						num = num2 + 1;
						break;
					}
					num = num2;
				}
				if (num < 0 || num >= currentCounters.Count)
				{
					currentCounters.Add(counter);
					SetPosition(counter);
				}
				else
				{
					currentCounters.Insert(num, counter);
					for (int i = 0; i < currentCounters.Count; i++)
					{
						Transform obj = currentCounters[i].transform;
						Vector3 localPosition = obj.localPosition;
						float? y = yOffset * (float)i;
						obj.localPosition = localPosition.Where(null, y, null);
					}
				}
			}
		});
		CurrencyCounterBase currencyCounterBase2 = counter;
		currencyCounterBase2.Disappeared = (Action)Delegate.Combine(currencyCounterBase2.Disappeared, (Action)delegate
		{
			if (activeCounters.Remove(counter) && activeCounters.Count == 0)
			{
				currentCounters.Clear();
			}
		});
	}

	private void SetPosition(CurrencyCounterBase counter)
	{
		Transform obj = counter.transform;
		Vector3 localPosition = obj.localPosition;
		float? y = yOffset * (float)currentCounters.IndexOf(counter);
		obj.localPosition = localPosition.Where(null, y, null);
	}
}

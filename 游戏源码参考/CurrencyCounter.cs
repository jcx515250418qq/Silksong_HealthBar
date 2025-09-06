using System;
using System.Collections.Generic;
using GlobalSettings;
using UnityEngine;

public class CurrencyCounter : CurrencyCounterTyped<CurrencyType>
{
	private static readonly Dictionary<CurrencyType, List<CurrencyCounter>> _currencyCounters = new Dictionary<CurrencyType, List<CurrencyCounter>>();

	[Space]
	[SerializeField]
	private CurrencyType currencyType;

	[SerializeField]
	private TextBridge limitTextMesh;

	private string limitTextFormat;

	private bool hasStarted;

	private CurrencyCounterStack stack;

	protected override int Count
	{
		get
		{
			PlayerData instance = PlayerData.instance;
			return currencyType switch
			{
				CurrencyType.Money => instance.geo, 
				CurrencyType.Shard => instance.ShellShards, 
				_ => 0, 
			};
		}
	}

	protected override CurrencyType CounterType => currencyType;

	protected override void Awake()
	{
		base.Awake();
		if (!_currencyCounters.TryGetValue(currencyType, out var value))
		{
			value = (_currencyCounters[currencyType] = new List<CurrencyCounter>());
		}
		value.Add(this);
		Transform parent = base.transform.parent;
		stack = (parent ? parent.GetComponent<CurrencyCounterStack>() : null);
		if (stack != null)
		{
			stack.AddNewCounter(this);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (_currencyCounters.TryGetValue(currencyType, out var value))
		{
			value.Remove(this);
		}
	}

	private void OnEnable()
	{
		if ((bool)limitTextMesh)
		{
			if (limitTextFormat == null)
			{
				limitTextFormat = limitTextMesh.Text;
			}
			limitTextMesh.Text = string.Format(limitTextFormat, Gameplay.GetCurrencyCap(currencyType));
		}
		if (hasStarted)
		{
			UpdateCounterStart();
		}
	}

	protected override void Start()
	{
		hasStarted = true;
		base.Start();
		UpdateCounterStart();
	}

	protected override void RefreshText(bool isCountingUp)
	{
		Color color = ((!isCountingUp || Count < Gameplay.GetCurrencyCap(currencyType)) ? Color.white : UI.MaxItemsTextColor);
		geoTextMesh.Color = color;
		if ((bool)limitTextMesh)
		{
			limitTextMesh.Color = color;
		}
	}

	public void SetStackVisible(bool isVisible)
	{
		if (stack != null)
		{
			stack.ReportVisibleChange(isVisible);
		}
	}

	public static void Add(int amount, CurrencyType type)
	{
		if (!_currencyCounters.TryGetValue(type, out var value))
		{
			return;
		}
		foreach (CurrencyCounter item in value)
		{
			if (item.isActiveAndEnabled)
			{
				item.QueueAdd(amount);
			}
		}
	}

	public static void Take(int amount, CurrencyType type)
	{
		if (!_currencyCounters.TryGetValue(type, out var value))
		{
			return;
		}
		foreach (CurrencyCounter item in value)
		{
			if (item.isActiveAndEnabled)
			{
				item.QueueTake(amount);
			}
		}
	}

	public static void ToValue(int amount, CurrencyType type)
	{
		if (!_currencyCounters.TryGetValue(type, out var value))
		{
			return;
		}
		foreach (CurrencyCounter item in value)
		{
			if (item.isActiveAndEnabled)
			{
				item.QueueToValue(amount);
			}
		}
	}

	public static void ToZero(CurrencyType type)
	{
		if (!_currencyCounters.TryGetValue(type, out var value))
		{
			return;
		}
		foreach (CurrencyCounter item in value)
		{
			if (item.isActiveAndEnabled)
			{
				item.QueueToZero();
			}
		}
	}

	public static void Show(CurrencyType type, bool setStackVisible = false)
	{
		if (!_currencyCounters.TryGetValue(type, out var value))
		{
			return;
		}
		foreach (CurrencyCounter item in value)
		{
			if (item.isActiveAndEnabled)
			{
				if (setStackVisible)
				{
					item.SetStackVisible(isVisible: true);
				}
				if (!item.IsActive)
				{
					item.UpdateCounterStart();
				}
				item.InternalShow();
			}
		}
	}

	public static void Hide(CurrencyType type)
	{
		if (!_currencyCounters.TryGetValue(type, out var value))
		{
			return;
		}
		foreach (CurrencyCounter item in value)
		{
			if (item.isActiveAndEnabled)
			{
				item.InternalHide();
			}
		}
	}

	public static void HideForced(CurrencyType type)
	{
		if (!_currencyCounters.TryGetValue(type, out var value))
		{
			return;
		}
		foreach (CurrencyCounter item in value)
		{
			if (item.isActiveAndEnabled)
			{
				item.InternalHide(forced: true);
			}
		}
	}

	public static void ReportFail(CurrencyType type)
	{
		if (!_currencyCounters.TryGetValue(type, out var value))
		{
			return;
		}
		foreach (CurrencyCounter item in value)
		{
			if (item.isActiveAndEnabled)
			{
				item.InternalFail();
			}
		}
	}

	public static void RefreshStartCount(CurrencyType type)
	{
		if (!_currencyCounters.TryGetValue(type, out var value))
		{
			return;
		}
		foreach (CurrencyCounter item in value)
		{
			if (item.isActiveAndEnabled)
			{
				if (item.IsActive)
				{
					break;
				}
				item.UpdateCounterStart();
			}
		}
	}

	private static void DoCounterAction(CurrencyType type, Action<CurrencyCounter> forEach)
	{
		if (forEach == null || !_currencyCounters.TryGetValue(type, out var value))
		{
			return;
		}
		foreach (CurrencyCounter item in value)
		{
			if (item.isActiveAndEnabled)
			{
				forEach(item);
			}
		}
	}
}

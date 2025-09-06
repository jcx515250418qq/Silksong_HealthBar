using System;
using System.Collections;
using System.Collections.Generic;
using TMProOld;
using TeamCherry.Localization;
using UnityEngine;
using UnityEngine.UI;

public class DialogueYesNoBox : YesNoBox
{
	[Space]
	[SerializeField]
	private Animator animator;

	[Space]
	[SerializeField]
	private float appearedEventDelay = 4f / 15f;

	[SerializeField]
	private TMP_Text text;

	[SerializeField]
	private float textRevealWait;

	[SerializeField]
	private float textRevealSpeed = 20f;

	[SerializeField]
	private GameObject currencyParent;

	[SerializeField]
	private TMP_Text currencyText;

	[SerializeField]
	[ArrayForEnum(typeof(CurrencyType))]
	private GameObject[] currencyDisplays;

	[SerializeField]
	private SavedItemDisplay itemTemplate;

	[SerializeField]
	private LayoutGroup itemsLayout;

	[Space]
	[SerializeField]
	private LocalisedString notEnoughText;

	[SerializeField]
	private LocalisedString atMaxText;

	private CurrencyType? requiredCurrencyType;

	private int requiredCurrencyAmount;

	private readonly List<SavedItem> requiredItems = new List<SavedItem>();

	private readonly List<int> requiredItemAmounts = new List<int>();

	private SavedItem willGetItem;

	private readonly List<SavedItemDisplay> instantiatedItems = new List<SavedItemDisplay>();

	private static DialogueYesNoBox _instance;

	private static readonly int _textFinishedPropId = Animator.StringToHash("Text Finished");

	protected override string InactiveYesText
	{
		get
		{
			if ((bool)willGetItem && !willGetItem.CanGetMore())
			{
				return atMaxText;
			}
			if (requiredItems.Count > 0)
			{
				for (int i = 0; i < requiredItems.Count; i++)
				{
					SavedItem savedItem = requiredItems[i];
					int num = requiredItemAmounts[i];
					if (savedItem.GetSavedAmount() < num)
					{
						return notEnoughText;
					}
				}
			}
			if (requiredCurrencyType.HasValue)
			{
				if (CurrencyManager.GetCurrencyAmount(requiredCurrencyType.Value) >= requiredCurrencyAmount)
				{
					return string.Empty;
				}
				return notEnoughText;
			}
			return string.Empty;
		}
	}

	protected override bool ShouldHideHud
	{
		get
		{
			if (requiredItemAmounts.Count <= 0)
			{
				return requiredCurrencyAmount <= 0;
			}
			return false;
		}
	}

	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize(ref currencyDisplays, typeof(CurrencyType));
	}

	protected override void Awake()
	{
		base.Awake();
		OnValidate();
		if (!_instance)
		{
			_instance = this;
		}
	}

	private void OnDestroy()
	{
		if (_instance == this)
		{
			_instance = null;
		}
	}

	public static void Open(Action yes, Action no, bool returnHud, string text, SavedItem willGetItem = null)
	{
		if (!_instance)
		{
			return;
		}
		if ((bool)_instance.animator)
		{
			_instance.animator.SetBool(_textFinishedPropId, value: false);
		}
		if (_instance.instantiatedItems != null)
		{
			foreach (SavedItemDisplay instantiatedItem in _instance.instantiatedItems)
			{
				instantiatedItem.gameObject.SetActive(value: false);
			}
		}
		_instance.PreOpen(willGetItem, text);
		_instance.InternalOpen(yes, no, returnHud);
	}

	public static void Open(Action yes, Action no, bool returnHud, SavedItem item, int amount, bool displayHudPopup = true, bool consumeCurrency = false, SavedItem willGetItem = null)
	{
		string format = Language.Get("GIVE_ITEM_PROMPT", "UI");
		format = string.Format(format, item.GetPopupName());
		Open(yes, no, returnHud, format, CurrencyType.Money, 0, new SavedItem[1] { item }, new int[1] { amount }, displayHudPopup, consumeCurrency, willGetItem);
	}

	public static void Open(Action yes, Action no, bool returnHud, string formatText, SavedItem item, int amount, bool displayHudPopup = true, bool consumeCurrency = false, SavedItem willGetItem = null)
	{
		string text;
		try
		{
			CollectableItem collectableItem = item as CollectableItem;
			text = string.Format(formatText, collectableItem ? collectableItem.GetDisplayName(CollectableItem.ReadSource.TakePopup) : item.GetPopupName());
		}
		catch (FormatException)
		{
			text = formatText;
		}
		Open(yes, no, returnHud, text, CurrencyType.Money, 0, new SavedItem[1] { item }, new int[1] { amount }, displayHudPopup, consumeCurrency, willGetItem);
	}

	private void PreOpen(SavedItem willGet, string textValue)
	{
		GameObject[] array = currencyDisplays;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: false);
		}
		requiredCurrencyType = null;
		requiredCurrencyAmount = 0;
		requiredItems.Clear();
		requiredItemAmounts.Clear();
		currencyParent.SetActive(value: false);
		itemsLayout.gameObject.SetActive(value: false);
		_instance.itemTemplate.gameObject.SetActive(value: false);
		willGetItem = willGet;
		StartCoroutine(_instance.AnimateOut(textValue));
	}

	public static void Open(Action yes, Action no, bool returnHud, string text, CurrencyType currencyType, int amount, bool displayHudPopup = true, bool consumeCurrency = true, SavedItem willGetItem = null)
	{
		Open(yes, no, returnHud, text, currencyType, amount, null, null, displayHudPopup, consumeCurrency, willGetItem);
	}

	public static void Open(Action yes, Action no, bool returnHud, string text, IReadOnlyList<SavedItem> items, IReadOnlyList<int> amounts, bool displayHudPopup, bool consumeCurrency, SavedItem willGetItem)
	{
		Open(yes, no, returnHud, text, CurrencyType.Money, 0, items, amounts, displayHudPopup, consumeCurrency, willGetItem);
	}

	public static void Open(Action yes, Action no, bool returnHud, string text, CurrencyType currencyType, int currencyAmount, IReadOnlyList<SavedItem> items, IReadOnlyList<int> amounts, bool displayHudPopup, bool consumeCurrency, SavedItem willGetItem, TakeItemTypes takeItemType = TakeItemTypes.Silent)
	{
		if (!_instance)
		{
			return;
		}
		if (string.IsNullOrEmpty(text))
		{
			text = Language.Get("GIVE_ITEMS_PROMPT", "UI");
		}
		_instance.PreOpen(willGetItem, text);
		if (currencyAmount > 0)
		{
			string text2 = ((currencyAmount > 1) ? currencyAmount.ToString() : string.Empty);
			_instance.currencyDisplays[(int)currencyType].SetActive(value: true);
			if ((bool)_instance.currencyText)
			{
				_instance.currencyText.text = text2;
			}
			_instance.currencyParent.SetActive(value: true);
			_instance.requiredCurrencyAmount = currencyAmount;
			_instance.requiredCurrencyType = currencyType;
			if (displayHudPopup)
			{
				CurrencyCounter.Show(currencyType);
			}
		}
		if (items != null && items.Count > 0)
		{
			while (_instance.instantiatedItems.Count < items.Count)
			{
				_instance.instantiatedItems.Add(UnityEngine.Object.Instantiate(_instance.itemTemplate, _instance.itemTemplate.transform.parent));
			}
			for (int i = 0; i < _instance.instantiatedItems.Count; i++)
			{
				SavedItemDisplay savedItemDisplay = _instance.instantiatedItems[i];
				if (i < items.Count)
				{
					savedItemDisplay.gameObject.SetActive(value: true);
					SavedItem item = items[i];
					int amount = amounts[i];
					savedItemDisplay.Setup(item, amount);
				}
				else
				{
					savedItemDisplay.gameObject.SetActive(value: false);
				}
			}
			_instance.itemsLayout.gameObject.SetActive(value: true);
			_instance.itemsLayout.ForceUpdateLayoutNoCanvas();
			_instance.requiredItemAmounts.AddRange(amounts);
			_instance.requiredItems.AddRange(items);
			if (displayHudPopup && takeItemType == TakeItemTypes.Silent)
			{
				foreach (SavedItem item6 in items)
				{
					if (item6 is CollectableItem item2)
					{
						ItemCurrencyCounter.Show(item2);
					}
				}
			}
		}
		Action yes2 = delegate
		{
			bool flag = false;
			if (consumeCurrency)
			{
				int waitingCount = 0;
				if (currencyAmount > 0)
				{
					CurrencyManager.TakeCurrency(currencyAmount, currencyType, displayHudPopup);
					if (yes != null && displayHudPopup)
					{
						waitingCount++;
						CurrencyCounterTyped<CurrencyType>.RegisterTempCounterStateChangedHandler(delegate(CurrencyType type, CurrencyCounterBase.StateEvents state)
						{
							if (type != currencyType || state != CurrencyCounterBase.StateEvents.FadeDelayElapsed)
							{
								return false;
							}
							CurrencyCounter.Hide(currencyType);
							waitingCount--;
							if (waitingCount == 0)
							{
								yes();
							}
							return true;
						});
						flag = true;
					}
				}
				if (items != null)
				{
					for (int j = 0; j < items.Count; j++)
					{
						SavedItem item3 = items[j];
						if (item3 is ToolItem toolItem)
						{
							toolItem.Lock();
						}
						else
						{
							CollectableItem collectableItem = item3 as CollectableItem;
							if ((object)collectableItem != null)
							{
								int amount2 = amounts[j];
								bool flag2;
								if (takeItemType == TakeItemTypes.Silent)
								{
									collectableItem.Take(amount2, displayHudPopup);
									flag2 = displayHudPopup;
								}
								else
								{
									CollectableUIMsg.ShowTakeMsg(collectableItem, takeItemType);
									collectableItem.Take(amount2, showCounter: false);
									flag2 = false;
								}
								if (yes != null && flag2)
								{
									int num = waitingCount;
									waitingCount = num + 1;
									CurrencyCounterTyped<CollectableItem>.RegisterTempCounterStateChangedHandler(delegate(CollectableItem type, CurrencyCounterBase.StateEvents state)
									{
										if (type != item3 || state != CurrencyCounterBase.StateEvents.FadeDelayElapsed)
										{
											return false;
										}
										ItemCurrencyCounter.Hide(collectableItem);
										int num2 = waitingCount;
										waitingCount = num2 - 1;
										if (waitingCount == 0)
										{
											yes();
										}
										return true;
									});
									flag = true;
								}
							}
						}
					}
				}
			}
			else
			{
				if (currencyAmount > 0)
				{
					CurrencyCounter.Hide(currencyType);
				}
				if (items != null && takeItemType == TakeItemTypes.Silent)
				{
					foreach (SavedItem item7 in items)
					{
						if (item7 is CollectableItem item4)
						{
							ItemCurrencyCounter.Hide(item4);
						}
					}
				}
			}
			if (!flag && yes != null)
			{
				yes();
			}
		};
		Action no2 = delegate
		{
			if (displayHudPopup)
			{
				if (currencyAmount > 0)
				{
					CurrencyCounter.Hide(currencyType);
				}
				if (items != null && takeItemType == TakeItemTypes.Silent)
				{
					foreach (SavedItem item8 in items)
					{
						if (item8 is CollectableItem item5)
						{
							ItemCurrencyCounter.Hide(item5);
						}
					}
				}
			}
			no?.Invoke();
		};
		_instance.InternalOpen(yes2, no2, returnHud);
	}

	private IEnumerator AnimateOut(string textValue)
	{
		OnAppearing();
		if ((bool)text)
		{
			text.text = textValue;
			text.maxVisibleCharacters = 0;
			yield return new WaitForSeconds(textRevealWait);
			float visibleCharacters = 0f;
			while (text.maxVisibleCharacters < textValue.Length)
			{
				yield return null;
				visibleCharacters += textRevealSpeed * Time.deltaTime;
				text.maxVisibleCharacters = Mathf.RoundToInt(visibleCharacters);
			}
		}
		yield return null;
		if ((bool)animator)
		{
			animator.SetBool(_textFinishedPropId, value: true);
		}
		if (appearedEventDelay > 0f)
		{
			yield return new WaitForSeconds(appearedEventDelay);
		}
		OnAppeared();
	}

	public static void ForceClose()
	{
		if ((bool)_instance)
		{
			_instance.DoEnd();
		}
	}
}

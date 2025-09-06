using System;
using GlobalEnums;
using TeamCherry.Localization;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Hornet/Shop Item")]
public class ShopItem : ScriptableObject
{
	[Serializable]
	private class ConditionalSpawn
	{
		public PlayerDataTest Condition;

		public GameObject[] GameObjectsToSpawn;

		public bool EnsurePool(GameObject gameObject)
		{
			if (!Condition.IsFulfilled)
			{
				return false;
			}
			bool result = false;
			GameObject[] gameObjectsToSpawn = GameObjectsToSpawn;
			foreach (GameObject gameObject2 in gameObjectsToSpawn)
			{
				if ((bool)gameObject2)
				{
					PersonalObjectPool.EnsurePooledInScene(gameObject, gameObject2, 1, finished: false);
					result = true;
				}
			}
			return result;
		}

		public void TryInstantiate()
		{
			if (!Condition.IsFulfilled)
			{
				return;
			}
			GameObject[] gameObjectsToSpawn = GameObjectsToSpawn;
			foreach (GameObject gameObject in gameObjectsToSpawn)
			{
				if ((bool)gameObject)
				{
					gameObject.Spawn();
				}
			}
		}
	}

	[Serializable]
	private struct LocalisedStringPlural
	{
		public LocalisedString Plural;

		[SerializeField]
		[LocalisedString.NotRequired]
		public LocalisedString Single;
	}

	[Flags]
	public enum TypeFlags
	{
		None = 0,
		Item = 1,
		Map = 2,
		MapUpdateItem = 4,
		BellhomeFurnishing = 8
	}

	public enum PurchaseTypes
	{
		Purchase = 0,
		Craft = 1,
		Repair = 2
	}

	[Serializable]
	public struct SubItem
	{
		public BellhomePaintColours Value;

		public Sprite Icon;

		public Sprite PurchaseIcon;

		public PlayerDataTest Condition;
	}

	[Space]
	[SerializeField]
	private LocalisedString displayName;

	[SerializeField]
	private LocalisedString description;

	[SerializeField]
	[LocalisedString.NotRequired]
	private LocalisedString descriptionMultiple;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("ItemSpriteValidation")]
	private Sprite itemSprite;

	[SerializeField]
	private float itemSpriteScale = 1f;

	[SerializeField]
	private PurchaseTypes purchaseType;

	[Space]
	[SerializeField]
	[EnumPickerBitmask]
	private TypeFlags typeFlags;

	[Space]
	[SerializeField]
	private CurrencyType currencyType;

	[SerializeField]
	private CostReference costReference;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsUsingCostReference", false, true, false)]
	private int cost;

	[Space]
	[SerializeField]
	private CollectableItem requiredItem;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("requiredItem", true, false, false)]
	private int requiredItemAmount = 1;

	[SerializeField]
	[EnumPickerBitmask]
	private ToolItemManager.OwnToolsCheckFlags requiredTools;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsUsingRequiredTools", true, true, false)]
	private int requiredToolsAmount;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsUsingRequiredTools", true, true, false)]
	private LocalisedStringPlural requiredToolsDescription;

	[SerializeField]
	private CollectableItem upgradeFromItem;

	[Space]
	[SerializeField]
	private PlayerDataTest extraAppearConditions;

	[SerializeField]
	private QuestTest[] questsAppearConditions;

	[Space]
	[SerializeField]
	[PlayerDataField(typeof(bool), false)]
	private string playerDataBoolName;

	[SerializeField]
	private SavedItem savedItem;

	[Space]
	[SerializeField]
	[PlayerDataField(typeof(BellhomePaintColours), false)]
	private string playerDataIntName;

	[SerializeField]
	[Conditional("playerDataIntName", true, false, false)]
	private SubItem[] subItems;

	[SerializeField]
	[Conditional("playerDataIntName", true, false, false)]
	private LocalisedString subItemSelectPrompt;

	[Space]
	[SerializeField]
	private UnityEvent onPurchase;

	[SerializeField]
	private ConditionalSpawn[] spawnOnPurchaseConditionals;

	[SerializeField]
	[PlayerDataField(typeof(bool), true)]
	private string[] setExtraPlayerDataBools;

	[SerializeField]
	private PlayerDataIntOperation[] setExtraPlayerDataInts;

	[Space]
	[SerializeField]
	private string eventAfterPurchased;

	public Func<int> OverrideCostDelegate { get; set; }

	public string DisplayName => displayName;

	public string Description
	{
		get
		{
			LocalisedString localisedString;
			if (!descriptionMultiple.IsEmpty && (bool)savedItem)
			{
				try
				{
					localisedString = ((savedItem.GetSavedAmount() > 0) ? descriptionMultiple : description);
				}
				catch (NotImplementedException)
				{
					localisedString = description;
				}
			}
			else
			{
				localisedString = description;
			}
			if (requiredTools == ToolItemManager.OwnToolsCheckFlags.None)
			{
				return localisedString;
			}
			int num = requiredToolsAmount - ToolItemManager.GetOwnedToolsCount(requiredTools);
			if (num <= 0)
			{
				return localisedString;
			}
			string text;
			if (num == 1 && !requiredToolsDescription.Single.IsEmpty)
			{
				text = requiredToolsDescription.Single;
			}
			else
			{
				try
				{
					text = string.Format(requiredToolsDescription.Plural, num);
				}
				catch (FormatException exception)
				{
					text = requiredToolsDescription.Plural;
					Debug.LogException(exception, this);
				}
			}
			return string.Concat(localisedString, "\n\n", text);
		}
	}

	public Sprite ItemSprite
	{
		get
		{
			if ((bool)itemSprite)
			{
				return itemSprite;
			}
			if ((bool)upgradeFromItem)
			{
				return upgradeFromItem.GetIcon(CollectableItem.ReadSource.Shop);
			}
			if (!savedItem)
			{
				return null;
			}
			return savedItem.GetPopupIcon();
		}
	}

	public float ItemSpriteScale => itemSpriteScale;

	public CurrencyType CurrencyType => currencyType;

	public int Cost
	{
		get
		{
			if (OverrideCostDelegate != null)
			{
				return OverrideCostDelegate();
			}
			if ((bool)costReference)
			{
				return costReference.Value;
			}
			return cost;
		}
	}

	public CollectableItem RequiredItem => requiredItem;

	public int RequiredItemAmount => requiredItemAmount;

	public ToolItemManager.OwnToolsCheckFlags RequiredTools => requiredTools;

	public int RequiredToolsAmount => requiredToolsAmount;

	public CollectableItem UpgradeFromItem => upgradeFromItem;

	public bool IsAvailable
	{
		get
		{
			if (IsPurchased)
			{
				return false;
			}
			if ((bool)UpgradeFromItem && UpgradeFromItem.CollectedAmount <= 0)
			{
				return false;
			}
			QuestTest[] array = questsAppearConditions;
			foreach (QuestTest questTest in array)
			{
				if (!questTest.IsFulfilled)
				{
					return false;
				}
			}
			return extraAppearConditions.IsFulfilled;
		}
	}

	public bool IsPurchased
	{
		get
		{
			PlayerData instance = PlayerData.instance;
			if (!string.IsNullOrEmpty(playerDataBoolName) && instance.GetVariable<bool>(playerDataBoolName))
			{
				return true;
			}
			if (!string.IsNullOrEmpty(playerDataIntName) && instance.GetVariable<BellhomePaintColours>(playerDataIntName) != 0)
			{
				return true;
			}
			if ((bool)savedItem && !savedItem.CanGetMore())
			{
				return true;
			}
			return false;
		}
	}

	public bool IsAvailableNotInfinite
	{
		get
		{
			if (!IsAvailable)
			{
				return false;
			}
			if (!string.IsNullOrEmpty(playerDataBoolName))
			{
				return true;
			}
			if ((bool)savedItem && savedItem.IsUnique)
			{
				return true;
			}
			return false;
		}
	}

	public SavedItem Item => savedItem;

	public string EventAfterPurchase => eventAfterPurchased;

	public int SubItemsCount
	{
		get
		{
			int num = 0;
			if (subItems != null)
			{
				SubItem[] array = subItems;
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].Condition.IsFulfilled)
					{
						num++;
					}
				}
			}
			return num;
		}
	}

	public bool HasSubItems => SubItemsCount > 0;

	public LocalisedString SubItemSelectPrompt => subItemSelectPrompt;

	public bool IsUsingCostReference()
	{
		return costReference;
	}

	public bool IsUsingRequiredTools()
	{
		return requiredTools != ToolItemManager.OwnToolsCheckFlags.None;
	}

	private bool? ItemSpriteValidation()
	{
		if ((bool)savedItem && (bool)ItemSprite && itemSprite == null)
		{
			return null;
		}
		return itemSprite;
	}

	private void OnValidate()
	{
		if (requiredItemAmount < 1 && (bool)requiredItem)
		{
			requiredItemAmount = 1;
		}
	}

	private void Awake()
	{
		OnValidate();
		if (questsAppearConditions == null)
		{
			questsAppearConditions = Array.Empty<QuestTest>();
		}
	}

	public void SetPurchased(Action onComplete, int subItemIndex)
	{
		switch (CurrencyType)
		{
		case CurrencyType.Money:
			CurrencyManager.TakeGeo(Cost);
			break;
		case CurrencyType.Shard:
			CurrencyManager.TakeShards(Cost);
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		ConditionalSpawn[] array = spawnOnPurchaseConditionals;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].TryInstantiate();
		}
		if ((bool)requiredItem)
		{
			requiredItem.Consume(requiredItemAmount, showCounter: true);
		}
		if (!string.IsNullOrEmpty(playerDataBoolName))
		{
			PlayerData.instance.SetVariable(playerDataBoolName, value: true);
		}
		if (!string.IsNullOrEmpty(playerDataIntName) && subItemIndex >= 0 && subItemIndex < SubItemsCount)
		{
			PlayerData.instance.SetVariable(playerDataIntName, GetSubItem(subItemIndex).Value);
		}
		if ((bool)upgradeFromItem)
		{
			upgradeFromItem.Consume(upgradeFromItem.CollectedAmount, showCounter: true);
		}
		if (onPurchase != null)
		{
			onPurchase.Invoke();
		}
		string[] array2 = setExtraPlayerDataBools;
		foreach (string text in array2)
		{
			if (!string.IsNullOrEmpty(text))
			{
				PlayerData.instance.SetVariable(text, value: true);
			}
		}
		PlayerDataIntOperation[] array3 = setExtraPlayerDataInts;
		foreach (PlayerDataIntOperation playerDataIntOperation in array3)
		{
			playerDataIntOperation.Execute();
		}
		if ((typeFlags & TypeFlags.Map) != 0)
		{
			GameManager instance = GameManager.instance;
			instance.UpdateGameMap();
			instance.DidPurchaseMap = true;
			instance.CheckMapAchievements();
		}
		else if (typeFlags.HasFlag(TypeFlags.MapUpdateItem))
		{
			GameManager.instance.UpdateGameMapPins();
		}
		CollectableItemManager.IncrementVersion();
		ToolItem toolItem = savedItem as ToolItem;
		if (toolItem != null)
		{
			toolItem.Unlock(onComplete, ToolItem.PopupFlags.Tutorial);
			return;
		}
		if ((bool)savedItem)
		{
			savedItem.Get(showPopup: false);
		}
		onComplete?.Invoke();
	}

	public bool IsToolItem()
	{
		if ((bool)savedItem)
		{
			return savedItem is ToolItem;
		}
		return false;
	}

	public bool IsAtMax()
	{
		CollectableItem collectableItem = savedItem as CollectableItem;
		if (collectableItem == null)
		{
			return false;
		}
		return collectableItem.IsAtMax();
	}

	public ToolItemType GetToolType()
	{
		ToolItem toolItem = savedItem as ToolItem;
		if (toolItem == null)
		{
			return ToolItemType.Red;
		}
		return toolItem.Type;
	}

	public PurchaseTypes GetPurchaseType()
	{
		return purchaseType;
	}

	public TypeFlags GetTypeFlags()
	{
		return typeFlags;
	}

	public bool EnsurePool(GameObject gameObject)
	{
		bool result = false;
		ConditionalSpawn[] array = spawnOnPurchaseConditionals;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].EnsurePool(gameObject))
			{
				result = true;
			}
		}
		return result;
	}

	public SubItem GetSubItem(int index)
	{
		int num = 0;
		SubItem[] array = subItems;
		for (int i = 0; i < array.Length; i++)
		{
			SubItem result = array[i];
			if (result.Condition.IsFulfilled)
			{
				if (num == index)
				{
					return result;
				}
				num++;
			}
		}
		throw new IndexOutOfRangeException();
	}

	public static ShopItem CreateTemp(string name)
	{
		ShopItem shopItem = ScriptableObject.CreateInstance<ShopItem>();
		shopItem.displayName.Sheet = "TEMP";
		shopItem.displayName.Key = name;
		shopItem.extraAppearConditions = new PlayerDataTest();
		shopItem.questsAppearConditions = new QuestTest[0];
		shopItem.spawnOnPurchaseConditionals = new ConditionalSpawn[0];
		shopItem.setExtraPlayerDataBools = new string[0];
		shopItem.setExtraPlayerDataInts = new PlayerDataIntOperation[0];
		return shopItem;
	}
}

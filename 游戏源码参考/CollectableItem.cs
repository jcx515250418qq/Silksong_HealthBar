using System;
using System.Collections.Generic;
using System.Linq;
using GlobalSettings;
using TeamCherry.Localization;
using TeamCherry.SharedUtils;
using UnityEngine;

public abstract class CollectableItem : QuestTargetCounter, ICollectionViewerItem
{
	public enum ReadSource
	{
		Inventory = 0,
		GetPopup = 1,
		Tiny = 2,
		Shop = 3,
		TakePopup = 4
	}

	public enum UseTypes
	{
		None = 0,
		Rosaries = 1,
		Shards = 2,
		ReturnCocoon = 3,
		GetSilk = 4
	}

	[Serializable]
	public struct UseResponse
	{
		public UseTypes UseType;

		[ModifiableProperty]
		[Conditional("UsesAmount", true, true, false)]
		public int Amount;

		[ModifiableProperty]
		[Conditional("UsesAmountRange", true, true, false)]
		public MinMaxInt AmountRange;

		[LocalisedString.NotRequired]
		public LocalisedString Description;

		public int GetAmount()
		{
			if (Amount != 0)
			{
				return Amount;
			}
			return AmountRange.GetRandomValue();
		}

		public string GetAmountText()
		{
			if (Amount != 0)
			{
				return Amount.ToString();
			}
			return $"{AmountRange.Start}-{AmountRange.End}";
		}

		private bool UsesAmount()
		{
			return UseType != UseTypes.ReturnCocoon;
		}

		private bool UsesAmountRange()
		{
			if (UsesAmount())
			{
				return Amount == 0;
			}
			return false;
		}
	}

	[SerializeField]
	private float popupIconScale = 1f;

	[Space]
	[SerializeField]
	private UseResponse[] useResponses;

	[SerializeField]
	[LocalisedString.NotRequired]
	private LocalisedString useResponseTextOverride;

	[SerializeField]
	private bool preventUseChaining;

	[SerializeField]
	private AudioEventRandom useSounds;

	[SerializeField]
	private AudioEventRandom instantUseSounds;

	[SerializeField]
	private bool alwaysPlayInstantUse;

	[SerializeField]
	private bool skipBenchUseEffect;

	[SerializeField]
	private GameObject extraUseEffect;

	[Space]
	[SerializeField]
	private CustomInventoryItemCollectableDisplay customInventoryDisplay;

	[SerializeField]
	private GameObject extraDescriptionSection;

	[SerializeField]
	private bool resetIsSeen;

	[SerializeField]
	private bool isVisibleWithBareInventory;

	[SerializeField]
	private bool isHidden;

	[SerializeField]
	private bool hideInShopCounters;

	[Space]
	[SerializeField]
	private Quest useQuestForCap;

	[SerializeField]
	private int customMaxAmount;

	[Space]
	[SerializeField]
	private PlayerStory.EventTypes storyEvent = PlayerStory.EventTypes.None;

	public LocalisedString UseResponseTextOverride => useResponseTextOverride;

	public bool PreventUseChaining => preventUseChaining;

	public AudioEventRandom UseSounds => useSounds;

	public AudioEventRandom InstantUseSounds => instantUseSounds;

	public bool AlwaysPlayInstantUse => alwaysPlayInstantUse;

	public bool SkipBenchUseEffect => skipBenchUseEffect;

	public GameObject ExtraUseEffect => extraUseEffect;

	public CustomInventoryItemCollectableDisplay CustomInventoryDisplay => customInventoryDisplay;

	public GameObject ExtraDescriptionSection => extraDescriptionSection;

	public bool HideInShopCounters => hideInShopCounters;

	public abstract bool DisplayAmount { get; }

	public CollectableItemsData.Data SaveData
	{
		get
		{
			return PlayerData.instance.Collectables.GetData(base.name);
		}
		set
		{
			PlayerData.instance.Collectables.SetData(base.name, value);
		}
	}

	public virtual int CollectedAmount
	{
		get
		{
			if (!Application.isPlaying)
			{
				return 0;
			}
			if (CollectableItemManager.IsInHiddenMode())
			{
				return SaveData.AmountWhileHidden;
			}
			return SaveData.Amount;
		}
	}

	public bool IsVisible
	{
		get
		{
			if (!isHidden)
			{
				return CollectedAmount > 0;
			}
			return false;
		}
	}

	public bool IsSeen
	{
		get
		{
			int isSeenIndex = IsSeenIndex;
			if (isSeenIndex < 0)
			{
				return true;
			}
			int isSeenMask = SaveData.IsSeenMask;
			int num = 1 << isSeenIndex;
			return (isSeenMask & num) == num;
		}
		set
		{
			int isSeenIndex = IsSeenIndex;
			if (isSeenIndex >= 0)
			{
				CollectableItemsData.Data saveData = SaveData;
				int isSeenMask = saveData.IsSeenMask;
				int num = 1 << isSeenIndex;
				isSeenMask = ((!value) ? (isSeenMask & ~num) : (isSeenMask | num));
				saveData.IsSeenMask = isSeenMask;
				SaveData = saveData;
			}
		}
	}

	protected virtual int IsSeenIndex => 0;

	public bool IsVisibleWithBareInventory => isVisibleWithBareInventory;

	public Quest UseQuestForCap => useQuestForCap;

	protected virtual bool CanShowQuestUpdatedForItem => true;

	public virtual bool TakeItemOnConsume => true;

	public override bool CanConsume => true;

	string ICollectionViewerItem.name => base.name;

	public abstract string GetDisplayName(ReadSource readSource);

	public abstract string GetDescription(ReadSource readSource);

	public abstract Sprite GetIcon(ReadSource readSource);

	public virtual InventoryItemButtonPromptData[] GetButtonPromptData()
	{
		return null;
	}

	protected virtual IEnumerable<UseResponse> GetUseResponses()
	{
		return useResponses;
	}

	public string[] GetUseResponseDescriptions()
	{
		return (from response in GetUseResponses()
			where !response.Description.IsEmpty
			select string.Format(response.Description, response.GetAmountText())).ToArray();
	}

	public virtual bool ShouldStopCollectNoMsg()
	{
		return false;
	}

	public void Collect(int amount = 1, bool showPopup = true)
	{
		if (ShouldStopCollectNoMsg())
		{
			return;
		}
		if (IsAtMax())
		{
			if (showPopup)
			{
				UIMsgDisplay uIMsgDisplay = default(UIMsgDisplay);
				uIMsgDisplay.Name = UI.MaxItemsPopup;
				uIMsgDisplay.Icon = GetIcon(ReadSource.GetPopup);
				uIMsgDisplay.IconScale = GetUIMsgIconScale();
				uIMsgDisplay.RepresentingObject = this;
				CollectableUIMsg.Spawn(uIMsgDisplay, UI.MaxItemsTextColor);
				CollectableItemHeroReaction.DoReaction();
			}
			return;
		}
		if (resetIsSeen)
		{
			IsSeen = false;
		}
		AddAmount(amount);
		if (showPopup)
		{
			CollectableUIMsg itemUiMsg = CollectableUIMsg.Spawn(this, IsAtMax() ? UI.MaxItemsTextColor : Color.white);
			if (CanShowQuestUpdatedForItem && QuestManager.MaybeShowQuestUpdated(this, itemUiMsg))
			{
				showPopup = false;
			}
			CollectableItemHeroReaction.DoReaction();
		}
		SetHasNew(showPopup);
		EventRegister.SendEvent(EventRegisterEvents.ItemCollected);
		PlayerStory.RecordEvent(storyEvent);
		OnCollected();
		ToolItemManager.ReportAllBoundAttackToolsUpdated();
		ItemCurrencyCounter.UpdateValue(this);
	}

	public override void SetHasNew(bool hasPopup)
	{
		if (hasPopup)
		{
			CollectableItemManager.CollectedItem = this;
			InventoryPaneList.SetNextOpen("Inv");
		}
		if (resetIsSeen)
		{
			IsSeen = false;
		}
		PlayerData.instance.InvPaneHasNew = true;
	}

	protected virtual void AddAmount(int amount)
	{
		CollectableItemManager.AddItem(this, amount);
	}

	protected virtual void OnCollected()
	{
	}

	public void Take(int amount = 1, bool showCounter = true)
	{
		CollectableItemManager.RemoveItem(this, amount);
		if (showCounter)
		{
			ItemCurrencyCounter.Take(this, amount);
		}
		else
		{
			ItemCurrencyCounter.UpdateValue(this);
		}
		OnTaken();
	}

	protected virtual void OnTaken()
	{
	}

	public override int GetCompletionAmount(QuestCompletionData.Completion sourceCompletion)
	{
		if (CollectableItemManager.IsInHiddenMode())
		{
			return SaveData.Amount + SaveData.AmountWhileHidden;
		}
		return SaveData.Amount;
	}

	public override void Consume(int amount, bool showCounter)
	{
		Take(amount, showCounter && ShowCounterOnConsume);
	}

	public virtual bool IsAtMax()
	{
		if (customMaxAmount > 0)
		{
			return CollectedAmount >= customMaxAmount;
		}
		if ((bool)useQuestForCap)
		{
			foreach (FullQuestBase.QuestTarget target in useQuestForCap.Targets)
			{
				if (!(target.Counter != this) && CollectedAmount >= target.Count)
				{
					return true;
				}
			}
			return false;
		}
		int consumableItemCap = Gameplay.ConsumableItemCap;
		if (consumableItemCap > 0)
		{
			return CollectedAmount >= consumableItemCap;
		}
		return false;
	}

	public virtual void ConsumeItemResponse()
	{
		HeroController instance = HeroController.instance;
		if (!instance)
		{
			return;
		}
		PlayerData instance2 = PlayerData.instance;
		foreach (UseResponse useResponse in GetUseResponses())
		{
			switch (useResponse.UseType)
			{
			case UseTypes.Rosaries:
				CurrencyManager.AddCurrency(useResponse.GetAmount(), CurrencyType.Money);
				break;
			case UseTypes.Shards:
				if (CurrencyManager.GetCurrencyAmount(CurrencyType.Shard) < Gameplay.GetCurrencyCap(CurrencyType.Shard))
				{
					CurrencyManager.AddCurrency(useResponse.GetAmount(), CurrencyType.Shard);
				}
				else
				{
					CurrencyCounter.ReportFail(CurrencyType.Shard);
				}
				break;
			case UseTypes.ReturnCocoon:
				if (!string.IsNullOrEmpty(instance2.HeroCorpseScene))
				{
					instance.CocoonBroken(doAirPause: true, forceCanBind: true);
					EventRegister.SendEvent(EventRegisterEvents.BreakHeroCorpse);
				}
				break;
			case UseTypes.GetSilk:
				instance.AddSilk(useResponse.GetAmount(), heroEffect: false);
				break;
			}
		}
	}

	public bool IsConsumeAtMax()
	{
		HeroController instance = HeroController.instance;
		if (!instance)
		{
			return false;
		}
		PlayerData playerData = instance.playerData;
		bool flag = false;
		using (IEnumerator<UseResponse> enumerator = GetUseResponses().GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				switch (enumerator.Current.UseType)
				{
				case UseTypes.Rosaries:
				case UseTypes.Shards:
				case UseTypes.ReturnCocoon:
					flag = true;
					break;
				case UseTypes.GetSilk:
					if (playerData.silk < playerData.CurrentSilkMax)
					{
						flag = true;
					}
					break;
				}
			}
		}
		return !flag;
	}

	public override void Get(bool showPopup = true)
	{
		Collect(1, showPopup);
	}

	public override bool CanGetMore()
	{
		if (IsConsumable())
		{
			return true;
		}
		return !IsAtMax();
	}

	public override Sprite GetQuestCounterSprite(int index)
	{
		return GetIcon(ReadSource.Inventory);
	}

	public override float GetUIMsgIconScale()
	{
		return popupIconScale;
	}

	public virtual bool IsConsumable()
	{
		return HasUseResponse();
	}

	private bool HasUseResponse()
	{
		return GetUseResponses().Any((UseResponse use) => use.UseType != UseTypes.None);
	}

	public bool CanConsumeRightNow()
	{
		if (HasUseResponse() && !IsConsumeAtMax())
		{
			return !GameManager.instance.IsMemoryScene();
		}
		return false;
	}

	public bool ConsumeClosesInventory(bool extraCondition)
	{
		using (IEnumerator<UseResponse> enumerator = GetUseResponses().GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				switch (enumerator.Current.UseType)
				{
				case UseTypes.ReturnCocoon:
				{
					PlayerData instance = PlayerData.instance;
					if (!extraCondition || !string.IsNullOrEmpty(instance.HeroCorpseScene))
					{
						return true;
					}
					break;
				}
				case UseTypes.GetSilk:
					return true;
				}
			}
		}
		return false;
	}

	public override Sprite GetPopupIcon()
	{
		return GetIcon(ReadSource.GetPopup);
	}

	public override string GetPopupName()
	{
		return GetDisplayName(ReadSource.GetPopup);
	}

	public override int GetSavedAmount()
	{
		return CollectedAmount;
	}

	public string GetCollectionName()
	{
		return GetDisplayName(ReadSource.Shop);
	}

	public string GetCollectionDesc()
	{
		return GetDescription(ReadSource.Shop);
	}

	public Sprite GetCollectionIcon()
	{
		return GetIcon(ReadSource.Shop);
	}

	public virtual bool IsVisibleInCollection()
	{
		return CollectedAmount > 0;
	}

	public bool IsRequiredInCollection()
	{
		return true;
	}

	public virtual void ReportPreviouslyCollected()
	{
	}
}

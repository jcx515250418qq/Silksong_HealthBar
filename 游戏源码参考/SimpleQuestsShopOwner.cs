using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TeamCherry.Localization;
using TeamCherry.SharedUtils;
using UnityEngine;

public class SimpleQuestsShopOwner : SimpleShopMenuOwner
{
	[Serializable]
	public class ShopItemInfo : ISimpleShopItem
	{
		public Quest Quest;

		public PlayerDataTest AppearCondition;

		public FullQuestBase[] AppearAfterCompleted;

		public int AppearAfterCountCompleted;

		public CollectableItem CustomDelivery;

		[ModifiableProperty]
		[Conditional("CustomDelivery", false, false, false)]
		public bool IsRepeatable = true;

		[Space]
		public LocalisedString PurchasedDlg;

		[LocalisedString.NotRequired]
		public LocalisedString RePurchasedDlg;

		public string GetDisplayName()
		{
			if ((bool)CustomDelivery)
			{
				return CustomDelivery.GetDisplayName(CollectableItem.ReadSource.GetPopup);
			}
			if (!Quest)
			{
				return string.Empty;
			}
			return Quest.DisplayName;
		}

		public Sprite GetIcon()
		{
			if ((bool)CustomDelivery)
			{
				return CustomDelivery.GetPopupIcon();
			}
			if (!Quest)
			{
				return null;
			}
			DeliveryQuestItem deliveryQuestItem = Quest.Targets.Select((FullQuestBase.QuestTarget target) => target.Counter).OfType<DeliveryQuestItem>().FirstOrDefault();
			if (!deliveryQuestItem)
			{
				return null;
			}
			return deliveryQuestItem.GetPopupIcon();
		}

		public int GetCost()
		{
			return 0;
		}

		public bool DelayPurchase()
		{
			return true;
		}
	}

	[SerializeField]
	private List<ShopItemInfo> quests;

	[SerializeField]
	[PlayerDataField(typeof(int), false)]
	private string purchasedDlgBitmask;

	[SerializeField]
	[PlayerDataField(typeof(List<string>), false)]
	private string genericQuestsList;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("genericQuestsList", true, false, false)]
	private int genericQuestCap;

	private List<ShopItemInfo> runningList;

	private List<ShopItemInfo> runningGenericList;

	private List<ISimpleShopItem> currentList;

	[UsedImplicitly]
	public Quest QueuedQuest { get; private set; }

	[UsedImplicitly]
	public CollectableItem QueuedCustomDelivery { get; private set; }

	[UsedImplicitly]
	public string QueuedPurchaseDlg { get; private set; }

	protected override List<ISimpleShopItem> GetItems()
	{
		PlayerData instance = PlayerData.instance;
		List<string> list;
		bool flag;
		if (!string.IsNullOrEmpty(genericQuestsList))
		{
			list = instance.GetVariable<List<string>>(genericQuestsList) ?? new List<string>();
			if (list.Count < 2)
			{
				list.Clear();
				flag = true;
			}
			else
			{
				flag = false;
			}
		}
		else
		{
			list = null;
			flag = false;
		}
		if (runningList == null)
		{
			runningList = new List<ShopItemInfo>();
		}
		else
		{
			runningList.Clear();
		}
		if (runningGenericList == null)
		{
			runningGenericList = new List<ShopItemInfo>();
		}
		else
		{
			runningGenericList.Clear();
		}
		int num = 0;
		foreach (ShopItemInfo quest2 in quests)
		{
			if ((bool)quest2.Quest && quest2.Quest.IsCompleted)
			{
				num++;
			}
		}
		foreach (ShopItemInfo quest3 in quests)
		{
			if (!quest3.AppearCondition.IsFulfilled || (quest3.AppearAfterCountCompleted > 0 && num < quest3.AppearAfterCountCompleted))
			{
				continue;
			}
			bool flag2 = false;
			FullQuestBase[] appearAfterCompleted = quest3.AppearAfterCompleted;
			foreach (FullQuestBase fullQuestBase in appearAfterCompleted)
			{
				if ((bool)fullQuestBase && !fullQuestBase.IsCompleted)
				{
					flag2 = true;
					break;
				}
			}
			if (flag2)
			{
				continue;
			}
			Quest quest = quest3.Quest;
			if ((bool)quest3.CustomDelivery)
			{
				if (((bool)quest && (!quest.IsAccepted || quest.IsCompleted)) || quest3.CustomDelivery.CollectedAmount > 0)
				{
					continue;
				}
			}
			else
			{
				if (!quest || !quest.IsAvailable)
				{
					continue;
				}
				if (quest3.IsRepeatable)
				{
					if (quest.IsAccepted && !quest.IsCompleted)
					{
						continue;
					}
					if (flag)
					{
						runningGenericList.Add(quest3);
						continue;
					}
					if (list != null && !list.Contains(quest.name))
					{
						continue;
					}
				}
				else if (quest.IsAccepted || quest.IsCompleted)
				{
					continue;
				}
			}
			runningList.Add(quest3);
		}
		if (flag)
		{
			runningGenericList.Shuffle();
			while (runningGenericList.Count > genericQuestCap)
			{
				runningGenericList.RemoveAt(runningGenericList.Count - 1);
			}
			foreach (ShopItemInfo quest4 in quests)
			{
				if (runningGenericList.Contains(quest4))
				{
					list.Add(quest4.Quest.name);
					runningList.Add(quest4);
				}
			}
			instance.SetVariable(genericQuestsList, list);
		}
		if (currentList == null)
		{
			currentList = new List<ISimpleShopItem>();
		}
		else
		{
			currentList.Clear();
		}
		currentList.AddRange(runningList);
		runningList.Clear();
		runningGenericList.Clear();
		if (flag)
		{
			return GetItems();
		}
		return currentList;
	}

	protected override void OnPurchasedItem(int itemIndex)
	{
		if (itemIndex < 0 || itemIndex >= currentList.Count)
		{
			Debug.LogError("Can't handle purchase for itemIndex out of range: " + itemIndex, this);
			return;
		}
		ShopItemInfo shopItemInfo = (ShopItemInfo)currentList[itemIndex];
		QueuedCustomDelivery = shopItemInfo.CustomDelivery;
		QueuedQuest = (QueuedCustomDelivery ? null : shopItemInfo.Quest);
		if (string.IsNullOrEmpty(purchasedDlgBitmask))
		{
			QueuedPurchaseDlg = shopItemInfo.PurchasedDlg;
			return;
		}
		int num = quests.IndexOf(shopItemInfo);
		bool flag = num < 0;
		PlayerData instance = PlayerData.instance;
		int variable = instance.GetVariable<int>(purchasedDlgBitmask);
		if (!flag && variable.IsBitSet(num))
		{
			QueuedPurchaseDlg = (shopItemInfo.RePurchasedDlg.IsEmpty ? shopItemInfo.PurchasedDlg : shopItemInfo.RePurchasedDlg);
			return;
		}
		QueuedPurchaseDlg = shopItemInfo.PurchasedDlg;
		if (!flag)
		{
			variable = variable.SetBitAtIndex(num);
			instance.SetVariable(purchasedDlgBitmask, variable);
		}
	}

	[UsedImplicitly]
	public bool IsAnyQuestInProgress()
	{
		foreach (ShopItemInfo quest2 in quests)
		{
			if ((bool)quest2.CustomDelivery)
			{
				if (quest2.CustomDelivery.CollectedAmount > 0)
				{
					return true;
				}
				continue;
			}
			Quest quest = quest2.Quest;
			if ((bool)quest && quest.IsAccepted && !quest.IsCompleted)
			{
				return true;
			}
		}
		return false;
	}
}

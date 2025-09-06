using System;
using System.Collections.Generic;
using GlobalEnums;
using TeamCherry.Localization;
using UnityEngine;

[CreateAssetMenu(menuName = "Hornet/Materium/Materium Item")]
public class MateriumItem : SavedItem, ICollectionViewerItem
{
	private enum RequiredTypes
	{
		NotRequired = 0,
		Required = 1,
		RequiredSteelSoul = 2
	}

	[SerializeField]
	private LocalisedString displayName;

	[SerializeField]
	private LocalisedString description;

	[SerializeField]
	private Sprite icon;

	[Space]
	[SerializeField]
	[Obsolete]
	[HideInInspector]
	private Quest itemQuest;

	[SerializeField]
	private List<Quest> itemQuests;

	[SerializeField]
	private PlayerDataTest playerDataCondition;

	[SerializeField]
	private bool collectedByDefault;

	[SerializeField]
	private RequiredTypes requiredType = RequiredTypes.Required;

	public bool IsRequiredForCompletion => requiredType switch
	{
		RequiredTypes.NotRequired => false, 
		RequiredTypes.Required => true, 
		RequiredTypes.RequiredSteelSoul => PlayerData.instance.permadeathMode == PermadeathModes.On, 
		_ => throw new ArgumentOutOfRangeException(), 
	};

	public LocalisedString DisplayName => displayName;

	public LocalisedString Description => description;

	public Sprite Icon => icon;

	private MateriumItemsData.Data SavedData
	{
		get
		{
			return PlayerData.instance.MateriumCollected.GetData(base.name);
		}
		set
		{
			PlayerData.instance.MateriumCollected.SetData(base.name, value);
		}
	}

	public bool IsCollected
	{
		get
		{
			if (SavedData.IsCollected)
			{
				return true;
			}
			if (!playerDataCondition.IsFulfilled)
			{
				return false;
			}
			OnValidate();
			if (itemQuests.Count > 0)
			{
				foreach (Quest itemQuest in itemQuests)
				{
					if (itemQuest.IsCompleted)
					{
						return true;
					}
				}
				return false;
			}
			if (!collectedByDefault)
			{
				return playerDataCondition.IsDefined;
			}
			return true;
		}
	}

	public bool IsSeen
	{
		get
		{
			return SavedData.HasSeenInRelicBoard;
		}
		set
		{
			MateriumItemsData.Data savedData = SavedData;
			savedData.HasSeenInRelicBoard = value;
			SavedData = savedData;
		}
	}

	string ICollectionViewerItem.name => base.name;

	private void OnValidate()
	{
		if (itemQuest != null)
		{
			itemQuests.Add(itemQuest);
			itemQuest = null;
		}
		itemQuests.RemoveNulls();
	}

	public string GetCollectionName()
	{
		return displayName;
	}

	public string GetCollectionDesc()
	{
		return description;
	}

	public Sprite GetCollectionIcon()
	{
		return icon;
	}

	public bool IsVisibleInCollection()
	{
		return IsCollected;
	}

	public bool IsRequiredInCollection()
	{
		return IsRequiredForCompletion;
	}

	public override void Get(bool showPopup = true)
	{
		PlayerData instance = PlayerData.instance;
		MateriumItemsData.Data data = instance.MateriumCollected.GetData(base.name);
		if (data.IsCollected)
		{
			return;
		}
		data.IsCollected = true;
		instance.MateriumCollected.SetData(base.name, data);
		if (MateriumItemManager.ConstructedMaterium)
		{
			MateriumItemManager.CheckAchievements();
			if (showPopup)
			{
				MateriumItemManager.ShowUpdateMessage();
			}
		}
	}

	public override bool CanGetMore()
	{
		return !IsCollected;
	}

	public override Sprite GetPopupIcon()
	{
		return icon;
	}

	public override string GetPopupName()
	{
		return displayName;
	}
}

using TeamCherry.Localization;
using UnityEngine;

public abstract class BasicQuestBase : QuestGroupBase
{
	public enum ReadSource
	{
		Inventory = 0,
		QuestBoard = 1
	}

	[Header("- Basic Quest Base")]
	[SerializeField]
	private LocalisedString displayName;

	[SerializeField]
	[LocalisedString.NotRequired]
	private LocalisedString location;

	private bool init;

	public abstract QuestType QuestType { get; }

	public LocalisedString DisplayName => displayName;

	public string Location
	{
		get
		{
			if (!location.IsEmpty)
			{
				return location;
			}
			return string.Empty;
		}
	}

	public abstract bool IsAvailable { get; }

	public abstract bool IsAccepted { get; }

	public abstract bool IsHidden { get; }

	public abstract bool HasBeenSeen { get; set; }

	public abstract bool IsMapMarkerVisible { get; }

	public abstract string GetDescription(ReadSource readSource);

	public void Init()
	{
		if (!init)
		{
			init = true;
			DoInit();
		}
	}

	protected virtual void DoInit()
	{
	}

	public void OnSelected()
	{
		HasBeenSeen = true;
	}

	public override string GetPopupName()
	{
		return DisplayName;
	}

	public override void Get(bool showPopup = true)
	{
		QuestManager.IncrementVersion();
	}

	public override Sprite GetPopupIcon()
	{
		QuestType questType = QuestType;
		if (!questType)
		{
			return null;
		}
		return questType.Icon;
	}

	public static void SetInventoryNewItem(BasicQuestBase quest)
	{
		InventoryPaneList.SetNextOpen("Quests");
		QuestManager.UpdatedQuest = quest;
		PlayerData.instance.QuestPaneHasNew = true;
	}
}

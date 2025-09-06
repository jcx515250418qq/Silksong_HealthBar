using System.Collections.Generic;
using TeamCherry.Localization;
using UnityEngine;

[CreateAssetMenu(menuName = "Hornet/Quests/Quest Rumour")]
public class QuestRumour : BasicQuestBase
{
	[Header("- Quest Rumour")]
	[SerializeField]
	private LocalisedString description;

	[Space]
	[SerializeField]
	private LocalisedString typeDisplayName = new LocalisedString
	{
		Sheet = "Quests",
		Key = "TYPE_"
	};

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private Sprite typeIcon;

	[Space]
	[SerializeField]
	private FullQuestBase linkedQuest;

	[Header("Appear Conditions")]
	[SerializeField]
	private PlayerDataTest playerDataTest;

	[SerializeField]
	private Quest[] requiredCompleteQuests;

	private QuestType questType;

	public override QuestType QuestType => questType;

	public override bool IsAvailable
	{
		get
		{
			if ((bool)linkedQuest && linkedQuest.IsAccepted)
			{
				return false;
			}
			Quest[] array = requiredCompleteQuests;
			for (int i = 0; i < array.Length; i++)
			{
				if (!array[i].IsCompleted)
				{
					return false;
				}
			}
			return playerDataTest.IsFulfilled;
		}
	}

	public override bool IsAccepted => SavedData.IsAccepted;

	public override bool IsHidden => !IsAvailable;

	public override bool HasBeenSeen
	{
		get
		{
			return SavedData.HasBeenSeen;
		}
		set
		{
			QuestRumourData.Data savedData = SavedData;
			savedData.HasBeenSeen = value;
			SavedData = savedData;
		}
	}

	public override bool IsMapMarkerVisible
	{
		get
		{
			if ((bool)linkedQuest && linkedQuest.IsAccepted)
			{
				return false;
			}
			return IsAccepted;
		}
	}

	public QuestRumourData.Data SavedData
	{
		get
		{
			return GameManager.instance.playerData.QuestRumourData.GetData(base.name);
		}
		set
		{
			GameManager.instance.playerData.QuestRumourData.SetData(base.name, value);
			QuestManager.IncrementVersion();
		}
	}

	private void OnEnable()
	{
		questType = QuestType.Create(typeDisplayName, typeIcon, Color.white, null, null);
	}

	private void OnDisable()
	{
		Object.DestroyImmediate(questType);
		questType = null;
	}

	public override void Get(bool showPopup = true)
	{
		QuestRumourData.Data savedData = SavedData;
		if (!savedData.IsAccepted)
		{
			savedData.IsAccepted = true;
			SavedData = savedData;
			if (showPopup)
			{
				UIMsgDisplay uIMsgDisplay = default(UIMsgDisplay);
				uIMsgDisplay.Name = "TODO: Can still take rumours?";
				uIMsgDisplay.Icon = typeIcon;
				uIMsgDisplay.IconScale = 1f;
				uIMsgDisplay.RepresentingObject = this;
				CollectableUIMsg.Spawn(uIMsgDisplay, null, forceReplacingEffect: true);
			}
		}
	}

	public override bool CanGetMore()
	{
		return !SavedData.IsAccepted;
	}

	public override IEnumerable<BasicQuestBase> GetQuests()
	{
		yield return this;
	}

	public override string GetDescription(ReadSource readSource)
	{
		return description;
	}
}

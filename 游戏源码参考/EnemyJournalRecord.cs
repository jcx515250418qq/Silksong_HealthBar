using System;
using System.Collections.Generic;
using GlobalEnums;
using TeamCherry.Localization;
using UnityEngine;

[CreateAssetMenu(menuName = "Hornet/Enemy Journal Record")]
public class EnemyJournalRecord : QuestTargetCounter
{
	public enum RecordTypes
	{
		Enemy = 0,
		Other = 1
	}

	private enum RequiredTypes
	{
		NotRequired = 0,
		Required = 1,
		RequiredSteelSoul = 2
	}

	[SerializeField]
	private Sprite iconSprite;

	[SerializeField]
	private Sprite enemySprite;

	[SerializeField]
	private LocalisedString displayName = new LocalisedString
	{
		Sheet = "Journal"
	};

	[SerializeField]
	private LocalisedString description = new LocalisedString
	{
		Sheet = "Journal"
	};

	[SerializeField]
	private LocalisedString notes = new LocalisedString
	{
		Sheet = "Journal"
	};

	[SerializeField]
	private int killsRequired = 10;

	[SerializeField]
	private bool isAlwaysUnlocked;

	[SerializeField]
	private RecordTypes recordType;

	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private bool isRequiredForCompletion = true;

	[SerializeField]
	private RequiredTypes requiredType = RequiredTypes.Required;

	[Space]
	[SerializeField]
	private EnemyJournalRecord[] completeOthers;

	public Sprite IconSprite => iconSprite;

	public Sprite EnemySprite => enemySprite;

	public LocalisedString DisplayName => displayName;

	public LocalisedString Description => description;

	public LocalisedString Notes => notes;

	public int KillsRequired => killsRequired;

	public bool IsAlwaysUnlocked => isAlwaysUnlocked;

	public RecordTypes RecordType => recordType;

	public bool IsRequiredForCompletion => requiredType switch
	{
		RequiredTypes.NotRequired => false, 
		RequiredTypes.Required => true, 
		RequiredTypes.RequiredSteelSoul => PlayerData.instance.permadeathMode == PermadeathModes.On, 
		_ => throw new ArgumentOutOfRangeException(), 
	};

	public bool SeenInJournal => EnemyJournalManager.GetKillData(this).HasBeenSeen;

	public int KillCount => EnemyJournalManager.GetKillData(this).Kills;

	public bool IsVisible
	{
		get
		{
			if (!IsAlwaysUnlocked)
			{
				return KillCount > 0;
			}
			return true;
		}
	}

	public IEnumerable<EnemyJournalRecord> CompleteOthers => completeOthers;

	private void OnValidate()
	{
		if (isRequiredForCompletion)
		{
			requiredType = RequiredTypes.Required;
			isRequiredForCompletion = false;
		}
	}

	private void Awake()
	{
		OnValidate();
	}

	public override Sprite GetQuestCounterSprite(int index)
	{
		return EnemySprite;
	}

	public void PopulateLocalisations(string sheet, string subKey)
	{
		displayName = new LocalisedString
		{
			Sheet = sheet,
			Key = "NAME_" + subKey
		};
		description = new LocalisedString
		{
			Sheet = sheet,
			Key = "DESC_" + subKey
		};
		notes = new LocalisedString
		{
			Sheet = sheet,
			Key = "NOTE_" + subKey
		};
	}

	public override void Get(bool showPopup = true)
	{
		EnemyJournalManager.RecordKill(this, showPopup);
	}

	public override bool CanGetMore()
	{
		return true;
	}

	public override Sprite GetPopupIcon()
	{
		return IconSprite;
	}

	public override int GetCompletionAmount(QuestCompletionData.Completion sourceCompletion)
	{
		return KillCount;
	}
}

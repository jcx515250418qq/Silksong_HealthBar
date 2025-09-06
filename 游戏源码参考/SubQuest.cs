using System.Collections.Generic;
using System.Linq;
using TeamCherry.Localization;
using TeamCherry.SharedUtils;
using UnityEngine;

[CreateAssetMenu(menuName = "Hornet/Quests/Sub Quest")]
public class SubQuest : BasicQuestBase, ICollectableUIMsgItem, IUIMsgPopupItem, IQuestWithCompletion, IMasterListExclude
{
	[Header("- Sub Quest")]
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

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private Sprite typeIconGlow;

	[SerializeField]
	private LocalisedString inventoryDescription;

	[SerializeField]
	[LocalisedString.NotRequired]
	private LocalisedString inventoryCompletableDescription;

	[Space]
	[SerializeField]
	[PlayerDataField(typeof(bool), false)]
	private string linkedBool;

	[SerializeField]
	private int targetCount;

	[SerializeField]
	private QuestTargetCounter targetCounter;

	[SerializeField]
	private bool showQuestUpdated = true;

	[Space]
	[SerializeField]
	[PlayerDataField(typeof(bool), false)]
	private string seenBool;

	[SerializeField]
	private AutoSaveName queueAutoSaveOnGet;

	[SerializeField]
	private bool queueSaveGame;

	[Space]
	[SerializeField]
	private SubQuest nextSubQuest;

	private QuestType questType;

	public override QuestType QuestType => questType;

	public QuestTargetCounter TargetCounter => targetCounter;

	public int TargetCount => targetCount;

	public override bool IsAvailable => !CanGetMore();

	public override bool IsAccepted => true;

	public override bool IsHidden => false;

	public bool CanComplete => !CanGetMore();

	public bool IsCompleted => !CanGetMore();

	public override bool HasBeenSeen
	{
		get
		{
			if (!string.IsNullOrEmpty(seenBool))
			{
				return PlayerData.instance.GetVariable<bool>(seenBool);
			}
			return true;
		}
		set
		{
			if (!string.IsNullOrEmpty(seenBool))
			{
				PlayerData.instance.SetVariable(seenBool, value);
			}
		}
	}

	public override bool IsMapMarkerVisible
	{
		get
		{
			if (CanComplete)
			{
				return false;
			}
			if ((bool)FindActiveMainQuest())
			{
				return true;
			}
			return false;
		}
	}

	public bool IsLinkedBoolComplete
	{
		get
		{
			if (!string.IsNullOrEmpty(linkedBool))
			{
				return PlayerData.instance.GetVariable<bool>(linkedBool);
			}
			return true;
		}
	}

	private void OnEnable()
	{
		questType = QuestType.Create(typeDisplayName, typeIcon, new Color(0.6f, 0.6f, 0.6f), null, null, typeIconGlow);
	}

	private void OnDisable()
	{
		Object.DestroyImmediate(questType);
		questType = null;
	}

	public override string GetDescription(ReadSource readSource)
	{
		if (!inventoryCompletableDescription.IsEmpty && CanComplete)
		{
			return inventoryCompletableDescription;
		}
		return inventoryDescription;
	}

	public override void Get(bool showPopup = true)
	{
		QuestManager.IncrementVersion();
		if (!string.IsNullOrEmpty(linkedBool))
		{
			PlayerData.instance.SetVariable(linkedBool, value: true);
		}
		if ((bool)targetCounter)
		{
			targetCounter.Get(showPopup);
		}
		else if (showPopup)
		{
			QuestManager.ShowQuestUpdatedStandalone(this);
		}
		GameManager instance = GameManager.instance;
		instance.CheckSubQuestAchievements();
		if (queueSaveGame)
		{
			instance.QueueSaveGame();
		}
		if (queueAutoSaveOnGet > AutoSaveName.NONE)
		{
			instance.QueueAutoSave(queueAutoSaveOnGet);
		}
	}

	public override bool CanGetMore()
	{
		foreach (BasicQuestBase allQuest in QuestManager.GetAllQuests())
		{
			MainQuest mainQuest = allQuest as MainQuest;
			if (!mainQuest)
			{
				continue;
			}
			foreach (SubQuest subQuest2 in mainQuest.SubQuests)
			{
				SubQuest subQuest = subQuest2;
				while ((bool)subQuest)
				{
					if (!(subQuest != this) && mainQuest.IsCompleted)
					{
						return false;
					}
					subQuest = subQuest.GetNext();
				}
			}
		}
		if (!IsLinkedBoolComplete)
		{
			return true;
		}
		if ((bool)targetCounter && targetCounter.GetCompletionAmount(default(QuestCompletionData.Completion)) < targetCount)
		{
			return true;
		}
		return false;
	}

	public override IEnumerable<BasicQuestBase> GetQuests()
	{
		yield break;
	}

	public Object GetRepresentingObject()
	{
		return this;
	}

	public Sprite GetUIMsgSprite()
	{
		return typeIcon;
	}

	public string GetUIMsgName()
	{
		return base.DisplayName;
	}

	public float GetUIMsgIconScale()
	{
		return 1f;
	}

	public SubQuest GetCurrent()
	{
		if ((bool)nextSubQuest && (CanComplete || nextSubQuest.CanComplete))
		{
			return nextSubQuest.GetCurrent();
		}
		return this;
	}

	public SubQuest GetNext()
	{
		return nextSubQuest;
	}

	public bool CanShowUpdated(bool isStandalone)
	{
		if (isStandalone || showQuestUpdated)
		{
			return FindActiveMainQuest();
		}
		return false;
	}

	private MainQuest FindActiveMainQuest()
	{
		foreach (MainQuest item in QuestManager.GetActiveQuests().OfType<MainQuest>())
		{
			if (!item.WouldMapMarkerBeVisible)
			{
				continue;
			}
			foreach (SubQuest subQuest2 in item.SubQuests)
			{
				SubQuest subQuest = subQuest2;
				while ((bool)subQuest)
				{
					if (subQuest.nextSubQuest == this && subQuest.GetCurrent() != this)
					{
						return null;
					}
					if (!(subQuest != this))
					{
						return item;
					}
					subQuest = subQuest.GetNext();
				}
			}
		}
		return null;
	}
}

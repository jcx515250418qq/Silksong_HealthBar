using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GlobalSettings;
using TeamCherry.Localization;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class FullQuestBase : BasicQuestBase, IQuestWithCompletion
{
	public enum DescCounterTypes
	{
		Icons = 0,
		Text = 1,
		ProgressBar = 2,
		None = 3,
		Custom = 4
	}

	public enum ListCounterTypes
	{
		Dots = 0,
		Bar = 1,
		None = 2
	}

	public enum IconTypes
	{
		None = -1,
		Image = 0,
		Silhouette = 1
	}

	private enum AppendDescBehaviour
	{
		None = 0,
		Append = 1,
		Prepend = 2
	}

	private enum AppendDescFormat
	{
		None = 0,
		Bullet = 1
	}

	[Serializable]
	private struct UIMsgDisplay : ICollectableUIMsgItem, IUIMsgPopupItem
	{
		public LocalisedString Name;

		public Sprite Icon;

		public float IconScale;

		[NonSerialized]
		public UnityEngine.Object RepresentingObject;

		public float GetUIMsgIconScale()
		{
			return IconScale;
		}

		public bool HasUpgradeIcon()
		{
			return false;
		}

		public string GetUIMsgName()
		{
			return Name;
		}

		public Sprite GetUIMsgSprite()
		{
			return Icon;
		}

		public UnityEngine.Object GetRepresentingObject()
		{
			return RepresentingObject;
		}
	}

	[Serializable]
	public struct QuestTarget
	{
		public QuestTargetCounter Counter;

		public int Count;

		public PlayerDataTest AltTest;

		public Sprite AltSprite;

		[LocalisedString.NotRequired]
		[ModifiableProperty]
		[Conditional("Counter", false, false, false)]
		public LocalisedString ItemName;

		public bool HideInCount;
	}

	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private int targetCount;

	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private QuestTargetCounter targetCounter;

	[Header("- Full Quest Base")]
	[SerializeField]
	private QuestTarget[] targets;

	[SerializeField]
	private bool consumeTargetIfApplicable;

	[SerializeField]
	private PlayerDataTest getTargetCondition;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("ShowTurnInAtBoard", true, true, false)]
	private bool canTurnInAtBoard = true;

	[Space]
	[SerializeField]
	[LocalisedString.NotRequired]
	private LocalisedString giveNameOverride;

	[SerializeField]
	[LocalisedString.NotRequired]
	private LocalisedString invItemAppendDesc;

	[Space]
	[SerializeField]
	[ModifiableProperty]
	[Conditional("ShowCustomPickupDisplay", true, true, false)]
	private UIMsgDisplay customPickupDisplay;

	[Space]
	[SerializeField]
	private SavedItem rewardItem;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("ShowRewardCount", true, true, false)]
	private int rewardCount;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("RewardIconValidation")]
	private Sprite rewardIcon;

	[SerializeField]
	private IconTypes rewardIconType;

	[Space]
	[SerializeField]
	private string awardAchievementOnComplete;

	[Header("Inventory")]
	[SerializeField]
	private LocalisedString inventoryDescription;

	[SerializeField]
	[Obsolete]
	[HideInInspector]
	private bool descAppendItemList;

	[SerializeField]
	private AppendDescBehaviour descAppendBehaviour;

	[SerializeField]
	private AppendDescFormat descAppendFormat = AppendDescFormat.Bullet;

	[SerializeField]
	[LocalisedString.NotRequired]
	private LocalisedString inventoryCompletableDescription;

	[SerializeField]
	[LocalisedString.NotRequired]
	private LocalisedString inventoryCompletedDescription;

	[Space]
	[SerializeField]
	[FormerlySerializedAs("counterDisplayType")]
	private DescCounterTypes descCounterType;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsDescCounterTypeCustom", true, false, false)]
	private GameObject customDescPrefab;

	[SerializeField]
	private Color progressBarTint = Color.white;

	[SerializeField]
	private ListCounterTypes listCounterType;

	[SerializeField]
	private bool hideMax;

	[SerializeField]
	private bool hideCountersWhenCompletable;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("ShowCounterIconOverride", true, true, false)]
	private Sprite counterIconOverride;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("HasCounterIcon", true, true, false)]
	private float counterIconScale = 1f;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("HasCounterIcon", true, true, false)]
	private Vector2 counterIconPadding;

	[Space]
	[SerializeField]
	private OverrideFloat overrideFontSize;

	[SerializeField]
	private OverrideFloat overrideParagraphSpacing;

	[SerializeField]
	private OverrideFloat overrideParagraphSpacingShort;

	[SerializeField]
	private LanguageCode[] hideDescCounterForLangs;

	[Header("Quest Board")]
	[SerializeField]
	[LocalisedString.NotRequired]
	private LocalisedString wallDescription;

	[Space]
	[SerializeField]
	private PlayerDataTest playerDataTest;

	[SerializeField]
	private FullQuestBase[] requiredCompleteQuests;

	[SerializeField]
	private ToolItem[] requiredUnlockedTools;

	[SerializeField]
	private QuestCompleteTotalGroup[] requiredCompleteTotalGroups;

	[Header("Progress")]
	[SerializeField]
	[Tooltip("Previous step will be hidden when this quest is shown.")]
	private FullQuestBase previousQuestStep;

	[NonSerialized]
	private FullQuestBase oldPreviousQuestStep;

	[NonSerialized]
	private FullQuestBase nextQuestStep;

	[SerializeField]
	[Tooltip("Will be silently marked as completed when this quest begins, but will not be hidden.")]
	private FullQuestBase[] markCompleted;

	[SerializeField]
	private FullQuestBase[] cancelIfIncomplete;

	[Space]
	[SerializeField]
	[Tooltip("Will silently hide this quest if any of these are complete, without marking as completed.")]
	private FullQuestBase[] hideIfComplete;

	public LocalisedString GiveNameOverride => giveNameOverride;

	public LocalisedString InvItemAppendDesc => invItemAppendDesc;

	public SavedItem RewardItem => rewardItem;

	public int RewardCount => rewardCount;

	public Sprite RewardIcon
	{
		get
		{
			if (rewardIconType == IconTypes.None)
			{
				return null;
			}
			if ((bool)rewardIcon)
			{
				return rewardIcon;
			}
			if (!rewardItem)
			{
				return null;
			}
			return rewardItem.GetPopupIcon();
		}
	}

	public IconTypes RewardIconType => rewardIconType;

	public IReadOnlyList<QuestTarget> Targets => targets ?? Array.Empty<QuestTarget>();

	public bool ConsumeTargetIfApplicable => consumeTargetIfApplicable;

	public DescCounterTypes DescCounterType
	{
		get
		{
			if (hideCountersWhenCompletable && CanComplete)
			{
				return DescCounterTypes.None;
			}
			if (hideDescCounterForLangs != null)
			{
				LanguageCode languageCode = Language.CurrentLanguage();
				LanguageCode[] array = hideDescCounterForLangs;
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] == languageCode)
					{
						return DescCounterTypes.None;
					}
				}
			}
			return descCounterType;
		}
	}

	public bool IsDescCounterTypeCustom => DescCounterType == DescCounterTypes.Custom;

	public GameObject CustomDescPrefab => customDescPrefab;

	public Color ProgressBarTint => progressBarTint;

	public float CounterIconScale => counterIconScale;

	public Vector2 CounterIconPadding => counterIconPadding;

	public ListCounterTypes ListCounterType
	{
		get
		{
			if (!hideCountersWhenCompletable || !CanComplete)
			{
				return listCounterType;
			}
			return ListCounterTypes.None;
		}
	}

	public bool HideMax => hideMax;

	public OverrideFloat OverrideFontSize => overrideFontSize;

	public OverrideFloat OverrideParagraphSpacing
	{
		get
		{
			LanguageCode languageCode = Language.CurrentLanguage();
			if (languageCode == LanguageCode.DE || languageCode == LanguageCode.FR)
			{
				return overrideParagraphSpacingShort;
			}
			return overrideParagraphSpacing;
		}
	}

	public override bool IsAvailable
	{
		get
		{
			FullQuestBase[] array = requiredCompleteQuests;
			foreach (FullQuestBase fullQuestBase in array)
			{
				if ((bool)fullQuestBase && !fullQuestBase.IsCompleted)
				{
					return false;
				}
			}
			ToolItem[] array2 = requiredUnlockedTools;
			foreach (ToolItem toolItem in array2)
			{
				if ((bool)toolItem && !toolItem.IsUnlocked)
				{
					return false;
				}
			}
			QuestCompleteTotalGroup[] array3 = requiredCompleteTotalGroups;
			foreach (QuestCompleteTotalGroup questCompleteTotalGroup in array3)
			{
				if ((bool)questCompleteTotalGroup && !questCompleteTotalGroup.IsFulfilled)
				{
					return false;
				}
			}
			return playerDataTest.IsFulfilled;
		}
	}

	public IEnumerable<int> Counters
	{
		get
		{
			if (IsCompleted)
			{
				return Targets.Select((QuestTarget target) => target.Count);
			}
			return Targets.Select(delegate(QuestTarget target)
			{
				if (target.AltTest.IsDefined && target.AltTest.IsFulfilled)
				{
					return target.Count;
				}
				return (!target.Counter) ? Completion.CompletedCount : target.Counter.GetCompletionAmount(Completion);
			});
		}
	}

	public IEnumerable<(QuestTarget target, int count)> TargetsAndCountersNotHidden => Targets.Zip(Counters, (QuestTarget target, int count) => (target: target, count: count));

	public IEnumerable<(QuestTarget target, int count)> TargetsAndCounters => TargetsAndCountersNotHidden.Where(((QuestTarget target, int count) target) => !target.target.HideInCount);

	public bool IsDonateType
	{
		get
		{
			if ((bool)QuestType)
			{
				return QuestType.IsDonateType;
			}
			return false;
		}
	}

	public virtual bool CanComplete => TargetsAndCounters.All(((QuestTarget target, int count) pair) => pair.count >= pair.target.Count);

	public override bool IsAccepted => Completion.IsAccepted;

	public bool IsCompleted => Completion.IsCompleted;

	public bool WasEverCompleted => Completion.WasEverCompleted;

	public override bool HasBeenSeen
	{
		get
		{
			return Completion.HasBeenSeen;
		}
		set
		{
			QuestCompletionData.Completion completion = Completion;
			completion.HasBeenSeen = value;
			Completion = completion;
		}
	}

	public override bool IsHidden
	{
		get
		{
			if ((bool)nextQuestStep && nextQuestStep.IsAccepted && (string)nextQuestStep.DisplayName == (string)base.DisplayName)
			{
				return true;
			}
			FullQuestBase[] array = hideIfComplete;
			foreach (FullQuestBase fullQuestBase in array)
			{
				if ((bool)fullQuestBase && fullQuestBase.IsCompleted)
				{
					return true;
				}
			}
			return false;
		}
	}

	public override bool IsMapMarkerVisible
	{
		get
		{
			if (IsAccepted && !IsCompleted)
			{
				return !IsHidden;
			}
			return false;
		}
	}

	private QuestCompletionData.Completion Completion
	{
		get
		{
			return GameManager.instance.playerData.QuestCompletionData.GetData(base.name);
		}
		set
		{
			GameManager.instance.playerData.QuestCompletionData.SetData(base.name, value);
			QuestManager.IncrementVersion();
		}
	}

	private bool ShowCustomPickupDisplay()
	{
		return Targets.Any((QuestTarget target) => target.Counter == null && target.Count > 0);
	}

	private bool ShowCounterIconOverride()
	{
		if (HasCounterIcon())
		{
			return descCounterType == DescCounterTypes.Icons;
		}
		return false;
	}

	private bool HasCounterIcon()
	{
		DescCounterTypes descCounterTypes = descCounterType;
		return descCounterTypes == DescCounterTypes.Icons || descCounterTypes == DescCounterTypes.Text;
	}

	private bool? RewardIconValidation()
	{
		if (rewardIconType == IconTypes.None)
		{
			return null;
		}
		if ((bool)rewardItem && (bool)RewardIcon && rewardIcon == null)
		{
			return null;
		}
		return rewardIcon;
	}

	private bool ShowRewardCount()
	{
		if (rewardItem != null)
		{
			return rewardItem.CanGetMultipleAtOnce;
		}
		return false;
	}

	private bool ShowTurnInAtBoard()
	{
		QuestTarget[] array = targets;
		return ((array != null && array.Length != 0) ? 1 : 0) > (false ? 1 : 0);
	}

	private void OnValidate()
	{
		if ((bool)oldPreviousQuestStep && oldPreviousQuestStep.nextQuestStep == this)
		{
			oldPreviousQuestStep.nextQuestStep = null;
		}
		if ((bool)previousQuestStep)
		{
			previousQuestStep.nextQuestStep = this;
		}
		if (rewardCount <= 0)
		{
			rewardCount = 1;
		}
		oldPreviousQuestStep = previousQuestStep;
		if (targetCounter != null || targetCount > 0)
		{
			targets = new QuestTarget[1]
			{
				new QuestTarget
				{
					Counter = targetCounter,
					Count = targetCount
				}
			};
			targetCounter = null;
			targetCount = 0;
		}
		if (descAppendItemList)
		{
			descAppendItemList = false;
			descAppendBehaviour = AppendDescBehaviour.Append;
		}
	}

	protected override void DoInit()
	{
		OnValidate();
	}

	protected virtual void OnEnable()
	{
		Init();
		QuestTargetCounter.OnIncrement += IncrementCounterHandler;
	}

	protected virtual void OnDisable()
	{
		QuestTargetCounter.OnIncrement -= IncrementCounterHandler;
	}

	public Sprite GetCounterSpriteOverride(QuestTarget forTarget, int index)
	{
		if ((bool)counterIconOverride)
		{
			return counterIconOverride;
		}
		if ((bool)forTarget.AltSprite)
		{
			return forTarget.AltSprite;
		}
		if (!forTarget.Counter)
		{
			return customPickupDisplay.Icon;
		}
		return forTarget.Counter.GetQuestCounterSprite(index);
	}

	public override string GetDescription(ReadSource readSource)
	{
		switch (readSource)
		{
		case ReadSource.Inventory:
			if (IsCompleted)
			{
				if (inventoryCompletedDescription.IsEmpty)
				{
					return MaybeAppendItemList(inventoryDescription);
				}
				return inventoryCompletedDescription;
			}
			if (!inventoryCompletableDescription.IsEmpty && CanComplete)
			{
				return inventoryCompletableDescription;
			}
			return MaybeAppendItemList(inventoryDescription);
		case ReadSource.QuestBoard:
			return wallDescription;
		default:
			throw new ArgumentOutOfRangeException("readSource", readSource, null);
		}
		string MaybeAppendItemList(string desc)
		{
			StringBuilder tempStringBuilder;
			bool flag;
			bool flag2;
			switch (descAppendBehaviour)
			{
			case AppendDescBehaviour.None:
				return desc;
			case AppendDescBehaviour.Append:
				tempStringBuilder = Helper.GetTempStringBuilder(desc);
				flag = false;
				flag2 = false;
				break;
			case AppendDescBehaviour.Prepend:
				tempStringBuilder = Helper.GetTempStringBuilder();
				flag = true;
				flag2 = true;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			string text = descAppendFormat switch
			{
				AppendDescFormat.None => string.Empty, 
				AppendDescFormat.Bullet => "• ", 
				_ => throw new ArgumentOutOfRangeException(), 
			};
			string value = "<alpha=#55>" + text + "<s>";
			bool isCompleted = IsCompleted;
			foreach (var (questTarget, num) in TargetsAndCounters)
			{
				if (flag2)
				{
					flag2 = false;
				}
				else
				{
					tempStringBuilder.AppendLine();
				}
				LocalisedString itemName = questTarget.ItemName;
				string value2;
				if (!itemName.IsEmpty)
				{
					value2 = questTarget.ItemName;
				}
				else
				{
					if (!questTarget.Counter)
					{
						continue;
					}
					value2 = questTarget.Counter.GetPopupName();
				}
				if (!isCompleted && num >= questTarget.Count)
				{
					tempStringBuilder.Append(value);
					tempStringBuilder.Append(value2);
					tempStringBuilder.Append("</s><alpha=#FF>");
				}
				else
				{
					if (!string.IsNullOrWhiteSpace(text))
					{
						tempStringBuilder.Append(text);
					}
					tempStringBuilder.Append(value2);
				}
			}
			if (flag)
			{
				tempStringBuilder.AppendLine();
				tempStringBuilder.AppendLine(desc);
			}
			return tempStringBuilder.ToString();
		}
	}

	public void IncrementCounterHandler(QuestTargetCounter counter)
	{
		bool flag = false;
		QuestTarget[] array = targets;
		for (int i = 0; i < array.Length; i++)
		{
			QuestTarget questTarget = array[i];
			if ((bool)questTarget.Counter && questTarget.Counter.ShouldIncrementQuestCounter(counter))
			{
				flag = true;
				break;
			}
		}
		if (flag && IsAccepted && !IsCompleted)
		{
			IncrementQuestCounter();
		}
	}

	public void BeginQuest(Action afterPrompt, bool showPrompt = true)
	{
		if (QuestManager.IsQuestInList(this))
		{
			bool hasPreviousQuest = (bool)previousQuestStep && previousQuestStep.IsAccepted;
			SilentlyCompletePrevious();
			QuestCompletionData.Completion completion = Completion;
			completion.IsAccepted = true;
			if (completion.IsCompleted && !completion.WasEverCompleted)
			{
				completion.WasEverCompleted = true;
			}
			completion.IsCompleted = false;
			completion.HasBeenSeen = false;
			Completion = completion;
			BasicQuestBase.SetInventoryNewItem(this);
			if (showPrompt)
			{
				ShowQuestAccepted(afterPrompt, hasPreviousQuest);
			}
			else
			{
				afterPrompt?.Invoke();
			}
		}
	}

	public void SilentlyCompletePrevious()
	{
		if ((bool)previousQuestStep)
		{
			previousQuestStep.SilentlyComplete();
		}
		FullQuestBase[] array;
		if (markCompleted != null)
		{
			array = markCompleted;
			foreach (FullQuestBase fullQuestBase in array)
			{
				if ((bool)fullQuestBase)
				{
					fullQuestBase.SilentlyComplete();
				}
			}
		}
		if (cancelIfIncomplete == null)
		{
			return;
		}
		array = cancelIfIncomplete;
		foreach (FullQuestBase fullQuestBase2 in array)
		{
			if ((bool)fullQuestBase2 && !fullQuestBase2.IsCompleted)
			{
				QuestCompletionData.Completion completion = fullQuestBase2.Completion;
				completion.IsAccepted = false;
				fullQuestBase2.Completion = completion;
			}
		}
	}

	public void SilentlyComplete()
	{
		QuestCompletionData.Completion completion = Completion;
		if (!completion.IsCompleted)
		{
			completion.IsAccepted = true;
			completion.SetCompleted();
			Completion = completion;
			SilentlyCompletePrevious();
		}
	}

	protected virtual void ShowQuestAccepted(Action afterPrompt, bool hasPreviousQuest)
	{
		if (hasPreviousQuest)
		{
			UIMsgDisplay uIMsgDisplay = default(UIMsgDisplay);
			uIMsgDisplay.Name = UI.QuestContinuePopup;
			uIMsgDisplay.Icon = QuestType.Icon;
			uIMsgDisplay.IconScale = 1f;
			uIMsgDisplay.RepresentingObject = this;
			CollectableUIMsg.Spawn(uIMsgDisplay, null, forceReplacingEffect: true);
			UI.QuestContinuePopupSound.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, Vector3.zero);
			afterPrompt?.Invoke();
		}
		else
		{
			QuestManager.ShowQuestAccepted(this, afterPrompt);
		}
	}

	public bool ConsumeTarget()
	{
		if (!consumeTargetIfApplicable)
		{
			return false;
		}
		return ConsumeTargets();
	}

	protected virtual bool ConsumeTargets()
	{
		bool result = false;
		QuestTarget[] array = targets;
		for (int i = 0; i < array.Length; i++)
		{
			QuestTarget questTarget = array[i];
			if ((bool)questTarget.Counter && questTarget.Counter.CanConsume)
			{
				questTarget.Counter.Consume(questTarget.Count, showCounter: true);
				result = true;
			}
		}
		return result;
	}

	public bool TryEndQuest(Action afterPrompt, bool consumeCurrency, bool forceEnd = false, bool showPrompt = true)
	{
		QuestCompletionData.Completion completion = Completion;
		if (!forceEnd && !CanComplete)
		{
			afterPrompt?.Invoke();
			return false;
		}
		FullQuestBase fullQuestBase = previousQuestStep;
		while ((bool)fullQuestBase)
		{
			fullQuestBase.SilentlyComplete();
			fullQuestBase = fullQuestBase.previousQuestStep;
		}
		if (QuestType.IsDonateType)
		{
			completion.IsAccepted = true;
		}
		completion.SetCompleted();
		Completion = completion;
		if (consumeCurrency)
		{
			ConsumeTarget();
		}
		InventoryPaneList.SetNextOpen("Quests");
		QuestManager.UpdatedQuest = this;
		GameManager instance = GameManager.instance;
		if (showPrompt && completion.IsAccepted)
		{
			if (!(this is MainQuest))
			{
				instance.QueueAchievement("FIRST_WISH");
			}
			if (!string.IsNullOrWhiteSpace(awardAchievementOnComplete))
			{
				instance.QueueAchievement(awardAchievementOnComplete);
			}
			MateriumItemManager.CheckAchievements(queue: true);
			ShowQuestCompleted(afterPrompt);
		}
		else
		{
			afterPrompt?.Invoke();
			if (!string.IsNullOrWhiteSpace(awardAchievementOnComplete))
			{
				instance.AwardAchievement(awardAchievementOnComplete);
			}
			MateriumItemManager.CheckAchievements();
			if (!(this is MainQuest))
			{
				instance.AwardAchievement("FIRST_WISH");
			}
		}
		instance.QueueSaveGame();
		if ((bool)QuestType)
		{
			QuestType.OnQuestCompleted(this);
		}
		return true;
	}

	protected virtual void ShowQuestCompleted(Action afterPrompt)
	{
		QuestManager.ShowQuestCompleted(this, afterPrompt);
	}

	public void IncrementQuestCounter()
	{
		QuestCompletionData.Completion completion = Completion;
		completion.CompletedCount++;
		Completion = completion;
	}

	public override void Get(bool showPopup = true)
	{
		QuestTarget[] array = targets;
		for (int i = 0; i < array.Length; i++)
		{
			QuestTarget questTarget = array[i];
			if ((bool)questTarget.Counter)
			{
				questTarget.Counter.Get(showPopup);
			}
		}
		IncrementQuestCounter();
		customPickupDisplay.RepresentingObject = this;
		if (showPopup && ShowCustomPickupDisplay())
		{
			CollectableUIMsg itemUiMsg = CollectableUIMsg.Spawn(customPickupDisplay, CanComplete ? UI.MaxItemsTextColor : Color.white);
			if (CanComplete)
			{
				QuestManager.ShowQuestUpdatedForItemMsg(itemUiMsg, this);
			}
		}
	}

	public override bool CanGetMore()
	{
		if (!IsAccepted || IsCompleted)
		{
			return false;
		}
		if (!getTargetCondition.IsFulfilled)
		{
			return false;
		}
		bool flag = false;
		QuestTarget[] array = targets;
		for (int i = 0; i < array.Length; i++)
		{
			QuestTarget questTarget = array[i];
			if (!questTarget.Counter)
			{
				if (Completion.CompletedCount >= questTarget.Count)
				{
					return false;
				}
				continue;
			}
			if (questTarget.Counter.CanGetMore())
			{
				return true;
			}
			flag = true;
		}
		return !flag;
	}

	public override IEnumerable<BasicQuestBase> GetQuests()
	{
		yield return this;
	}

	public bool GetIsReadyToTurnIn(bool atQuestBoard)
	{
		if ((bool)nextQuestStep)
		{
			return false;
		}
		if (atQuestBoard && (!canTurnInAtBoard || !ShowTurnInAtBoard()))
		{
			return false;
		}
		if (!IsAccepted || IsCompleted)
		{
			return false;
		}
		return CanComplete;
	}

	public int GetCollectedCountOverride(QuestTarget target, int baseCount)
	{
		return Mathf.Clamp(baseCount, 0, target.Count);
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

public class QuestBoardInteractable : NPCControlBase
{
	private struct BoardQuest
	{
		public BasicQuestBase Quest;

		public QuestBadge Display;
	}

	[Serializable]
	private class SerializableQuestList
	{
		public List<string> QuestNames;
	}

	[Serializable]
	private struct QuestTypeColor
	{
		public QuestType Type;

		public Color Color;
	}

	[Serializable]
	private class QuestBadge
	{
		public NestedFadeGroupBase Fade;

		public SpriteRenderer Tint;
	}

	private struct PreSpawnedItemData
	{
		public PreSpawnedItem preSpawnedItem;

		public ISavedItemPreSpawn itemPreSpawner;
	}

	[SerializeField]
	private QuestItemBoard questBoardPrefab;

	[SerializeField]
	private QuestBoardList questList;

	[SerializeField]
	private NestedFadeGroupBase newQuestIndicator;

	[SerializeField]
	private float questIndicatorFadeTime;

	[SerializeField]
	private List<QuestBadge> questDisplays;

	[SerializeField]
	private float questDisplayFadeTime;

	[SerializeField]
	private List<QuestTypeColor> questTypeColors;

	[Space]
	[SerializeField]
	private PlayerDataTest activeCondition;

	[SerializeField]
	private GameObject[] activeObjects;

	[SerializeField]
	private GameObject[] inactiveObjects;

	[Space]
	[SerializeField]
	private PlayMakerFSM handInSequenceFsm;

	private readonly List<BoardQuest> boardQuests = new List<BoardQuest>();

	private Queue<FullQuestBase> queuedCompletions;

	private QuestItemBoard questBoard;

	private bool questDisplayHidden;

	private FullQuestBase donateQuest;

	private Dictionary<FullQuestBase, PreSpawnedItemData> preSpawnedItems = new Dictionary<FullQuestBase, PreSpawnedItemData>();

	public const string IN_DEPOSIT_SEQUENCE_STATIC_BOOL = "IsInQuestBoardDepositSequence";

	public override string InteractLabelDisplay
	{
		get
		{
			if (!IsAnyQuestsCompletable)
			{
				return base.InteractLabelDisplay;
			}
			return PromptLabels.TurnIn.ToString();
		}
	}

	public IReadOnlyCollection<QuestGroupBase> Quests => questList;

	private bool IsAnyQuestsCompletable => Quests.SelectMany((QuestGroupBase group) => group.GetQuests()).OfType<FullQuestBase>().Any((FullQuestBase quest) => quest.GetIsReadyToTurnIn(atQuestBoard: true));

	private IReadOnlyList<BasicQuestBase> CurrentQuests
	{
		get
		{
			if (!CheatManager.ShowAllQuestBoardQuest && !activeCondition.IsFulfilled)
			{
				return Array.Empty<BasicQuestBase>();
			}
			return questBoard.Quests;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		foreach (BasicQuestBase item2 in Quests.SelectMany((QuestGroupBase group) => group.GetQuests()))
		{
			FullQuestBase fullQuestBase = item2 as FullQuestBase;
			if (!(fullQuestBase == null) && fullQuestBase.GetIsReadyToTurnIn(atQuestBoard: true) && !(fullQuestBase.RewardItem == null) && fullQuestBase.RewardItem is ISavedItemPreSpawn savedItemPreSpawn && savedItemPreSpawn.TryGetPrespawnedItem(out var item))
			{
				if (!preSpawnedItems.TryAdd(fullQuestBase, new PreSpawnedItemData
				{
					preSpawnedItem = item,
					itemPreSpawner = savedItemPreSpawn
				}))
				{
					item.Dispose();
					continue;
				}
				item.SpawnedObject.SetActive(value: true);
				item.OnAwake();
			}
		}
	}

	protected override void Start()
	{
		foreach (KeyValuePair<FullQuestBase, PreSpawnedItemData> preSpawnedItem in preSpawnedItems)
		{
			preSpawnedItem.Value.preSpawnedItem.OnStart();
			preSpawnedItem.Value.preSpawnedItem.SpawnedObject.SetActive(value: false);
		}
		if ((bool)questBoardPrefab)
		{
			questBoard = UnityEngine.Object.Instantiate(questBoardPrefab, base.transform, worldPositionStays: true);
			questBoard.transform.SetParent(null, worldPositionStays: true);
			questBoard.GetAvailableQuestsFunc = GetDisplayedQuests;
			questBoard.IsActionsBlocked = true;
			questBoard.UpdateList();
			questBoard.IsActionsBlocked = false;
			questBoard.QuestAccepted += UpdateQuestDisplay;
			questBoard.DonateQuestAccepted += delegate(FullQuestBase quest)
			{
				donateQuest = quest;
				UpdateQuestDisplay();
			};
		}
		bool isFulfilled = activeCondition.IsFulfilled;
		if (!isFulfilled)
		{
			Deactivate(allowQueueing: false);
		}
		activeObjects.SetAllActive(isFulfilled);
		inactiveObjects.SetAllActive(!isFulfilled);
		if ((bool)questBoard)
		{
			SetupQuestDisplay();
		}
		EventRegister.GetRegisterGuaranteed(base.gameObject, "CONTINUE QUEST BOARD DEPOSIT").ReceivedEvent += delegate
		{
			if (StaticVariableList.GetValue<bool>("IsInQuestBoardDepositSequence"))
			{
				ProcessQueuedCompletions();
			}
		};
	}

	private void OnDestroy()
	{
		foreach (KeyValuePair<FullQuestBase, PreSpawnedItemData> preSpawnedItem in preSpawnedItems)
		{
			preSpawnedItem.Value.preSpawnedItem?.Dispose();
		}
	}

	private IReadOnlyCollection<QuestGroupBase> GetDisplayedQuests()
	{
		GameObject gameObject = base.gameObject;
		string variableName = "QuestBoardState_" + gameObject.name + "_" + gameObject.scene.name;
		IEnumerable<QuestGroupBase> source = Quests;
		string value = StaticVariableList.GetValue<string>(variableName, null);
		if (!string.IsNullOrEmpty(value))
		{
			SerializableQuestList cachedQuestsDeserialized = JsonUtility.FromJson<SerializableQuestList>(value);
			source = source.Where((QuestGroupBase quest) => cachedQuestsDeserialized.QuestNames.Contains(quest.name));
		}
		if (CheatManager.ShowAllQuestBoardQuest)
		{
			return source.ToList();
		}
		List<BasicQuestBase> list = (from quest in source.SelectMany((QuestGroupBase questBase) => questBase.GetQuests())
			where quest != donateQuest && !quest.IsAccepted && quest.IsAvailable && (!(quest is IQuestWithCompletion questWithCompletion) || !questWithCompletion.IsCompleted)
			select quest).Take(6).ToList();
		List<string> questNames = list.Select((BasicQuestBase quest) => quest.name).ToList();
		value = JsonUtility.ToJson(new SerializableQuestList
		{
			QuestNames = questNames
		});
		StaticVariableList.SetValue(variableName, value, 3);
		return list;
	}

	protected override void OnStartDialogue()
	{
		donateQuest = null;
		if ((bool)questBoard)
		{
			DisableInteraction();
			if (queuedCompletions == null)
			{
				queuedCompletions = new Queue<FullQuestBase>();
			}
			else
			{
				queuedCompletions.Clear();
			}
			bool flag = false;
			foreach (BasicQuestBase item in Quests.SelectMany((QuestGroupBase group) => group.GetQuests()))
			{
				FullQuestBase fullQuestBase = item as FullQuestBase;
				if (fullQuestBase == null || !fullQuestBase.GetIsReadyToTurnIn(atQuestBoard: true))
				{
					continue;
				}
				queuedCompletions.Enqueue(fullQuestBase);
				foreach (FullQuestBase.QuestTarget target in fullQuestBase.Targets)
				{
					if (!target.Counter || target.Counter.CanConsume)
					{
						flag = true;
					}
				}
			}
			if (queuedCompletions.Count > 0)
			{
				if ((bool)handInSequenceFsm)
				{
					handInSequenceFsm.SendEvent(flag ? "QUESTS" : "QUESTS_NOCONSUME");
					return;
				}
				DepositQuestItems();
				ProcessQueuedCompletions();
			}
			else
			{
				OpenBoard();
			}
		}
		else
		{
			Debug.LogError("No quest board assigned!", this);
		}
	}

	[UsedImplicitly]
	public void DepositQuestItems()
	{
		StaticVariableList.SetValue("IsInQuestBoardDepositSequence", true);
		foreach (FullQuestBase queuedCompletion in queuedCompletions)
		{
			if (!queuedCompletion.ConsumeTarget())
			{
				continue;
			}
			foreach (FullQuestBase.QuestTarget target in queuedCompletion.Targets)
			{
				CollectableUIMsg.ShowTakeMsg(target.Counter, TakeItemTypes.Deposited);
			}
		}
	}

	[UsedImplicitly]
	public void ProcessQueuedCompletions()
	{
		if (queuedCompletions.Count == 0)
		{
			StaticVariableList.SetValue("IsInQuestBoardDepositSequence", false);
			UpdateQuestDisplay();
			EndDialogue();
			return;
		}
		FullQuestBase completeQuest = queuedCompletions.Dequeue();
		completeQuest.TryEndQuest(delegate
		{
			bool flag = false;
			if (preSpawnedItems.TryGetValue(completeQuest, out var value))
			{
				value.itemPreSpawner.PreSpawnGet();
				value.preSpawnedItem.SpawnedObject.SetActive(value: true);
				flag = true;
			}
			SavedItem rewardItem = completeQuest.RewardItem;
			if ((bool)rewardItem)
			{
				if (!flag)
				{
					rewardItem.Get(completeQuest.RewardCount);
				}
				if (rewardItem.GetTakesHeroControl())
				{
					return;
				}
			}
			this.ExecuteDelayed(1f, ProcessQueuedCompletions);
		}, consumeCurrency: false, forceEnd: true);
	}

	private void OpenBoard()
	{
		UpdateQuestIndicator(null, isInstant: false, forceOff: true);
		questBoard.BoardClosed += AcceptQuests;
		questBoard.OpenPane();
	}

	private void AcceptQuests(List<BasicQuestBase> acceptedQuests)
	{
		questBoard.BoardClosed -= AcceptQuests;
		UpdateQuestDisplay();
		EndDialogue();
	}

	protected override void OnEndDialogue()
	{
		if (donateQuest == null)
		{
			EnableInteraction();
		}
		else
		{
			OnDonateQuestAccepted(donateQuest);
		}
	}

	private void SetupQuestDisplay()
	{
		IReadOnlyList<BasicQuestBase> readOnlyList = CurrentQuests;
		if (CheatManager.ShowAllQuestBoardQuest)
		{
			readOnlyList = questBoard.Quests;
		}
		for (int i = 0; i < questDisplays.Count; i++)
		{
			QuestBadge questBadge = questDisplays[i];
			if (i < readOnlyList.Count)
			{
				if ((bool)questBadge.Tint)
				{
					QuestTypeColor questTypeColor = default(QuestTypeColor);
					bool flag = false;
					foreach (QuestTypeColor questTypeColor2 in questTypeColors)
					{
						if (!(questTypeColor2.Type != readOnlyList[i].QuestType))
						{
							questTypeColor = questTypeColor2;
							flag = true;
							break;
						}
					}
					questBadge.Tint.color = (flag ? questTypeColor.Color : Color.white);
				}
				if ((bool)questBadge.Fade)
				{
					questBadge.Fade.AlphaSelf = 1f;
				}
				boardQuests.Add(new BoardQuest
				{
					Quest = readOnlyList[i],
					Display = questBadge
				});
			}
			else if ((bool)questBadge.Fade)
			{
				questBadge.Fade.AlphaSelf = 0f;
			}
		}
		UpdateQuestIndicator(readOnlyList, isInstant: true, forceOff: false);
	}

	private void UpdateQuestDisplay()
	{
		IReadOnlyList<BasicQuestBase> currentQuests = CurrentQuests;
		foreach (BoardQuest boardQuest in boardQuests)
		{
			NestedFadeGroupBase fade = boardQuest.Display.Fade;
			if ((bool)fade && !currentQuests.Contains(boardQuest.Quest) && fade.AlphaSelf > 0f)
			{
				fade.FadeTo(0f, questDisplayFadeTime);
			}
		}
		UpdateQuestIndicator(currentQuests, isInstant: false, forceOff: true);
	}

	private void UpdateQuestIndicator(IEnumerable<BasicQuestBase> quests, bool isInstant, bool forceOff)
	{
		if (questDisplayHidden || IsAnyQuestsCompletable)
		{
			return;
		}
		bool flag = true;
		if (!forceOff)
		{
			foreach (BasicQuestBase quest in quests)
			{
				if (!quest.HasBeenSeen && !quest.IsAccepted)
				{
					flag = false;
					break;
				}
			}
		}
		if (flag)
		{
			questDisplayHidden = true;
			if ((bool)newQuestIndicator)
			{
				newQuestIndicator.FadeTo(0f, isInstant ? 0f : questIndicatorFadeTime);
			}
		}
	}

	private void OnDonateQuestAccepted(FullQuestBase quest)
	{
		if ((bool)handInSequenceFsm)
		{
			handInSequenceFsm.FsmVariables.GetFsmObject("Quest").Value = quest;
			handInSequenceFsm.SendEvent("DONATE");
		}
	}

	public void TakeDonateCurrency()
	{
		questBoard.HideCurrencyCounters(donateQuest);
		donateQuest.ConsumeTarget();
	}

	public void OnDonationSequenceComplete()
	{
		donateQuest = null;
		UpdateQuestDisplay();
		if (questBoard.AvailableQuestsCount > 0)
		{
			HeroTalkAnimation.EnterConversation(this);
			OpenBoard();
		}
		else
		{
			EnableInteraction();
			GameManager.instance.DoQueuedSaveGame();
		}
	}
}

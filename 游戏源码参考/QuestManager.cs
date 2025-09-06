using System;
using System.Collections.Generic;
using System.Linq;
using GlobalSettings;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
	private static QuestManager instance;

	[SerializeField]
	private QuestList masterList;

	[Space]
	[SerializeField]
	private GameObject questAcceptedSequence;

	[SerializeField]
	private GameObject questFinishedSequence;

	[SerializeField]
	private EventRegister sequenceEndEvent;

	private GameObject spawnedQuestAcceptedSequence;

	private GameObject spawnedQuestFinishedSequence;

	private BasicQuestBase updatedQuest;

	private static ObjectCache<IEnumerable<BasicQuestBase>> _acceptedQuests;

	private static ObjectCache<IEnumerable<FullQuestBase>> _activeQuests;

	private static FullQuestBase[] _allFullQuests;

	private static Dictionary<string, FullQuestBase> _fullQuestLookup;

	private static QuestManager Instance
	{
		get
		{
			if (!instance)
			{
				instance = UnityEngine.Object.FindAnyObjectByType<QuestManager>();
			}
			return instance;
		}
	}

	public static BasicQuestBase UpdatedQuest
	{
		get
		{
			if (!instance)
			{
				return null;
			}
			return instance.updatedQuest;
		}
		set
		{
			if ((bool)instance)
			{
				instance.updatedQuest = value;
			}
		}
	}

	public static int Version { get; private set; }

	public static void IncrementVersion()
	{
		Version++;
	}

	public static void UpgradeQuests()
	{
		if (!instance)
		{
			return;
		}
		foreach (FullQuestBase allFullQuest in GetAllFullQuests())
		{
			if (allFullQuest.IsAccepted || allFullQuest.IsCompleted)
			{
				allFullQuest.SilentlyCompletePrevious();
			}
		}
	}

	private void Awake()
	{
		instance = this;
		foreach (BasicQuestBase master in masterList)
		{
			master.Init();
		}
		Version++;
	}

	private void Start()
	{
		PreSpawnSequence(questAcceptedSequence, ref spawnedQuestAcceptedSequence);
		PreSpawnSequence(questFinishedSequence, ref spawnedQuestFinishedSequence);
	}

	private void OnDestroy()
	{
		Version++;
		if ((bool)spawnedQuestAcceptedSequence)
		{
			UnityEngine.Object.Destroy(spawnedQuestAcceptedSequence);
		}
		if ((bool)spawnedQuestFinishedSequence)
		{
			UnityEngine.Object.Destroy(spawnedQuestFinishedSequence);
		}
		if (instance == this)
		{
			_acceptedQuests = null;
			_activeQuests = null;
			_allFullQuests = null;
			_fullQuestLookup = null;
			instance = null;
		}
	}

	private void OnValidate()
	{
		_allFullQuests = null;
		_fullQuestLookup = null;
	}

	private void PreSpawnSequence(GameObject original, ref GameObject target)
	{
		if ((bool)original)
		{
			target = UnityEngine.Object.Instantiate(original);
			target.SetActive(value: false);
			Transform obj = target.transform;
			obj.SetParent(base.transform, worldPositionStays: true);
			obj.SetParent(null, worldPositionStays: true);
		}
	}

	public static IEnumerable<BasicQuestBase> GetAllQuests()
	{
		if ((bool)Instance && (bool)Instance.masterList)
		{
			return Instance.masterList;
		}
		return Enumerable.Empty<BasicQuestBase>();
	}

	public static IEnumerable<BasicQuestBase> GetAcceptedQuests()
	{
		if (_acceptedQuests == null)
		{
			_acceptedQuests = new ObjectCache<IEnumerable<BasicQuestBase>>();
		}
		if (_acceptedQuests.ShouldUpdate(Version))
		{
			_acceptedQuests.UpdateCache((from quest in GetAllQuests()
				where quest.IsAccepted
				select quest).ToArray(), Version);
		}
		return _acceptedQuests.Value;
	}

	public static IEnumerable<FullQuestBase> GetActiveQuests()
	{
		if (_activeQuests == null)
		{
			_activeQuests = new ObjectCache<IEnumerable<FullQuestBase>>();
		}
		if (_activeQuests.ShouldUpdate(Version))
		{
			_activeQuests.UpdateCache((from quest in GetAcceptedQuests().OfType<FullQuestBase>()
				where !quest.IsCompleted
				select quest).ToArray(), Version);
		}
		return _activeQuests.Value;
	}

	public static FullQuestBase GetQuest(string questName)
	{
		if (TryGetFullQuestBase(questName, out var fullQuestBase))
		{
			return fullQuestBase;
		}
		Debug.LogError("Couldn't get quest: " + questName);
		return null;
	}

	private static IEnumerable<FullQuestBase> GetAllFullQuests()
	{
		if (_allFullQuests == null)
		{
			QuestManager questManager = Instance;
			if (!questManager || !questManager.masterList)
			{
				return ArraySegment<FullQuestBase>.Empty;
			}
			_allFullQuests = questManager.masterList.OfType<FullQuestBase>().ToArray();
		}
		return _allFullQuests;
	}

	private static bool TryGetFullQuestBase(string questName, out FullQuestBase fullQuestBase)
	{
		if (_fullQuestLookup == null)
		{
			IEnumerable<FullQuestBase> allFullQuests = GetAllFullQuests();
			if (allFullQuests == null || !allFullQuests.Any())
			{
				fullQuestBase = null;
				return false;
			}
			_fullQuestLookup = new Dictionary<string, FullQuestBase>();
			foreach (FullQuestBase item in allFullQuests)
			{
				if (!(item == null))
				{
					_fullQuestLookup[item.name] = item;
				}
			}
		}
		return _fullQuestLookup.TryGetValue(questName, out fullQuestBase);
	}

	public static bool IsAnyQuestVisible()
	{
		return GameManager.instance.playerData.QuestCompletionData.GetValidNames((QuestCompletionData.Completion completion) => completion.IsAccepted || completion.IsCompleted).Count > 0;
	}

	public static bool IsQuestInList(BasicQuestBase quest)
	{
		QuestManager questManager = Instance;
		if ((bool)questManager && (bool)questManager.masterList)
		{
			return questManager.masterList.Contains(quest);
		}
		return false;
	}

	public static void ShowQuestAccepted(FullQuestBase quest, Action afterPrompt)
	{
		if ((bool)Instance)
		{
			Instance.ShowQuestPromptInternal(quest, Instance.spawnedQuestAcceptedSequence, afterPrompt);
		}
	}

	public static void ShowQuestCompleted(FullQuestBase quest, Action afterPrompt)
	{
		if ((bool)Instance)
		{
			Instance.ShowQuestPromptInternal(quest, Instance.spawnedQuestFinishedSequence, afterPrompt);
		}
	}

	private void ShowQuestPromptInternal(FullQuestBase quest, GameObject prompt, Action afterPrompt)
	{
		if (!prompt)
		{
			Debug.LogError("No quest accepted object had been previously spawned!", this);
			return;
		}
		prompt.SetActive(value: true);
		QuestIconDisplay component = prompt.GetComponent<QuestIconDisplay>();
		if ((bool)component)
		{
			component.SetQuest(quest);
		}
		if (!sequenceEndEvent)
		{
			return;
		}
		Action temp = null;
		temp = delegate
		{
			sequenceEndEvent.ReceivedEvent -= temp;
			if (afterPrompt != null)
			{
				afterPrompt();
			}
		};
		sequenceEndEvent.ReceivedEvent += temp;
	}

	public static bool MaybeShowQuestUpdated(QuestTargetCounter item, CollectableUIMsg itemUiMsg)
	{
		FullQuestBase quest = ((!(item is CollectableItem collectableItem)) ? null : collectableItem.UseQuestForCap);
		return MaybeShowQuestUpdated(quest, item, itemUiMsg);
	}

	public static bool MaybeShowQuestUpdated(FullQuestBase quest, QuestTargetCounter item, CollectableUIMsg itemUiMsg)
	{
		BasicQuestBase basicQuestBase;
		if ((bool)quest)
		{
			if (!quest.IsAccepted || quest.IsCompleted || !quest.CanComplete)
			{
				return false;
			}
			basicQuestBase = quest;
		}
		else
		{
			basicQuestBase = null;
			foreach (FullQuestBase activeQuest in GetActiveQuests())
			{
				MainQuest mainQuest = activeQuest as MainQuest;
				if ((bool)mainQuest)
				{
					foreach (SubQuest subQuest2 in mainQuest.SubQuests)
					{
						SubQuest subQuest = subQuest2;
						do
						{
							if (IsQuestForItem(subQuest.TargetCounter, item) && subQuest.CanShowUpdated(isStandalone: false) && subQuest.CanComplete)
							{
								basicQuestBase = subQuest;
							}
							else
							{
								subQuest = subQuest.GetNext();
							}
						}
						while ((bool)subQuest && !basicQuestBase);
						if ((bool)basicQuestBase)
						{
							break;
						}
					}
					if ((bool)basicQuestBase)
					{
						break;
					}
					foreach (MainQuest.AltQuestTarget altTarget in mainQuest.AltTargets)
					{
						if (IsQuestForItem(altTarget.Counter, item))
						{
							basicQuestBase = mainQuest;
							break;
						}
					}
				}
				if (!activeQuest.TargetsAndCounters.All(((FullQuestBase.QuestTarget target, int count) pair) => pair.target.Counter != item || pair.count != pair.target.Count))
				{
					basicQuestBase = activeQuest;
					break;
				}
			}
		}
		if (!basicQuestBase)
		{
			return false;
		}
		if ((bool)itemUiMsg)
		{
			ShowQuestUpdatedForItemMsg(itemUiMsg, basicQuestBase);
		}
		else
		{
			ShowQuestUpdatedStandalone(basicQuestBase);
		}
		return true;
	}

	private static bool IsQuestForItem(QuestTargetCounter questItem, QuestTargetCounter item)
	{
		if (!questItem)
		{
			return false;
		}
		if (!(questItem == item))
		{
			return questItem.EnumerateSubTargets().Contains(item);
		}
		return true;
	}

	public static void ShowQuestUpdatedForItemMsg(CollectableUIMsg itemUiMsg, BasicQuestBase quest)
	{
		itemUiMsg.Replace(UI.ItemQuestMaxPopupDelay, new UIMsgDisplay
		{
			Name = UI.ItemQuestMaxPopup,
			Icon = quest.GetPopupIcon(),
			IconScale = 1f,
			RepresentingObject = quest
		});
		BasicQuestBase.SetInventoryNewItem(quest);
	}

	public static void ShowQuestUpdatedStandalone(BasicQuestBase quest)
	{
		bool flag = false;
		bool flag2 = false;
		if (quest is MainQuest)
		{
			flag = true;
		}
		if (!flag && quest is SubQuest subQuest)
		{
			if (!subQuest.CanShowUpdated(isStandalone: true))
			{
				return;
			}
			flag2 = true;
		}
		UIMsgDisplay uIMsgDisplay = default(UIMsgDisplay);
		uIMsgDisplay.Name = ((flag || flag2) ? UI.MainQuestProgressPopup : UI.ItemQuestMaxPopup);
		uIMsgDisplay.Icon = quest.GetPopupIcon();
		uIMsgDisplay.IconScale = 1f;
		uIMsgDisplay.RepresentingObject = quest;
		CollectableUIMsg collectableUIMsg = CollectableUIMsg.Spawn(uIMsgDisplay);
		if ((bool)collectableUIMsg)
		{
			collectableUIMsg.DoReplacingEffects();
		}
		BasicQuestBase.SetInventoryNewItem(quest);
	}
}

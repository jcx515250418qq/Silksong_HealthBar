using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyJournalManager : MonoBehaviour
{
	private enum CheckTypes
	{
		Seen = 0,
		Completed = 1,
		Required = 2,
		All = 3
	}

	private static EnemyJournalManager _instance;

	[SerializeField]
	private EnemyJournalRecordList recordList;

	[SerializeField]
	private GameObject journalUpdateMessagePrefab;

	private GameObject journalUpdateMessage;

	private EnemyJournalRecord updatedRecord;

	private static EnemyJournalManager Instance
	{
		get
		{
			if (!_instance)
			{
				_instance = UnityEngine.Object.FindObjectOfType<EnemyJournalManager>();
			}
			return _instance;
		}
	}

	public static EnemyJournalRecord UpdatedRecord
	{
		get
		{
			if (!Instance)
			{
				return null;
			}
			return Instance.updatedRecord;
		}
		set
		{
			if ((bool)Instance)
			{
				Instance.updatedRecord = value;
			}
		}
	}

	private void Awake()
	{
		_instance = this;
	}

	private void Start()
	{
		if ((bool)journalUpdateMessagePrefab)
		{
			journalUpdateMessage = UnityEngine.Object.Instantiate(journalUpdateMessagePrefab, base.transform, worldPositionStays: true);
			journalUpdateMessage.SetActive(value: false);
		}
	}

	private void OnDestroy()
	{
		if (_instance == this)
		{
			_instance = null;
		}
	}

	public static void RecordKill(EnemyJournalRecord journalRecord, bool showPopup, bool forcePopup)
	{
		GameManager instance = GameManager.instance;
		if ((bool)instance)
		{
			RecordKillToJournalData(journalRecord.name, instance.playerData.EnemyJournalKillData, out var previousKills, out var currentKills);
			bool flag = currentKills == journalRecord.KillsRequired || (previousKills == 0 && journalRecord.KillsRequired == 0);
			if (flag)
			{
				foreach (EnemyJournalRecord completeOther in journalRecord.CompleteOthers)
				{
					if (!(completeOther == null))
					{
						while (completeOther.KillCount < completeOther.KillsRequired)
						{
							RecordKill(completeOther, showPopup: false, forcePopup: false);
						}
					}
				}
			}
			if (showPopup && (bool)Instance && instance.playerData.hasJournal && ToolItemManager.ActiveState != ToolsActiveStates.Cutscene && !HeroController.instance.Config.ForceBareInventory)
			{
				Instance.ShowJournalUpdateMessage(previousKills == 0, flag || forcePopup, journalRecord);
			}
		}
		journalRecord.Increment();
		if (showPopup)
		{
			QuestManager.MaybeShowQuestUpdated(null, journalRecord, null);
		}
	}

	public static void RecordKill(EnemyJournalRecord journalRecord, bool showPopup = true)
	{
		RecordKill(journalRecord, showPopup, forcePopup: false);
	}

	[Obsolete]
	public static void CheckJournalAchievements()
	{
	}

	public static int GetCompletedEnemiesCount()
	{
		if (!Instance)
		{
			return 0;
		}
		int num = 0;
		foreach (EnemyJournalRecord record in Instance.recordList)
		{
			if (record.IsRequiredForCompletion && record.KillCount >= record.KillsRequired)
			{
				num++;
			}
		}
		return num;
	}

	public static void RecordKillToJournalData(string journalRecordName, EnemyJournalKillData journalData, out int previousKills, out int currentKills)
	{
		EnemyJournalKillData.KillData killData = journalData.GetKillData(journalRecordName);
		previousKills = killData.Kills;
		killData.Kills++;
		journalData.RecordKillData(journalRecordName, killData);
		currentKills = killData.Kills;
	}

	public static EnemyJournalKillData.KillData GetKillData(EnemyJournalRecord journalRecord)
	{
		GameManager instance = GameManager.instance;
		if (!journalRecord || !instance)
		{
			return default(EnemyJournalKillData.KillData);
		}
		return instance.playerData.EnemyJournalKillData.GetKillData(journalRecord.name);
	}

	public static void SetJournalSeen(EnemyJournalRecord journalRecord)
	{
		GameManager instance = GameManager.instance;
		if ((bool)journalRecord && (bool)instance)
		{
			EnemyJournalKillData.KillData killData = instance.playerData.EnemyJournalKillData.GetKillData(journalRecord.name);
			killData.HasBeenSeen = true;
			instance.playerData.EnemyJournalKillData.RecordKillData(journalRecord.name, killData);
		}
	}

	private void ShowJournalUpdateMessage(bool isFirstKill, bool isFinalKill, EnemyJournalRecord record)
	{
		bool flag = false;
		if (isFinalKill)
		{
			flag = true;
		}
		else if (isFirstKill)
		{
			flag = true;
		}
		if (!flag)
		{
			return;
		}
		updatedRecord = record;
		InventoryPaneList.SetNextOpen("Journal");
		if ((bool)journalUpdateMessage)
		{
			if (journalUpdateMessage.activeSelf)
			{
				journalUpdateMessage.SetActive(value: false);
			}
			journalUpdateMessage.SetActive(value: true);
			PlayMakerFSM playMakerFSM = PlayMakerFSM.FindFsmOnGameObject(journalUpdateMessage, "Journal Msg");
			if ((bool)playMakerFSM)
			{
				FSMUtility.SetBool(playMakerFSM, "Full", isFinalKill);
				FSMUtility.SetBool(playMakerFSM, "Should Recycle", value: true);
			}
		}
	}

	public static List<EnemyJournalRecord> GetKilledEnemies()
	{
		return GetEnemies(CheckTypes.Seen);
	}

	public static List<EnemyJournalRecord> GetCompletedEnemies()
	{
		return GetEnemies(CheckTypes.Completed);
	}

	public static List<EnemyJournalRecord> GetRequiredEnemies()
	{
		return GetEnemies(CheckTypes.Required);
	}

	public static List<EnemyJournalRecord> GetAllEnemies()
	{
		return GetEnemies(CheckTypes.All);
	}

	public static EnemyJournalRecord GetRecord(string name)
	{
		if (!Instance || !Instance.recordList)
		{
			return null;
		}
		return Instance.recordList.GetByName(name);
	}

	private static List<EnemyJournalRecord> GetEnemies(CheckTypes checkType)
	{
		if (!Instance || !Instance.recordList)
		{
			return new List<EnemyJournalRecord>();
		}
		if (checkType == CheckTypes.All || !Application.isPlaying)
		{
			return Instance.recordList.ToList();
		}
		return checkType switch
		{
			CheckTypes.Seen => Instance.recordList.Where((EnemyJournalRecord record) => record.IsVisible).ToList(), 
			CheckTypes.Completed => Instance.recordList.Where((EnemyJournalRecord record) => record.KillCount >= record.KillsRequired).ToList(), 
			CheckTypes.Required => Instance.recordList.Where((EnemyJournalRecord record) => record.IsRequiredForCompletion || record.IsVisible).ToList(), 
			_ => throw new ArgumentOutOfRangeException("checkType", checkType, null), 
		};
	}

	public static bool IsAllRequiredComplete()
	{
		EnemyJournalManager instance = Instance;
		if (!instance || !instance.recordList)
		{
			return false;
		}
		foreach (EnemyJournalRecord record in instance.recordList)
		{
			if (record.IsRequiredForCompletion && !record.IsAlwaysUnlocked && record.KillCount < record.KillsRequired)
			{
				return false;
			}
		}
		return true;
	}
}

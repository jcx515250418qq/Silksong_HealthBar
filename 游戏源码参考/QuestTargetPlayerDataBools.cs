using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using TeamCherry.SharedUtils;
using UnityEngine;

[CreateAssetMenu(menuName = "Hornet/Quests/Quest Target PlayerData Bools")]
public class QuestTargetPlayerDataBools : QuestTargetCounter, SceneLintLogger.ILogProvider
{
	[Serializable]
	private struct BoolInfo
	{
		[PlayerDataField(typeof(bool), true)]
		public string BoolName;

		public Sprite AltSprite;
	}

	[SerializeField]
	private Sprite questCounterSprite;

	[Space]
	[SerializeField]
	private BoolInfo[] pdBools;

	[SerializeField]
	[PlayerDataField(typeof(List<int>), true)]
	private string orderListPd;

	[Space]
	[SerializeField]
	private string pdFieldTemplate;

	[SerializeField]
	private bool getAppendsSceneName;

	[Space]
	[SerializeField]
	private string linkedAchievementHalf;

	[SerializeField]
	private string linkedAchievementFull;

	private bool[] pdBoolsFoundIcon;

	[UsedImplicitly]
	public int CompletedCount => CountCompleted();

	[UsedImplicitly]
	public bool AllCollected => !CanGetMore();

	public override bool CanGetMore()
	{
		GetCounts(out var completed, out var total);
		return completed < total;
	}

	public override int GetCompletionAmount(QuestCompletionData.Completion sourceCompletion)
	{
		return CountCompleted();
	}

	public int CountCompleted()
	{
		GetCounts(out var completed, out var _);
		return completed;
	}

	private void GetCounts(out int completed, out int total)
	{
		PlayerData instance = PlayerData.instance;
		completed = 0;
		total = 0;
		if (!string.IsNullOrWhiteSpace(pdFieldTemplate))
		{
			foreach (bool variable in instance.GetVariables<bool>(pdFieldTemplate))
			{
				total++;
				if (variable)
				{
					completed++;
				}
			}
		}
		BoolInfo[] array = pdBools;
		for (int i = 0; i < array.Length; i++)
		{
			BoolInfo boolInfo = array[i];
			total++;
			if (instance.GetVariable<bool>(boolInfo.BoolName))
			{
				completed++;
			}
		}
	}

	private bool UsesSceneBools()
	{
		if (!string.IsNullOrWhiteSpace(pdFieldTemplate))
		{
			return getAppendsSceneName;
		}
		return false;
	}

	public override void Get(bool showPopup = true)
	{
		if (!UsesSceneBools())
		{
			base.Get(showPopup);
			return;
		}
		PlayerData instance = PlayerData.instance;
		GameManager instance2 = GameManager.instance;
		string text = pdFieldTemplate + instance2.GetSceneNameString();
		if (VariableExtensions.VariableExists<bool, PlayerData>(text))
		{
			instance.SetVariable(text, value: true);
		}
		else
		{
			Debug.LogWarning("PD Variable " + text + " does not exist! This is fine if we're just calling this to display quest updated after bool set.", this);
		}
		if (showPopup)
		{
			QuestManager.MaybeShowQuestUpdated(null, this, null);
		}
		CheckAchievements();
	}

	public bool GetSceneBoolValue()
	{
		if (!UsesSceneBools())
		{
			return false;
		}
		GameManager instance = GameManager.instance;
		PlayerData instance2 = PlayerData.instance;
		string fieldName = pdFieldTemplate + instance.GetSceneNameString();
		return instance2.GetVariable<bool>(fieldName);
	}

	public string GetSceneLintLog(string sceneName)
	{
		if (!UsesSceneBools())
		{
			return null;
		}
		string text = pdFieldTemplate + GameManager.GetBaseSceneName(sceneName);
		if (!VariableExtensions.VariableExists<PlayerData>(text, typeof(bool)))
		{
			return base.name + " missing " + text + " in PlayerData";
		}
		return null;
	}

	public void CheckAchievements()
	{
		GetCounts(out var completed, out var total);
		GameManager instance = GameManager.instance;
		if (!string.IsNullOrWhiteSpace(linkedAchievementHalf))
		{
			int max = Mathf.RoundToInt((float)total * 0.5f);
			instance.UpdateAchievementProgress(linkedAchievementHalf, completed, max);
		}
		if (!string.IsNullOrWhiteSpace(linkedAchievementFull))
		{
			instance.UpdateAchievementProgress(linkedAchievementFull, completed, total);
		}
		RecordOrder(completed);
	}

	public override Sprite GetPopupIcon()
	{
		return questCounterSprite;
	}

	public override Sprite GetQuestCounterSprite(int index)
	{
		PlayerData instance = PlayerData.instance;
		if (!string.IsNullOrEmpty(orderListPd))
		{
			List<int> variable = instance.GetVariable<List<int>>(orderListPd);
			if (variable != null && variable.Count > 0)
			{
				if (index >= variable.Count)
				{
					return base.GetQuestCounterSprite(index);
				}
				int num = variable[index];
				if (num < 0 || num >= pdBools.Length)
				{
					return base.GetQuestCounterSprite(index);
				}
				BoolInfo boolInfo = pdBools[num];
				if (!boolInfo.AltSprite)
				{
					return base.GetQuestCounterSprite(index);
				}
				return boolInfo.AltSprite;
			}
		}
		if (index >= 0 && index < pdBools.Length)
		{
			BoolInfo boolInfo2 = pdBools[index];
			if ((bool)boolInfo2.AltSprite && instance.GetVariable<bool>(boolInfo2.BoolName))
			{
				return boolInfo2.AltSprite;
			}
		}
		return base.GetQuestCounterSprite(index);
	}

	public void RecordOrder(int completedCount)
	{
		if (string.IsNullOrEmpty(orderListPd))
		{
			return;
		}
		PlayerData instance = PlayerData.instance;
		List<int> list = instance.GetVariable<List<int>>(orderListPd);
		if (list == null)
		{
			list = new List<int>();
			instance.SetVariable(orderListPd, list);
		}
		if (pdBoolsFoundIcon == null || pdBoolsFoundIcon.Length != pdBools.Length)
		{
			pdBoolsFoundIcon = new bool[pdBools.Length];
		}
		for (int i = 0; i < pdBoolsFoundIcon.Length; i++)
		{
			pdBoolsFoundIcon[i] = false;
		}
		foreach (int item in list)
		{
			if (item >= 0 && item < pdBoolsFoundIcon.Length)
			{
				pdBoolsFoundIcon[item] = true;
			}
		}
		for (int j = list.Count; j < completedCount; j++)
		{
			list.Add(-1);
			for (int k = 0; k < pdBools.Length; k++)
			{
				if (!pdBoolsFoundIcon[k])
				{
					BoolInfo boolInfo = pdBools[k];
					if (instance.GetVariable<bool>(boolInfo.BoolName))
					{
						list[j] = k;
						pdBoolsFoundIcon[k] = true;
						break;
					}
				}
			}
		}
	}
}

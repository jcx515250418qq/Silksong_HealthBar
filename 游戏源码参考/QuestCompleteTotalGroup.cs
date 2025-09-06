using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Hornet/Quests/Quest Complete Total Group")]
public class QuestCompleteTotalGroup : ScriptableObject
{
	[Serializable]
	public struct CompleteQuest
	{
		public FullQuestBase Quest;

		public float Value;

		public bool IsRequired;
	}

	[SerializeField]
	private CompleteQuest[] quests;

	[SerializeField]
	private float target;

	[Space]
	[SerializeField]
	private PlayerDataTest additionalTest;

	public bool IsFulfilled
	{
		get
		{
			if (!additionalTest.IsFulfilled)
			{
				return false;
			}
			foreach (CompleteQuest quest in Quests)
			{
				if (!quest.Quest)
				{
					Debug.LogError("Skipping null quest", this);
				}
				else if (quest.IsRequired && !quest.Quest.IsCompleted)
				{
					return false;
				}
			}
			return CurrentValueCount >= target;
		}
	}

	public float CurrentValueCount
	{
		get
		{
			float num = 0f;
			foreach (CompleteQuest quest in Quests)
			{
				if (quest.Quest.IsCompleted)
				{
					num += quest.Value;
				}
			}
			return num;
		}
	}

	public IEnumerable<CompleteQuest> Quests => quests.Where((CompleteQuest c) => c.Quest);

	private void OnValidate()
	{
		for (int i = 0; i < quests.Length; i++)
		{
			CompleteQuest completeQuest = quests[i];
			if (!(completeQuest.Value >= 0f))
			{
				completeQuest.Value = 0f;
				quests[i] = completeQuest;
			}
		}
	}
}

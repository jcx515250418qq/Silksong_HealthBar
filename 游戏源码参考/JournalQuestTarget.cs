using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Hornet/Quests/Quest Target Journal")]
public class JournalQuestTarget : QuestTargetCounter
{
	private enum CheckTypes
	{
		Seen = 0,
		Completed = 1
	}

	[SerializeField]
	private Sprite counterSprite;

	[SerializeField]
	private CheckTypes checkType;

	public override Sprite GetQuestCounterSprite(int index)
	{
		return counterSprite;
	}

	public override bool CanGetMore()
	{
		return false;
	}

	public override int GetCompletionAmount(QuestCompletionData.Completion sourceCompletion)
	{
		return checkType switch
		{
			CheckTypes.Seen => EnemyJournalManager.GetKilledEnemies().Count, 
			CheckTypes.Completed => EnemyJournalManager.GetCompletedEnemies().Count, 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}
}

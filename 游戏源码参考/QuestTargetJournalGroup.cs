using UnityEngine;

[CreateAssetMenu(menuName = "Hornet/Quests/Quest Target Journal Group")]
public class QuestTargetJournalGroup : QuestTargetCounter
{
	[SerializeField]
	private Sprite icon;

	[Space]
	[SerializeField]
	private EnemyJournalRecord[] enemies;

	public override bool CanGetMore()
	{
		return false;
	}

	public override bool ShouldIncrementQuestCounter(QuestTargetCounter eventSender)
	{
		EnemyJournalRecord[] array = enemies;
		foreach (EnemyJournalRecord enemyJournalRecord in array)
		{
			if (eventSender == enemyJournalRecord)
			{
				return true;
			}
		}
		return base.ShouldIncrementQuestCounter(eventSender);
	}

	public override Sprite GetPopupIcon()
	{
		return icon;
	}
}

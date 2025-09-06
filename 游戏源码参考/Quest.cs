using UnityEngine;

[CreateAssetMenu(menuName = "Hornet/Quests/Quest")]
public class Quest : FullQuestBase
{
	[Header("Quest")]
	[SerializeField]
	private QuestType questType;

	public override QuestType QuestType => questType;
}

using HutongGames.PlayMaker;

namespace QuestPlaymakerActions
{
	[ActionCategory("Quests")]
	public class IncrementQuestCounter : QuestFsmAction
	{
		protected override void DoQuestAction(FullQuestBase quest)
		{
			quest.IncrementQuestCounter();
		}
	}
}

using HutongGames.PlayMaker;

namespace QuestPlaymakerActions
{
	[ActionCategory("Quests")]
	public class BeginQuest : QuestFsmAction
	{
		protected override bool CustomFinish => true;

		protected override void DoQuestAction(FullQuestBase quest)
		{
			quest.BeginQuest(base.Finish);
		}
	}
}

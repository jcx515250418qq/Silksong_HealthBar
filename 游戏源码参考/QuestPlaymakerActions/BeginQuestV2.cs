using HutongGames.PlayMaker;

namespace QuestPlaymakerActions
{
	[ActionCategory("Quests")]
	public class BeginQuestV2 : QuestFsmAction
	{
		public FsmBool ShowPrompt;

		protected override bool CustomFinish => ShowPrompt.Value;

		public override void Reset()
		{
			base.Reset();
			ShowPrompt = true;
		}

		protected override void DoQuestAction(FullQuestBase quest)
		{
			if (ShowPrompt.Value)
			{
				quest.BeginQuest(base.Finish);
			}
			else
			{
				quest.BeginQuest(null, showPrompt: false);
			}
		}
	}
}

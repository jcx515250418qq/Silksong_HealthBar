using System.Linq;
using HutongGames.PlayMaker;

namespace QuestPlaymakerActions
{
	[ActionCategory("Quests")]
	public class GetQuestState : QuestFsmAction
	{
		[UIHint(UIHint.Variable)]
		public FsmBool IsAccepted;

		[UIHint(UIHint.Variable)]
		public FsmBool IsCompleted;

		public override void Reset()
		{
			base.Reset();
			IsAccepted = null;
			IsCompleted = null;
		}

		protected override void DoQuestAction(FullQuestBase quest)
		{
			IsAccepted.Value = QuestManager.GetAcceptedQuests().Contains(quest);
			IsCompleted.Value = quest.IsCompleted;
		}
	}
}

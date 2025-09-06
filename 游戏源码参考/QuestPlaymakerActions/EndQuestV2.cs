using HutongGames.PlayMaker;

namespace QuestPlaymakerActions
{
	[ActionCategory("Quests")]
	[Tooltip("Force ends a quest - assumes something like QuestCompleteYesNo was previously used to check if quest can complete.")]
	public class EndQuestV2 : QuestFsmAction
	{
		public FsmBool ConsumeCurrency;

		public FsmBool ShowPrompt;

		protected override bool CustomFinish => true;

		public override void Reset()
		{
			base.Reset();
			ConsumeCurrency = null;
			ShowPrompt = true;
		}

		protected override void DoQuestAction(FullQuestBase quest)
		{
			quest.TryEndQuest(base.Finish, ConsumeCurrency.Value, forceEnd: true, ShowPrompt.Value);
		}
	}
}

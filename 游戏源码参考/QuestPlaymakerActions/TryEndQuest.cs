using HutongGames.PlayMaker;

namespace QuestPlaymakerActions
{
	[ActionCategory("Quests")]
	public class TryEndQuest : QuestFsmAction
	{
		public FsmEvent FailEvent;

		public FsmEvent SuccessEvent;

		protected override bool CustomFinish => true;

		public override void Reset()
		{
			base.Reset();
			FailEvent = null;
			SuccessEvent = null;
		}

		protected override void DoQuestAction(FullQuestBase quest)
		{
			FsmEvent fsmEvent = (quest.CanComplete ? SuccessEvent : FailEvent);
			quest.TryEndQuest(delegate
			{
				base.Fsm.Event(fsmEvent);
				Finish();
			}, consumeCurrency: true);
		}
	}
}

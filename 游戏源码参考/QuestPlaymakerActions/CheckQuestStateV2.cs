using HutongGames.PlayMaker;

namespace QuestPlaymakerActions
{
	[ActionCategory("Quests")]
	public class CheckQuestStateV2 : QuestFsmAction
	{
		public FsmEvent NotTrackedEvent;

		public FsmEvent IncompleteEvent;

		public FsmEvent CompletedEvent;

		public override void Reset()
		{
			base.Reset();
			NotTrackedEvent = null;
			IncompleteEvent = null;
			CompletedEvent = null;
		}

		protected override void DoQuestAction(FullQuestBase quest)
		{
			if (quest.IsCompleted)
			{
				base.Fsm.Event(CompletedEvent);
			}
			else if (quest.IsAccepted)
			{
				base.Fsm.Event(IncompleteEvent);
			}
			else
			{
				base.Fsm.Event(NotTrackedEvent);
			}
		}
	}
}

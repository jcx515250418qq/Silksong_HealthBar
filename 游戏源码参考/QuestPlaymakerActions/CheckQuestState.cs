using System.Linq;
using HutongGames.PlayMaker;

namespace QuestPlaymakerActions
{
	[ActionCategory("Quests")]
	public class CheckQuestState : QuestFsmAction
	{
		public FsmEvent NotTrackedEvent;

		public FsmEvent TrackedEvent;

		public FsmEvent CompletedEvent;

		public override void Reset()
		{
			base.Reset();
			NotTrackedEvent = null;
			TrackedEvent = null;
			CompletedEvent = null;
		}

		protected override void DoQuestAction(FullQuestBase quest)
		{
			if (QuestManager.GetAcceptedQuests().Contains(quest))
			{
				if (quest.IsCompleted)
				{
					base.Fsm.Event(CompletedEvent);
				}
				base.Fsm.Event(TrackedEvent);
			}
			else
			{
				base.Fsm.Event(NotTrackedEvent);
			}
		}
	}
}

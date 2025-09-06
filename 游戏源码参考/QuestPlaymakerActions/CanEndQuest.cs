using HutongGames.PlayMaker;

namespace QuestPlaymakerActions
{
	[ActionCategory("Quests")]
	public class CanEndQuest : QuestFsmAction
	{
		public FsmEvent CannotEndEvent;

		public FsmEvent CanEndEvent;

		[UIHint(UIHint.Variable)]
		public FsmBool StoreValue;

		public override void Reset()
		{
			base.Reset();
			CannotEndEvent = null;
			CanEndEvent = null;
			StoreValue = null;
		}

		protected override void DoQuestAction(FullQuestBase quest)
		{
			bool canComplete = quest.CanComplete;
			StoreValue.Value = canComplete;
			base.Fsm.Event(canComplete ? CanEndEvent : CannotEndEvent);
		}
	}
}

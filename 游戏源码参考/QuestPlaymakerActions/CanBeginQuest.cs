using HutongGames.PlayMaker;

namespace QuestPlaymakerActions
{
	[ActionCategory("Quests")]
	public class CanBeginQuest : QuestFsmAction
	{
		public FsmEvent CannotBeginEvent;

		public FsmEvent CanBeginEvent;

		[UIHint(UIHint.Variable)]
		public FsmBool StoreValue;

		public override void Reset()
		{
			base.Reset();
			CannotBeginEvent = null;
			CanBeginEvent = null;
			StoreValue = null;
		}

		protected override void DoQuestAction(FullQuestBase quest)
		{
			bool isAvailable = quest.IsAvailable;
			StoreValue.Value = isAvailable;
			base.Fsm.Event(isAvailable ? CanBeginEvent : CannotBeginEvent);
		}
	}
}

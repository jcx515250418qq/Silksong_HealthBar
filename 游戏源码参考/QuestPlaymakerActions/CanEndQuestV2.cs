using HutongGames.PlayMaker;

namespace QuestPlaymakerActions
{
	[ActionCategory("Quests")]
	public class CanEndQuestV2 : QuestFsmAction
	{
		public FsmBool RequireActive;

		public FsmEvent CannotEndEvent;

		public FsmEvent CanEndEvent;

		[UIHint(UIHint.Variable)]
		public FsmBool StoreValue;

		public override void Reset()
		{
			base.Reset();
			RequireActive = null;
			CannotEndEvent = null;
			CanEndEvent = null;
			StoreValue = null;
		}

		protected override void DoQuestAction(FullQuestBase quest)
		{
			bool flag = (RequireActive.Value ? quest.GetIsReadyToTurnIn(atQuestBoard: false) : quest.CanComplete);
			StoreValue.Value = flag;
			base.Fsm.Event(flag ? CanEndEvent : CannotEndEvent);
		}
	}
}

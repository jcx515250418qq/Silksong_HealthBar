using HutongGames.PlayMaker;

namespace QuestPlaymakerActions
{
	[ActionCategory("Quests")]
	public class GetQuestReward : QuestFsmAction
	{
		[ObjectType(typeof(SavedItem))]
		[UIHint(UIHint.Variable)]
		public FsmObject StoreReward;

		public override void Reset()
		{
			base.Reset();
			StoreReward = null;
		}

		protected override void DoQuestAction(FullQuestBase quest)
		{
			StoreReward.Value = quest.RewardItem;
		}
	}
}

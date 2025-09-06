using HutongGames.PlayMaker;

namespace QuestPlaymakerActions
{
	[ActionCategory("Quests")]
	public class GetQuestRewardV2 : QuestFsmAction
	{
		[ObjectType(typeof(SavedItem))]
		[UIHint(UIHint.Variable)]
		public FsmObject StoreReward;

		[UIHint(UIHint.Variable)]
		public FsmInt StoreAmount;

		public override void Reset()
		{
			base.Reset();
			StoreReward = null;
			StoreAmount = null;
		}

		protected override void DoQuestAction(FullQuestBase quest)
		{
			StoreReward.Value = quest.RewardItem;
			StoreAmount.Value = quest.RewardCount;
		}
	}
}

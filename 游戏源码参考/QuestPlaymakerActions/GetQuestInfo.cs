using HutongGames.PlayMaker;

namespace QuestPlaymakerActions
{
	[ActionCategory("Quests")]
	public class GetQuestInfo : QuestFsmAction
	{
		[UIHint(UIHint.Variable)]
		public FsmInt TargetCount;

		[UIHint(UIHint.Variable)]
		public FsmInt CurrentCount;

		public override void Reset()
		{
			base.Reset();
			TargetCount = null;
			CurrentCount = null;
		}

		protected override void DoQuestAction(FullQuestBase quest)
		{
			int num = 0;
			int num2 = 0;
			foreach (var targetsAndCounter in quest.TargetsAndCounters)
			{
				FullQuestBase.QuestTarget item = targetsAndCounter.target;
				int item2 = targetsAndCounter.count;
				num += item.Count;
				num2 += item2;
			}
			TargetCount.Value = num;
			CurrentCount.Value = num2;
		}
	}
}

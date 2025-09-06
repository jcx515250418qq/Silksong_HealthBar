using System.Linq;
using HutongGames.PlayMaker;

namespace QuestPlaymakerActions
{
	[ActionCategory("Quests")]
	public class GetQuestInfoV2 : QuestFsmAction
	{
		[UIHint(UIHint.Variable)]
		[ArrayEditor(VariableType.Int, "", 0, 0, 65536)]
		public FsmArray TargetCounts;

		[UIHint(UIHint.Variable)]
		[ArrayEditor(VariableType.Int, "", 0, 0, 65536)]
		public FsmArray CurrentCounts;

		public override void Reset()
		{
			base.Reset();
			TargetCounts = null;
			CurrentCounts = null;
		}

		protected override void DoQuestAction(FullQuestBase quest)
		{
			TargetCounts.intValues = quest.Targets.Select((FullQuestBase.QuestTarget target) => target.Count).ToArray();
			CurrentCounts.intValues = quest.Counters.ToArray();
		}
	}
}

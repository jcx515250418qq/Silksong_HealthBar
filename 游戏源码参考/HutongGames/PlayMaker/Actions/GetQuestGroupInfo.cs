using System.Collections.Generic;
using System.Linq;

namespace HutongGames.PlayMaker.Actions
{
	public class GetQuestGroupInfo : FsmStateAction
	{
		[ObjectType(typeof(QuestGroup))]
		public FsmObject QuestGroup;

		[UIHint(UIHint.Variable)]
		public FsmInt StoreCompleteCount;

		[UIHint(UIHint.Variable)]
		public FsmInt StoreTotalCount;

		public override void Reset()
		{
			QuestGroup = null;
			StoreCompleteCount = null;
			StoreTotalCount = null;
		}

		public override void OnEnter()
		{
			QuestGroup questGroup = QuestGroup.Value as QuestGroup;
			if (questGroup != null)
			{
				IEnumerable<FullQuestBase> fullQuests = questGroup.GetFullQuests();
				StoreCompleteCount.Value = fullQuests.Count((FullQuestBase quest) => quest.IsCompleted);
				StoreTotalCount.Value = fullQuests.Count();
			}
			else
			{
				StoreCompleteCount.Value = 0;
				StoreTotalCount.Value = 0;
			}
			Finish();
		}
	}
}

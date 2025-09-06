namespace HutongGames.PlayMaker.Actions
{
	public class EvaluateQuestGroup : FsmStateAction
	{
		[ObjectType(typeof(QuestGroup))]
		public FsmObject QuestGroup;

		[ObjectType(typeof(FullQuestBase))]
		[UIHint(UIHint.Variable)]
		public FsmObject StoreQuest;

		[UIHint(UIHint.Variable)]
		public FsmInt StoreIndex;

		public override void Reset()
		{
			QuestGroup = null;
			StoreQuest = null;
			StoreIndex = null;
		}

		public override void OnEnter()
		{
			QuestGroup questGroup = QuestGroup.Value as QuestGroup;
			if (questGroup != null)
			{
				questGroup.Evaluate(out var quest, out var index);
				StoreQuest.Value = quest;
				StoreIndex.Value = index;
			}
			else
			{
				StoreQuest.Value = null;
				StoreIndex.Value = -1;
			}
			Finish();
		}
	}
}

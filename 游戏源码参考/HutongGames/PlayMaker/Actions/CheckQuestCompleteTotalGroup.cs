namespace HutongGames.PlayMaker.Actions
{
	public class CheckQuestCompleteTotalGroup : FSMUtility.CheckFsmStateAction
	{
		[RequiredField]
		[ObjectType(typeof(QuestCompleteTotalGroup))]
		public FsmObject TotalGroup;

		public override bool IsTrue
		{
			get
			{
				QuestCompleteTotalGroup questCompleteTotalGroup = TotalGroup.Value as QuestCompleteTotalGroup;
				if ((bool)questCompleteTotalGroup)
				{
					return questCompleteTotalGroup.IsFulfilled;
				}
				return false;
			}
		}

		public override void Reset()
		{
			base.Reset();
			TotalGroup = null;
		}
	}
}

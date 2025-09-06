namespace HutongGames.PlayMaker.Actions
{
	public class CheckQuestPdSceneBool : FSMUtility.CheckFsmStateAction
	{
		[ObjectType(typeof(QuestTargetPlayerDataBools))]
		[RequiredField]
		public FsmObject QuestTarget;

		[RequiredField]
		public FsmBool ExpectedValue;

		public override bool IsTrue
		{
			get
			{
				QuestTargetPlayerDataBools questTargetPlayerDataBools = QuestTarget.Value as QuestTargetPlayerDataBools;
				if (questTargetPlayerDataBools == null)
				{
					return false;
				}
				return questTargetPlayerDataBools.GetSceneBoolValue() == ExpectedValue.Value;
			}
		}

		public override void Reset()
		{
			base.Reset();
			QuestTarget = null;
			ExpectedValue = null;
		}
	}
}

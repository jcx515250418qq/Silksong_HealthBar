namespace HutongGames.PlayMaker.Actions
{
	public class CheckIsInQuestBoardDepositSequence : FSMUtility.CheckFsmStateAction
	{
		public override bool IsTrue => StaticVariableList.GetValue("IsInQuestBoardDepositSequence", defaultValue: false);
	}
}

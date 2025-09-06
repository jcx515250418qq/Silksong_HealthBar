namespace HutongGames.PlayMaker.Actions
{
	public class CheckBackerCreditsSetting : FSMUtility.CheckFsmStateAction
	{
		public override bool IsTrue => GameManager.instance.gameSettings.backerCredits != 0;
	}
}

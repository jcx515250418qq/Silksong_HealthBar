namespace HutongGames.PlayMaker.Actions
{
	public class CheckIsDemoMode : FSMUtility.CheckFsmStateAction
	{
		public override bool IsTrue => DemoHelper.IsDemoMode;
	}
}

namespace HutongGames.PlayMaker.Actions
{
	public class CheckIsCheatManagerOpen : FSMUtility.CheckFsmStateEveryFrameAction
	{
		public override bool IsTrue => CheatManager.IsOpen;
	}
}

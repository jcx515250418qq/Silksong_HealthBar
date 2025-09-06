namespace HutongGames.PlayMaker.Actions
{
	public class CheckIsSilkDrainDisabled : FSMUtility.CheckFsmStateAction
	{
		public override bool IsTrue => CheatManager.IsSilkDrainDisabled;
	}
}

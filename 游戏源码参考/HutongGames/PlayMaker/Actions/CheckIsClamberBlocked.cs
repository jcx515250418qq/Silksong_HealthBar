namespace HutongGames.PlayMaker.Actions
{
	public class CheckIsClamberBlocked : FSMUtility.CheckFsmStateAction
	{
		public override bool IsTrue => NoClamberRegion.IsClamberBlocked;
	}
}

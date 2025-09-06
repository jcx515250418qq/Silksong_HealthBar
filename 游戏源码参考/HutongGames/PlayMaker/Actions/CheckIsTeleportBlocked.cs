namespace HutongGames.PlayMaker.Actions
{
	public class CheckIsTeleportBlocked : FSMUtility.CheckFsmStateAction
	{
		public override bool IsTrue => GameManager.instance.IsMemoryScene();
	}
}

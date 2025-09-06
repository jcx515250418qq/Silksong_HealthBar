public class CheckStartedOnThisScene : FSMUtility.CheckFsmStateAction
{
	public override bool IsTrue => GameManager.instance.startedOnThisScene;
}

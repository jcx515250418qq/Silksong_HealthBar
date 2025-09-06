using HutongGames.PlayMaker;

public class QueueSaveGame : FsmStateAction
{
	public override void OnEnter()
	{
		GameManager unsafeInstance = GameManager.UnsafeInstance;
		if ((bool)unsafeInstance)
		{
			unsafeInstance.QueueSaveGame();
		}
		Finish();
	}
}

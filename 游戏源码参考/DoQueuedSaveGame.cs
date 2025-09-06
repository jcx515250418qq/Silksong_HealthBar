using HutongGames.PlayMaker;

public class DoQueuedSaveGame : FsmStateAction
{
	public override void OnEnter()
	{
		GameManager unsafeInstance = GameManager.UnsafeInstance;
		if ((bool)unsafeInstance)
		{
			unsafeInstance.DoQueuedSaveGame();
		}
		Finish();
	}
}

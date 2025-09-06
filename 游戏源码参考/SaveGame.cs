using HutongGames.PlayMaker;

public class SaveGame : FsmStateAction
{
	public override void OnEnter()
	{
		GameManager unsafeInstance = GameManager.UnsafeInstance;
		if ((bool)unsafeInstance)
		{
			unsafeInstance.SaveGame(null);
		}
		Finish();
	}
}

using HutongGames.PlayMaker;

public class QueueSaveGameV2 : FsmStateAction
{
	public FsmBool createAutoSave;

	[ObjectType(typeof(AutoSaveName))]
	public FsmEnum nameEnum;

	public override void Reset()
	{
		createAutoSave = null;
		nameEnum = null;
	}

	public override void OnEnter()
	{
		GameManager unsafeInstance = GameManager.UnsafeInstance;
		if ((bool)unsafeInstance)
		{
			if (createAutoSave.Value)
			{
				unsafeInstance.QueueAutoSave((AutoSaveName)(object)nameEnum.Value);
			}
			unsafeInstance.QueueSaveGame();
		}
		Finish();
	}
}

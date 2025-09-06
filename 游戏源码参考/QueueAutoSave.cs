using HutongGames.PlayMaker;

[Tooltip("Queue Auto Save without queuing normal save.")]
public sealed class QueueAutoSave : FsmStateAction
{
	[ObjectType(typeof(AutoSaveName))]
	public FsmEnum nameEnum;

	public override void Reset()
	{
		nameEnum = null;
	}

	public override void OnEnter()
	{
		GameManager unsafeInstance = GameManager.UnsafeInstance;
		if ((bool)unsafeInstance)
		{
			unsafeInstance.QueueAutoSave((AutoSaveName)(object)nameEnum.Value);
		}
		Finish();
	}
}

using HutongGames.PlayMaker;

[Tooltip("Silently create an Auto Save without normal save. (No Save Icon Shown)")]
public sealed class CreateAutoSaveSilent : FsmStateAction
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
			unsafeInstance.CreateRestorePoint((AutoSaveName)(object)nameEnum.Value);
		}
		Finish();
	}
}

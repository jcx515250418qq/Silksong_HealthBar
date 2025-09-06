using HutongGames.PlayMaker;
using UnityEngine;

public class SaveGameV2 : FsmStateAction
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
				unsafeInstance.SaveGameWithAutoSave((AutoSaveName)(object)nameEnum.Value, null);
			}
			else
			{
				unsafeInstance.SaveGame(null);
			}
		}
		else
		{
			Debug.LogError("Failed to save. Missing Game Manager.");
		}
		Finish();
	}
}

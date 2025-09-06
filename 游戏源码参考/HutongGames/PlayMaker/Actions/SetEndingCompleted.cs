namespace HutongGames.PlayMaker.Actions
{
	public class SetEndingCompleted : FsmStateAction
	{
		[RequiredField]
		[ObjectType(typeof(SaveSlotCompletionIcons.CompletionState))]
		public FsmEnum EndingType;

		public override void Reset()
		{
			EndingType = null;
		}

		public override void OnEnter()
		{
			PlayerData instance = PlayerData.instance;
			SaveSlotCompletionIcons.CompletionState completionState = (SaveSlotCompletionIcons.CompletionState)(object)EndingType.Value;
			instance.CompletedEndings |= completionState;
			instance.LastCompletedEnding = completionState;
			GameManager instance2 = GameManager.instance;
			if ((bool)instance2)
			{
				instance2.RecordGameComplete();
			}
			Finish();
		}
	}
}

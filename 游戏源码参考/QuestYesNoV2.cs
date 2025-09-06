using HutongGames.PlayMaker;

[ActionCategory("Dialogue")]
public class QuestYesNoV2 : YesNoAction
{
	[ObjectType(typeof(FullQuestBase))]
	[RequiredField]
	public FsmObject Quest;

	public FsmBool BeginQuest;

	public override void Reset()
	{
		base.Reset();
		Quest = null;
		BeginQuest = true;
	}

	protected override void DoOpen()
	{
		QuestYesNoBox.Open(delegate
		{
			SendEvent(isYes: true);
		}, delegate
		{
			SendEvent(isYes: false);
		}, ReturnHUDAfter.Value, (FullQuestBase)Quest.Value, BeginQuest.Value);
	}

	protected override void DoForceClose()
	{
		QuestYesNoBox.ForceClose();
	}
}

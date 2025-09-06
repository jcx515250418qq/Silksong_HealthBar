using HutongGames.PlayMaker;

[ActionCategory("Dialogue")]
public class QuestYesNo : YesNoAction
{
	[ObjectType(typeof(FullQuestBase))]
	[RequiredField]
	public FsmObject Quest;

	public override void Reset()
	{
		base.Reset();
		Quest = null;
	}

	protected override void DoOpen()
	{
		QuestYesNoBox.Open(delegate
		{
			SendEvent(isYes: true);
		}, delegate
		{
			SendEvent(isYes: false);
		}, ReturnHUDAfter.Value, (FullQuestBase)Quest.Value, beginQuest: true);
	}

	protected override void DoForceClose()
	{
		QuestYesNoBox.ForceClose();
	}
}

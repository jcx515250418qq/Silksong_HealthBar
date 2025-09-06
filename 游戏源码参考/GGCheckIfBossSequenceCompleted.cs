using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight/GG")]
public class GGCheckIfBossSequenceCompleted : FsmStateAction
{
	public FsmEvent completedEvent;

	public FsmEvent notCompletedEvent;

	public override void Reset()
	{
		completedEvent = null;
		notCompletedEvent = null;
	}

	public override void OnEnter()
	{
		if (BossSequenceController.WasCompleted)
		{
			base.Fsm.Event(completedEvent);
		}
		else
		{
			base.Fsm.Event(notCompletedEvent);
		}
		Finish();
	}
}

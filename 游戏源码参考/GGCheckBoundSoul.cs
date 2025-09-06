using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight/GG")]
public class GGCheckBoundSoul : FsmStateAction
{
	public FsmEvent boundEvent;

	public FsmEvent unboundEvent;

	public override void Reset()
	{
		boundEvent = null;
		unboundEvent = null;
	}

	public override void OnEnter()
	{
		if (BossSequenceController.IsInSequence)
		{
			if (BossSequenceController.BoundSoul)
			{
				base.Fsm.Event(boundEvent);
			}
			else
			{
				base.Fsm.Event(unboundEvent);
			}
		}
		else
		{
			base.Fsm.Event(unboundEvent);
		}
		Finish();
	}
}

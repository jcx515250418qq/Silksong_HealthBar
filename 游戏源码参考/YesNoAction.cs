using HutongGames.PlayMaker;

public abstract class YesNoAction : FsmStateAction
{
	public FsmEvent YesEvent;

	public FsmEvent NoEvent;

	public FsmBool ReturnHUDAfter;

	private bool succeeded;

	public override void Reset()
	{
		YesEvent = null;
		NoEvent = null;
		ReturnHUDAfter = null;
	}

	public override void OnEnter()
	{
		succeeded = false;
		DoOpen();
	}

	protected void SendEvent(bool isYes)
	{
		succeeded = true;
		base.Fsm.Event(isYes ? YesEvent : NoEvent);
		Finish();
	}

	public override void OnExit()
	{
		if (!succeeded)
		{
			DoForceClose();
		}
	}

	protected abstract void DoOpen();

	protected abstract void DoForceClose();
}

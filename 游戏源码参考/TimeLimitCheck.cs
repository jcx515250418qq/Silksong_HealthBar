using HutongGames.PlayMaker;
using UnityEngine;

public class TimeLimitCheck : FsmStateAction
{
	[UIHint(UIHint.Variable)]
	public FsmFloat storedValue;

	public FsmEvent aboveEvent;

	public FsmEvent belowEvent;

	public bool EveryFrame;

	public override void Reset()
	{
		storedValue = null;
		aboveEvent = null;
		belowEvent = null;
		EveryFrame = false;
	}

	public override void OnEnter()
	{
		DoAction();
		if (!EveryFrame)
		{
			Finish();
		}
	}

	public override void OnUpdate()
	{
		DoAction();
	}

	private void DoAction()
	{
		base.Fsm.Event((Time.time >= storedValue.Value) ? aboveEvent : belowEvent);
	}
}

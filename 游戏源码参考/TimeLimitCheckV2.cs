using HutongGames.PlayMaker;
using UnityEngine;

public class TimeLimitCheckV2 : FsmStateAction
{
	[UIHint(UIHint.Variable)]
	public FsmFloat StoredValue;

	public FsmEvent AboveEvent;

	public FsmEvent BelowEvent;

	[UIHint(UIHint.Variable)]
	public FsmBool StoreBool;

	public bool EveryFrame;

	public override void Reset()
	{
		StoredValue = null;
		AboveEvent = null;
		BelowEvent = null;
		StoreBool = null;
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
		bool flag = Time.time >= StoredValue.Value;
		StoreBool.Value = flag;
		if (flag)
		{
			base.Fsm.Event(AboveEvent);
			Finish();
		}
		else
		{
			base.Fsm.Event(BelowEvent);
		}
	}
}

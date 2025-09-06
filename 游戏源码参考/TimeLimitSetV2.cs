using HutongGames.PlayMaker;
using UnityEngine;

public class TimeLimitSetV2 : FsmStateAction
{
	public FsmFloat TimeDelay;

	[UIHint(UIHint.Variable)]
	public FsmFloat StoreValue;

	public FsmBool CanSet;

	public bool EveryFrame;

	public override void Reset()
	{
		TimeDelay = null;
		StoreValue = null;
		CanSet = new FsmBool
		{
			UseVariable = true
		};
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
		if (CanSet.IsNone || CanSet.Value)
		{
			StoreValue.Value = Time.time + TimeDelay.Value;
		}
	}
}

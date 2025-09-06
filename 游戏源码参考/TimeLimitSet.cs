using HutongGames.PlayMaker;
using UnityEngine;

public class TimeLimitSet : FsmStateAction
{
	public FsmFloat timeDelay;

	[UIHint(UIHint.Variable)]
	public FsmFloat storeValue;

	public override void Reset()
	{
		timeDelay = null;
		storeValue = null;
	}

	public override void OnEnter()
	{
		storeValue.Value = Time.time + timeDelay.Value;
		Finish();
	}
}

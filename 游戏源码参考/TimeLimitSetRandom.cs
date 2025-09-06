using HutongGames.PlayMaker;
using UnityEngine;

public class TimeLimitSetRandom : FsmStateAction
{
	public FsmFloat MinDelay;

	public FsmFloat MaxDelay;

	[UIHint(UIHint.Variable)]
	public FsmFloat StoreValue;

	public override void Reset()
	{
		MinDelay = null;
		MaxDelay = null;
		StoreValue = null;
	}

	public override void OnEnter()
	{
		StoreValue.Value = Time.time + Random.Range(MinDelay.Value, MaxDelay.Value);
		Finish();
	}
}

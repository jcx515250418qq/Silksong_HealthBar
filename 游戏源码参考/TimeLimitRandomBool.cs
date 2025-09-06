using HutongGames.PlayMaker;
using UnityEngine;

public class TimeLimitRandomBool : FsmStateAction
{
	public FsmFloat MinDelay;

	public FsmFloat MaxDelay;

	[UIHint(UIHint.Variable)]
	public FsmBool StoreValue;

	public FsmBool AboveValue;

	private float time;

	public override void Reset()
	{
		MinDelay = null;
		MaxDelay = null;
		StoreValue = null;
		AboveValue = true;
	}

	public override void OnEnter()
	{
		float num = Random.Range(MinDelay.Value, MaxDelay.Value);
		if (num <= 0f)
		{
			StoreValue.Value = AboveValue.Value;
			Finish();
		}
		else
		{
			time = Time.time + num;
			StoreValue.Value = !AboveValue.Value;
		}
	}

	public override void OnUpdate()
	{
		if (Time.time >= time)
		{
			StoreValue.Value = AboveValue.Value;
			Finish();
		}
		else
		{
			StoreValue.Value = !AboveValue.Value;
		}
	}
}

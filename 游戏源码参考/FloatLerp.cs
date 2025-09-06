using HutongGames.PlayMaker;
using UnityEngine;

public class FloatLerp : FsmStateAction
{
	public FsmFloat StartValue;

	public FsmFloat EndValue;

	public FsmFloat Time;

	[UIHint(UIHint.Variable)]
	public FsmFloat StoreValue;

	public override void Reset()
	{
		StartValue = null;
		EndValue = null;
		Time = null;
		StoreValue = null;
	}

	public override void OnEnter()
	{
		StoreValue.Value = Mathf.Lerp(StartValue.Value, EndValue.Value, Time.Value);
		Finish();
	}
}

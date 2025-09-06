using HutongGames.PlayMaker;

public class CheckStaticBool : FsmStateAction
{
	public FsmString variableName;

	public FsmEvent trueEvent;

	public FsmEvent falseEvent;

	public bool EveryFrame;

	public override void Reset()
	{
		variableName = null;
		trueEvent = null;
		falseEvent = null;
		EveryFrame = false;
	}

	public override void OnEnter()
	{
		DoCheck();
		if (!EveryFrame)
		{
			Finish();
		}
	}

	public override void OnUpdate()
	{
		DoCheck();
	}

	private void DoCheck()
	{
		if (!variableName.IsNone && StaticVariableList.Exists(variableName.Value) && StaticVariableList.GetValue<bool>(variableName.Value))
		{
			base.Fsm.Event(trueEvent);
		}
		else
		{
			base.Fsm.Event(falseEvent);
		}
	}
}

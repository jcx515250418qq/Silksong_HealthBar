using HutongGames.PlayMaker;

public class SetStaticVariableV2 : FsmStateAction
{
	public FsmString variableName;

	public FsmVar setValue;

	public FsmInt sceneTransitionsLimit;

	public FsmBool everyFrame;

	private object previousValue;

	public override void Reset()
	{
		variableName = null;
		setValue = null;
		sceneTransitionsLimit = null;
		everyFrame = null;
	}

	public override void OnEnter()
	{
		int num;
		if (!variableName.IsNone)
		{
			num = ((!setValue.IsNone) ? 1 : 0);
			if (num != 0)
			{
				object valueFromFsmVar = PlayMakerUtils.GetValueFromFsmVar(base.Fsm, setValue);
				StaticVariableList.SetValue(variableName.Value, valueFromFsmVar, sceneTransitionsLimit.Value);
				previousValue = valueFromFsmVar;
			}
		}
		else
		{
			num = 0;
		}
		if (num == 0 || !everyFrame.Value)
		{
			Finish();
		}
	}

	public override void OnUpdate()
	{
		object valueFromFsmVar = PlayMakerUtils.GetValueFromFsmVar(base.Fsm, setValue);
		if (!valueFromFsmVar.Equals(previousValue))
		{
			previousValue = valueFromFsmVar;
			StaticVariableList.SetValue(variableName.Value, valueFromFsmVar, sceneTransitionsLimit.Value);
		}
	}
}

using HutongGames.PlayMaker;

public class SetStaticVariable : FsmStateAction
{
	public FsmString variableName;

	public FsmVar setValue;

	public FsmInt sceneTransitionsLimit;

	public override void Reset()
	{
		variableName = null;
		setValue = null;
		sceneTransitionsLimit = null;
	}

	public override void OnEnter()
	{
		if (!variableName.IsNone && !setValue.IsNone)
		{
			object valueFromFsmVar = PlayMakerUtils.GetValueFromFsmVar(base.Fsm, setValue);
			StaticVariableList.SetValue(variableName.Value, valueFromFsmVar, sceneTransitionsLimit.Value);
		}
		Finish();
	}
}

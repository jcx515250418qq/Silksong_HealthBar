using HutongGames.PlayMaker;

public class GetStaticVariable : FsmStateAction
{
	public FsmString variableName;

	[UIHint(UIHint.Variable)]
	public FsmVar storeValue;

	public override void Reset()
	{
		variableName = null;
		storeValue = null;
	}

	public override void OnEnter()
	{
		if (!variableName.IsNone && !storeValue.IsNone && StaticVariableList.Exists(variableName.Value))
		{
			storeValue.SetValue(StaticVariableList.GetValue(variableName.Value));
		}
		Finish();
	}
}

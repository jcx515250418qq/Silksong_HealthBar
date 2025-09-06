using HutongGames.PlayMaker;

public class GetEntryGateName : FsmStateAction
{
	[UIHint(UIHint.Variable)]
	public FsmString StoreValue;

	public override void Reset()
	{
		StoreValue = null;
	}

	public override void OnEnter()
	{
		StoreValue.Value = GameManager.instance.GetEntryGateName();
		Finish();
	}
}

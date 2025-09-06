using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight")]
public class GetRespawningHero : FsmStateAction
{
	[RequiredField]
	[UIHint(UIHint.Variable)]
	public FsmBool variable;

	public override void Reset()
	{
		variable = new FsmBool();
	}

	public override void OnEnter()
	{
		if ((bool)GameManager.instance)
		{
			variable.Value = GameManager.instance.RespawningHero;
		}
		Finish();
	}
}

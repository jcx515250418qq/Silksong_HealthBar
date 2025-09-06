using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight")]
public class GetHero : FsmStateAction
{
	[UIHint(UIHint.Variable)]
	public FsmGameObject storeResult;

	public override void Reset()
	{
		base.Reset();
		storeResult = new FsmGameObject();
	}

	public override void OnEnter()
	{
		base.OnEnter();
		HeroController instance = HeroController.instance;
		storeResult.Value = ((instance != null) ? instance.gameObject : null);
		Finish();
	}
}

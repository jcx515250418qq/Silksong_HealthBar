using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight")]
public class SetRespawningHero : FsmStateAction
{
	public FsmBool value;

	public override void Reset()
	{
		value = null;
	}

	public override void OnEnter()
	{
		if ((bool)GameManager.instance)
		{
			GameManager.instance.RespawningHero = value.Value;
		}
		Finish();
	}
}

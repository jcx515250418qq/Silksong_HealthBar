using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight")]
public class DestroyPersonalPools : FsmStateAction
{
	public override void OnEnter()
	{
		if ((bool)GameManager.instance)
		{
			GameManager.instance.DoDestroyPersonalPools();
		}
		Finish();
	}
}

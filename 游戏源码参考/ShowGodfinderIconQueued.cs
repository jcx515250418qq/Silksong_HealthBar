using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight")]
public class ShowGodfinderIconQueued : FsmStateAction
{
	public FsmFloat delay;

	public override void Reset()
	{
		delay = null;
	}

	public override void OnEnter()
	{
		GodfinderIcon.ShowIconQueued(delay.Value);
		Finish();
	}
}

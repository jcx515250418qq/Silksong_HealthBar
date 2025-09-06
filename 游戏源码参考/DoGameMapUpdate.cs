using HutongGames.PlayMaker;

public sealed class DoGameMapUpdate : FsmStateAction
{
	public FsmBool silent;

	[HideIf("IsSilent")]
	public FsmFloat delay;

	public FsmEvent didUpdateEvent;

	private bool IsSilent()
	{
		return silent.Value;
	}

	public override void Reset()
	{
		silent = null;
		delay = null;
		didUpdateEvent = null;
	}

	public override void OnEnter()
	{
		GameManager instance = GameManager.instance;
		if ((bool)instance)
		{
			bool flag = false;
			if ((!silent.Value) ? instance.UpdateGameMapWithPopup(delay.Value) : instance.UpdateGameMap())
			{
				base.Fsm.Event(didUpdateEvent);
			}
		}
		Finish();
	}
}

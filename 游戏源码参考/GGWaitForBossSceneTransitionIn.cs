using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight/GG")]
public class GGWaitForBossSceneTransitionIn : FsmStateAction
{
	public FsmEvent finishEvent;

	public override void Reset()
	{
		finishEvent = null;
	}

	public override void OnEnter()
	{
		DoCheck();
	}

	public override void OnUpdate()
	{
		DoCheck();
	}

	private void DoCheck()
	{
		if ((bool)BossSceneController.Instance)
		{
			if (BossSceneController.Instance.HasTransitionedIn)
			{
				base.Fsm.Event(finishEvent);
			}
		}
		else
		{
			base.Fsm.Event(finishEvent);
		}
	}
}

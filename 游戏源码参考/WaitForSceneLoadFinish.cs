using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight")]
public class WaitForSceneLoadFinish : FsmStateAction
{
	[RequiredField]
	public FsmEvent sendEvent;

	public override void Reset()
	{
		sendEvent = null;
	}

	public override void OnEnter()
	{
		if ((bool)GameManager.instance && GameManager.instance.IsInSceneTransition)
		{
			GameManager.SceneTransitionFinishEvent temp = null;
			temp = delegate
			{
				base.Fsm.Event(sendEvent);
				GameManager.instance.OnFinishedSceneTransition -= temp;
				Finish();
			};
			GameManager.instance.OnFinishedSceneTransition += temp;
		}
		else
		{
			Finish();
		}
	}
}

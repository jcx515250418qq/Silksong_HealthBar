using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight")]
public class WaitForHeroInPosition : FsmStateAction
{
	public FsmEvent sendEvent;

	public FsmBool skipIfAlreadyPositioned;

	private bool subscribed;

	public override void Reset()
	{
		sendEvent = null;
		skipIfAlreadyPositioned = new FsmBool(false);
	}

	public override void OnEnter()
	{
		if ((bool)HeroController.instance && !HeroController.instance.isHeroInPosition)
		{
			HeroController.instance.heroInPosition += DoHeroInPosition;
			subscribed = true;
			return;
		}
		if (skipIfAlreadyPositioned.Value)
		{
			base.Fsm.Event(sendEvent);
		}
		Finish();
	}

	public override void OnExit()
	{
		if (subscribed)
		{
			HeroController silentInstance = HeroController.SilentInstance;
			if ((bool)silentInstance)
			{
				silentInstance.heroInPosition -= DoHeroInPosition;
			}
			subscribed = false;
		}
	}

	private void DoHeroInPosition(bool forceDirect)
	{
		base.Fsm.Event(sendEvent);
		Finish();
		HeroController.instance.heroInPosition -= DoHeroInPosition;
		subscribed = false;
	}
}

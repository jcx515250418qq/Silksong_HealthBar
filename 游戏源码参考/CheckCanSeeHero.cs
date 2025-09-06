using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight")]
public class CheckCanSeeHero : FsmStateAction
{
	[UIHint(UIHint.Variable)]
	public FsmBool storeResult;

	public FsmString sendEvent;

	public FsmEventTarget eventTarget;

	public bool everyFrame;

	private LineOfSightDetector source;

	public override void Reset()
	{
		storeResult = new FsmBool();
	}

	public override void OnEnter()
	{
		source = base.Owner.GetComponent<LineOfSightDetector>();
		Apply();
		if (!everyFrame)
		{
			Finish();
		}
	}

	public override void OnUpdate()
	{
		Apply();
	}

	private void Apply()
	{
		if (source != null)
		{
			if (!storeResult.IsNone)
			{
				storeResult.Value = source.CanSeeHero;
			}
			if (source.CanSeeHero)
			{
				base.Fsm.Event(eventTarget, sendEvent.Value);
			}
		}
		else if (!storeResult.IsNone)
		{
			storeResult.Value = false;
		}
	}
}

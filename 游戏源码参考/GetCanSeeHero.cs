using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight")]
public class GetCanSeeHero : FsmStateAction
{
	[UIHint(UIHint.Variable)]
	[ObjectType(typeof(LineOfSightDetector))]
	public FsmObject lineOfSightDetector;

	[UIHint(UIHint.Variable)]
	public FsmBool storeResult;

	public FsmString sendEvent;

	public FsmEventTarget eventTarget;

	public bool everyFrame;

	public override void Reset()
	{
		lineOfSightDetector = new FsmObject();
		storeResult = new FsmBool();
	}

	public override void OnEnter()
	{
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
		LineOfSightDetector lineOfSightDetector = this.lineOfSightDetector.Value as LineOfSightDetector;
		if (lineOfSightDetector != null)
		{
			if (!storeResult.IsNone)
			{
				storeResult.Value = lineOfSightDetector.CanSeeHero;
			}
			if (lineOfSightDetector.CanSeeHero)
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

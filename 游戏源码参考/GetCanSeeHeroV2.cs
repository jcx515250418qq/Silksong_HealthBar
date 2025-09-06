using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight")]
public class GetCanSeeHeroV2 : FsmStateAction
{
	[UIHint(UIHint.Variable)]
	[ObjectType(typeof(LineOfSightDetector))]
	public FsmGameObject lineOfSightDetector;

	[UIHint(UIHint.Variable)]
	public FsmBool storeResult;

	public FsmString inRangeEvent;

	public FsmString outOfRangeEvent;

	public FsmEventTarget eventTarget;

	private LineOfSightDetector detector;

	public bool everyFrame;

	public override void Reset()
	{
		lineOfSightDetector = new FsmGameObject();
		storeResult = new FsmBool();
	}

	public override void OnEnter()
	{
		Apply();
		if (!everyFrame)
		{
			Finish();
		}
		detector = lineOfSightDetector.Value.GetComponent<LineOfSightDetector>();
	}

	public override void OnUpdate()
	{
		Apply();
	}

	private void Apply()
	{
		if (detector != null)
		{
			if (!storeResult.IsNone)
			{
				storeResult.Value = detector.CanSeeHero;
			}
			if (detector.CanSeeHero)
			{
				base.Fsm.Event(eventTarget, inRangeEvent.Value);
			}
			else
			{
				base.Fsm.Event(eventTarget, outOfRangeEvent.Value);
			}
		}
		else if (!storeResult.IsNone)
		{
			storeResult.Value = false;
		}
	}
}

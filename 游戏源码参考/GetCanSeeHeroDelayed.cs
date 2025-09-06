using HutongGames.PlayMaker;
using UnityEngine;

[ActionCategory("Hollow Knight")]
public class GetCanSeeHeroDelayed : FsmStateAction
{
	[CheckForComponent(typeof(LineOfSightDetector))]
	public FsmGameObject LineOfSightDetector;

	[UIHint(UIHint.Variable)]
	public FsmBool StoreResult;

	public FsmEvent InRangeEvent;

	public FsmEvent OutOfRangeEvent;

	public FsmFloat OutOfRangeDelay;

	private float outOfRangeTimer;

	private LineOfSightDetector detector;

	public override void Reset()
	{
		LineOfSightDetector = new FsmGameObject
		{
			UseVariable = true
		};
		StoreResult = null;
		InRangeEvent = null;
		OutOfRangeEvent = null;
		OutOfRangeDelay = null;
	}

	public override void OnEnter()
	{
		detector = LineOfSightDetector.Value.GetComponent<LineOfSightDetector>();
	}

	public override void OnUpdate()
	{
		DoAction();
	}

	private void DoAction()
	{
		bool flag = detector.CanSeeHero;
		outOfRangeTimer -= Time.deltaTime;
		if (flag)
		{
			outOfRangeTimer = OutOfRangeDelay.Value;
		}
		else if (outOfRangeTimer > 0f)
		{
			flag = true;
		}
		if (detector != null)
		{
			if (!StoreResult.IsNone)
			{
				StoreResult.Value = flag;
			}
			base.Fsm.Event(flag ? InRangeEvent : OutOfRangeEvent);
		}
		else if (!StoreResult.IsNone)
		{
			StoreResult.Value = false;
		}
	}
}

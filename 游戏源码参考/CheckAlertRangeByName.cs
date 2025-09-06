using HutongGames.PlayMaker;
using UnityEngine;

[ActionCategory("Hollow Knight")]
public class CheckAlertRangeByName : FsmStateAction
{
	[UIHint(UIHint.Variable)]
	public FsmEventTarget eventTarget;

	public string alertRangeName;

	public FsmBool storeResult;

	public FsmString sendEvent;

	public FsmString outOfRangeEvent;

	public bool everyFrame;

	private GameObject alertRange_go;

	private AlertRange source;

	public override void Reset()
	{
		eventTarget = null;
		storeResult = new FsmBool();
		sendEvent = null;
	}

	public override void OnEnter()
	{
		source = AlertRange.Find(base.Owner, alertRangeName);
		if (source != null)
		{
			alertRange_go = source.gameObject;
		}
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
		if (source != null && source.gameObject.activeSelf)
		{
			if (!storeResult.IsNone)
			{
				storeResult.Value = source.IsHeroInRange();
			}
			if (!sendEvent.IsNone && source.IsHeroInRange())
			{
				base.Fsm.Event(eventTarget, sendEvent.Value);
			}
			if (!outOfRangeEvent.IsNone && !source.IsHeroInRange())
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

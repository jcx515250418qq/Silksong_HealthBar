using HutongGames.PlayMaker;
using UnityEngine;

[ActionCategory("Hollow Knight")]
public class CheckAlertRange : FsmStateAction
{
	[ObjectType(typeof(AlertRange))]
	public FsmObject alertRange;

	[UIHint(UIHint.Variable)]
	public FsmBool storeResult;

	public FsmEvent InRangeEvent;

	[HideIf("HideInRangeDelay")]
	public FsmFloat InRangeDelay;

	public FsmEvent OutOfRangeEvent;

	[HideIf("HideOutOfRangeDelay")]
	public FsmFloat OutOfRangeDelay;

	public bool everyFrame;

	private bool isCurrentlyInRange;

	private float currentRangeTimer;

	public bool HideInRangeDelay()
	{
		if (everyFrame)
		{
			return InRangeEvent == null;
		}
		return true;
	}

	public bool HideOutOfRangeDelay()
	{
		if (everyFrame)
		{
			return OutOfRangeEvent == null;
		}
		return true;
	}

	public override void OnPreprocess()
	{
		base.Fsm.HandleFixedUpdate = true;
	}

	public override void Reset()
	{
		alertRange = new FsmObject
		{
			UseVariable = true
		};
		storeResult = new FsmBool();
		InRangeEvent = null;
		InRangeDelay = new FsmFloat();
		OutOfRangeEvent = null;
		OutOfRangeDelay = new FsmFloat();
	}

	public override void OnEnter()
	{
		if (alertRange.Value == null || alertRange.IsNone)
		{
			Finish();
			return;
		}
		Apply(isFirst: true);
		if (!everyFrame)
		{
			Finish();
		}
	}

	public override void OnUpdate()
	{
		Apply(isFirst: false);
	}

	private void Apply(bool isFirst)
	{
		if (this.alertRange.Value != null)
		{
			AlertRange alertRange = this.alertRange.Value as AlertRange;
			bool flag = isCurrentlyInRange;
			isCurrentlyInRange = alertRange != null && alertRange.IsHeroInRange();
			if (isCurrentlyInRange != flag || isFirst)
			{
				currentRangeTimer = GetCurrentRangeDelay();
			}
			if (currentRangeTimer <= 0f || !everyFrame)
			{
				storeResult.Value = isCurrentlyInRange;
				base.Fsm.Event(storeResult.Value ? InRangeEvent : OutOfRangeEvent);
			}
			else
			{
				currentRangeTimer -= Time.deltaTime;
			}
		}
	}

	private float GetCurrentRangeDelay()
	{
		if (!isCurrentlyInRange)
		{
			return OutOfRangeDelay.Value;
		}
		return InRangeDelay.Value;
	}
}

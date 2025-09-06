using HutongGames.PlayMaker;
using UnityEngine;

[ActionCategory("Hollow Knight")]
public class UnalertTimer : FsmStateAction
{
	[ObjectType(typeof(AlertRange))]
	public FsmObject alertRange;

	[UIHint(UIHint.Variable)]
	public FsmFloat timerVariable;

	public FsmFloat unalertTime;

	public FsmEventTarget eventTarget;

	public FsmEvent unalertEvent;

	public bool resetOnStateEntry;

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
		timerVariable = new FsmFloat
		{
			UseVariable = true
		};
		unalertTime = null;
		eventTarget = null;
		unalertEvent = null;
	}

	public override void OnEnter()
	{
		if (alertRange.Value == null || alertRange.IsNone)
		{
			Finish();
		}
		if (resetOnStateEntry)
		{
			timerVariable = 0f;
		}
	}

	public override void OnUpdate()
	{
		if (!(alertRange.Value != null))
		{
			return;
		}
		if ((alertRange.Value as AlertRange).IsHeroInRange())
		{
			timerVariable.Value = 0f;
			return;
		}
		timerVariable.Value += Time.deltaTime;
		if (timerVariable.Value >= unalertTime.Value)
		{
			base.Fsm.Event(eventTarget, unalertEvent);
		}
	}
}

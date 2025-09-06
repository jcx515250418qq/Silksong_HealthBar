using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight")]
public class CompareUnalertTime : FsmStateAction
{
	[ObjectType(typeof(AlertRange))]
	public FsmObject alertRange;

	public FsmFloat compareTo;

	public FsmEvent lessThanOrEqualEvent;

	public FsmEvent greatherThanEvent;

	public FsmBool greatherThanBool;

	public bool everyFrame;

	public override void Reset()
	{
		alertRange = new FsmObject
		{
			UseVariable = true
		};
		compareTo = new FsmFloat();
	}

	public override void OnEnter()
	{
		if (alertRange.Value == null || alertRange.IsNone)
		{
			Finish();
		}
		DoCompare();
		if (!everyFrame)
		{
			Finish();
		}
	}

	public override void OnUpdate()
	{
		DoCompare();
	}

	private void DoCompare()
	{
		if (alertRange.Value != null)
		{
			if ((alertRange.Value as AlertRange).GetUnalertTime() <= compareTo.Value)
			{
				base.Fsm.Event(lessThanOrEqualEvent);
				greatherThanBool.Value = false;
			}
			else
			{
				base.Fsm.Event(greatherThanEvent);
				greatherThanBool.Value = true;
			}
		}
	}
}

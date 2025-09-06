using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight")]
public class GetUnalertTime : FsmStateAction
{
	[ObjectType(typeof(AlertRange))]
	public FsmObject alertRange;

	[UIHint(UIHint.Variable)]
	public FsmFloat storeTime;

	public override void Reset()
	{
		alertRange = new FsmObject
		{
			UseVariable = true
		};
		storeTime = new FsmFloat
		{
			UseVariable = true
		};
	}

	public override void OnEnter()
	{
		if (alertRange.Value == null || alertRange.IsNone)
		{
			Finish();
		}
	}

	public override void OnUpdate()
	{
		if (this.alertRange.Value != null)
		{
			AlertRange alertRange = this.alertRange.Value as AlertRange;
			storeTime.Value = alertRange.GetUnalertTime();
		}
	}
}

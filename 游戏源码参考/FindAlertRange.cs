using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight")]
public class FindAlertRange : FsmStateAction
{
	[UIHint(UIHint.Variable)]
	public FsmOwnerDefault target;

	[UIHint(UIHint.Variable)]
	[ObjectType(typeof(AlertRange))]
	public FsmObject storeResult;

	public string childName;

	public override void Reset()
	{
		target = new FsmOwnerDefault();
		storeResult = new FsmObject();
	}

	public override void OnEnter()
	{
		storeResult.Value = AlertRange.Find(target.GetSafe(this), childName);
		Finish();
	}
}

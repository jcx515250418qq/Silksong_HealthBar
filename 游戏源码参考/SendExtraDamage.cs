using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight")]
[Note("DEPRECATED")]
public class SendExtraDamage : FsmStateAction
{
	[UIHint(UIHint.Variable)]
	public FsmOwnerDefault target;

	public FsmEnum extraDamageType;

	public override void Reset()
	{
	}

	public override void OnEnter()
	{
		Finish();
	}
}

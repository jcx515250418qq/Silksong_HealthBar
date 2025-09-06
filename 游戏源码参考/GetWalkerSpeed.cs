using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight")]
public class GetWalkerSpeed : WalkerAction
{
	[UIHint(UIHint.Variable)]
	public FsmFloat WalkSpeedL;

	[UIHint(UIHint.Variable)]
	public FsmFloat WalkSpeedR;

	[UIHint(UIHint.Variable)]
	public FsmFloat WalkSpeed;

	public override void Reset()
	{
		base.Reset();
		WalkSpeedL = new FsmFloat();
		WalkSpeedR = new FsmFloat();
		WalkSpeed = new FsmFloat();
	}

	protected override void Apply(Walker walker)
	{
		WalkSpeed.Value = walker.walkSpeedR;
		WalkSpeedL.Value = walker.walkSpeedL;
		WalkSpeedR.Value = walker.walkSpeedR;
	}
}

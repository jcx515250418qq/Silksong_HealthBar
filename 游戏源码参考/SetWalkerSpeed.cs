using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight")]
public class SetWalkerSpeed : WalkerAction
{
	[HideIf("IsUsingSingleSpeed")]
	public FsmFloat WalkSpeedL;

	[HideIf("IsUsingSingleSpeed")]
	public FsmFloat WalkSpeedR;

	public FsmFloat WalkSpeed;

	public bool IsUsingSingleSpeed()
	{
		return !WalkSpeed.IsNone;
	}

	public override void Reset()
	{
		base.Reset();
		WalkSpeedL = new FsmFloat();
		WalkSpeedR = new FsmFloat();
		WalkSpeed = new FsmFloat();
	}

	protected override void Apply(Walker walker)
	{
		float walkSpeedL = WalkSpeedL.Value;
		float value = WalkSpeedR.Value;
		if (IsUsingSingleSpeed())
		{
			walkSpeedL = 0f - WalkSpeed.Value;
			value = WalkSpeed.Value;
		}
		if (!WalkSpeedL.IsNone)
		{
			walker.walkSpeedL = walkSpeedL;
		}
		if (!WalkSpeedR.IsNone)
		{
			walker.walkSpeedR = value;
		}
	}
}

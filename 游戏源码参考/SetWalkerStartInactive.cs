using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight")]
public class SetWalkerStartInactive : WalkerAction
{
	public FsmBool startInactive;

	public override void Reset()
	{
		base.Reset();
		startInactive = new FsmBool();
	}

	protected override void Apply(Walker walker)
	{
		walker.startInactive = startInactive.Value;
	}
}

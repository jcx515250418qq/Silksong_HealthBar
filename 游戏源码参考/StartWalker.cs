using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight")]
public class StartWalker : WalkerAction
{
	public FsmBool walkRight;

	public override void Reset()
	{
		base.Reset();
		walkRight = new FsmBool
		{
			UseVariable = true
		};
	}

	protected override void Apply(Walker walker)
	{
		if (walkRight.IsNone)
		{
			walker.StartMoving();
		}
		else
		{
			walker.Go(walkRight.Value ? 1 : (-1));
		}
		walker.ClearTurnCooldown();
	}
}

using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight")]
public class StopWalker : WalkerAction
{
	protected override void Apply(Walker walker)
	{
		walker.Stop(Walker.StopReasons.Controlled);
	}
}

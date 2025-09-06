using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight")]
public class StopLiftChain : LiftChainAction
{
	protected override void Apply(LiftChain liftChain)
	{
		liftChain.Stop();
	}
}

using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight")]
public class MoveLiftChain : LiftChainAction
{
	public FsmBool goUp = new FsmBool();

	protected override void Apply(LiftChain liftChain)
	{
		if (!goUp.IsNone)
		{
			if (goUp.Value)
			{
				liftChain.GoUp();
			}
			else
			{
				liftChain.GoDown();
			}
		}
	}
}

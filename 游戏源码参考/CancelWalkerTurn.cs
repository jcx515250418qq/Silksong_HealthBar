using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight")]
public class CancelWalkerTurn : WalkerAction
{
	protected override void Apply(Walker walker)
	{
		walker.CancelTurn();
	}
}

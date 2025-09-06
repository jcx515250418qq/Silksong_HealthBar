using HutongGames.PlayMaker;
using UnityEngine;

[ActionCategory("Hollow Knight")]
public class SetWalkerFacing : WalkerAction
{
	public FsmBool walkRight;

	public FsmBool randomStartDir;

	public override void Reset()
	{
		base.Reset();
		walkRight = new FsmBool
		{
			UseVariable = true
		};
		randomStartDir = new FsmBool();
	}

	protected override void Apply(Walker walker)
	{
		if (randomStartDir.Value)
		{
			walker.ChangeFacing((Random.Range(0, 2) != 0) ? 1 : (-1));
		}
		else if (!walkRight.IsNone)
		{
			walker.ChangeFacing(walkRight.Value ? 1 : (-1));
		}
	}
}

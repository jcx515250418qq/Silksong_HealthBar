using HutongGames.PlayMaker;
using UnityEngine;

[ActionCategory("Hollow Knight")]
public abstract class LiftChainAction : FsmStateAction
{
	public FsmOwnerDefault target;

	public bool everyFrame;

	private LiftChain liftChain;

	public override void Reset()
	{
		base.Reset();
		target = new FsmOwnerDefault();
		everyFrame = false;
	}

	public override void OnEnter()
	{
		base.OnEnter();
		GameObject safe = target.GetSafe(this);
		if (safe != null)
		{
			liftChain = safe.GetComponent<LiftChain>();
			if (liftChain != null)
			{
				Apply(liftChain);
			}
		}
		else
		{
			liftChain = null;
		}
		if (!everyFrame)
		{
			Finish();
		}
	}

	public override void OnUpdate()
	{
		base.OnUpdate();
		if (liftChain != null)
		{
			Apply(liftChain);
		}
	}

	protected abstract void Apply(LiftChain liftChain);
}

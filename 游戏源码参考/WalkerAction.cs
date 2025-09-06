using HutongGames.PlayMaker;
using UnityEngine;

[ActionCategory("Hollow Knight")]
public abstract class WalkerAction : FsmStateAction
{
	public FsmOwnerDefault target;

	public bool everyFrame;

	private Walker walker;

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
			walker = safe.GetComponent<Walker>();
			if (walker != null)
			{
				Apply(walker);
			}
		}
		else
		{
			walker = null;
		}
		if (!everyFrame)
		{
			Finish();
		}
	}

	public override void OnUpdate()
	{
		base.OnUpdate();
		if (walker != null)
		{
			Apply(walker);
		}
	}

	protected abstract void Apply(Walker walker);
}

using HutongGames.PlayMaker;
using UnityEngine;

[ActionCategory("Hollow Knight")]
public class VibrationPlayerPlayV2 : FsmStateAction
{
	public FsmOwnerDefault target;

	public FsmBool stopOnStateExit;

	public override void Reset()
	{
		base.Reset();
		target = new FsmOwnerDefault();
		stopOnStateExit = null;
	}

	public override void OnEnter()
	{
		base.OnEnter();
		GameObject safe = target.GetSafe(this);
		if (safe != null)
		{
			VibrationPlayer component = safe.GetComponent<VibrationPlayer>();
			if (component != null)
			{
				if (ObjectPool.IsCreatingPool)
				{
					return;
				}
				component.Play();
			}
		}
		Finish();
	}

	public override void OnExit()
	{
		if (!stopOnStateExit.Value)
		{
			return;
		}
		GameObject safe = target.GetSafe(this);
		if (safe != null)
		{
			VibrationPlayer component = safe.GetComponent<VibrationPlayer>();
			if (component != null)
			{
				component.Stop();
			}
		}
	}
}

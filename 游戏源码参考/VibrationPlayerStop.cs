using HutongGames.PlayMaker;
using UnityEngine;

[ActionCategory("Hollow Knight")]
public class VibrationPlayerStop : FsmStateAction
{
	public FsmOwnerDefault target;

	public override void Reset()
	{
		base.Reset();
		target = new FsmOwnerDefault();
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
				component.Stop();
			}
		}
		Finish();
	}
}

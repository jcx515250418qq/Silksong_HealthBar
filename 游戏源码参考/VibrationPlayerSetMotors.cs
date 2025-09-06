using HutongGames.PlayMaker;
using UnityEngine;

[ActionCategory("Hollow Knight")]
public class VibrationPlayerSetMotors : FsmStateAction
{
	public FsmOwnerDefault target;

	[ObjectType(typeof(VibrationMotors))]
	public FsmEnum motors;

	public override void Reset()
	{
		base.Reset();
		target = new FsmOwnerDefault();
		motors = new FsmEnum
		{
			Value = VibrationMotors.All
		};
	}

	public override void OnEnter()
	{
		base.OnEnter();
		GameObject safe = target.GetSafe(this);
		if (safe != null)
		{
			VibrationPlayer component = safe.GetComponent<VibrationPlayer>();
			if (component != null && !motors.IsNone)
			{
				component.Target = new VibrationTarget((VibrationMotors)(object)motors.Value);
			}
		}
		Finish();
	}
}

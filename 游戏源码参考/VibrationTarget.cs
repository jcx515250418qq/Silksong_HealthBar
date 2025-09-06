using System;
using UnityEngine;

[Serializable]
public struct VibrationTarget
{
	[SerializeField]
	private VibrationMotors motors;

	public VibrationMotors Motors => motors;

	public VibrationTarget(VibrationMotors motors)
	{
		this.motors = motors;
	}
}

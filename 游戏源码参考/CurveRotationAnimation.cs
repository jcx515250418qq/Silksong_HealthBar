using UnityEngine;

public class CurveRotationAnimation : VectorCurveAnimator
{
	protected override Vector3 Vector
	{
		get
		{
			if (space != Space.Self)
			{
				return base.CurrentTransform.eulerAngles;
			}
			return base.CurrentTransform.localEulerAngles;
		}
		set
		{
			if (space == Space.Self)
			{
				base.CurrentTransform.localEulerAngles = value;
			}
			else
			{
				base.CurrentTransform.eulerAngles = value;
			}
		}
	}
}

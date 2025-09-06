using UnityEngine;

public class CurveScaleAnimation : VectorCurveAnimator
{
	protected override Vector3 Vector
	{
		get
		{
			return base.CurrentTransform.localScale;
		}
		set
		{
			base.CurrentTransform.localScale = value;
		}
	}

	protected override bool UsesSpace()
	{
		return false;
	}
}

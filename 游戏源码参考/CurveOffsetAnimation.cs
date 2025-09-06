using UnityEngine;

public class CurveOffsetAnimation : VectorCurveAnimator
{
	protected override Vector3 Vector
	{
		get
		{
			if (space != Space.Self)
			{
				return base.CurrentTransform.position;
			}
			return base.CurrentTransform.localPosition;
		}
		set
		{
			if (space == Space.Self)
			{
				base.CurrentTransform.localPosition = value;
			}
			else
			{
				base.CurrentTransform.position = value;
			}
		}
	}
}

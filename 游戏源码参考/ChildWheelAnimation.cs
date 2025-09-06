using System;
using UnityEngine;

public class ChildWheelAnimation : MonoBehaviour
{
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private Transform trackPosition;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private Transform rotate;

	[SerializeField]
	private float rotateAmount;

	private float previousXPos;

	private float sign;

	private void Reset()
	{
		rotate = base.transform;
	}

	private void OnEnable()
	{
		if (!trackPosition || !rotate)
		{
			base.enabled = false;
			return;
		}
		previousXPos = trackPosition.localPosition.x;
		sign = Mathf.Sign(trackPosition.lossyScale.x);
	}

	private void LateUpdate()
	{
		float x = trackPosition.localPosition.x;
		float num = x - previousXPos;
		if (!(Math.Abs(num) < 0.001f))
		{
			rotate.Rotate(new Vector3(0f, 0f, num * rotateAmount * sign));
			previousXPos = x;
		}
	}
}

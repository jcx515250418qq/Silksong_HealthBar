using System;
using JetBrains.Annotations;
using TeamCherry.Splines;
using UnityEngine;

[ExecuteInEditMode]
public class PinTransformToSpline : MonoBehaviour
{
	private enum RotateBehaviours
	{
		None = 0,
		RotateWith = 1,
		RotateAlong = 2
	}

	[SerializeField]
	private SplineBase spline;

	[SerializeField]
	[Range(0f, 1f)]
	private float splinePosition;

	[SerializeField]
	private RotateBehaviours rotateBehaviour = RotateBehaviours.RotateWith;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsRotateAlong", true, true, true)]
	private float rotateAmount;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsRotate", true, true, true)]
	private float rotationOffset;

	public SplineBase Spline
	{
		get
		{
			return spline;
		}
		set
		{
			spline = value;
		}
	}

	[UsedImplicitly]
	private bool IsRotate()
	{
		return rotateBehaviour != RotateBehaviours.None;
	}

	[UsedImplicitly]
	private bool IsRotateAlong()
	{
		return rotateBehaviour == RotateBehaviours.RotateAlong;
	}

	private void LateUpdate()
	{
		if (!spline)
		{
			return;
		}
		int pointCount = spline.GetPointCount();
		if (pointCount < 1)
		{
			return;
		}
		float num = (float)pointCount * splinePosition;
		int index = Mathf.Clamp(Mathf.FloorToInt(num), 0, pointCount - 1);
		int index2 = Mathf.Clamp(Mathf.CeilToInt(num), 0, pointCount - 1);
		float t = num % 1f;
		SplineBase.Point point = spline.GetPoint(index);
		SplineBase.Point point2 = spline.GetPoint(index2);
		Transform transform = spline.transform;
		Vector3 position = Vector3.Lerp(point.Position, point2.Position, t);
		Vector3 vector = Vector3.Lerp(point.Tangent, point2.Tangent, t);
		vector = transform.TransformVector(vector).normalized;
		base.transform.SetPosition2D(transform.TransformPoint(position));
		Quaternion quaternion;
		switch (rotateBehaviour)
		{
		case RotateBehaviours.None:
			return;
		case RotateBehaviours.RotateWith:
			quaternion = Quaternion.LookRotation(Vector3.forward, -vector);
			break;
		case RotateBehaviours.RotateAlong:
		{
			float length = spline.Length;
			if (Math.Abs(length) <= Mathf.Epsilon)
			{
				return;
			}
			quaternion = Quaternion.Euler(0f, 0f, rotateAmount * splinePosition * length);
			break;
		}
		default:
			throw new ArgumentOutOfRangeException();
		}
		Quaternion rotation = transform.rotation * quaternion;
		if (Math.Abs(rotationOffset) > Mathf.Epsilon)
		{
			rotation *= Quaternion.Euler(0f, 0f, rotationOffset);
		}
		base.transform.rotation = rotation;
	}
}

using System;
using TeamCherry.Splines;
using UnityEngine;

[ExecuteInEditMode]
public class HarpoonRingPoleConfigure : MonoBehaviour
{
	[SerializeField]
	private Transform ring;

	[SerializeField]
	private QuadraticBezierSpline pole;

	[SerializeField]
	private float poleTopOffset;

	[SerializeField]
	[Range(0f, 1f)]
	private float controlPointLoc;

	[SerializeField]
	private float textureTiling;

	private void Awake()
	{
		Setup();
	}

	private void Setup()
	{
		if ((bool)ring && (bool)pole)
		{
			float x = ring.localPosition.x;
			Transform endPoint = pole.EndPoint;
			Vector3 localPosition = endPoint.localPosition;
			localPosition.x = x + poleTopOffset;
			if (Math.Abs(endPoint.localPosition.x - localPosition.x) > Mathf.Epsilon)
			{
				endPoint.localPosition = localPosition;
			}
			Transform controlPoint = pole.ControlPoint;
			Vector3 localPosition2 = controlPoint.localPosition;
			localPosition2.x = localPosition.x * controlPointLoc;
			if (Math.Abs(controlPoint.localPosition.x - localPosition2.x) > Mathf.Epsilon)
			{
				controlPoint.localPosition = localPosition2;
			}
			float num = textureTiling * localPosition.x;
			if (Math.Abs(pole.TextureTiling - num) > Mathf.Epsilon)
			{
				pole.TextureTiling = num;
			}
		}
	}
}

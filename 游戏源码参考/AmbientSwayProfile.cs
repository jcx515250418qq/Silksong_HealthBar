using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Profiles/Ambient Float")]
public class AmbientSwayProfile : ScriptableObject
{
	[Serializable]
	private struct SineCurve
	{
		public Vector3 MoveAmount;

		public float Magnitude;

		public float Offset;

		public float TimeScale;
	}

	[SerializeField]
	private Vector3 totalMoveAmount = Vector3.one;

	[SerializeField]
	private float moveSpeed;

	[SerializeField]
	[Range(-1f, 1f)]
	private float randomOffset;

	[SerializeField]
	private float fps;

	[Space]
	[SerializeField]
	private SineCurve[] sineCurves;

	public float Fps => fps;

	public Vector3 GetOffset(float time, float timeOffset)
	{
		Vector3 zero = Vector3.zero;
		time += timeOffset * randomOffset;
		SineCurve[] array = sineCurves;
		for (int i = 0; i < array.Length; i++)
		{
			SineCurve sineCurve = array[i];
			zero += sineCurve.MoveAmount * (Mathf.Sin(time * sineCurve.TimeScale * moveSpeed + sineCurve.Offset) * sineCurve.Magnitude);
		}
		if (sineCurves.Length != 0)
		{
			zero /= (float)sineCurves.Length;
		}
		return zero.MultiplyElements(totalMoveAmount);
	}
}

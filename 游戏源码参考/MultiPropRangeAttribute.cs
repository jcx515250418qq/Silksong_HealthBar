using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class MultiPropRangeAttribute : PropertyAttribute
{
	public readonly float Min;

	public readonly float Max;

	public MultiPropRangeAttribute(float min, float max)
	{
		Min = min;
		Max = max;
	}
}

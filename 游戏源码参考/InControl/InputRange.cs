using System;
using UnityEngine;

namespace InControl
{
	[Serializable]
	public struct InputRange
	{
		public static readonly InputRange None = new InputRange(0f, 0f, InputRangeType.None);

		public static readonly InputRange MinusOneToOne = new InputRange(-1f, 1f, InputRangeType.MinusOneToOne);

		public static readonly InputRange OneToMinusOne = new InputRange(1f, -1f, InputRangeType.OneToMinusOne);

		public static readonly InputRange ZeroToOne = new InputRange(0f, 1f, InputRangeType.ZeroToOne);

		public static readonly InputRange ZeroToMinusOne = new InputRange(0f, -1f, InputRangeType.ZeroToMinusOne);

		public static readonly InputRange OneToZero = new InputRange(1f, 0f, InputRangeType.OneToZero);

		public static readonly InputRange MinusOneToZero = new InputRange(-1f, 0f, InputRangeType.MinusOneToZero);

		private static readonly InputRange[] typeToRange = new InputRange[7] { None, MinusOneToOne, OneToMinusOne, ZeroToOne, ZeroToMinusOne, OneToZero, MinusOneToZero };

		public readonly float Value0;

		public readonly float Value1;

		public readonly InputRangeType Type;

		private InputRange(float value0, float value1, InputRangeType type)
		{
			Value0 = value0;
			Value1 = value1;
			Type = type;
		}

		public InputRange(InputRangeType type)
		{
			Value0 = typeToRange[(int)type].Value0;
			Value1 = typeToRange[(int)type].Value1;
			Type = type;
		}

		public bool Includes(float value)
		{
			return !Excludes(value);
		}

		private bool Excludes(float value)
		{
			if (Type == InputRangeType.None)
			{
				return true;
			}
			if (!(value < Mathf.Min(Value0, Value1)))
			{
				return value > Mathf.Max(Value0, Value1);
			}
			return true;
		}

		public static bool Excludes(InputRangeType rangeType, float value)
		{
			return typeToRange[(int)rangeType].Excludes(value);
		}

		private static float Remap(float value, InputRange sourceRange, InputRange targetRange)
		{
			if (sourceRange.Excludes(value))
			{
				return 0f;
			}
			float t = Mathf.InverseLerp(sourceRange.Value0, sourceRange.Value1, value);
			return Mathf.Lerp(targetRange.Value0, targetRange.Value1, t);
		}

		public static float Remap(float value, InputRangeType sourceRangeType, InputRangeType targetRangeType)
		{
			InputRange sourceRange = typeToRange[(int)sourceRangeType];
			InputRange targetRange = typeToRange[(int)targetRangeType];
			return Remap(value, sourceRange, targetRange);
		}
	}
}

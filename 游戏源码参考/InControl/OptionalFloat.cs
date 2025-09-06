using System;
using System.Globalization;
using UnityEngine;

namespace InControl
{
	[Serializable]
	public struct OptionalFloat
	{
		[SerializeField]
		private bool hasValue;

		[SerializeField]
		private float value;

		private const float epsilon = 1E-07f;

		public bool HasValue => hasValue;

		public bool HasNoValue => !hasValue;

		public float Value
		{
			get
			{
				if (!hasValue)
				{
					throw new OptionalTypeHasNoValueException("Trying to get a value from an OptionalFloat that has no value.");
				}
				return value;
			}
			set
			{
				this.value = value;
				hasValue = true;
			}
		}

		public OptionalFloat(float value)
		{
			this.value = value;
			hasValue = true;
		}

		public void Clear()
		{
			value = 0f;
			hasValue = false;
		}

		public float GetValueOrDefault(float defaultValue)
		{
			if (!hasValue)
			{
				return defaultValue;
			}
			return value;
		}

		public float GetValueOrZero()
		{
			if (!hasValue)
			{
				return 0f;
			}
			return value;
		}

		public void SetValue(float value)
		{
			this.value = value;
			hasValue = true;
		}

		public override bool Equals(object other)
		{
			if (other != null || hasValue)
			{
				return value.Equals(other);
			}
			return true;
		}

		public bool Equals(OptionalFloat other)
		{
			if (hasValue && other.hasValue)
			{
				return IsApproximatelyEqual(value, other.value);
			}
			return false;
		}

		public bool Equals(float other)
		{
			if (hasValue)
			{
				return IsApproximatelyEqual(value, other);
			}
			return false;
		}

		public static bool operator ==(OptionalFloat a, OptionalFloat b)
		{
			if (a.hasValue && b.hasValue)
			{
				return IsApproximatelyEqual(a.value, b.value);
			}
			return false;
		}

		public static bool operator !=(OptionalFloat a, OptionalFloat b)
		{
			return !(a == b);
		}

		public static bool operator ==(OptionalFloat a, float b)
		{
			if (a.hasValue)
			{
				return IsApproximatelyEqual(a.value, b);
			}
			return false;
		}

		public static bool operator !=(OptionalFloat a, float b)
		{
			if (a.hasValue)
			{
				return !IsApproximatelyEqual(a.value, b);
			}
			return true;
		}

		private static int CombineHashCodes(int h1, int h2)
		{
			return ((h1 << 5) + h1) ^ h2;
		}

		public override int GetHashCode()
		{
			return CombineHashCodes(hasValue.GetHashCode(), value.GetHashCode());
		}

		public override string ToString()
		{
			if (!hasValue)
			{
				return "";
			}
			return value.ToString(CultureInfo.InvariantCulture);
		}

		public static implicit operator OptionalFloat(float value)
		{
			return new OptionalFloat(value);
		}

		public static explicit operator float(OptionalFloat optional)
		{
			return optional.Value;
		}

		private static bool IsApproximatelyEqual(float a, float b)
		{
			float num = a - b;
			if (num >= -1E-07f)
			{
				return num <= 1E-07f;
			}
			return false;
		}

		public bool ApproximatelyEquals(float other)
		{
			if (!hasValue)
			{
				return false;
			}
			float num = value - other;
			if (num >= -1E-07f)
			{
				return num <= 1E-07f;
			}
			return false;
		}
	}
}

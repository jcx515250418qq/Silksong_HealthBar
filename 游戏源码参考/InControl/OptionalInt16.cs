using System;
using System.Globalization;
using UnityEngine;

namespace InControl
{
	[Serializable]
	public struct OptionalInt16
	{
		[SerializeField]
		private bool hasValue;

		[SerializeField]
		private short value;

		public bool HasValue => hasValue;

		public bool HasNoValue => !hasValue;

		public short Value
		{
			get
			{
				if (!hasValue)
				{
					throw new OptionalTypeHasNoValueException("Trying to get a value from an OptionalInt16 that has no value.");
				}
				return value;
			}
			set
			{
				this.value = value;
				hasValue = true;
			}
		}

		public OptionalInt16(short value)
		{
			this.value = value;
			hasValue = true;
		}

		public void Clear()
		{
			value = 0;
			hasValue = false;
		}

		public short GetValueOrDefault(short defaultValue)
		{
			if (!hasValue)
			{
				return defaultValue;
			}
			return value;
		}

		public short GetValueOrZero()
		{
			if (!hasValue)
			{
				return 0;
			}
			return value;
		}

		public void SetValue(short value)
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

		public bool Equals(OptionalInt16 other)
		{
			if (hasValue && other.hasValue)
			{
				return value == other.value;
			}
			return false;
		}

		public bool Equals(short other)
		{
			if (hasValue)
			{
				return value == other;
			}
			return false;
		}

		public static bool operator ==(OptionalInt16 a, OptionalInt16 b)
		{
			if (a.hasValue && b.hasValue)
			{
				return a.value == b.value;
			}
			return false;
		}

		public static bool operator !=(OptionalInt16 a, OptionalInt16 b)
		{
			return !(a == b);
		}

		public static bool operator ==(OptionalInt16 a, short b)
		{
			if (a.hasValue)
			{
				return a.value == b;
			}
			return false;
		}

		public static bool operator !=(OptionalInt16 a, short b)
		{
			if (a.hasValue)
			{
				return a.value != b;
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

		public static implicit operator OptionalInt16(short value)
		{
			return new OptionalInt16(value);
		}

		public static explicit operator short(OptionalInt16 optional)
		{
			return optional.Value;
		}
	}
}

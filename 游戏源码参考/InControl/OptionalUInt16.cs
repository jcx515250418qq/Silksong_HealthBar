using System;
using System.Globalization;
using UnityEngine;

namespace InControl
{
	[Serializable]
	public struct OptionalUInt16
	{
		[SerializeField]
		private bool hasValue;

		[SerializeField]
		private ushort value;

		public bool HasValue => hasValue;

		public bool HasNoValue => !hasValue;

		public ushort Value
		{
			get
			{
				if (!hasValue)
				{
					throw new OptionalTypeHasNoValueException("Trying to get a value from an OptionalUInt16 that has no value.");
				}
				return value;
			}
			set
			{
				this.value = value;
				hasValue = true;
			}
		}

		public OptionalUInt16(ushort value)
		{
			this.value = value;
			hasValue = true;
		}

		public void Clear()
		{
			value = 0;
			hasValue = false;
		}

		public ushort GetValueOrDefault(ushort defaultValue)
		{
			if (!hasValue)
			{
				return defaultValue;
			}
			return value;
		}

		public ushort GetValueOrZero()
		{
			if (!hasValue)
			{
				return 0;
			}
			return value;
		}

		public void SetValue(ushort value)
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

		public bool Equals(OptionalUInt16 other)
		{
			if (hasValue && other.hasValue)
			{
				return value == other.value;
			}
			return false;
		}

		public bool Equals(ushort other)
		{
			if (hasValue)
			{
				return value == other;
			}
			return false;
		}

		public static bool operator ==(OptionalUInt16 a, OptionalUInt16 b)
		{
			if (a.hasValue && b.hasValue)
			{
				return a.value == b.value;
			}
			return false;
		}

		public static bool operator !=(OptionalUInt16 a, OptionalUInt16 b)
		{
			return !(a == b);
		}

		public static bool operator ==(OptionalUInt16 a, ushort b)
		{
			if (a.hasValue)
			{
				return a.value == b;
			}
			return false;
		}

		public static bool operator !=(OptionalUInt16 a, ushort b)
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

		public static implicit operator OptionalUInt16(ushort value)
		{
			return new OptionalUInt16(value);
		}

		public static explicit operator ushort(OptionalUInt16 optional)
		{
			return optional.Value;
		}
	}
}

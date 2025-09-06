using System;
using System.Globalization;
using UnityEngine;

namespace InControl
{
	[Serializable]
	public struct OptionalUInt32
	{
		[SerializeField]
		private bool hasValue;

		[SerializeField]
		private uint value;

		public bool HasValue => hasValue;

		public bool HasNoValue => !hasValue;

		public uint Value
		{
			get
			{
				if (!hasValue)
				{
					throw new OptionalTypeHasNoValueException("Trying to get a value from an OptionalUInt32 that has no value.");
				}
				return value;
			}
			set
			{
				this.value = value;
				hasValue = true;
			}
		}

		public OptionalUInt32(uint value)
		{
			this.value = value;
			hasValue = true;
		}

		public void Clear()
		{
			value = 0u;
			hasValue = false;
		}

		public uint GetValueOrDefault(uint defaultValue)
		{
			if (!hasValue)
			{
				return defaultValue;
			}
			return value;
		}

		public uint GetValueOrZero()
		{
			if (!hasValue)
			{
				return 0u;
			}
			return value;
		}

		public void SetValue(uint value)
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

		public bool Equals(OptionalUInt32 other)
		{
			if (hasValue && other.hasValue)
			{
				return value == other.value;
			}
			return false;
		}

		public bool Equals(uint other)
		{
			if (hasValue)
			{
				return value == other;
			}
			return false;
		}

		public static bool operator ==(OptionalUInt32 a, OptionalUInt32 b)
		{
			if (a.hasValue && b.hasValue)
			{
				return a.value == b.value;
			}
			return false;
		}

		public static bool operator !=(OptionalUInt32 a, OptionalUInt32 b)
		{
			return !(a == b);
		}

		public static bool operator ==(OptionalUInt32 a, uint b)
		{
			if (a.hasValue)
			{
				return a.value == b;
			}
			return false;
		}

		public static bool operator !=(OptionalUInt32 a, uint b)
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

		public static implicit operator OptionalUInt32(uint value)
		{
			return new OptionalUInt32(value);
		}

		public static explicit operator uint(OptionalUInt32 optional)
		{
			return optional.Value;
		}
	}
}

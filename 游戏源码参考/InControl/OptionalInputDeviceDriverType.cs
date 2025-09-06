using System;
using UnityEngine;

namespace InControl
{
	[Serializable]
	public struct OptionalInputDeviceDriverType
	{
		[SerializeField]
		private bool hasValue;

		[SerializeField]
		private InputDeviceDriverType value;

		public bool HasValue => hasValue;

		public bool HasNoValue => !hasValue;

		public InputDeviceDriverType Value
		{
			get
			{
				if (!hasValue)
				{
					throw new OptionalTypeHasNoValueException("Trying to get a value from an OptionalInputDeviceDriverType that has no value.");
				}
				return value;
			}
			set
			{
				this.value = value;
				hasValue = true;
			}
		}

		public OptionalInputDeviceDriverType(InputDeviceDriverType value)
		{
			this.value = value;
			hasValue = true;
		}

		public void Clear()
		{
			value = InputDeviceDriverType.Unknown;
			hasValue = false;
		}

		public InputDeviceDriverType GetValueOrDefault(InputDeviceDriverType defaultValue)
		{
			if (!hasValue)
			{
				return defaultValue;
			}
			return value;
		}

		public InputDeviceDriverType GetValueOrZero()
		{
			if (!hasValue)
			{
				return InputDeviceDriverType.Unknown;
			}
			return value;
		}

		public void SetValue(InputDeviceDriverType value)
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

		public bool Equals(OptionalInputDeviceDriverType other)
		{
			if (hasValue && other.hasValue)
			{
				return value == other.value;
			}
			return false;
		}

		public bool Equals(InputDeviceDriverType other)
		{
			if (hasValue)
			{
				return value == other;
			}
			return false;
		}

		public static bool operator ==(OptionalInputDeviceDriverType a, OptionalInputDeviceDriverType b)
		{
			if (a.hasValue && b.hasValue)
			{
				return a.value == b.value;
			}
			return false;
		}

		public static bool operator !=(OptionalInputDeviceDriverType a, OptionalInputDeviceDriverType b)
		{
			return !(a == b);
		}

		public static bool operator ==(OptionalInputDeviceDriverType a, InputDeviceDriverType b)
		{
			if (a.hasValue)
			{
				return a.value == b;
			}
			return false;
		}

		public static bool operator !=(OptionalInputDeviceDriverType a, InputDeviceDriverType b)
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
			return value.ToString();
		}

		public static implicit operator OptionalInputDeviceDriverType(InputDeviceDriverType value)
		{
			return new OptionalInputDeviceDriverType(value);
		}

		public static explicit operator InputDeviceDriverType(OptionalInputDeviceDriverType optional)
		{
			return optional.Value;
		}
	}
}

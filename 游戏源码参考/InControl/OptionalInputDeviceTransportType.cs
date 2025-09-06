using System;
using UnityEngine;

namespace InControl
{
	[Serializable]
	public struct OptionalInputDeviceTransportType
	{
		[SerializeField]
		private bool hasValue;

		[SerializeField]
		private InputDeviceTransportType value;

		public bool HasValue => hasValue;

		public bool HasNoValue => !hasValue;

		public InputDeviceTransportType Value
		{
			get
			{
				if (!hasValue)
				{
					throw new OptionalTypeHasNoValueException("Trying to get a value from an OptionalInputDeviceTransportType that has no value.");
				}
				return value;
			}
			set
			{
				this.value = value;
				hasValue = true;
			}
		}

		public OptionalInputDeviceTransportType(InputDeviceTransportType value)
		{
			this.value = value;
			hasValue = true;
		}

		public void Clear()
		{
			value = InputDeviceTransportType.Unknown;
			hasValue = false;
		}

		public InputDeviceTransportType GetValueOrDefault(InputDeviceTransportType defaultValue)
		{
			if (!hasValue)
			{
				return defaultValue;
			}
			return value;
		}

		public InputDeviceTransportType GetValueOrZero()
		{
			if (!hasValue)
			{
				return InputDeviceTransportType.Unknown;
			}
			return value;
		}

		public void SetValue(InputDeviceTransportType value)
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

		public bool Equals(OptionalInputDeviceTransportType other)
		{
			if (hasValue && other.hasValue)
			{
				return value == other.value;
			}
			return false;
		}

		public bool Equals(InputDeviceTransportType other)
		{
			if (hasValue)
			{
				return value == other;
			}
			return false;
		}

		public static bool operator ==(OptionalInputDeviceTransportType a, OptionalInputDeviceTransportType b)
		{
			if (a.hasValue && b.hasValue)
			{
				return a.value == b.value;
			}
			return false;
		}

		public static bool operator !=(OptionalInputDeviceTransportType a, OptionalInputDeviceTransportType b)
		{
			return !(a == b);
		}

		public static bool operator ==(OptionalInputDeviceTransportType a, InputDeviceTransportType b)
		{
			if (a.hasValue)
			{
				return a.value == b;
			}
			return false;
		}

		public static bool operator !=(OptionalInputDeviceTransportType a, InputDeviceTransportType b)
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

		public static implicit operator OptionalInputDeviceTransportType(InputDeviceTransportType value)
		{
			return new OptionalInputDeviceTransportType(value);
		}

		public static explicit operator InputDeviceTransportType(OptionalInputDeviceTransportType optional)
		{
			return optional.Value;
		}
	}
}

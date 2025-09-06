using System;
using System.IO;

namespace InControl
{
	public struct UnknownDeviceControl : IEquatable<UnknownDeviceControl>
	{
		public static readonly UnknownDeviceControl None = new UnknownDeviceControl(InputControlType.None, InputRangeType.None);

		public InputControlType Control;

		public InputRangeType SourceRange;

		public bool IsButton;

		public bool IsAnalog;

		public int Index => (int)(Control - (IsButton ? 500 : 400));

		public UnknownDeviceControl(InputControlType control, InputRangeType sourceRange)
		{
			Control = control;
			SourceRange = sourceRange;
			IsButton = Utility.TargetIsButton(control);
			IsAnalog = !IsButton;
		}

		internal float GetValue(InputDevice device)
		{
			if (device == null)
			{
				return 0f;
			}
			return InputRange.Remap(device.GetControl(Control).Value, SourceRange, InputRangeType.ZeroToOne);
		}

		public static bool operator ==(UnknownDeviceControl a, UnknownDeviceControl b)
		{
			if ((object)a == null)
			{
				return (object)b == null;
			}
			return a.Equals(b);
		}

		public static bool operator !=(UnknownDeviceControl a, UnknownDeviceControl b)
		{
			return !(a == b);
		}

		public bool Equals(UnknownDeviceControl other)
		{
			if (Control == other.Control)
			{
				return SourceRange == other.SourceRange;
			}
			return false;
		}

		public override bool Equals(object other)
		{
			return Equals((UnknownDeviceControl)other);
		}

		public override int GetHashCode()
		{
			return Control.GetHashCode() ^ SourceRange.GetHashCode();
		}

		public static implicit operator bool(UnknownDeviceControl control)
		{
			return control.Control != InputControlType.None;
		}

		public override string ToString()
		{
			return $"UnknownDeviceControl( {Control.ToString()}, {SourceRange.ToString()} )";
		}

		internal void Save(BinaryWriter writer)
		{
			writer.Write((int)Control);
			writer.Write((int)SourceRange);
		}

		internal void Load(BinaryReader reader)
		{
			Control = (InputControlType)reader.ReadInt32();
			SourceRange = (InputRangeType)reader.ReadInt32();
			IsButton = Utility.TargetIsButton(Control);
			IsAnalog = !IsButton;
		}
	}
}

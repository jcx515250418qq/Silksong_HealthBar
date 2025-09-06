using System;
using System.IO;

namespace InControl
{
	public abstract class BindingSource : IEquatable<BindingSource>
	{
		public abstract string Name { get; }

		public abstract string DeviceName { get; }

		public abstract InputDeviceClass DeviceClass { get; }

		public abstract InputDeviceStyle DeviceStyle { get; }

		public abstract BindingSourceType BindingSourceType { get; }

		internal PlayerAction BoundTo { get; set; }

		internal virtual bool IsValid => true;

		public abstract float GetValue(InputDevice inputDevice);

		public abstract bool GetState(InputDevice inputDevice);

		public abstract bool Equals(BindingSource other);

		public static bool operator ==(BindingSource a, BindingSource b)
		{
			if ((object)a == b)
			{
				return true;
			}
			if ((object)a == null || (object)b == null)
			{
				return false;
			}
			if (a.BindingSourceType != b.BindingSourceType)
			{
				return false;
			}
			return a.Equals(b);
		}

		public static bool operator !=(BindingSource a, BindingSource b)
		{
			return !(a == b);
		}

		public override bool Equals(object obj)
		{
			return Equals((BindingSource)obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public abstract void Save(BinaryWriter writer);

		public abstract void Load(BinaryReader reader, ushort dataFormatVersion);
	}
}

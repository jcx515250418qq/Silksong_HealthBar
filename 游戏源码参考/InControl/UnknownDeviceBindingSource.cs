using System.IO;

namespace InControl
{
	public class UnknownDeviceBindingSource : BindingSource
	{
		public UnknownDeviceControl Control { get; protected set; }

		public override string Name
		{
			get
			{
				if (base.BoundTo == null)
				{
					return "";
				}
				string text = "";
				if (Control.SourceRange == InputRangeType.ZeroToMinusOne)
				{
					text = "Negative ";
				}
				else if (Control.SourceRange == InputRangeType.ZeroToOne)
				{
					text = "Positive ";
				}
				InputDevice device = base.BoundTo.Device;
				if (device == InputDevice.Null)
				{
					return text + Control.Control;
				}
				InputControl control = device.GetControl(Control.Control);
				if (control == InputControl.Null)
				{
					return text + Control.Control;
				}
				return text + control.Handle;
			}
		}

		public override string DeviceName
		{
			get
			{
				if (base.BoundTo == null)
				{
					return "";
				}
				InputDevice device = base.BoundTo.Device;
				if (device == InputDevice.Null)
				{
					return "Unknown Controller";
				}
				return device.Name;
			}
		}

		public override InputDeviceClass DeviceClass => InputDeviceClass.Controller;

		public override InputDeviceStyle DeviceStyle => InputDeviceStyle.Unknown;

		public override BindingSourceType BindingSourceType => BindingSourceType.UnknownDeviceBindingSource;

		internal override bool IsValid
		{
			get
			{
				if (base.BoundTo == null)
				{
					Logger.LogError("Cannot query property 'IsValid' for unbound BindingSource.");
					return false;
				}
				InputDevice device = base.BoundTo.Device;
				if (device != InputDevice.Null)
				{
					return device.HasControl(Control.Control);
				}
				return true;
			}
		}

		internal UnknownDeviceBindingSource()
		{
			Control = UnknownDeviceControl.None;
		}

		public UnknownDeviceBindingSource(UnknownDeviceControl control)
		{
			Control = control;
		}

		public override float GetValue(InputDevice device)
		{
			return Control.GetValue(device);
		}

		public override bool GetState(InputDevice device)
		{
			if (device == null)
			{
				return false;
			}
			return Utility.IsNotZero(GetValue(device));
		}

		public override bool Equals(BindingSource other)
		{
			if (other == null)
			{
				return false;
			}
			UnknownDeviceBindingSource unknownDeviceBindingSource = other as UnknownDeviceBindingSource;
			if (unknownDeviceBindingSource != null)
			{
				return Control == unknownDeviceBindingSource.Control;
			}
			return false;
		}

		public override bool Equals(object other)
		{
			if (other == null)
			{
				return false;
			}
			UnknownDeviceBindingSource unknownDeviceBindingSource = other as UnknownDeviceBindingSource;
			if (unknownDeviceBindingSource != null)
			{
				return Control == unknownDeviceBindingSource.Control;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Control.GetHashCode();
		}

		public override void Load(BinaryReader reader, ushort dataFormatVersion)
		{
			UnknownDeviceControl control = default(UnknownDeviceControl);
			control.Load(reader);
			Control = control;
		}

		public override void Save(BinaryWriter writer)
		{
			Control.Save(writer);
		}
	}
}

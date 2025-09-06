using System.IO;

namespace InControl
{
	public class KeyBindingSource : BindingSource
	{
		public KeyCombo Control { get; protected set; }

		public override string Name => Control.ToString();

		public override string DeviceName => "Keyboard";

		public override InputDeviceClass DeviceClass => InputDeviceClass.Keyboard;

		public override InputDeviceStyle DeviceStyle => InputDeviceStyle.Unknown;

		public override BindingSourceType BindingSourceType => BindingSourceType.KeyBindingSource;

		internal KeyBindingSource()
		{
		}

		public KeyBindingSource(KeyCombo keyCombo)
		{
			Control = keyCombo;
		}

		public KeyBindingSource(params Key[] keys)
		{
			Control = new KeyCombo(keys);
		}

		public override float GetValue(InputDevice inputDevice)
		{
			if (!GetState(inputDevice))
			{
				return 0f;
			}
			return 1f;
		}

		public override bool GetState(InputDevice inputDevice)
		{
			return Control.IsPressed;
		}

		public override bool Equals(BindingSource other)
		{
			if (other == null)
			{
				return false;
			}
			KeyBindingSource keyBindingSource = other as KeyBindingSource;
			if (keyBindingSource != null)
			{
				return Control == keyBindingSource.Control;
			}
			return false;
		}

		public override bool Equals(object other)
		{
			if (other == null)
			{
				return false;
			}
			KeyBindingSource keyBindingSource = other as KeyBindingSource;
			if (keyBindingSource != null)
			{
				return Control == keyBindingSource.Control;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Control.GetHashCode();
		}

		public override void Load(BinaryReader reader, ushort dataFormatVersion)
		{
			KeyCombo control = default(KeyCombo);
			control.Load(reader, dataFormatVersion);
			Control = control;
		}

		public override void Save(BinaryWriter writer)
		{
			Control.Save(writer);
		}
	}
}

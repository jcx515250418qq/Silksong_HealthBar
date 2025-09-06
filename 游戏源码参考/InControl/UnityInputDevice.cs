using UnityEngine;

namespace InControl
{
	public class UnityInputDevice : InputDevice
	{
		private static string[,] analogQueries;

		private static string[,] buttonQueries;

		public const int MaxDevices = 10;

		public const int MaxButtons = 20;

		public const int MaxAnalogs = 20;

		private readonly InputDeviceProfile profile;

		public int JoystickId { get; private set; }

		public override bool IsSupportedOnThisPlatform
		{
			get
			{
				if (profile != null)
				{
					return profile.IsSupportedOnThisPlatform;
				}
				return true;
			}
		}

		public override bool IsKnown => profile != null;

		public override int NumUnknownButtons => 20;

		public override int NumUnknownAnalogs => 20;

		public UnityInputDevice(int joystickId, string joystickName)
			: this(null, joystickId, joystickName)
		{
		}

		public UnityInputDevice(InputDeviceProfile deviceProfile, int joystickId, string joystickName)
		{
			profile = deviceProfile;
			JoystickId = joystickId;
			if (joystickId != 0)
			{
				base.SortOrder = 100 + joystickId;
			}
			SetupAnalogQueries();
			SetupButtonQueries();
			base.AnalogSnapshot = null;
			if (IsKnown)
			{
				base.Name = profile.DeviceName;
				base.Meta = profile.DeviceNotes;
				base.DeviceClass = profile.DeviceClass;
				base.DeviceStyle = profile.DeviceStyle;
				int analogCount = profile.AnalogCount;
				for (int i = 0; i < analogCount; i++)
				{
					InputControlMapping inputControlMapping = profile.AnalogMappings[i];
					if (Utility.TargetIsAlias(inputControlMapping.Target))
					{
						Logger.LogError("Cannot map control \"" + inputControlMapping.Name + "\" as InputControlType." + inputControlMapping.Target.ToString() + " in profile \"" + deviceProfile.DeviceName + "\" because this target is reserved as an alias. The mapping will be ignored.");
					}
					else
					{
						InputControl inputControl = AddControl(inputControlMapping.Target, inputControlMapping.Name);
						inputControl.Sensitivity = Mathf.Min(profile.Sensitivity, inputControlMapping.Sensitivity);
						inputControl.LowerDeadZone = Mathf.Max(profile.LowerDeadZone, inputControlMapping.LowerDeadZone);
						inputControl.UpperDeadZone = Mathf.Min(profile.UpperDeadZone, inputControlMapping.UpperDeadZone);
						inputControl.Raw = inputControlMapping.Raw;
						inputControl.Passive = inputControlMapping.Passive;
					}
				}
				int buttonCount = profile.ButtonCount;
				for (int j = 0; j < buttonCount; j++)
				{
					InputControlMapping inputControlMapping2 = profile.ButtonMappings[j];
					if (Utility.TargetIsAlias(inputControlMapping2.Target))
					{
						Logger.LogError("Cannot map control \"" + inputControlMapping2.Name + "\" as InputControlType." + inputControlMapping2.Target.ToString() + " in profile \"" + deviceProfile.DeviceName + "\" because this target is reserved as an alias. The mapping will be ignored.");
					}
					else
					{
						AddControl(inputControlMapping2.Target, inputControlMapping2.Name).Passive = inputControlMapping2.Passive;
					}
				}
			}
			else
			{
				base.Name = "Unknown Device";
				base.Meta = "\"" + joystickName + "\"";
				for (int k = 0; k < NumUnknownButtons; k++)
				{
					AddControl((InputControlType)(500 + k), "Button " + k);
				}
				for (int l = 0; l < NumUnknownAnalogs; l++)
				{
					AddControl((InputControlType)(400 + l), "Analog " + l, 0.2f, 0.9f);
				}
			}
		}

		public override void Update(ulong updateTick, float deltaTime)
		{
			if (IsKnown)
			{
				int analogCount = profile.AnalogCount;
				for (int i = 0; i < analogCount; i++)
				{
					InputControlMapping inputControlMapping = profile.AnalogMappings[i];
					float value = inputControlMapping.Source.GetValue(this);
					InputControl control = GetControl(inputControlMapping.Target);
					if (!inputControlMapping.IgnoreInitialZeroValue || !control.IsOnZeroTick || !Utility.IsZero(value))
					{
						float value2 = inputControlMapping.ApplyToValue(value);
						control.UpdateWithValue(value2, updateTick, deltaTime);
					}
				}
				int buttonCount = profile.ButtonCount;
				for (int j = 0; j < buttonCount; j++)
				{
					InputControlMapping inputControlMapping2 = profile.ButtonMappings[j];
					bool state = inputControlMapping2.Source.GetState(this);
					UpdateWithState(inputControlMapping2.Target, state, updateTick, deltaTime);
				}
			}
			else
			{
				for (int k = 0; k < NumUnknownButtons; k++)
				{
					UpdateWithState((InputControlType)(500 + k), ReadRawButtonState(k), updateTick, deltaTime);
				}
				for (int l = 0; l < NumUnknownAnalogs; l++)
				{
					UpdateWithValue((InputControlType)(400 + l), ReadRawAnalogValue(l), updateTick, deltaTime);
				}
			}
		}

		private static void SetupAnalogQueries()
		{
			if (analogQueries != null)
			{
				return;
			}
			analogQueries = new string[10, 20];
			for (int i = 1; i <= 10; i++)
			{
				for (int j = 0; j < 20; j++)
				{
					analogQueries[i - 1, j] = "joystick " + i + " analog " + j;
				}
			}
		}

		private static void SetupButtonQueries()
		{
			if (buttonQueries != null)
			{
				return;
			}
			buttonQueries = new string[10, 20];
			for (int i = 1; i <= 10; i++)
			{
				for (int j = 0; j < 20; j++)
				{
					buttonQueries[i - 1, j] = "joystick " + i + " button " + j;
				}
			}
		}

		public override bool ReadRawButtonState(int index)
		{
			if (index < 20)
			{
				return Input.GetKey(buttonQueries[JoystickId - 1, index]);
			}
			return false;
		}

		public override float ReadRawAnalogValue(int index)
		{
			if (index < 20)
			{
				return Input.GetAxisRaw(analogQueries[JoystickId - 1, index]);
			}
			return 0f;
		}
	}
}

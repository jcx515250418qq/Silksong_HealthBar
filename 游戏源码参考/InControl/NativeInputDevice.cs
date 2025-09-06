using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace InControl
{
	public class NativeInputDevice : InputDevice
	{
		private const int maxUnknownButtons = 20;

		private const int maxUnknownAnalogs = 20;

		private short[] buttons;

		private short[] analogs;

		private InputDeviceProfile profile;

		private int skipUpdateFrames;

		private int numUnknownButtons;

		private int numUnknownAnalogs;

		private InputControlSource[] controlSourceByTarget;

		private bool sendVibrate;

		private float lastTimeVibrateWasSent;

		private Vector2 vibrateToSend;

		private bool sendVibrateTriggers;

		private float lastTimeVibrateTriggersWasSent;

		private Vector2 vibrateTriggersToSend;

		private bool sendLightColor;

		private float lastTimeLightColorWasSent;

		private Vector3 lightColorToSend;

		private bool sendLightFlash;

		private float lastTimeLightFlashWasSent;

		private Vector2 lightFlashToSend;

		private readonly StringBuilder glyphName = new StringBuilder(256);

		private const string defaultGlyphName = "";

		public uint Handle { get; private set; }

		public InputDeviceInfo Info { get; private set; }

		public string ProfileName
		{
			get
			{
				if (profile != null)
				{
					return profile.GetType().Name;
				}
				return "N/A";
			}
		}

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

		public override int NumUnknownButtons => numUnknownButtons;

		public override int NumUnknownAnalogs => numUnknownAnalogs;

		internal NativeInputDevice()
		{
		}

		internal void Initialize(uint deviceHandle, InputDeviceInfo deviceInfo, InputDeviceProfile deviceProfile)
		{
			Handle = deviceHandle;
			Info = deviceInfo;
			profile = deviceProfile;
			base.SortOrder = (int)(1000 + Handle);
			numUnknownButtons = Math.Min((int)Info.numButtons, 20);
			numUnknownAnalogs = Math.Min((int)Info.numAnalogs, 20);
			buttons = new short[Info.numButtons];
			analogs = new short[Info.numAnalogs];
			base.AnalogSnapshot = null;
			controlSourceByTarget = new InputControlSource[531];
			ClearInputState();
			ClearControls();
			if (IsKnown)
			{
				base.Name = profile.DeviceName ?? Info.name;
				base.Name = base.Name.Replace("{NAME}", Info.name).Trim();
				base.Meta = profile.DeviceNotes ?? Info.name;
				base.DeviceClass = profile.DeviceClass;
				base.DeviceStyle = profile.DeviceStyle;
				int analogCount = profile.AnalogCount;
				for (int i = 0; i < analogCount; i++)
				{
					InputControlMapping inputControlMapping = profile.AnalogMappings[i];
					InputControl inputControl = AddControl(inputControlMapping.Target, inputControlMapping.Name);
					inputControl.Sensitivity = Mathf.Min(profile.Sensitivity, inputControlMapping.Sensitivity);
					inputControl.LowerDeadZone = Mathf.Max(profile.LowerDeadZone, inputControlMapping.LowerDeadZone);
					inputControl.UpperDeadZone = Mathf.Min(profile.UpperDeadZone, inputControlMapping.UpperDeadZone);
					inputControl.Raw = inputControlMapping.Raw;
					inputControl.Passive = inputControlMapping.Passive;
					controlSourceByTarget[(int)inputControlMapping.Target] = inputControlMapping.Source;
				}
				int buttonCount = profile.ButtonCount;
				for (int j = 0; j < buttonCount; j++)
				{
					InputControlMapping inputControlMapping2 = profile.ButtonMappings[j];
					AddControl(inputControlMapping2.Target, inputControlMapping2.Name).Passive = inputControlMapping2.Passive;
					controlSourceByTarget[(int)inputControlMapping2.Target] = inputControlMapping2.Source;
				}
			}
			else
			{
				base.Name = "Unknown Device";
				base.Meta = Info.name;
				for (int k = 0; k < NumUnknownButtons; k++)
				{
					AddControl((InputControlType)(500 + k), "Button " + k);
				}
				for (int l = 0; l < NumUnknownAnalogs; l++)
				{
					AddControl((InputControlType)(400 + l), "Analog " + l, 0.2f, 0.9f);
				}
			}
			skipUpdateFrames = 1;
		}

		internal void Initialize(uint deviceHandle, InputDeviceInfo deviceInfo)
		{
			Initialize(deviceHandle, deviceInfo, profile);
		}

		public override void Update(ulong updateTick, float deltaTime)
		{
			SendStatusUpdates();
			if (skipUpdateFrames > 0)
			{
				skipUpdateFrames--;
				return;
			}
			if (Native.GetDeviceState(Handle, out var deviceState))
			{
				Marshal.Copy(deviceState, buttons, 0, buttons.Length);
				deviceState = new IntPtr(deviceState.ToInt64() + buttons.Length * 2);
				Marshal.Copy(deviceState, analogs, 0, analogs.Length);
			}
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

		public override bool ReadRawButtonState(int index)
		{
			if (index < buttons.Length)
			{
				return buttons[index] > -32767;
			}
			return false;
		}

		public override float ReadRawAnalogValue(int index)
		{
			if (index < analogs.Length)
			{
				return (float)analogs[index] / 32767f;
			}
			return 0f;
		}

		private static byte FloatToByte(float value)
		{
			return (byte)(Mathf.Clamp01(value) * 255f);
		}

		public override void Vibrate(float leftSpeed, float rightSpeed)
		{
			sendVibrate = true;
			vibrateToSend = new Vector2(leftSpeed, rightSpeed);
		}

		public override void VibrateTriggers(float leftTriggerSpeed, float rightTriggerSpeed)
		{
			sendVibrateTriggers = true;
			vibrateTriggersToSend = new Vector2(leftTriggerSpeed, rightTriggerSpeed);
		}

		public override void SetLightColor(float red, float green, float blue)
		{
			sendLightColor = true;
			lightColorToSend = new Vector3(red, green, blue);
		}

		public override void SetLightFlash(float flashOnDuration, float flashOffDuration)
		{
			sendLightFlash = true;
			lightFlashToSend = new Vector2(flashOnDuration, flashOffDuration);
		}

		private void SendStatusUpdates()
		{
			if (sendVibrate && InputManager.CurrentTime - lastTimeVibrateWasSent > 0.02f)
			{
				Native.SetHapticState(Handle, FloatToByte(vibrateToSend.x), FloatToByte(vibrateToSend.y));
				sendVibrate = false;
				lastTimeVibrateWasSent = InputManager.CurrentTime;
				vibrateToSend = Vector2.zero;
			}
			if (sendVibrateTriggers && InputManager.CurrentTime - lastTimeVibrateTriggersWasSent > 0.02f)
			{
				Native.SetTriggersHapticState(Handle, FloatToByte(vibrateTriggersToSend.x), FloatToByte(vibrateTriggersToSend.y));
				sendVibrateTriggers = false;
				lastTimeVibrateTriggersWasSent = InputManager.CurrentTime;
				vibrateTriggersToSend = Vector2.zero;
			}
			if (sendLightColor && InputManager.CurrentTime - lastTimeLightColorWasSent > 0.02f)
			{
				Native.SetLightColor(Handle, FloatToByte(lightColorToSend.x), FloatToByte(lightColorToSend.y), FloatToByte(lightColorToSend.z));
				sendLightColor = false;
				lastTimeLightColorWasSent = InputManager.CurrentTime;
				lightColorToSend = Vector3.zero;
			}
			if (sendLightFlash && InputManager.CurrentTime - lastTimeLightFlashWasSent > 0.02f)
			{
				Native.SetLightFlash(Handle, FloatToByte(lightFlashToSend.x), FloatToByte(lightFlashToSend.y));
				sendLightFlash = false;
				lastTimeLightFlashWasSent = InputManager.CurrentTime;
				lightFlashToSend = Vector2.zero;
			}
		}

		public string GetAppleGlyphNameForControl(InputControlType controlType)
		{
			InputControlSource inputControlSource = controlSourceByTarget[(int)controlType];
			if (inputControlSource.SourceType != 0)
			{
				uint num;
				IntPtr zero;
				switch (inputControlSource.SourceType)
				{
				case InputControlSourceType.Button:
					num = Native.GetButtonGlyphName(Handle, (uint)inputControlSource.Index, out zero);
					break;
				case InputControlSourceType.Analog:
					num = Native.GetAnalogGlyphName(Handle, (uint)inputControlSource.Index, out zero);
					break;
				default:
					zero = IntPtr.Zero;
					num = 0u;
					break;
				}
				if (num != 0)
				{
					glyphName.Clear();
					for (int i = 0; i < num; i++)
					{
						glyphName.Append((char)Marshal.ReadByte(zero, i));
					}
					return glyphName.ToString();
				}
			}
			return "";
		}

		public bool HasSameVendorID(InputDeviceInfo deviceInfo)
		{
			return Info.HasSameVendorID(deviceInfo);
		}

		public bool HasSameProductID(InputDeviceInfo deviceInfo)
		{
			return Info.HasSameProductID(deviceInfo);
		}

		public bool HasSameVersionNumber(InputDeviceInfo deviceInfo)
		{
			return Info.HasSameVersionNumber(deviceInfo);
		}

		public bool HasSameLocation(InputDeviceInfo deviceInfo)
		{
			return Info.HasSameLocation(deviceInfo);
		}

		public bool HasSameSerialNumber(InputDeviceInfo deviceInfo)
		{
			return Info.HasSameSerialNumber(deviceInfo);
		}
	}
}

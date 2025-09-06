using UnityEngine;

namespace InControl
{
	public class PS5SimpleInputDevice : InputDevice
	{
		private class ButtonMap
		{
			public InputControlType ControlType;

			public string ButtonName;

			public string UnityKeyName;
		}

		private const float LowerDeadZone = 0.2f;

		private const float UpperDeadZone = 0.9f;

		private const int VibrationMotorMax = 255;

		public bool IsConnected => false;

		public PS5SimpleInputDevice()
			: base("DualSense™")
		{
			base.Meta = "PS5 DualSense™";
			AddControl(InputControlType.LeftStickLeft, "Left Stick Left", 0.2f, 0.9f);
			AddControl(InputControlType.LeftStickRight, "Left Stick Right", 0.2f, 0.9f);
			AddControl(InputControlType.LeftStickUp, "Left Stick Up", 0.2f, 0.9f);
			AddControl(InputControlType.LeftStickDown, "Left Stick Down", 0.2f, 0.9f);
			AddControl(InputControlType.RightStickLeft, "Right Stick Left", 0.2f, 0.9f);
			AddControl(InputControlType.RightStickRight, "Right Stick Right", 0.2f, 0.9f);
			AddControl(InputControlType.RightStickUp, "Right Stick Up", 0.2f, 0.9f);
			AddControl(InputControlType.RightStickDown, "Right Stick Down", 0.2f, 0.9f);
			AddControl(InputControlType.LeftTrigger, "Left Trigger", 0.2f, 0.9f);
			AddControl(InputControlType.RightTrigger, "Right Trigger", 0.2f, 0.9f);
			AddControl(InputControlType.DPadUp, "DPad Up", 0.2f, 0.9f);
			AddControl(InputControlType.DPadDown, "DPad Down", 0.2f, 0.9f);
			AddControl(InputControlType.DPadLeft, "DPad Left", 0.2f, 0.9f);
			AddControl(InputControlType.DPadRight, "DPad Right", 0.2f, 0.9f);
			AddControl(InputControlType.Action1, "Cross");
			AddControl(InputControlType.Action2, "Circle");
			AddControl(InputControlType.Action3, "Square");
			AddControl(InputControlType.Action4, "Triangle");
			AddControl(InputControlType.LeftBumper, "Left Bumper");
			AddControl(InputControlType.RightBumper, "Right Bumper");
			AddControl(InputControlType.LeftStickButton, "Left Stick Button");
			AddControl(InputControlType.RightStickButton, "Right Stick Button");
			AddControl(InputControlType.TouchPadButton, "Touchpad Click");
			AddControl(InputControlType.Options, "Options");
		}

		public override void Update(ulong updateTick, float deltaTime)
		{
			Commit(updateTick, deltaTime);
		}

		private static int GetNativeVibrationValue(float strength)
		{
			return Mathf.Clamp(Mathf.FloorToInt(strength * 256f), 0, 255);
		}
	}
}

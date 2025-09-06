namespace InControl
{
	public class SwitchSimpleInputDevice : InputDevice, VibrationManager.IVibrationMixerProvider
	{
		private const float LowerDeadZone = 0.2f;

		private const float UpperDeadZone = 0.9f;

		private const float AnalogStickNormalize = 3.051851E-05f;

		public bool IsConnected => false;

		public SwitchSimpleInputDevice()
			: base("Switch")
		{
			base.Meta = "JoyCon/Pro Controller";
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
			AddControl(InputControlType.Action1, "B");
			AddControl(InputControlType.Action2, "A");
			AddControl(InputControlType.Action3, "Y");
			AddControl(InputControlType.Action4, "X");
			AddControl(InputControlType.LeftBumper, "Left Bumper");
			AddControl(InputControlType.RightBumper, "Right Bumper");
			AddControl(InputControlType.LeftStickButton, "Left Stick Button");
			AddControl(InputControlType.RightStickButton, "Right Stick Button");
			AddControl(InputControlType.Select, "Minus");
			AddControl(InputControlType.Start, "Plus");
		}

		~SwitchSimpleInputDevice()
		{
		}

		public override void Update(ulong updateTick, float deltaTime)
		{
			Commit(updateTick, deltaTime);
		}

		VibrationMixer VibrationManager.IVibrationMixerProvider.GetVibrationMixer()
		{
			return null;
		}
	}
}

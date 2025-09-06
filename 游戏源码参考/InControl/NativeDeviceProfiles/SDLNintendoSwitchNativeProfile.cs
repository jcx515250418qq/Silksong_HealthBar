namespace InControl.NativeDeviceProfiles
{
	[Preserve]
	[NativeInputDeviceProfile]
	public class SDLNintendoSwitchNativeProfile : SDLControllerNativeProfile
	{
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Nintendo Switch Pro Controller";
			base.DeviceStyle = InputDeviceStyle.NintendoSwitch;
			base.Matchers = new InputDeviceMatcher[1]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.SDLController,
					VendorID = (ushort)1406
				}
			};
			base.ButtonMappings = new InputControlMapping[20]
			{
				SDLControllerNativeProfile.Action1Mapping("B"),
				SDLControllerNativeProfile.Action2Mapping("A"),
				SDLControllerNativeProfile.Action3Mapping("Y"),
				SDLControllerNativeProfile.Action4Mapping("X"),
				SDLControllerNativeProfile.LeftCommandMapping("Minus", InputControlType.Minus),
				SDLControllerNativeProfile.RightCommandMapping("Plus", InputControlType.Plus),
				SDLControllerNativeProfile.SystemMapping("Home", InputControlType.Home),
				SDLControllerNativeProfile.LeftStickButtonMapping(),
				SDLControllerNativeProfile.RightStickButtonMapping(),
				SDLControllerNativeProfile.LeftBumperMapping("L"),
				SDLControllerNativeProfile.RightBumperMapping("R"),
				SDLControllerNativeProfile.DPadUpMapping(),
				SDLControllerNativeProfile.DPadDownMapping(),
				SDLControllerNativeProfile.DPadLeftMapping(),
				SDLControllerNativeProfile.DPadRightMapping(),
				SDLControllerNativeProfile.Misc1Mapping("Capture", InputControlType.Capture),
				SDLControllerNativeProfile.Paddle1Mapping(),
				SDLControllerNativeProfile.Paddle2Mapping(),
				SDLControllerNativeProfile.Paddle3Mapping(),
				SDLControllerNativeProfile.Paddle4Mapping()
			};
			base.AnalogMappings = new InputControlMapping[16]
			{
				SDLControllerNativeProfile.LeftStickLeftMapping(),
				SDLControllerNativeProfile.LeftStickRightMapping(),
				SDLControllerNativeProfile.LeftStickUpMapping(),
				SDLControllerNativeProfile.LeftStickDownMapping(),
				SDLControllerNativeProfile.RightStickLeftMapping(),
				SDLControllerNativeProfile.RightStickRightMapping(),
				SDLControllerNativeProfile.RightStickUpMapping(),
				SDLControllerNativeProfile.RightStickDownMapping(),
				SDLControllerNativeProfile.LeftTriggerMapping("ZL"),
				SDLControllerNativeProfile.RightTriggerMapping("ZR"),
				SDLControllerNativeProfile.AccelerometerXMapping(),
				SDLControllerNativeProfile.AccelerometerYMapping(),
				SDLControllerNativeProfile.AccelerometerZMapping(),
				SDLControllerNativeProfile.GyroscopeXMapping(),
				SDLControllerNativeProfile.GyroscopeYMapping(),
				SDLControllerNativeProfile.GyroscopeZMapping()
			};
		}
	}
}

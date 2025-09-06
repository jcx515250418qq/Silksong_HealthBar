namespace InControl.UnityDeviceProfiles
{
	[Preserve]
	[UnityInputDeviceProfile]
	public class Xbox360WindowsUnityProfile : InputDeviceProfile
	{
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Xbox 360 Controller";
			base.DeviceNotes = "Xbox 360 Controller on Windows";
			base.DeviceClass = InputDeviceClass.Controller;
			base.DeviceStyle = InputDeviceStyle.Xbox360;
			base.IncludePlatforms = new string[1] { "Windows" };
			base.Matchers = new InputDeviceMatcher[31]
			{
				new InputDeviceMatcher
				{
					NameLiteral = "AIRFLO             "
				},
				new InputDeviceMatcher
				{
					NameLiteral = "AxisPad"
				},
				new InputDeviceMatcher
				{
					NameLiteral = "Controller (Afterglow Gamepad for Xbox 360)"
				},
				new InputDeviceMatcher
				{
					NameLiteral = "Controller (Batarang wired controller (XBOX))"
				},
				new InputDeviceMatcher
				{
					NameLiteral = "Controller (Gamepad for Xbox 360)"
				},
				new InputDeviceMatcher
				{
					NameLiteral = "Controller (GPX Gamepad)"
				},
				new InputDeviceMatcher
				{
					NameLiteral = "Controller (Infinity Controller 360)"
				},
				new InputDeviceMatcher
				{
					NameLiteral = "Controller (Mad Catz FPS Pro GamePad)"
				},
				new InputDeviceMatcher
				{
					NameLiteral = "Controller (MadCatz Call of Duty GamePad)"
				},
				new InputDeviceMatcher
				{
					NameLiteral = "Controller (MadCatz GamePad)"
				},
				new InputDeviceMatcher
				{
					NameLiteral = "Controller (MLG GamePad for Xbox 360)"
				},
				new InputDeviceMatcher
				{
					NameLiteral = "Controller (Razer Sabertooth Elite)"
				},
				new InputDeviceMatcher
				{
					NameLiteral = "Controller (Rock Candy Gamepad for Xbox 360)"
				},
				new InputDeviceMatcher
				{
					NameLiteral = "Controller (SL-6566)"
				},
				new InputDeviceMatcher
				{
					NameLiteral = "Controller (Xbox 360 For Windows)"
				},
				new InputDeviceMatcher
				{
					NameLiteral = "Controller (XBOX 360 For Windows)"
				},
				new InputDeviceMatcher
				{
					NameLiteral = "Controller (Xbox 360 Wireless Receiver for Windows)"
				},
				new InputDeviceMatcher
				{
					NameLiteral = "Controller (Xbox Airflo wired controller)"
				},
				new InputDeviceMatcher
				{
					NameLiteral = "Controller (XEOX Gamepad)"
				},
				new InputDeviceMatcher
				{
					NameLiteral = "Cyborg V.3 Rumble Pad"
				},
				new InputDeviceMatcher
				{
					NameLiteral = "Generic USB Joystick "
				},
				new InputDeviceMatcher
				{
					NameLiteral = "MadCatz GamePad (Controller)"
				},
				new InputDeviceMatcher
				{
					NameLiteral = "Saitek P990 Dual Analog Pad"
				},
				new InputDeviceMatcher
				{
					NameLiteral = "SL-6566 (Controller)"
				},
				new InputDeviceMatcher
				{
					NameLiteral = "USB Gamepad "
				},
				new InputDeviceMatcher
				{
					NameLiteral = "WingMan RumblePad"
				},
				new InputDeviceMatcher
				{
					NameLiteral = "XBOX 360 For Windows (Controller)"
				},
				new InputDeviceMatcher
				{
					NameLiteral = "XEOX Gamepad (Controller)"
				},
				new InputDeviceMatcher
				{
					NameLiteral = "XEQX Gamepad SL-6556-BK"
				},
				new InputDeviceMatcher
				{
					NameLiteral = "Controller (<BETOP GAME FOR WINDOWS>)"
				},
				new InputDeviceMatcher
				{
					NameLiteral = "Controller (Inno GamePad..)"
				}
			};
			base.LastResortMatchers = new InputDeviceMatcher[1]
			{
				new InputDeviceMatcher
				{
					NamePattern = "360|xbox|catz"
				}
			};
			base.ButtonMappings = new InputControlMapping[10]
			{
				new InputControlMapping
				{
					Name = "A",
					Target = InputControlType.Action1,
					Source = InputDeviceProfile.Button(0)
				},
				new InputControlMapping
				{
					Name = "B",
					Target = InputControlType.Action2,
					Source = InputDeviceProfile.Button(1)
				},
				new InputControlMapping
				{
					Name = "X",
					Target = InputControlType.Action3,
					Source = InputDeviceProfile.Button(2)
				},
				new InputControlMapping
				{
					Name = "Y",
					Target = InputControlType.Action4,
					Source = InputDeviceProfile.Button(3)
				},
				new InputControlMapping
				{
					Name = "Left Bumper",
					Target = InputControlType.LeftBumper,
					Source = InputDeviceProfile.Button(4)
				},
				new InputControlMapping
				{
					Name = "Right Bumper",
					Target = InputControlType.RightBumper,
					Source = InputDeviceProfile.Button(5)
				},
				new InputControlMapping
				{
					Name = "Left Stick Button",
					Target = InputControlType.LeftStickButton,
					Source = InputDeviceProfile.Button(8)
				},
				new InputControlMapping
				{
					Name = "Right Stick Button",
					Target = InputControlType.RightStickButton,
					Source = InputDeviceProfile.Button(9)
				},
				new InputControlMapping
				{
					Name = "Back",
					Target = InputControlType.Back,
					Source = InputDeviceProfile.Button(6)
				},
				new InputControlMapping
				{
					Name = "Start",
					Target = InputControlType.Start,
					Source = InputDeviceProfile.Button(7)
				}
			};
			base.AnalogMappings = new InputControlMapping[14]
			{
				InputDeviceProfile.LeftStickLeftMapping(0),
				InputDeviceProfile.LeftStickRightMapping(0),
				InputDeviceProfile.LeftStickUpMapping(1),
				InputDeviceProfile.LeftStickDownMapping(1),
				InputDeviceProfile.RightStickLeftMapping(3),
				InputDeviceProfile.RightStickRightMapping(3),
				InputDeviceProfile.RightStickUpMapping(4),
				InputDeviceProfile.RightStickDownMapping(4),
				InputDeviceProfile.DPadLeftMapping(5),
				InputDeviceProfile.DPadRightMapping(5),
				InputDeviceProfile.DPadUpMapping2(6),
				InputDeviceProfile.DPadDownMapping2(6),
				new InputControlMapping
				{
					Name = "Left Trigger",
					Target = InputControlType.LeftTrigger,
					Source = InputDeviceProfile.Analog(8),
					SourceRange = InputRangeType.ZeroToOne,
					TargetRange = InputRangeType.ZeroToOne
				},
				new InputControlMapping
				{
					Name = "Right Trigger",
					Target = InputControlType.RightTrigger,
					Source = InputDeviceProfile.Analog(9),
					SourceRange = InputRangeType.ZeroToOne,
					TargetRange = InputRangeType.ZeroToOne
				}
			};
		}
	}
}

namespace InControl.NativeDeviceProfiles
{
	[Preserve]
	[NativeInputDeviceProfile]
	public class SDLControllerNativeProfile : InputDeviceProfile
	{
		protected enum SDLButtonType
		{
			SDL_CONTROLLER_BUTTON_INVALID = -1,
			SDL_CONTROLLER_BUTTON_A = 0,
			SDL_CONTROLLER_BUTTON_B = 1,
			SDL_CONTROLLER_BUTTON_X = 2,
			SDL_CONTROLLER_BUTTON_Y = 3,
			SDL_CONTROLLER_BUTTON_BACK = 4,
			SDL_CONTROLLER_BUTTON_GUIDE = 5,
			SDL_CONTROLLER_BUTTON_START = 6,
			SDL_CONTROLLER_BUTTON_LEFTSTICK = 7,
			SDL_CONTROLLER_BUTTON_RIGHTSTICK = 8,
			SDL_CONTROLLER_BUTTON_LEFTSHOULDER = 9,
			SDL_CONTROLLER_BUTTON_RIGHTSHOULDER = 10,
			SDL_CONTROLLER_BUTTON_DPAD_UP = 11,
			SDL_CONTROLLER_BUTTON_DPAD_DOWN = 12,
			SDL_CONTROLLER_BUTTON_DPAD_LEFT = 13,
			SDL_CONTROLLER_BUTTON_DPAD_RIGHT = 14,
			SDL_CONTROLLER_BUTTON_MISC1 = 15,
			SDL_CONTROLLER_BUTTON_PADDLE1 = 16,
			SDL_CONTROLLER_BUTTON_PADDLE2 = 17,
			SDL_CONTROLLER_BUTTON_PADDLE3 = 18,
			SDL_CONTROLLER_BUTTON_PADDLE4 = 19,
			SDL_CONTROLLER_BUTTON_TOUCHPAD = 20,
			SDL_CONTROLLER_BUTTON_MAX = 21
		}

		public override void Define()
		{
			base.Define();
			base.DeviceName = "{NAME}";
			base.DeviceNotes = "";
			base.DeviceClass = InputDeviceClass.Controller;
			base.IncludePlatforms = new string[2] { "OS X", "Windows" };
		}

		protected static InputControlMapping Action1Mapping(string name)
		{
			return new InputControlMapping
			{
				Name = name,
				Target = InputControlType.Action1,
				Source = InputDeviceProfile.Button(0)
			};
		}

		protected static InputControlMapping Action2Mapping(string name)
		{
			return new InputControlMapping
			{
				Name = name,
				Target = InputControlType.Action2,
				Source = InputDeviceProfile.Button(1)
			};
		}

		protected static InputControlMapping Action3Mapping(string name)
		{
			return new InputControlMapping
			{
				Name = name,
				Target = InputControlType.Action3,
				Source = InputDeviceProfile.Button(2)
			};
		}

		protected static InputControlMapping Action4Mapping(string name)
		{
			return new InputControlMapping
			{
				Name = name,
				Target = InputControlType.Action4,
				Source = InputDeviceProfile.Button(3)
			};
		}

		protected static InputControlMapping LeftCommandMapping(string name, InputControlType target)
		{
			return new InputControlMapping
			{
				Name = name,
				Target = target,
				Source = InputDeviceProfile.Button(4)
			};
		}

		protected static InputControlMapping SystemMapping(string name, InputControlType target)
		{
			return new InputControlMapping
			{
				Name = name,
				Target = target,
				Source = InputDeviceProfile.Button(5)
			};
		}

		protected static InputControlMapping RightCommandMapping(string name, InputControlType target)
		{
			return new InputControlMapping
			{
				Name = name,
				Target = target,
				Source = InputDeviceProfile.Button(6)
			};
		}

		protected static InputControlMapping LeftStickButtonMapping()
		{
			return new InputControlMapping
			{
				Name = "Left Stick Button",
				Target = InputControlType.LeftStickButton,
				Source = InputDeviceProfile.Button(7)
			};
		}

		protected static InputControlMapping RightStickButtonMapping()
		{
			return new InputControlMapping
			{
				Name = "Right Stick Button",
				Target = InputControlType.RightStickButton,
				Source = InputDeviceProfile.Button(8)
			};
		}

		protected static InputControlMapping LeftBumperMapping(string name = "Left Bumper")
		{
			return new InputControlMapping
			{
				Name = name,
				Target = InputControlType.LeftBumper,
				Source = InputDeviceProfile.Button(9)
			};
		}

		protected static InputControlMapping RightBumperMapping(string name = "Right Bumper")
		{
			return new InputControlMapping
			{
				Name = name,
				Target = InputControlType.RightBumper,
				Source = InputDeviceProfile.Button(10)
			};
		}

		protected static InputControlMapping DPadUpMapping()
		{
			return new InputControlMapping
			{
				Name = "DPad Up",
				Target = InputControlType.DPadUp,
				Source = InputDeviceProfile.Button(11)
			};
		}

		protected static InputControlMapping DPadDownMapping()
		{
			return new InputControlMapping
			{
				Name = "DPad Down",
				Target = InputControlType.DPadDown,
				Source = InputDeviceProfile.Button(12)
			};
		}

		protected static InputControlMapping DPadLeftMapping()
		{
			return new InputControlMapping
			{
				Name = "DPad Left",
				Target = InputControlType.DPadLeft,
				Source = InputDeviceProfile.Button(13)
			};
		}

		protected static InputControlMapping DPadRightMapping()
		{
			return new InputControlMapping
			{
				Name = "DPad Right",
				Target = InputControlType.DPadRight,
				Source = InputDeviceProfile.Button(14)
			};
		}

		protected static InputControlMapping Misc1Mapping(string name, InputControlType target)
		{
			return new InputControlMapping
			{
				Name = name,
				Target = target,
				Source = InputDeviceProfile.Button(15)
			};
		}

		protected static InputControlMapping Paddle1Mapping()
		{
			return new InputControlMapping
			{
				Name = "Paddle 1",
				Target = InputControlType.Paddle1,
				Source = InputDeviceProfile.Button(16)
			};
		}

		protected static InputControlMapping Paddle2Mapping()
		{
			return new InputControlMapping
			{
				Name = "Paddle 2",
				Target = InputControlType.Paddle2,
				Source = InputDeviceProfile.Button(17)
			};
		}

		protected static InputControlMapping Paddle3Mapping()
		{
			return new InputControlMapping
			{
				Name = "Paddle 3",
				Target = InputControlType.Paddle3,
				Source = InputDeviceProfile.Button(18)
			};
		}

		protected static InputControlMapping Paddle4Mapping()
		{
			return new InputControlMapping
			{
				Name = "Paddle 4",
				Target = InputControlType.Paddle4,
				Source = InputDeviceProfile.Button(19)
			};
		}

		protected static InputControlMapping TouchPadButtonMapping()
		{
			return new InputControlMapping
			{
				Name = "Touch Pad Button",
				Target = InputControlType.TouchPadButton,
				Source = InputDeviceProfile.Button(20)
			};
		}

		protected static InputControlMapping LeftStickLeftMapping()
		{
			return new InputControlMapping
			{
				Name = "Left Stick Left",
				Target = InputControlType.LeftStickLeft,
				Source = InputDeviceProfile.Analog(0),
				SourceRange = InputRangeType.ZeroToMinusOne,
				TargetRange = InputRangeType.ZeroToOne
			};
		}

		protected static InputControlMapping LeftStickRightMapping()
		{
			return new InputControlMapping
			{
				Name = "Left Stick Right",
				Target = InputControlType.LeftStickRight,
				Source = InputDeviceProfile.Analog(0),
				SourceRange = InputRangeType.ZeroToOne,
				TargetRange = InputRangeType.ZeroToOne
			};
		}

		protected static InputControlMapping LeftStickUpMapping()
		{
			return new InputControlMapping
			{
				Name = "Left Stick Up",
				Target = InputControlType.LeftStickUp,
				Source = InputDeviceProfile.Analog(1),
				SourceRange = InputRangeType.ZeroToMinusOne,
				TargetRange = InputRangeType.ZeroToOne
			};
		}

		protected static InputControlMapping LeftStickDownMapping()
		{
			return new InputControlMapping
			{
				Name = "Left Stick Down",
				Target = InputControlType.LeftStickDown,
				Source = InputDeviceProfile.Analog(1),
				SourceRange = InputRangeType.ZeroToOne,
				TargetRange = InputRangeType.ZeroToOne
			};
		}

		protected static InputControlMapping RightStickLeftMapping()
		{
			return new InputControlMapping
			{
				Name = "Right Stick Left",
				Target = InputControlType.RightStickLeft,
				Source = InputDeviceProfile.Analog(2),
				SourceRange = InputRangeType.ZeroToMinusOne,
				TargetRange = InputRangeType.ZeroToOne
			};
		}

		protected static InputControlMapping RightStickRightMapping()
		{
			return new InputControlMapping
			{
				Name = "Right Stick Right",
				Target = InputControlType.RightStickRight,
				Source = InputDeviceProfile.Analog(2),
				SourceRange = InputRangeType.ZeroToOne,
				TargetRange = InputRangeType.ZeroToOne
			};
		}

		protected static InputControlMapping RightStickUpMapping()
		{
			return new InputControlMapping
			{
				Name = "Right Stick Up",
				Target = InputControlType.RightStickUp,
				Source = InputDeviceProfile.Analog(3),
				SourceRange = InputRangeType.ZeroToMinusOne,
				TargetRange = InputRangeType.ZeroToOne
			};
		}

		protected static InputControlMapping RightStickDownMapping()
		{
			return new InputControlMapping
			{
				Name = "Right Stick Down",
				Target = InputControlType.RightStickDown,
				Source = InputDeviceProfile.Analog(3),
				SourceRange = InputRangeType.ZeroToOne,
				TargetRange = InputRangeType.ZeroToOne
			};
		}

		protected static InputControlMapping LeftTriggerMapping(string name = "Left Trigger")
		{
			return new InputControlMapping
			{
				Name = name,
				Target = InputControlType.LeftTrigger,
				Source = InputDeviceProfile.Analog(4),
				SourceRange = InputRangeType.ZeroToOne,
				TargetRange = InputRangeType.ZeroToOne
			};
		}

		protected static InputControlMapping RightTriggerMapping(string name = "Right Trigger")
		{
			return new InputControlMapping
			{
				Name = name,
				Target = InputControlType.RightTrigger,
				Source = InputDeviceProfile.Analog(5),
				SourceRange = InputRangeType.ZeroToOne,
				TargetRange = InputRangeType.ZeroToOne
			};
		}

		protected static InputControlMapping AccelerometerXMapping()
		{
			return new InputControlMapping
			{
				Name = "Accelerometer X",
				Target = InputControlType.AccelerometerX,
				Source = InputDeviceProfile.Analog(6),
				SourceRange = InputRangeType.MinusOneToOne,
				TargetRange = InputRangeType.MinusOneToOne,
				Passive = true
			};
		}

		protected static InputControlMapping AccelerometerYMapping()
		{
			return new InputControlMapping
			{
				Name = "Accelerometer Y",
				Target = InputControlType.AccelerometerY,
				Source = InputDeviceProfile.Analog(7),
				SourceRange = InputRangeType.MinusOneToOne,
				TargetRange = InputRangeType.MinusOneToOne,
				Passive = true
			};
		}

		protected static InputControlMapping AccelerometerZMapping()
		{
			return new InputControlMapping
			{
				Name = "Accelerometer Z",
				Target = InputControlType.AccelerometerZ,
				Source = InputDeviceProfile.Analog(8),
				SourceRange = InputRangeType.MinusOneToOne,
				TargetRange = InputRangeType.MinusOneToOne,
				Passive = true
			};
		}

		protected static InputControlMapping GyroscopeXMapping()
		{
			return new InputControlMapping
			{
				Name = "Gyroscope X",
				Target = InputControlType.TiltX,
				Source = InputDeviceProfile.Analog(9),
				SourceRange = InputRangeType.MinusOneToOne,
				TargetRange = InputRangeType.MinusOneToOne,
				Passive = true
			};
		}

		protected static InputControlMapping GyroscopeYMapping()
		{
			return new InputControlMapping
			{
				Name = "Gyroscope Y",
				Target = InputControlType.TiltY,
				Source = InputDeviceProfile.Analog(10),
				SourceRange = InputRangeType.MinusOneToOne,
				TargetRange = InputRangeType.MinusOneToOne,
				Passive = true
			};
		}

		protected static InputControlMapping GyroscopeZMapping()
		{
			return new InputControlMapping
			{
				Name = "Gyroscope Z",
				Target = InputControlType.TiltZ,
				Source = InputDeviceProfile.Analog(11),
				SourceRange = InputRangeType.MinusOneToOne,
				TargetRange = InputRangeType.MinusOneToOne,
				Passive = true
			};
		}
	}
}

namespace InControl.UnityDeviceProfiles
{
	[Preserve]
	[UnityInputDeviceProfile]
	public class AppleTVRemoteUnityProfile : InputDeviceProfile
	{
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Apple TV Remote";
			base.DeviceNotes = "Apple TV Remote on tvOS";
			base.DeviceClass = InputDeviceClass.Remote;
			base.DeviceStyle = InputDeviceStyle.AppleMFi;
			base.IncludePlatforms = new string[1] { "AppleTV" };
			base.Matchers = new InputDeviceMatcher[1]
			{
				new InputDeviceMatcher
				{
					NamePattern = "Remote"
				}
			};
			base.LowerDeadZone = 0.05f;
			base.UpperDeadZone = 0.95f;
			base.ButtonMappings = new InputControlMapping[3]
			{
				new InputControlMapping
				{
					Name = "TouchPad Click",
					Target = InputControlType.Action1,
					Source = InputDeviceProfile.Button(14)
				},
				new InputControlMapping
				{
					Name = "Play/Pause",
					Target = InputControlType.Action2,
					Source = InputDeviceProfile.Button(15)
				},
				new InputControlMapping
				{
					Name = "Menu",
					Target = InputControlType.Menu,
					Source = InputDeviceProfile.Button(0)
				}
			};
			base.AnalogMappings = new InputControlMapping[11]
			{
				InputDeviceProfile.LeftStickLeftMapping(0),
				InputDeviceProfile.LeftStickRightMapping(0),
				InputDeviceProfile.LeftStickUpMapping(1),
				InputDeviceProfile.LeftStickDownMapping(1),
				new InputControlMapping
				{
					Name = "TouchPad X",
					Target = InputControlType.TouchPadXAxis,
					Source = InputDeviceProfile.Analog(0),
					Raw = true
				},
				new InputControlMapping
				{
					Name = "TouchPad Y",
					Target = InputControlType.TouchPadYAxis,
					Source = InputDeviceProfile.Analog(1),
					Raw = true
				},
				new InputControlMapping
				{
					Name = "Orientation X",
					Target = InputControlType.TiltX,
					Source = InputDeviceProfile.Analog(15),
					Passive = true
				},
				new InputControlMapping
				{
					Name = "Orientation Y",
					Target = InputControlType.TiltY,
					Source = InputDeviceProfile.Analog(16),
					Passive = true
				},
				new InputControlMapping
				{
					Name = "Orientation Z",
					Target = InputControlType.TiltZ,
					Source = InputDeviceProfile.Analog(17),
					Passive = true
				},
				new InputControlMapping
				{
					Name = "Acceleration X",
					Target = InputControlType.Analog0,
					Source = InputDeviceProfile.Analog(18),
					Passive = true
				},
				new InputControlMapping
				{
					Name = "Acceleration Y",
					Target = InputControlType.Analog1,
					Source = InputDeviceProfile.Analog(19),
					Passive = true
				}
			};
		}
	}
}

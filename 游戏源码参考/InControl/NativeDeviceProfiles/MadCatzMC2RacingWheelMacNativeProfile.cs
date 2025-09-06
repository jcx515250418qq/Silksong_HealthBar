namespace InControl.NativeDeviceProfiles
{
	[Preserve]
	[NativeInputDeviceProfile]
	public class MadCatzMC2RacingWheelMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		public override void Define()
		{
			base.Define();
			base.DeviceName = "MadCatz MC2 Racing Wheel";
			base.DeviceNotes = "MadCatz MC2 Racing Wheel on Mac";
			base.Matchers = new InputDeviceMatcher[1]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = (ushort)7085,
					ProductID = (ushort)61472
				}
			};
		}
	}
}

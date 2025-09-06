namespace InControl.NativeDeviceProfiles
{
	[Preserve]
	[NativeInputDeviceProfile]
	public class ThrustMasterFerrari458SpiderRacingWheelMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		public override void Define()
		{
			base.Define();
			base.DeviceName = "ThrustMaster Ferrari 458 Spider Racing Wheel";
			base.DeviceNotes = "ThrustMaster Ferrari 458 Spider Racing Wheel on Mac";
			base.Matchers = new InputDeviceMatcher[1]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = (ushort)1103,
					ProductID = (ushort)46705
				}
			};
		}
	}
}

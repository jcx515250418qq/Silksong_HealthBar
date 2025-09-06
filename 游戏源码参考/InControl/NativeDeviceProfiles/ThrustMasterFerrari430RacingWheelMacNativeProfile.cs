namespace InControl.NativeDeviceProfiles
{
	[Preserve]
	[NativeInputDeviceProfile]
	public class ThrustMasterFerrari430RacingWheelMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		public override void Define()
		{
			base.Define();
			base.DeviceName = "ThrustMaster Ferrari 430 Racing Wheel";
			base.DeviceNotes = "ThrustMaster Ferrari 430 Racing Wheel on Mac";
			base.Matchers = new InputDeviceMatcher[1]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = (ushort)1103,
					ProductID = (ushort)46683
				}
			};
		}
	}
}

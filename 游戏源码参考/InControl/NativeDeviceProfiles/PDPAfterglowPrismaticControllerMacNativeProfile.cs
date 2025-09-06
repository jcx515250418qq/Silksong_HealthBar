namespace InControl.NativeDeviceProfiles
{
	[Preserve]
	[NativeInputDeviceProfile]
	public class PDPAfterglowPrismaticControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		public override void Define()
		{
			base.Define();
			base.DeviceName = "PDP Afterglow Prismatic Controller";
			base.DeviceNotes = "PDP Afterglow Prismatic Controller on Mac";
			base.Matchers = new InputDeviceMatcher[3]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = (ushort)3695,
					ProductID = (ushort)313
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = (ushort)3695,
					ProductID = (ushort)691
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = (ushort)3695,
					ProductID = (ushort)696
				}
			};
		}
	}
}

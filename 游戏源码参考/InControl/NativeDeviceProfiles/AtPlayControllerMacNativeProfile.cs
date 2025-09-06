namespace InControl.NativeDeviceProfiles
{
	[Preserve]
	[NativeInputDeviceProfile]
	public class AtPlayControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		public override void Define()
		{
			base.Define();
			base.DeviceName = "At Play Controller";
			base.DeviceNotes = "At Play Controller on Mac";
			base.Matchers = new InputDeviceMatcher[3]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = (ushort)9414,
					ProductID = (ushort)64250
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = (ushort)9414,
					ProductID = (ushort)64251
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = (ushort)3695,
					ProductID = (ushort)690
				}
			};
		}
	}
}

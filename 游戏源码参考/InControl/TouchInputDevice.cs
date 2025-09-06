namespace InControl
{
	public class TouchInputDevice : InputDevice
	{
		public TouchInputDevice()
			: base("Touch Input Device", rawSticks: true)
		{
			base.DeviceClass = InputDeviceClass.TouchScreen;
		}
	}
}

using System.Collections.Generic;

namespace InControl
{
	public class SwitchSimpleInputDeviceManager : InputDeviceManager
	{
		private SwitchSimpleInputDevice device;

		private bool isDeviceAttached;

		public SwitchSimpleInputDevice Device => device;

		public SwitchSimpleInputDeviceManager()
		{
			device = new SwitchSimpleInputDevice();
			devices.Add(device);
			Update(0uL, 0f);
		}

		public override void Update(ulong updateTick, float deltaTime)
		{
			if (device.IsConnected != isDeviceAttached)
			{
				if (device.IsConnected)
				{
					InputManager.AttachDevice(device);
				}
				else
				{
					InputManager.DetachDevice(device);
				}
				isDeviceAttached = device.IsConnected;
			}
		}

		public static bool CheckPlatformSupport(ICollection<string> errors)
		{
			return false;
		}

		internal static bool Enable()
		{
			List<string> list = new List<string>();
			try
			{
				if (!CheckPlatformSupport(list))
				{
					return false;
				}
				InputManager.AddDeviceManager<SwitchSimpleInputDeviceManager>();
			}
			finally
			{
				foreach (string item in list)
				{
					Logger.LogError(item);
				}
			}
			return true;
		}
	}
}

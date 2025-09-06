using System.Collections.Generic;

namespace InControl
{
	public class PS4SimpleInputDeviceManager : InputDeviceManager
	{
		private PS4SimpleInputDevice device;

		private bool isDeviceAttached;

		public PS4SimpleInputDevice Device => device;

		public PS4SimpleInputDeviceManager()
		{
			device = new PS4SimpleInputDevice();
			device.CustomPlayerID = 0;
			devices.Add(device);
			Update(0uL, 0f);
		}

		public override void Update(ulong updateTick, float deltaTime)
		{
			if (!isDeviceAttached)
			{
				InputManager.AttachDevice(device);
				isDeviceAttached = true;
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
				InputManager.AddDeviceManager<PS4SimpleInputDeviceManager>();
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

using System;
using System.Collections.Generic;
using SharpDX.DirectInput;

namespace InControl
{
	public static class DirectInputManager
	{
		private static DirectInput directInput = new DirectInput();

		private static List<DeviceInstance> devices = new List<DeviceInstance>();

		private static bool init;

		public static void EnumerateDevices()
		{
			devices.Clear();
			foreach (DeviceInstance device in directInput.GetDevices(DeviceType.Gamepad, DeviceEnumerationFlags.AttachedOnly))
			{
				devices.Add(device);
			}
		}

		public static List<DeviceInstance> GetDevices()
		{
			EnumerateDevices();
			return new List<DeviceInstance>(devices);
		}

		public static DeviceInstance GetDevice(Guid deviceGuid)
		{
			EnumerateDevices();
			return devices.Find((DeviceInstance device) => device.InstanceGuid == deviceGuid);
		}
	}
}

using System;
using System.Collections.Generic;
using UnityEngine;

namespace InControl
{
	public class UnityInputDeviceManager : InputDeviceManager
	{
		private const float deviceRefreshInterval = 1f;

		private float deviceRefreshTimer;

		private readonly List<InputDeviceProfile> systemDeviceProfiles;

		private readonly List<InputDeviceProfile> customDeviceProfiles;

		private string[] joystickNames;

		private int lastJoystickCount;

		private int lastJoystickHash;

		private int joystickCount;

		private int joystickHash;

		private bool JoystickInfoHasChanged
		{
			get
			{
				if (joystickHash == lastJoystickHash)
				{
					return joystickCount != lastJoystickCount;
				}
				return true;
			}
		}

		public UnityInputDeviceManager()
		{
			systemDeviceProfiles = new List<InputDeviceProfile>(UnityInputDeviceProfileList.Profiles.Length);
			customDeviceProfiles = new List<InputDeviceProfile>();
			AddSystemDeviceProfiles();
			QueryJoystickInfo();
			AttachDevices();
		}

		public override void Update(ulong updateTick, float deltaTime)
		{
			deviceRefreshTimer += deltaTime;
			if (deviceRefreshTimer >= 1f)
			{
				deviceRefreshTimer = 0f;
				QueryJoystickInfo();
				if (JoystickInfoHasChanged)
				{
					Logger.LogInfo("Change in attached Unity joysticks detected; refreshing device list.");
					DetachDevices();
					AttachDevices();
				}
			}
		}

		private void QueryJoystickInfo()
		{
			joystickNames = Input.GetJoystickNames();
			joystickCount = joystickNames.Length;
			joystickHash = 527 + joystickCount;
			for (int i = 0; i < joystickCount; i++)
			{
				joystickHash = joystickHash * 31 + joystickNames[i].GetHashCode();
			}
		}

		private void AttachDevices()
		{
			try
			{
				for (int i = 0; i < joystickCount; i++)
				{
					DetectJoystickDevice(i + 1, joystickNames[i]);
				}
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.Message);
				Logger.LogError(ex.StackTrace);
			}
			lastJoystickCount = joystickCount;
			lastJoystickHash = joystickHash;
		}

		private void DetachDevices()
		{
			int count = devices.Count;
			for (int i = 0; i < count; i++)
			{
				InputManager.DetachDevice(devices[i]);
			}
			devices.Clear();
		}

		public void ReloadDevices()
		{
			QueryJoystickInfo();
			DetachDevices();
			AttachDevices();
		}

		private void AttachDevice(UnityInputDevice device)
		{
			devices.Add(device);
			InputManager.AttachDevice(device);
		}

		private bool HasAttachedDeviceWithJoystickId(int unityJoystickId)
		{
			int count = devices.Count;
			for (int i = 0; i < count; i++)
			{
				if (devices[i] is UnityInputDevice unityInputDevice && unityInputDevice.JoystickId == unityJoystickId)
				{
					return true;
				}
			}
			return false;
		}

		private void DetectJoystickDevice(int unityJoystickId, string unityJoystickName)
		{
			if (!HasAttachedDeviceWithJoystickId(unityJoystickId) && unityJoystickName.IndexOf("webcam", StringComparison.OrdinalIgnoreCase) == -1 && (!(InputManager.UnityVersion < new VersionInfo(4, 5, 0, 0)) || (Application.platform != 0 && Application.platform != RuntimePlatform.OSXPlayer) || !(unityJoystickName == "Unknown Wireless Controller")) && (!(InputManager.UnityVersion >= new VersionInfo(4, 6, 3, 0)) || (Application.platform != RuntimePlatform.WindowsEditor && Application.platform != RuntimePlatform.WindowsPlayer) || !string.IsNullOrEmpty(unityJoystickName)))
			{
				InputDeviceProfile inputDeviceProfile = DetectDevice(unityJoystickName);
				if (inputDeviceProfile == null)
				{
					UnityInputDevice device = new UnityInputDevice(unityJoystickId, unityJoystickName);
					AttachDevice(device);
					Logger.LogWarning("Device " + unityJoystickId + " with name \"" + unityJoystickName + "\" does not match any supported profiles and will be considered an unknown controller.");
				}
				else if (!inputDeviceProfile.IsHidden)
				{
					UnityInputDevice device2 = new UnityInputDevice(inputDeviceProfile, unityJoystickId, unityJoystickName);
					AttachDevice(device2);
					Logger.LogInfo("Device " + unityJoystickId + " matched profile " + inputDeviceProfile.GetType().Name + " (" + inputDeviceProfile.DeviceName + ")");
				}
				else
				{
					Logger.LogInfo("Device " + unityJoystickId + " matching profile " + inputDeviceProfile.GetType().Name + " (" + inputDeviceProfile.DeviceName + ") is hidden and will not be attached.");
				}
			}
		}

		private InputDeviceProfile DetectDevice(string unityJoystickName)
		{
			object obj = null;
			InputDeviceInfo deviceInfo = new InputDeviceInfo
			{
				name = unityJoystickName
			};
			if (obj == null)
			{
				obj = customDeviceProfiles.Find((InputDeviceProfile profile) => profile.Matches(deviceInfo));
			}
			if (obj == null)
			{
				obj = systemDeviceProfiles.Find((InputDeviceProfile profile) => profile.Matches(deviceInfo));
			}
			if (obj == null)
			{
				obj = customDeviceProfiles.Find((InputDeviceProfile profile) => profile.LastResortMatches(deviceInfo));
			}
			if (obj == null)
			{
				obj = systemDeviceProfiles.Find((InputDeviceProfile profile) => profile.LastResortMatches(deviceInfo));
			}
			return (InputDeviceProfile)obj;
		}

		private void AddSystemDeviceProfile(InputDeviceProfile deviceProfile)
		{
			if (deviceProfile != null && deviceProfile.IsSupportedOnThisPlatform)
			{
				systemDeviceProfiles.Add(deviceProfile);
			}
		}

		private void AddSystemDeviceProfiles()
		{
			for (int i = 0; i < UnityInputDeviceProfileList.Profiles.Length; i++)
			{
				InputDeviceProfile deviceProfile = InputDeviceProfile.CreateInstanceOfType(UnityInputDeviceProfileList.Profiles[i]);
				AddSystemDeviceProfile(deviceProfile);
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using InControl.NativeDeviceProfiles;
using UnityEngine;

namespace InControl
{
	public class NativeInputDeviceManager : InputDeviceManager
	{
		public static Func<InputDeviceInfo, ReadOnlyCollection<NativeInputDevice>, NativeInputDevice> CustomFindDetachedDevice;

		private readonly List<NativeInputDevice> attachedDevices;

		private readonly List<NativeInputDevice> detachedDevices;

		private readonly List<InputDeviceProfile> systemDeviceProfiles;

		private readonly List<InputDeviceProfile> customDeviceProfiles;

		private uint[] deviceEvents;

		public NativeInputDeviceManager()
		{
			attachedDevices = new List<NativeInputDevice>();
			detachedDevices = new List<NativeInputDevice>();
			systemDeviceProfiles = new List<InputDeviceProfile>(NativeInputDeviceProfileList.Profiles.Length);
			customDeviceProfiles = new List<InputDeviceProfile>();
			deviceEvents = new uint[32];
			AddSystemDeviceProfiles();
			NativeInputOptions options = new NativeInputOptions
			{
				enableXInput = (InputManager.NativeInputEnableXInput ? 1 : 0),
				enableMFi = (InputManager.NativeInputEnableMFi ? 1 : 0),
				preventSleep = (InputManager.NativeInputPreventSleep ? 1 : 0)
			};
			if (InputManager.NativeInputUpdateRate != 0)
			{
				options.updateRate = (ushort)InputManager.NativeInputUpdateRate;
			}
			else
			{
				options.updateRate = (ushort)Mathf.FloorToInt(1f / Time.fixedDeltaTime);
			}
			Native.Init(options);
		}

		public override void Destroy()
		{
			Native.Stop();
		}

		public override void Update(ulong updateTick, float deltaTime)
		{
			IntPtr source;
			int num = Native.GetDeviceEvents(out source);
			if (num <= 0)
			{
				return;
			}
			Utility.ArrayExpand(ref deviceEvents, num);
			MarshalUtility.Copy(source, deviceEvents, num);
			int num2 = 0;
			uint num3 = deviceEvents[num2++];
			for (int i = 0; i < num3; i++)
			{
				uint num4 = deviceEvents[num2++];
				StringBuilder stringBuilder = new StringBuilder(256);
				stringBuilder.Append("Attached native device with handle " + num4 + ":\n");
				if (Native.GetDeviceInfo(num4, out var deviceInfo))
				{
					stringBuilder.AppendFormat("Name: {0}\n", deviceInfo.name);
					stringBuilder.AppendFormat("Driver Type: {0}\n", deviceInfo.driverType);
					stringBuilder.AppendFormat("Location ID: {0}\n", deviceInfo.location);
					stringBuilder.AppendFormat("Serial Number: {0}\n", deviceInfo.serialNumber);
					stringBuilder.AppendFormat("Vendor ID: 0x{0:x}\n", deviceInfo.vendorID);
					stringBuilder.AppendFormat("Product ID: 0x{0:x}\n", deviceInfo.productID);
					stringBuilder.AppendFormat("Version Number: 0x{0:x}\n", deviceInfo.versionNumber);
					stringBuilder.AppendFormat("Buttons: {0}\n", deviceInfo.numButtons);
					stringBuilder.AppendFormat("Analogs: {0}\n", deviceInfo.numAnalogs);
					DetectDevice(num4, deviceInfo);
				}
				Logger.LogInfo(stringBuilder.ToString());
			}
			uint num5 = deviceEvents[num2++];
			for (int j = 0; j < num5; j++)
			{
				uint deviceHandle = deviceEvents[num2++];
				Logger.LogInfo("Detached native device with handle " + deviceHandle + ":");
				NativeInputDevice nativeInputDevice = FindAttachedDevice(deviceHandle);
				if (nativeInputDevice != null)
				{
					DetachDevice(nativeInputDevice);
				}
				else
				{
					Logger.LogWarning("Couldn't find device to detach with handle: " + deviceHandle);
				}
			}
		}

		private void DetectDevice(uint deviceHandle, InputDeviceInfo deviceInfo)
		{
			InputDeviceProfile inputDeviceProfile = null;
			inputDeviceProfile = inputDeviceProfile ?? customDeviceProfiles.Find((InputDeviceProfile profile) => profile.Matches(deviceInfo));
			inputDeviceProfile = inputDeviceProfile ?? systemDeviceProfiles.Find((InputDeviceProfile profile) => profile.Matches(deviceInfo));
			inputDeviceProfile = inputDeviceProfile ?? customDeviceProfiles.Find((InputDeviceProfile profile) => profile.LastResortMatches(deviceInfo));
			inputDeviceProfile = inputDeviceProfile ?? systemDeviceProfiles.Find((InputDeviceProfile profile) => profile.LastResortMatches(deviceInfo));
			if (inputDeviceProfile == null || inputDeviceProfile.IsNotHidden)
			{
				NativeInputDevice nativeInputDevice = FindDetachedDevice(deviceInfo) ?? new NativeInputDevice();
				nativeInputDevice.Initialize(deviceHandle, deviceInfo, inputDeviceProfile);
				AttachDevice(nativeInputDevice);
			}
		}

		private void AttachDevice(NativeInputDevice device)
		{
			detachedDevices.Remove(device);
			attachedDevices.Add(device);
			InputManager.AttachDevice(device);
		}

		private void DetachDevice(NativeInputDevice device)
		{
			attachedDevices.Remove(device);
			detachedDevices.Add(device);
			InputManager.DetachDevice(device);
		}

		private NativeInputDevice FindAttachedDevice(uint deviceHandle)
		{
			int count = attachedDevices.Count;
			for (int i = 0; i < count; i++)
			{
				NativeInputDevice nativeInputDevice = attachedDevices[i];
				if (nativeInputDevice.Handle == deviceHandle)
				{
					return nativeInputDevice;
				}
			}
			return null;
		}

		private NativeInputDevice FindDetachedDevice(InputDeviceInfo deviceInfo)
		{
			ReadOnlyCollection<NativeInputDevice> arg = new ReadOnlyCollection<NativeInputDevice>(detachedDevices);
			if (CustomFindDetachedDevice != null)
			{
				return CustomFindDetachedDevice(deviceInfo, arg);
			}
			return SystemFindDetachedDevice(deviceInfo, arg);
		}

		private static NativeInputDevice SystemFindDetachedDevice(InputDeviceInfo deviceInfo, ReadOnlyCollection<NativeInputDevice> detachedDevices)
		{
			int count = detachedDevices.Count;
			for (int i = 0; i < count; i++)
			{
				NativeInputDevice nativeInputDevice = detachedDevices[i];
				if (nativeInputDevice.Info.HasSameVendorID(deviceInfo) && nativeInputDevice.Info.HasSameProductID(deviceInfo) && nativeInputDevice.Info.HasSameSerialNumber(deviceInfo))
				{
					return nativeInputDevice;
				}
			}
			for (int j = 0; j < count; j++)
			{
				NativeInputDevice nativeInputDevice2 = detachedDevices[j];
				if (nativeInputDevice2.Info.HasSameVendorID(deviceInfo) && nativeInputDevice2.Info.HasSameProductID(deviceInfo) && nativeInputDevice2.Info.HasSameLocation(deviceInfo))
				{
					return nativeInputDevice2;
				}
			}
			for (int k = 0; k < count; k++)
			{
				NativeInputDevice nativeInputDevice3 = detachedDevices[k];
				if (nativeInputDevice3.Info.HasSameVendorID(deviceInfo) && nativeInputDevice3.Info.HasSameProductID(deviceInfo) && nativeInputDevice3.Info.HasSameVersionNumber(deviceInfo))
				{
					return nativeInputDevice3;
				}
			}
			for (int l = 0; l < count; l++)
			{
				NativeInputDevice nativeInputDevice4 = detachedDevices[l];
				if (nativeInputDevice4.Info.HasSameLocation(deviceInfo))
				{
					return nativeInputDevice4;
				}
			}
			return null;
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
			for (int i = 0; i < NativeInputDeviceProfileList.Profiles.Length; i++)
			{
				InputDeviceProfile deviceProfile = InputDeviceProfile.CreateInstanceOfType(NativeInputDeviceProfileList.Profiles[i]);
				AddSystemDeviceProfile(deviceProfile);
			}
		}

		public static bool CheckPlatformSupport(ICollection<string> errors)
		{
			if (Application.platform != RuntimePlatform.OSXPlayer && Application.platform != 0 && Application.platform != RuntimePlatform.WindowsPlayer && Application.platform != RuntimePlatform.WindowsEditor && Application.platform != RuntimePlatform.IPhonePlayer && Application.platform != RuntimePlatform.tvOS)
			{
				return false;
			}
			try
			{
				Native.GetVersionInfo(out var versionInfo);
				Logger.LogInfo("InControl Native (version " + versionInfo.major + "." + versionInfo.minor + "." + versionInfo.patch + ")");
			}
			catch (DllNotFoundException ex)
			{
				errors?.Add(ex.Message + Utility.PluginFileExtension() + " could not be found or is missing a dependency.");
				return false;
			}
			return true;
		}

		internal static bool Enable()
		{
			List<string> list = new List<string>();
			if (CheckPlatformSupport(list))
			{
				if (InputManager.NativeInputEnableMFi)
				{
					InputManager.HideDevicesWithProfile(typeof(XboxOneSBluetoothMacNativeProfile));
					InputManager.HideDevicesWithProfile(typeof(XboxSeriesXBluetoothMacNativeProfile));
					InputManager.HideDevicesWithProfile(typeof(PlayStation4MacNativeProfile));
					InputManager.HideDevicesWithProfile(typeof(PlayStation5USBMacNativeProfile));
					InputManager.HideDevicesWithProfile(typeof(PlayStation5BluetoothMacNativeProfile));
					InputManager.HideDevicesWithProfile(typeof(SteelseriesNimbusMacNativeProfile));
					InputManager.HideDevicesWithProfile(typeof(HoriPadUltimateMacNativeProfile));
					InputManager.HideDevicesWithProfile(typeof(NintendoSwitchProMacNativeProfile));
				}
				InputManager.AddDeviceManager<NativeInputDeviceManager>();
				return true;
			}
			foreach (string item in list)
			{
				Logger.LogError("Error enabling NativeInputDeviceManager: " + item);
			}
			return false;
		}
	}
}

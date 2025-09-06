using System.Runtime.InteropServices;

namespace InControl
{
	public struct InputDeviceInfo
	{
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string name;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string location;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
		public string serialNumber;

		public ushort vendorID;

		public ushort productID;

		public uint versionNumber;

		public InputDeviceDriverType driverType;

		public InputDeviceTransportType transportType;

		public uint numButtons;

		public uint numAnalogs;

		public bool HasSameVendorID(InputDeviceInfo deviceInfo)
		{
			return vendorID == deviceInfo.vendorID;
		}

		public bool HasSameProductID(InputDeviceInfo deviceInfo)
		{
			return productID == deviceInfo.productID;
		}

		public bool HasSameVersionNumber(InputDeviceInfo deviceInfo)
		{
			return versionNumber == deviceInfo.versionNumber;
		}

		public bool HasSameLocation(InputDeviceInfo deviceInfo)
		{
			if (string.IsNullOrEmpty(location))
			{
				return false;
			}
			return location == deviceInfo.location;
		}

		public bool HasSameSerialNumber(InputDeviceInfo deviceInfo)
		{
			if (string.IsNullOrEmpty(serialNumber))
			{
				return false;
			}
			return serialNumber == deviceInfo.serialNumber;
		}
	}
}

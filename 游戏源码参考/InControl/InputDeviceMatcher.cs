using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace InControl
{
	[Serializable]
	public struct InputDeviceMatcher
	{
		[SerializeField]
		[Hexadecimal]
		private OptionalUInt16 vendorID;

		[SerializeField]
		private OptionalUInt16 productID;

		[SerializeField]
		[Hexadecimal]
		private OptionalUInt32 versionNumber;

		[SerializeField]
		private OptionalInputDeviceDriverType driverType;

		[SerializeField]
		private OptionalInputDeviceTransportType transportType;

		[SerializeField]
		private string nameLiteral;

		[SerializeField]
		private string namePattern;

		public OptionalUInt16 VendorID
		{
			get
			{
				return vendorID;
			}
			set
			{
				vendorID = value;
			}
		}

		public OptionalUInt16 ProductID
		{
			get
			{
				return productID;
			}
			set
			{
				productID = value;
			}
		}

		public OptionalUInt32 VersionNumber
		{
			get
			{
				return versionNumber;
			}
			set
			{
				versionNumber = value;
			}
		}

		public OptionalInputDeviceDriverType DriverType
		{
			get
			{
				return driverType;
			}
			set
			{
				driverType = value;
			}
		}

		public OptionalInputDeviceTransportType TransportType
		{
			get
			{
				return transportType;
			}
			set
			{
				transportType = value;
			}
		}

		public string NameLiteral
		{
			get
			{
				return nameLiteral;
			}
			set
			{
				nameLiteral = value;
			}
		}

		public string NamePattern
		{
			get
			{
				return namePattern;
			}
			set
			{
				namePattern = value;
			}
		}

		internal bool Matches(InputDeviceInfo deviceInfo)
		{
			if (VendorID.HasValue && VendorID.Value != deviceInfo.vendorID)
			{
				return false;
			}
			if (ProductID.HasValue && ProductID.Value != deviceInfo.productID)
			{
				return false;
			}
			if (VersionNumber.HasValue && VersionNumber.Value != deviceInfo.versionNumber)
			{
				return false;
			}
			if (DriverType.HasValue && DriverType.Value != deviceInfo.driverType)
			{
				return false;
			}
			if (TransportType.HasValue && TransportType.Value != deviceInfo.transportType)
			{
				return false;
			}
			if (NameLiteral != null && !string.Equals(deviceInfo.name, NameLiteral, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			if (NamePattern != null && !Regex.IsMatch(deviceInfo.name, NamePattern, RegexOptions.IgnoreCase))
			{
				return false;
			}
			return true;
		}
	}
}

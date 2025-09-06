using System;
using System.Collections.Generic;
using UnityEngine;

namespace InControl
{
	[Serializable]
	[Preserve]
	public class InputDeviceProfile
	{
		private static readonly HashSet<Type> hiddenProfiles = new HashSet<Type>();

		[SerializeField]
		private InputDeviceProfileType profileType;

		[SerializeField]
		private string deviceName = "";

		[SerializeField]
		[TextArea]
		private string deviceNotes = "";

		[SerializeField]
		private InputDeviceClass deviceClass;

		[SerializeField]
		private InputDeviceStyle deviceStyle;

		[SerializeField]
		private float sensitivity = 1f;

		[SerializeField]
		private float lowerDeadZone = 0.2f;

		[SerializeField]
		private float upperDeadZone = 0.9f;

		[SerializeField]
		private string[] includePlatforms = new string[0];

		[SerializeField]
		private string[] excludePlatforms = new string[0];

		[SerializeField]
		private int minSystemBuildNumber;

		[SerializeField]
		private int maxSystemBuildNumber;

		[SerializeField]
		private VersionInfo minUnityVersion = VersionInfo.Min;

		[SerializeField]
		private VersionInfo maxUnityVersion = VersionInfo.Max;

		[SerializeField]
		private InputDeviceMatcher[] matchers = new InputDeviceMatcher[0];

		[SerializeField]
		private InputDeviceMatcher[] lastResortMatchers = new InputDeviceMatcher[0];

		[SerializeField]
		private InputControlMapping[] analogMappings = new InputControlMapping[0];

		[SerializeField]
		private InputControlMapping[] buttonMappings = new InputControlMapping[0];

		protected static readonly InputControlSource MenuKey = new InputControlSource(KeyCode.Menu);

		protected static readonly InputControlSource EscapeKey = new InputControlSource(KeyCode.Escape);

		public InputDeviceProfileType ProfileType
		{
			get
			{
				return profileType;
			}
			protected set
			{
				profileType = value;
			}
		}

		public string DeviceName
		{
			get
			{
				return deviceName;
			}
			protected set
			{
				deviceName = value;
			}
		}

		public string DeviceNotes
		{
			get
			{
				return deviceNotes;
			}
			protected set
			{
				deviceNotes = value;
			}
		}

		public InputDeviceClass DeviceClass
		{
			get
			{
				return deviceClass;
			}
			protected set
			{
				deviceClass = value;
			}
		}

		public InputDeviceStyle DeviceStyle
		{
			get
			{
				return deviceStyle;
			}
			protected set
			{
				deviceStyle = value;
			}
		}

		public float Sensitivity
		{
			get
			{
				return sensitivity;
			}
			protected set
			{
				sensitivity = Mathf.Clamp01(value);
			}
		}

		public float LowerDeadZone
		{
			get
			{
				return lowerDeadZone;
			}
			protected set
			{
				lowerDeadZone = Mathf.Clamp01(value);
			}
		}

		public float UpperDeadZone
		{
			get
			{
				return upperDeadZone;
			}
			protected set
			{
				upperDeadZone = Mathf.Clamp01(value);
			}
		}

		public InputControlMapping[] AnalogMappings
		{
			get
			{
				return analogMappings;
			}
			protected set
			{
				analogMappings = value;
			}
		}

		public InputControlMapping[] ButtonMappings
		{
			get
			{
				return buttonMappings;
			}
			protected set
			{
				buttonMappings = value;
			}
		}

		public string[] IncludePlatforms
		{
			get
			{
				return includePlatforms;
			}
			protected set
			{
				includePlatforms = value;
			}
		}

		public string[] ExcludePlatforms
		{
			get
			{
				return excludePlatforms;
			}
			protected set
			{
				excludePlatforms = value;
			}
		}

		public int MinSystemBuildNumber
		{
			get
			{
				return minSystemBuildNumber;
			}
			protected set
			{
				minSystemBuildNumber = value;
			}
		}

		public int MaxSystemBuildNumber
		{
			get
			{
				return maxSystemBuildNumber;
			}
			protected set
			{
				maxSystemBuildNumber = value;
			}
		}

		public VersionInfo MinUnityVersion
		{
			get
			{
				return minUnityVersion;
			}
			protected set
			{
				minUnityVersion = value;
			}
		}

		public VersionInfo MaxUnityVersion
		{
			get
			{
				return maxUnityVersion;
			}
			protected set
			{
				maxUnityVersion = value;
			}
		}

		public InputDeviceMatcher[] Matchers
		{
			get
			{
				return matchers;
			}
			protected set
			{
				matchers = value;
			}
		}

		public InputDeviceMatcher[] LastResortMatchers
		{
			get
			{
				return lastResortMatchers;
			}
			protected set
			{
				lastResortMatchers = value;
			}
		}

		public bool IsSupportedOnThisPlatform
		{
			get
			{
				VersionInfo versionInfo = VersionInfo.UnityVersion();
				if (versionInfo < MinUnityVersion || versionInfo > MaxUnityVersion)
				{
					return false;
				}
				int systemBuildNumber = Utility.GetSystemBuildNumber();
				if (MaxSystemBuildNumber > 0 && systemBuildNumber > MaxSystemBuildNumber)
				{
					return false;
				}
				if (MinSystemBuildNumber > 0 && systemBuildNumber < MinSystemBuildNumber)
				{
					return false;
				}
				if (ExcludePlatforms != null)
				{
					int num = ExcludePlatforms.Length;
					for (int i = 0; i < num; i++)
					{
						if (InputManager.Platform.Contains(ExcludePlatforms[i].ToUpper()))
						{
							return false;
						}
					}
				}
				if (IncludePlatforms == null || IncludePlatforms.Length == 0)
				{
					return true;
				}
				if (IncludePlatforms != null)
				{
					int num2 = IncludePlatforms.Length;
					for (int j = 0; j < num2; j++)
					{
						if (InputManager.Platform.Contains(IncludePlatforms[j].ToUpper()))
						{
							return true;
						}
					}
				}
				return false;
			}
		}

		public bool IsHidden => hiddenProfiles.Contains(GetType());

		public bool IsNotHidden => !IsHidden;

		public int AnalogCount => AnalogMappings.Length;

		public int ButtonCount => ButtonMappings.Length;

		public static InputDeviceProfile CreateInstanceOfType(Type type)
		{
			InputDeviceProfile obj = (InputDeviceProfile)Activator.CreateInstance(type);
			obj.Define();
			return obj;
		}

		public static InputDeviceProfile CreateInstanceOfType(string typeName)
		{
			Type type = Type.GetType(typeName);
			if (type == null)
			{
				Logger.LogWarning("Cannot find type: " + typeName + "(is the IL2CPP stripping level too high?)");
				return null;
			}
			return CreateInstanceOfType(type);
		}

		public virtual void Define()
		{
			bool flag = GetType().GetCustomAttributes(typeof(NativeInputDeviceProfileAttribute), inherit: false).Length != 0;
			profileType = (flag ? InputDeviceProfileType.Native : InputDeviceProfileType.Unity);
		}

		public bool Matches(InputDeviceInfo deviceInfo)
		{
			return Matches(deviceInfo, Matchers);
		}

		public bool LastResortMatches(InputDeviceInfo deviceInfo)
		{
			return Matches(deviceInfo, LastResortMatchers);
		}

		public bool Matches(InputDeviceInfo deviceInfo, InputDeviceMatcher[] matchers)
		{
			if (matchers != null)
			{
				int num = matchers.Length;
				for (int i = 0; i < num; i++)
				{
					if (matchers[i].Matches(deviceInfo))
					{
						return true;
					}
				}
			}
			return false;
		}

		public static void Hide(Type type)
		{
			hiddenProfiles.Add(type);
		}

		protected static InputControlSource Button(int index)
		{
			return new InputControlSource(InputControlSourceType.Button, index);
		}

		protected static InputControlSource Analog(int index)
		{
			return new InputControlSource(InputControlSourceType.Analog, index);
		}

		protected static InputControlMapping LeftStickLeftMapping(int analog)
		{
			return new InputControlMapping
			{
				Name = "Left Stick Left",
				Target = InputControlType.LeftStickLeft,
				Source = Analog(analog),
				SourceRange = InputRangeType.ZeroToMinusOne,
				TargetRange = InputRangeType.ZeroToOne
			};
		}

		protected static InputControlMapping LeftStickRightMapping(int analog)
		{
			return new InputControlMapping
			{
				Name = "Left Stick Right",
				Target = InputControlType.LeftStickRight,
				Source = Analog(analog),
				SourceRange = InputRangeType.ZeroToOne,
				TargetRange = InputRangeType.ZeroToOne
			};
		}

		protected static InputControlMapping LeftStickUpMapping(int analog)
		{
			return new InputControlMapping
			{
				Name = "Left Stick Up",
				Target = InputControlType.LeftStickUp,
				Source = Analog(analog),
				SourceRange = InputRangeType.ZeroToMinusOne,
				TargetRange = InputRangeType.ZeroToOne
			};
		}

		protected static InputControlMapping LeftStickDownMapping(int analog)
		{
			return new InputControlMapping
			{
				Name = "Left Stick Down",
				Target = InputControlType.LeftStickDown,
				Source = Analog(analog),
				SourceRange = InputRangeType.ZeroToOne,
				TargetRange = InputRangeType.ZeroToOne
			};
		}

		protected static InputControlMapping LeftStickUpMapping2(int analog)
		{
			return new InputControlMapping
			{
				Name = "Left Stick Up",
				Target = InputControlType.LeftStickUp,
				Source = Analog(analog),
				SourceRange = InputRangeType.ZeroToOne,
				TargetRange = InputRangeType.ZeroToOne
			};
		}

		protected static InputControlMapping LeftStickDownMapping2(int analog)
		{
			return new InputControlMapping
			{
				Name = "Left Stick Down",
				Target = InputControlType.LeftStickDown,
				Source = Analog(analog),
				SourceRange = InputRangeType.ZeroToMinusOne,
				TargetRange = InputRangeType.ZeroToOne
			};
		}

		protected static InputControlMapping RightStickLeftMapping(int analog)
		{
			return new InputControlMapping
			{
				Name = "Right Stick Left",
				Target = InputControlType.RightStickLeft,
				Source = Analog(analog),
				SourceRange = InputRangeType.ZeroToMinusOne,
				TargetRange = InputRangeType.ZeroToOne
			};
		}

		protected static InputControlMapping RightStickRightMapping(int analog)
		{
			return new InputControlMapping
			{
				Name = "Right Stick Right",
				Target = InputControlType.RightStickRight,
				Source = Analog(analog),
				SourceRange = InputRangeType.ZeroToOne,
				TargetRange = InputRangeType.ZeroToOne
			};
		}

		protected static InputControlMapping RightStickUpMapping(int analog)
		{
			return new InputControlMapping
			{
				Name = "Right Stick Up",
				Target = InputControlType.RightStickUp,
				Source = Analog(analog),
				SourceRange = InputRangeType.ZeroToMinusOne,
				TargetRange = InputRangeType.ZeroToOne
			};
		}

		protected static InputControlMapping RightStickDownMapping(int analog)
		{
			return new InputControlMapping
			{
				Name = "Right Stick Down",
				Target = InputControlType.RightStickDown,
				Source = Analog(analog),
				SourceRange = InputRangeType.ZeroToOne,
				TargetRange = InputRangeType.ZeroToOne
			};
		}

		protected static InputControlMapping RightStickUpMapping2(int analog)
		{
			return new InputControlMapping
			{
				Name = "Right Stick Up",
				Target = InputControlType.RightStickUp,
				Source = Analog(analog),
				SourceRange = InputRangeType.ZeroToOne,
				TargetRange = InputRangeType.ZeroToOne
			};
		}

		protected static InputControlMapping RightStickDownMapping2(int analog)
		{
			return new InputControlMapping
			{
				Name = "Right Stick Down",
				Target = InputControlType.RightStickDown,
				Source = Analog(analog),
				SourceRange = InputRangeType.ZeroToMinusOne,
				TargetRange = InputRangeType.ZeroToOne
			};
		}

		protected static InputControlMapping LeftTriggerMapping(int analog, string name = "Left Trigger")
		{
			return new InputControlMapping
			{
				Name = name,
				Target = InputControlType.LeftTrigger,
				Source = Analog(analog),
				SourceRange = InputRangeType.MinusOneToOne,
				TargetRange = InputRangeType.ZeroToOne,
				IgnoreInitialZeroValue = true
			};
		}

		protected static InputControlMapping RightTriggerMapping(int analog, string name = "Right Trigger")
		{
			return new InputControlMapping
			{
				Name = name,
				Target = InputControlType.RightTrigger,
				Source = Analog(analog),
				SourceRange = InputRangeType.MinusOneToOne,
				TargetRange = InputRangeType.ZeroToOne,
				IgnoreInitialZeroValue = true
			};
		}

		protected static InputControlMapping DPadLeftMapping(int analog)
		{
			return new InputControlMapping
			{
				Name = "DPad Left",
				Target = InputControlType.DPadLeft,
				Source = Analog(analog),
				SourceRange = InputRangeType.ZeroToMinusOne,
				TargetRange = InputRangeType.ZeroToOne
			};
		}

		protected static InputControlMapping DPadRightMapping(int analog)
		{
			return new InputControlMapping
			{
				Name = "DPad Right",
				Target = InputControlType.DPadRight,
				Source = Analog(analog),
				SourceRange = InputRangeType.ZeroToOne,
				TargetRange = InputRangeType.ZeroToOne
			};
		}

		protected static InputControlMapping DPadUpMapping(int analog)
		{
			return new InputControlMapping
			{
				Name = "DPad Up",
				Target = InputControlType.DPadUp,
				Source = Analog(analog),
				SourceRange = InputRangeType.ZeroToMinusOne,
				TargetRange = InputRangeType.ZeroToOne
			};
		}

		protected static InputControlMapping DPadDownMapping(int analog)
		{
			return new InputControlMapping
			{
				Name = "DPad Down",
				Target = InputControlType.DPadDown,
				Source = Analog(analog),
				SourceRange = InputRangeType.ZeroToOne,
				TargetRange = InputRangeType.ZeroToOne
			};
		}

		protected static InputControlMapping DPadUpMapping2(int analog)
		{
			return new InputControlMapping
			{
				Name = "DPad Up",
				Target = InputControlType.DPadUp,
				Source = Analog(analog),
				SourceRange = InputRangeType.ZeroToOne,
				TargetRange = InputRangeType.ZeroToOne
			};
		}

		protected static InputControlMapping DPadDownMapping2(int analog)
		{
			return new InputControlMapping
			{
				Name = "DPad Down",
				Target = InputControlType.DPadDown,
				Source = Analog(analog),
				SourceRange = InputRangeType.ZeroToMinusOne,
				TargetRange = InputRangeType.ZeroToOne
			};
		}
	}
}

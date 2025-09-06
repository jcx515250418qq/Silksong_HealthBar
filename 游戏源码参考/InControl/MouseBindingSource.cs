using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace InControl
{
	public class MouseBindingSource : BindingSource
	{
		public static float ScaleX = 0.05f;

		public static float ScaleY = 0.05f;

		public static float ScaleZ = 0.05f;

		public static float JitterThreshold = 0.05f;

		public Mouse Control { get; protected set; }

		public override string Name => Control.ToString();

		public override string DeviceName => "Mouse";

		public override InputDeviceClass DeviceClass => InputDeviceClass.Mouse;

		public override InputDeviceStyle DeviceStyle => InputDeviceStyle.Unknown;

		public override BindingSourceType BindingSourceType => BindingSourceType.MouseBindingSource;

		internal MouseBindingSource()
		{
		}

		public MouseBindingSource(Mouse mouseControl)
		{
			Control = mouseControl;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool ButtonIsPressed(Mouse control)
		{
			return InputManager.MouseProvider.GetButtonIsPressed(control);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool NegativeScrollWheelIsActive(float threshold)
		{
			return Mathf.Min(InputManager.MouseProvider.GetDeltaScroll() * ScaleZ, 0f) < 0f - threshold;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool PositiveScrollWheelIsActive(float threshold)
		{
			return Mathf.Max(0f, InputManager.MouseProvider.GetDeltaScroll() * ScaleZ) > threshold;
		}

		internal static float GetValue(Mouse mouseControl)
		{
			switch (mouseControl)
			{
			case Mouse.None:
				return 0f;
			case Mouse.NegativeX:
				return 0f - Mathf.Min(InputManager.MouseProvider.GetDeltaX() * ScaleX, 0f);
			case Mouse.PositiveX:
				return Mathf.Max(0f, InputManager.MouseProvider.GetDeltaX() * ScaleX);
			case Mouse.NegativeY:
				return 0f - Mathf.Min(InputManager.MouseProvider.GetDeltaY() * ScaleY, 0f);
			case Mouse.PositiveY:
				return Mathf.Max(0f, InputManager.MouseProvider.GetDeltaY() * ScaleY);
			case Mouse.NegativeScrollWheel:
				return 0f - Mathf.Min(InputManager.MouseProvider.GetDeltaScroll() * ScaleZ, 0f);
			case Mouse.PositiveScrollWheel:
				return Mathf.Max(0f, InputManager.MouseProvider.GetDeltaScroll() * ScaleZ);
			default:
				if (!InputManager.MouseProvider.GetButtonIsPressed(mouseControl))
				{
					return 0f;
				}
				return 1f;
			}
		}

		public override float GetValue(InputDevice inputDevice)
		{
			return GetValue(Control);
		}

		public override bool GetState(InputDevice inputDevice)
		{
			return Utility.IsNotZero(GetValue(inputDevice));
		}

		public override bool Equals(BindingSource other)
		{
			if (other == null)
			{
				return false;
			}
			MouseBindingSource mouseBindingSource = other as MouseBindingSource;
			if (mouseBindingSource != null)
			{
				return Control == mouseBindingSource.Control;
			}
			return false;
		}

		public override bool Equals(object other)
		{
			if (other == null)
			{
				return false;
			}
			MouseBindingSource mouseBindingSource = other as MouseBindingSource;
			if (mouseBindingSource != null)
			{
				return Control == mouseBindingSource.Control;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Control.GetHashCode();
		}

		public override void Save(BinaryWriter writer)
		{
			writer.Write((int)Control);
		}

		public override void Load(BinaryReader reader, ushort dataFormatVersion)
		{
			Control = (Mouse)reader.ReadInt32();
		}
	}
}

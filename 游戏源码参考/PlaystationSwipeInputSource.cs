using System.IO;
using InControl;

public sealed class PlaystationSwipeInputSource : BindingSource
{
	public enum Swipe
	{
		Up = 0,
		Right = 1,
		Down = 2,
		Left = 3
	}

	public Swipe SwipeDirection { get; private set; }

	public override string Name => $"Swipe {SwipeDirection}";

	public override string DeviceName
	{
		get
		{
			if (base.BoundTo == null)
			{
				return "";
			}
			InputDevice device = base.BoundTo.Device;
			if (device == InputDevice.Null)
			{
				return "Controller";
			}
			return device.Name;
		}
	}

	public override InputDeviceClass DeviceClass
	{
		get
		{
			if (base.BoundTo != null)
			{
				return base.BoundTo.Device.DeviceClass;
			}
			return InputDeviceClass.Unknown;
		}
	}

	public override InputDeviceStyle DeviceStyle
	{
		get
		{
			if (base.BoundTo != null)
			{
				return base.BoundTo.Device.DeviceStyle;
			}
			return InputDeviceStyle.Unknown;
		}
	}

	public override BindingSourceType BindingSourceType => BindingSourceType.DeviceBindingSource;

	private PlaystationSwipeInputSource()
	{
	}

	public PlaystationSwipeInputSource(Swipe swipeDirection)
	{
		SwipeDirection = swipeDirection;
	}

	public override float GetValue(InputDevice inputDevice)
	{
		return GetState(inputDevice) ? 1 : 0;
	}

	public override bool GetState(InputDevice inputDevice)
	{
		return false;
	}

	public override bool Equals(BindingSource other)
	{
		if (other == null)
		{
			return false;
		}
		PlaystationSwipeInputSource playstationSwipeInputSource = other as PlaystationSwipeInputSource;
		if (playstationSwipeInputSource != null)
		{
			return SwipeDirection == playstationSwipeInputSource.SwipeDirection;
		}
		return false;
	}

	public override void Save(BinaryWriter writer)
	{
		writer.Write((int)SwipeDirection);
	}

	public override void Load(BinaryReader reader, ushort dataFormatVersion)
	{
		SwipeDirection = (Swipe)reader.ReadInt32();
	}
}

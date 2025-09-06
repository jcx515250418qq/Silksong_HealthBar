using XInputDotNetPure;

namespace InControl
{
	public class XInputDevice : InputDevice
	{
		private const float LowerDeadZone = 0.2f;

		private const float UpperDeadZone = 0.9f;

		private readonly XInputDeviceManager owner;

		private GamePadState state;

		public int DeviceIndex { get; private set; }

		public bool IsConnected => state.IsConnected;

		public XInputDevice(int deviceIndex, XInputDeviceManager owner)
			: base("XInput Controller")
		{
			this.owner = owner;
			DeviceIndex = deviceIndex;
			base.SortOrder = deviceIndex;
			base.Meta = "XInput Device #" + deviceIndex;
			base.DeviceClass = InputDeviceClass.Controller;
			base.DeviceStyle = InputDeviceStyle.XboxOne;
			AddControl(InputControlType.LeftStickLeft, "Left Stick Left", 0.2f, 0.9f);
			AddControl(InputControlType.LeftStickRight, "Left Stick Right", 0.2f, 0.9f);
			AddControl(InputControlType.LeftStickUp, "Left Stick Up", 0.2f, 0.9f);
			AddControl(InputControlType.LeftStickDown, "Left Stick Down", 0.2f, 0.9f);
			AddControl(InputControlType.RightStickLeft, "Right Stick Left", 0.2f, 0.9f);
			AddControl(InputControlType.RightStickRight, "Right Stick Right", 0.2f, 0.9f);
			AddControl(InputControlType.RightStickUp, "Right Stick Up", 0.2f, 0.9f);
			AddControl(InputControlType.RightStickDown, "Right Stick Down", 0.2f, 0.9f);
			AddControl(InputControlType.LeftTrigger, "Left Trigger", 0.2f, 0.9f);
			AddControl(InputControlType.RightTrigger, "Right Trigger", 0.2f, 0.9f);
			AddControl(InputControlType.DPadUp, "DPad Up", 0.2f, 0.9f);
			AddControl(InputControlType.DPadDown, "DPad Down", 0.2f, 0.9f);
			AddControl(InputControlType.DPadLeft, "DPad Left", 0.2f, 0.9f);
			AddControl(InputControlType.DPadRight, "DPad Right", 0.2f, 0.9f);
			AddControl(InputControlType.Action1, "A");
			AddControl(InputControlType.Action2, "B");
			AddControl(InputControlType.Action3, "X");
			AddControl(InputControlType.Action4, "Y");
			AddControl(InputControlType.LeftBumper, "Left Bumper");
			AddControl(InputControlType.RightBumper, "Right Bumper");
			AddControl(InputControlType.LeftStickButton, "Left Stick Button");
			AddControl(InputControlType.RightStickButton, "Right Stick Button");
			AddControl(InputControlType.View, "View");
			AddControl(InputControlType.Menu, "Menu");
		}

		public override void Update(ulong updateTick, float deltaTime)
		{
			GetState();
			UpdateLeftStickWithValue(state.ThumbSticks.Left.Vector, updateTick, deltaTime);
			UpdateRightStickWithValue(state.ThumbSticks.Right.Vector, updateTick, deltaTime);
			UpdateWithValue(InputControlType.LeftTrigger, state.Triggers.Left, updateTick, deltaTime);
			UpdateWithValue(InputControlType.RightTrigger, state.Triggers.Right, updateTick, deltaTime);
			UpdateWithState(InputControlType.DPadUp, state.DPad.Up == ButtonState.Pressed, updateTick, deltaTime);
			UpdateWithState(InputControlType.DPadDown, state.DPad.Down == ButtonState.Pressed, updateTick, deltaTime);
			UpdateWithState(InputControlType.DPadLeft, state.DPad.Left == ButtonState.Pressed, updateTick, deltaTime);
			UpdateWithState(InputControlType.DPadRight, state.DPad.Right == ButtonState.Pressed, updateTick, deltaTime);
			UpdateWithState(InputControlType.Action1, state.Buttons.A == ButtonState.Pressed, updateTick, deltaTime);
			UpdateWithState(InputControlType.Action2, state.Buttons.B == ButtonState.Pressed, updateTick, deltaTime);
			UpdateWithState(InputControlType.Action3, state.Buttons.X == ButtonState.Pressed, updateTick, deltaTime);
			UpdateWithState(InputControlType.Action4, state.Buttons.Y == ButtonState.Pressed, updateTick, deltaTime);
			UpdateWithState(InputControlType.LeftBumper, state.Buttons.LeftShoulder == ButtonState.Pressed, updateTick, deltaTime);
			UpdateWithState(InputControlType.RightBumper, state.Buttons.RightShoulder == ButtonState.Pressed, updateTick, deltaTime);
			UpdateWithState(InputControlType.LeftStickButton, state.Buttons.LeftStick == ButtonState.Pressed, updateTick, deltaTime);
			UpdateWithState(InputControlType.RightStickButton, state.Buttons.RightStick == ButtonState.Pressed, updateTick, deltaTime);
			UpdateWithState(InputControlType.View, state.Buttons.Back == ButtonState.Pressed, updateTick, deltaTime);
			UpdateWithState(InputControlType.Menu, state.Buttons.Start == ButtonState.Pressed, updateTick, deltaTime);
		}

		public override void Vibrate(float leftSpeed, float rightSpeed)
		{
			GamePad.SetVibration((PlayerIndex)DeviceIndex, leftSpeed, rightSpeed);
		}

		internal void GetState()
		{
			state = owner.GetState(DeviceIndex);
		}
	}
}

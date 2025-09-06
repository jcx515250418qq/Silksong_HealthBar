using InControl;
using UnityEngine;

namespace TeamCherry.PS5
{
	public sealed class PS5InControlInput : PSInputBase
	{
		public override void Init(GamePad gamePad)
		{
		}

		public override Vector2 GetThumbStickLeft()
		{
			return InputManager.ActiveDevice.LeftStick.Value;
		}

		public override Vector2 GetThumbStickRight()
		{
			return InputManager.ActiveDevice.RightStick.Value;
		}

		public override bool GetL3()
		{
			return InputManager.ActiveDevice.LeftStick.IsPressed;
		}

		public override bool GetR3()
		{
			return InputManager.ActiveDevice.RightStick.IsPressed;
		}

		public override bool GetOptions()
		{
			return InputManager.ActiveDevice.GetControl(InputControlType.Options).IsPressed;
		}

		public override bool GetCross()
		{
			return InputManager.ActiveDevice.Action1.IsPressed;
		}

		public override bool GetCircle()
		{
			return InputManager.ActiveDevice.Action2.IsPressed;
		}

		public override bool GetSquare()
		{
			return InputManager.ActiveDevice.Action3.IsPressed;
		}

		public override bool GetTriangle()
		{
			return InputManager.ActiveDevice.Action4.IsPressed;
		}

		public override bool GetDpadRight()
		{
			return InputManager.ActiveDevice.DPadRight.IsPressed;
		}

		public override bool GetDpadLeft()
		{
			return InputManager.ActiveDevice.DPadLeft.IsPressed;
		}

		public override bool GetDpadUp()
		{
			return InputManager.ActiveDevice.DPadUp.IsPressed;
		}

		public override bool GetDpadDown()
		{
			return InputManager.ActiveDevice.DPadDown.IsPressed;
		}

		public override bool GetR1()
		{
			return InputManager.ActiveDevice.RightBumper.IsPressed;
		}

		public override bool GetR2()
		{
			return InputManager.ActiveDevice.RightTrigger.IsPressed;
		}

		public override bool GetL1()
		{
			return InputManager.ActiveDevice.LeftBumper.IsPressed;
		}

		public override bool GetL2()
		{
			return InputManager.ActiveDevice.LeftTrigger.IsPressed;
		}

		public override bool TouchPadButton()
		{
			return InputManager.ActiveDevice.GetControl(InputControlType.TouchPadButton).IsPressed;
		}
	}
}

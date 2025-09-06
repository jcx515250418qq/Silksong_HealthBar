using System;
using UnityEngine;

namespace TeamCherry.PS5
{
	public class PSSampleInput : PSInputBase
	{
		private KeyCode optionsBtnKeyCode;

		private string leftStickHorizontalAxis;

		private string leftStickVerticalAxis;

		private string rightStickHorizontalAxis;

		private string rightStickVerticalAxis;

		private KeyCode L1BtnKeyCode;

		private KeyCode R1BtnKeyCode;

		private string L2Axis;

		private string R2Axis;

		private KeyCode L3BtnKeyCode;

		private KeyCode R3BtnKeyCode;

		private KeyCode CrossBtnKeyCode;

		private KeyCode CircleBtnKeyCode;

		private KeyCode SquareBtnKeyCode;

		private KeyCode TriangleBtnKeyCode;

		private string DPadRightAxis;

		private string DPadLeftAxis;

		private string DPadUpAxis;

		private string DPadDownAxis;

		public override void Init(GamePad gamePad)
		{
			int num = gamePad.playerId + 1;
			optionsBtnKeyCode = (KeyCode)Enum.Parse(typeof(KeyCode), "Joystick" + num + "Button7", ignoreCase: true);
			leftStickHorizontalAxis = "leftstick" + num + "horizontal";
			leftStickVerticalAxis = "leftstick" + num + "vertical";
			rightStickHorizontalAxis = "rightstick" + num + "horizontal";
			rightStickVerticalAxis = "rightstick" + num + "vertical";
			CrossBtnKeyCode = (KeyCode)Enum.Parse(typeof(KeyCode), "Joystick" + num + "Button0", ignoreCase: true);
			CircleBtnKeyCode = (KeyCode)Enum.Parse(typeof(KeyCode), "Joystick" + num + "Button1", ignoreCase: true);
			SquareBtnKeyCode = (KeyCode)Enum.Parse(typeof(KeyCode), "Joystick" + num + "Button2", ignoreCase: true);
			TriangleBtnKeyCode = (KeyCode)Enum.Parse(typeof(KeyCode), "Joystick" + num + "Button3", ignoreCase: true);
			L1BtnKeyCode = (KeyCode)Enum.Parse(typeof(KeyCode), "Joystick" + num + "Button4", ignoreCase: true);
			R1BtnKeyCode = (KeyCode)Enum.Parse(typeof(KeyCode), "Joystick" + num + "Button5", ignoreCase: true);
			L3BtnKeyCode = (KeyCode)Enum.Parse(typeof(KeyCode), "Joystick" + num + "Button8", ignoreCase: true);
			R3BtnKeyCode = (KeyCode)Enum.Parse(typeof(KeyCode), "Joystick" + num + "Button9", ignoreCase: true);
			DPadRightAxis = "dpad" + num + "_horizontal";
			DPadLeftAxis = "dpad" + num + "_horizontal";
			DPadUpAxis = "dpad" + num + "_vertical";
			DPadDownAxis = "dpad" + num + "_vertical";
			L2Axis = "joystick" + num + "_left_trigger";
			R2Axis = "joystick" + num + "_left_trigger";
		}

		public override Vector2 GetThumbStickLeft()
		{
			return new Vector2(Input.GetAxis(leftStickHorizontalAxis), Input.GetAxis(leftStickVerticalAxis));
		}

		public override Vector2 GetThumbStickRight()
		{
			return new Vector2(Input.GetAxis(rightStickHorizontalAxis), Input.GetAxis(rightStickVerticalAxis));
		}

		public override bool GetL3()
		{
			return Input.GetKey(L3BtnKeyCode);
		}

		public override bool GetR3()
		{
			return Input.GetKey(R3BtnKeyCode);
		}

		public override bool GetOptions()
		{
			return Input.GetKey(optionsBtnKeyCode);
		}

		public override bool GetCross()
		{
			return Input.GetKey(CrossBtnKeyCode);
		}

		public override bool GetCircle()
		{
			return Input.GetKey(CircleBtnKeyCode);
		}

		public override bool GetSquare()
		{
			return Input.GetKey(SquareBtnKeyCode);
		}

		public override bool GetTriangle()
		{
			return Input.GetKey(TriangleBtnKeyCode);
		}

		public override bool GetDpadRight()
		{
			return Input.GetAxis(DPadRightAxis) > 0f;
		}

		public override bool GetDpadLeft()
		{
			return Input.GetAxis(DPadLeftAxis) < 0f;
		}

		public override bool GetDpadUp()
		{
			return Input.GetAxis(DPadUpAxis) > 0f;
		}

		public override bool GetDpadDown()
		{
			return Input.GetAxis(DPadDownAxis) < 0f;
		}

		public override bool GetR1()
		{
			return Input.GetKey(R1BtnKeyCode);
		}

		public override bool GetR2()
		{
			return Input.GetAxis(R2Axis) != 0f;
		}

		public override bool GetL1()
		{
			return Input.GetKey(L1BtnKeyCode);
		}

		public override bool GetL2()
		{
			return Input.GetAxis(L2Axis) != 0f;
		}

		public override bool TouchPadButton()
		{
			return false;
		}
	}
}

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace InControl
{
	public class UnityMouseProvider : IMouseProvider
	{
		private const string mouseXAxis = "mouse x";

		private const string mouseYAxis = "mouse y";

		private readonly bool[] lastButtonPressed = new bool[16];

		private readonly bool[] buttonPressed = new bool[16];

		private Vector2 lastPosition;

		private Vector2 position;

		private Vector2 delta;

		private Vector2 scroll;

		private static int maxSafeMouseButton = int.MaxValue;

		public void Setup()
		{
			ClearState();
		}

		public void Reset()
		{
			ClearState();
		}

		public void Update()
		{
			if (Input.mousePresent)
			{
				Array.Copy(buttonPressed, lastButtonPressed, buttonPressed.Length);
				buttonPressed[1] = SafeGetMouseButton(0);
				buttonPressed[2] = SafeGetMouseButton(1);
				buttonPressed[3] = SafeGetMouseButton(2);
				buttonPressed[10] = SafeGetMouseButton(3);
				buttonPressed[11] = SafeGetMouseButton(4);
				buttonPressed[12] = SafeGetMouseButton(5);
				buttonPressed[13] = SafeGetMouseButton(6);
				lastPosition = position;
				position = Input.mousePosition;
				delta = new Vector2(Input.GetAxisRaw("mouse x"), Input.GetAxisRaw("mouse y"));
				scroll = Input.mouseScrollDelta;
			}
			else
			{
				ClearState();
			}
		}

		private static bool SafeGetMouseButton(int button)
		{
			if (button < maxSafeMouseButton)
			{
				try
				{
					return Input.GetMouseButton(button);
				}
				catch (ArgumentException)
				{
					maxSafeMouseButton = Mathf.Min(button, maxSafeMouseButton);
				}
			}
			return false;
		}

		private void ClearState()
		{
			Array.Clear(lastButtonPressed, 0, lastButtonPressed.Length);
			Array.Clear(buttonPressed, 0, buttonPressed.Length);
			lastPosition = Vector2.zero;
			position = Vector2.zero;
			delta = Vector2.zero;
			scroll = Vector2.zero;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2 GetPosition()
		{
			return position;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float GetDeltaX()
		{
			return delta.x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float GetDeltaY()
		{
			return delta.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float GetDeltaScroll()
		{
			if (!(Utility.Abs(scroll.x) > Utility.Abs(scroll.y)))
			{
				return scroll.y;
			}
			return scroll.x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool GetButtonIsPressed(Mouse control)
		{
			return buttonPressed[(int)control];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool GetButtonWasPressed(Mouse control)
		{
			if (buttonPressed[(int)control])
			{
				return !lastButtonPressed[(int)control];
			}
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool GetButtonWasReleased(Mouse control)
		{
			if (!buttonPressed[(int)control])
			{
				return lastButtonPressed[(int)control];
			}
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool HasMousePresent()
		{
			return Input.mousePresent;
		}
	}
}

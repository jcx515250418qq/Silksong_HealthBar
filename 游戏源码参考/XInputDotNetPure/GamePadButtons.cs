namespace XInputDotNetPure
{
	public struct GamePadButtons
	{
		private ButtonState start;

		private ButtonState back;

		private ButtonState leftStick;

		private ButtonState rightStick;

		private ButtonState leftShoulder;

		private ButtonState rightShoulder;

		private ButtonState a;

		private ButtonState b;

		private ButtonState x;

		private ButtonState y;

		public ButtonState Start => start;

		public ButtonState Back => back;

		public ButtonState LeftStick => leftStick;

		public ButtonState RightStick => rightStick;

		public ButtonState LeftShoulder => leftShoulder;

		public ButtonState RightShoulder => rightShoulder;

		public ButtonState A => a;

		public ButtonState B => b;

		public ButtonState X => x;

		public ButtonState Y => y;

		internal GamePadButtons(ButtonState start, ButtonState back, ButtonState leftStick, ButtonState rightStick, ButtonState leftShoulder, ButtonState rightShoulder, ButtonState a, ButtonState b, ButtonState x, ButtonState y)
		{
			this.start = start;
			this.back = back;
			this.leftStick = leftStick;
			this.rightStick = rightStick;
			this.leftShoulder = leftShoulder;
			this.rightShoulder = rightShoulder;
			this.a = a;
			this.b = b;
			this.x = x;
			this.y = y;
		}
	}
}

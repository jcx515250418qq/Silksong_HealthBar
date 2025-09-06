namespace XInputDotNetPure
{
	public struct GamePadDPad
	{
		private ButtonState up;

		private ButtonState down;

		private ButtonState left;

		private ButtonState right;

		public ButtonState Up => up;

		public ButtonState Down => down;

		public ButtonState Left => left;

		public ButtonState Right => right;

		internal GamePadDPad(ButtonState up, ButtonState down, ButtonState left, ButtonState right)
		{
			this.up = up;
			this.down = down;
			this.left = left;
			this.right = right;
		}
	}
}

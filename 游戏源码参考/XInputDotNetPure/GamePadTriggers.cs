namespace XInputDotNetPure
{
	public struct GamePadTriggers
	{
		private float left;

		private float right;

		public float Left => left;

		public float Right => right;

		internal GamePadTriggers(float left, float right)
		{
			this.left = left;
			this.right = right;
		}
	}
}

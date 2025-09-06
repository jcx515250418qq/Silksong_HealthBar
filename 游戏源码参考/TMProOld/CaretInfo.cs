namespace TMProOld
{
	public struct CaretInfo
	{
		public int index;

		public CaretPosition position;

		public CaretInfo(int index, CaretPosition position)
		{
			this.index = index;
			this.position = position;
		}
	}
}

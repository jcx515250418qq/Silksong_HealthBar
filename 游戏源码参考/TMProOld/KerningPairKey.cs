namespace TMProOld
{
	public struct KerningPairKey
	{
		public int ascii_Left;

		public int ascii_Right;

		public int key;

		public KerningPairKey(int ascii_left, int ascii_right)
		{
			ascii_Left = ascii_left;
			ascii_Right = ascii_right;
			key = (ascii_right << 16) + ascii_left;
		}
	}
}

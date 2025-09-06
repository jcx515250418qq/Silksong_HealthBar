using System;

namespace TMProOld
{
	[Serializable]
	public class KerningPair
	{
		public int AscII_Left;

		public int AscII_Right;

		public float XadvanceOffset;

		public KerningPair(int left, int right, float offset)
		{
			AscII_Left = left;
			AscII_Right = right;
			XadvanceOffset = offset;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace TMProOld
{
	[Serializable]
	public class KerningTable
	{
		public List<KerningPair> kerningPairs;

		public KerningTable()
		{
			kerningPairs = new List<KerningPair>();
		}

		public void AddKerningPair()
		{
			if (kerningPairs.Count == 0)
			{
				kerningPairs.Add(new KerningPair(0, 0, 0f));
				return;
			}
			int ascII_Left = kerningPairs.Last().AscII_Left;
			int ascII_Right = kerningPairs.Last().AscII_Right;
			float xadvanceOffset = kerningPairs.Last().XadvanceOffset;
			kerningPairs.Add(new KerningPair(ascII_Left, ascII_Right, xadvanceOffset));
		}

		public int AddKerningPair(int left, int right, float offset)
		{
			if (kerningPairs.FindIndex((KerningPair item) => item.AscII_Left == left && item.AscII_Right == right) == -1)
			{
				kerningPairs.Add(new KerningPair(left, right, offset));
				return 0;
			}
			return -1;
		}

		public void RemoveKerningPair(int left, int right)
		{
			int num = kerningPairs.FindIndex((KerningPair item) => item.AscII_Left == left && item.AscII_Right == right);
			if (num != -1)
			{
				kerningPairs.RemoveAt(num);
			}
		}

		public void RemoveKerningPair(int index)
		{
			kerningPairs.RemoveAt(index);
		}

		public void SortKerningPairs()
		{
			if (kerningPairs.Count > 0)
			{
				kerningPairs = (from s in kerningPairs
					orderby s.AscII_Left, s.AscII_Right
					select s).ToList();
			}
		}
	}
}

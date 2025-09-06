using System;
using UnityEngine;

namespace TMProOld
{
	[Serializable]
	public struct Mesh_Extents
	{
		public Vector2 min;

		public Vector2 max;

		public Mesh_Extents(Vector2 min, Vector2 max)
		{
			this.min = min;
			this.max = max;
		}

		public override string ToString()
		{
			return "Min (" + min.x.ToString("f2") + ", " + min.y.ToString("f2") + ")   Max (" + max.x.ToString("f2") + ", " + max.y.ToString("f2") + ")";
		}
	}
}

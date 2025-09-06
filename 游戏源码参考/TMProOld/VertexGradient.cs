using System;
using UnityEngine;

namespace TMProOld
{
	[Serializable]
	public struct VertexGradient
	{
		public Color topLeft;

		public Color topRight;

		public Color bottomLeft;

		public Color bottomRight;

		public VertexGradient(Color color)
		{
			topLeft = color;
			topRight = color;
			bottomLeft = color;
			bottomRight = color;
		}

		public VertexGradient(Color color0, Color color1, Color color2, Color color3)
		{
			topLeft = color0;
			topRight = color1;
			bottomLeft = color2;
			bottomRight = color3;
		}
	}
}

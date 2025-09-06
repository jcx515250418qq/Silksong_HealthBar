using System;
using UnityEngine;

namespace TMProOld
{
	[Serializable]
	public class TMP_ColorGradient : ScriptableObject
	{
		public Color topLeft;

		public Color topRight;

		public Color bottomLeft;

		public Color bottomRight;

		public TMP_ColorGradient()
		{
			bottomRight = (bottomLeft = (topRight = (topLeft = Color.white)));
		}

		public TMP_ColorGradient(Color color)
		{
			topLeft = color;
			topRight = color;
			bottomLeft = color;
			bottomRight = color;
		}

		public TMP_ColorGradient(Color color0, Color color1, Color color2, Color color3)
		{
			topLeft = color0;
			topRight = color1;
			bottomLeft = color2;
			bottomRight = color3;
		}
	}
}

using UnityEngine;

namespace TMProOld
{
	public struct TMP_CharacterInfo
	{
		public char character;

		public short index;

		public TMP_TextElementType elementType;

		public TMP_TextElement textElement;

		public TMP_FontAsset fontAsset;

		public TMP_SpriteAsset spriteAsset;

		public int spriteIndex;

		public Material material;

		public int materialReferenceIndex;

		public bool isUsingAlternateTypeface;

		public float pointSize;

		public short lineNumber;

		public short pageNumber;

		public int vertexIndex;

		public TMP_Vertex vertex_TL;

		public TMP_Vertex vertex_BL;

		public TMP_Vertex vertex_TR;

		public TMP_Vertex vertex_BR;

		public Vector3 topLeft;

		public Vector3 bottomLeft;

		public Vector3 topRight;

		public Vector3 bottomRight;

		public float origin;

		public float ascender;

		public float baseLine;

		public float descender;

		public float xAdvance;

		public float aspectRatio;

		public float scale;

		public Color32 color;

		public FontStyles style;

		public bool isVisible;
	}
}

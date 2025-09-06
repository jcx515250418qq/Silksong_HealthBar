using UnityEngine;

namespace TMProOld
{
	public struct WordWrapState
	{
		public int previous_WordBreak;

		public int total_CharacterCount;

		public int visible_CharacterCount;

		public int visible_SpriteCount;

		public int visible_LinkCount;

		public int firstCharacterIndex;

		public int firstVisibleCharacterIndex;

		public int lastCharacterIndex;

		public int lastVisibleCharIndex;

		public int lineNumber;

		public float maxCapHeight;

		public float maxAscender;

		public float maxDescender;

		public float maxLineAscender;

		public float maxLineDescender;

		public float previousLineAscender;

		public float xAdvance;

		public float preferredWidth;

		public float preferredHeight;

		public float previousLineScale;

		public int wordCount;

		public FontStyles fontStyle;

		public float fontScale;

		public float fontScaleMultiplier;

		public float currentFontSize;

		public float baselineOffset;

		public float lineOffset;

		public TMP_TextInfo textInfo;

		public TMP_LineInfo lineInfo;

		public Color32 vertexColor;

		public TMP_XmlTagStack<Color32> colorStack;

		public TMP_XmlTagStack<float> sizeStack;

		public TMP_XmlTagStack<int> fontWeightStack;

		public TMP_XmlTagStack<int> styleStack;

		public TMP_XmlTagStack<int> actionStack;

		public TMP_XmlTagStack<MaterialReference> materialReferenceStack;

		public TMP_FontAsset currentFontAsset;

		public TMP_SpriteAsset currentSpriteAsset;

		public Material currentMaterial;

		public int currentMaterialIndex;

		public Extents meshExtents;

		public bool tagNoParsing;
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TMProOld
{
	[Serializable]
	public class TMP_FontAsset : TMP_Asset
	{
		public enum FontAssetTypes
		{
			None = 0,
			SDF = 1,
			Bitmap = 2
		}

		private static TMP_FontAsset s_defaultFontAsset;

		public FontAssetTypes fontAssetType;

		[SerializeField]
		private FaceInfo m_fontInfo;

		[SerializeField]
		public Texture2D atlas;

		[SerializeField]
		private List<TMP_Glyph> m_glyphInfoList;

		private Dictionary<int, TMP_Glyph> m_characterDictionary;

		private Dictionary<int, KerningPair> m_kerningDictionary;

		[SerializeField]
		private KerningTable m_kerningInfo;

		[SerializeField]
		private KerningPair m_kerningPair;

		[SerializeField]
		public List<TMP_FontAsset> fallbackFontAssets;

		[SerializeField]
		public FontCreationSetting fontCreationSettings;

		[SerializeField]
		public TMP_FontWeights[] fontWeights = new TMP_FontWeights[10];

		private int[] m_characterSet;

		public float normalStyle;

		public float normalSpacingOffset;

		public float boldStyle = 0.75f;

		public float boldSpacing = 7f;

		public byte italicStyle = 35;

		public byte tabSize = 10;

		private byte m_oldTabSize;

		public static TMP_FontAsset defaultFontAsset
		{
			get
			{
				if (s_defaultFontAsset == null)
				{
					s_defaultFontAsset = Resources.Load<TMP_FontAsset>("Fonts & Materials/ARIAL SDF");
				}
				return s_defaultFontAsset;
			}
		}

		public FaceInfo fontInfo => m_fontInfo;

		public Dictionary<int, TMP_Glyph> characterDictionary
		{
			get
			{
				if (m_characterDictionary == null)
				{
					ReadFontDefinition();
				}
				return m_characterDictionary;
			}
		}

		public Dictionary<int, KerningPair> kerningDictionary => m_kerningDictionary;

		public KerningTable kerningInfo => m_kerningInfo;

		private void OnEnable()
		{
		}

		private void OnDisable()
		{
		}

		public void AddFaceInfo(FaceInfo faceInfo)
		{
			m_fontInfo = faceInfo;
		}

		public void AddGlyphInfo(TMP_Glyph[] glyphInfo)
		{
			m_glyphInfoList = new List<TMP_Glyph>();
			int num = glyphInfo.Length;
			m_fontInfo.CharacterCount = num;
			m_characterSet = new int[num];
			for (int i = 0; i < num; i++)
			{
				TMP_Glyph tMP_Glyph = new TMP_Glyph();
				tMP_Glyph.id = glyphInfo[i].id;
				tMP_Glyph.x = glyphInfo[i].x;
				tMP_Glyph.y = glyphInfo[i].y;
				tMP_Glyph.width = glyphInfo[i].width;
				tMP_Glyph.height = glyphInfo[i].height;
				tMP_Glyph.xOffset = glyphInfo[i].xOffset;
				tMP_Glyph.yOffset = glyphInfo[i].yOffset;
				tMP_Glyph.xAdvance = glyphInfo[i].xAdvance;
				tMP_Glyph.scale = 1f;
				m_glyphInfoList.Add(tMP_Glyph);
				m_characterSet[i] = tMP_Glyph.id;
			}
			m_glyphInfoList = m_glyphInfoList.OrderBy((TMP_Glyph s) => s.id).ToList();
		}

		public void AddKerningInfo(KerningTable kerningTable)
		{
			m_kerningInfo = kerningTable;
		}

		public void ReadFontDefinition()
		{
			if (m_fontInfo == null)
			{
				return;
			}
			m_characterDictionary = new Dictionary<int, TMP_Glyph>();
			for (int i = 0; i < m_glyphInfoList.Count; i++)
			{
				TMP_Glyph tMP_Glyph = m_glyphInfoList[i];
				if (!m_characterDictionary.ContainsKey(tMP_Glyph.id))
				{
					m_characterDictionary.Add(tMP_Glyph.id, tMP_Glyph);
				}
				if (tMP_Glyph.scale == 0f)
				{
					tMP_Glyph.scale = 1f;
				}
			}
			TMP_Glyph tMP_Glyph2 = new TMP_Glyph();
			if (m_characterDictionary.ContainsKey(32))
			{
				m_characterDictionary[32].width = m_characterDictionary[32].xAdvance;
				m_characterDictionary[32].height = m_fontInfo.Ascender - m_fontInfo.Descender;
				m_characterDictionary[32].yOffset = m_fontInfo.Ascender;
				m_characterDictionary[32].scale = 1f;
			}
			else
			{
				tMP_Glyph2 = new TMP_Glyph();
				tMP_Glyph2.id = 32;
				tMP_Glyph2.x = 0f;
				tMP_Glyph2.y = 0f;
				tMP_Glyph2.width = m_fontInfo.Ascender / 5f;
				tMP_Glyph2.height = m_fontInfo.Ascender - m_fontInfo.Descender;
				tMP_Glyph2.xOffset = 0f;
				tMP_Glyph2.yOffset = m_fontInfo.Ascender;
				tMP_Glyph2.xAdvance = m_fontInfo.PointSize / 4f;
				tMP_Glyph2.scale = 1f;
				m_characterDictionary.Add(32, tMP_Glyph2);
			}
			if (!m_characterDictionary.ContainsKey(160))
			{
				tMP_Glyph2 = TMP_Glyph.Clone(m_characterDictionary[32]);
				m_characterDictionary.Add(160, tMP_Glyph2);
			}
			if (!m_characterDictionary.ContainsKey(8203))
			{
				tMP_Glyph2 = TMP_Glyph.Clone(m_characterDictionary[32]);
				tMP_Glyph2.width = 0f;
				tMP_Glyph2.xAdvance = 0f;
				m_characterDictionary.Add(8203, tMP_Glyph2);
			}
			if (!m_characterDictionary.ContainsKey(8288))
			{
				tMP_Glyph2 = TMP_Glyph.Clone(m_characterDictionary[32]);
				tMP_Glyph2.width = 0f;
				tMP_Glyph2.xAdvance = 0f;
				m_characterDictionary.Add(8288, tMP_Glyph2);
			}
			if (!m_characterDictionary.ContainsKey(10))
			{
				tMP_Glyph2 = new TMP_Glyph();
				tMP_Glyph2.id = 10;
				tMP_Glyph2.x = 0f;
				tMP_Glyph2.y = 0f;
				tMP_Glyph2.width = 10f;
				tMP_Glyph2.height = m_characterDictionary[32].height;
				tMP_Glyph2.xOffset = 0f;
				tMP_Glyph2.yOffset = m_characterDictionary[32].yOffset;
				tMP_Glyph2.xAdvance = 0f;
				tMP_Glyph2.scale = 1f;
				m_characterDictionary.Add(10, tMP_Glyph2);
				if (!m_characterDictionary.ContainsKey(13))
				{
					m_characterDictionary.Add(13, tMP_Glyph2);
				}
			}
			if (!m_characterDictionary.ContainsKey(9))
			{
				tMP_Glyph2 = new TMP_Glyph();
				tMP_Glyph2.id = 9;
				tMP_Glyph2.x = m_characterDictionary[32].x;
				tMP_Glyph2.y = m_characterDictionary[32].y;
				tMP_Glyph2.width = m_characterDictionary[32].width * (float)(int)tabSize + (m_characterDictionary[32].xAdvance - m_characterDictionary[32].width) * (float)(tabSize - 1);
				tMP_Glyph2.height = m_characterDictionary[32].height;
				tMP_Glyph2.xOffset = m_characterDictionary[32].xOffset;
				tMP_Glyph2.yOffset = m_characterDictionary[32].yOffset;
				tMP_Glyph2.xAdvance = m_characterDictionary[32].xAdvance * (float)(int)tabSize;
				tMP_Glyph2.scale = 1f;
				m_characterDictionary.Add(9, tMP_Glyph2);
			}
			m_fontInfo.TabWidth = m_characterDictionary[9].xAdvance;
			if (m_fontInfo.CapHeight == 0f && m_characterDictionary.ContainsKey(65))
			{
				m_fontInfo.CapHeight = m_characterDictionary[65].yOffset;
			}
			if (m_fontInfo.Scale == 0f)
			{
				m_fontInfo.Scale = 1f;
			}
			m_kerningDictionary = new Dictionary<int, KerningPair>();
			List<KerningPair> kerningPairs = m_kerningInfo.kerningPairs;
			for (int j = 0; j < kerningPairs.Count; j++)
			{
				KerningPair kerningPair = kerningPairs[j];
				KerningPairKey kerningPairKey = new KerningPairKey(kerningPair.AscII_Left, kerningPair.AscII_Right);
				if (!m_kerningDictionary.ContainsKey(kerningPairKey.key))
				{
					m_kerningDictionary.Add(kerningPairKey.key, kerningPair);
				}
				else if (!TMP_Settings.warningsDisabled)
				{
					Debug.LogWarning("Kerning Key for [" + kerningPairKey.ascii_Left + "] and [" + kerningPairKey.ascii_Right + "] already exists.");
				}
			}
			hashCode = TMP_TextUtilities.GetSimpleHashCode(base.name);
			materialHashCode = TMP_TextUtilities.GetSimpleHashCode(material.name);
		}

		public bool HasCharacter(int character)
		{
			if (m_characterDictionary == null)
			{
				return false;
			}
			if (m_characterDictionary.ContainsKey(character))
			{
				return true;
			}
			return false;
		}

		public bool HasCharacter(char character)
		{
			if (m_characterDictionary == null)
			{
				return false;
			}
			if (m_characterDictionary.ContainsKey(character))
			{
				return true;
			}
			return false;
		}

		public bool HasCharacter(char character, bool searchFallbacks)
		{
			if (m_characterDictionary == null)
			{
				return false;
			}
			if (m_characterDictionary.ContainsKey(character))
			{
				return true;
			}
			if (searchFallbacks)
			{
				if (fallbackFontAssets != null && fallbackFontAssets.Count > 0)
				{
					for (int i = 0; i < fallbackFontAssets.Count; i++)
					{
						if (fallbackFontAssets[i].characterDictionary != null && fallbackFontAssets[i].characterDictionary.ContainsKey(character))
						{
							return true;
						}
					}
				}
				if (TMP_Settings.fallbackFontAssets != null && TMP_Settings.fallbackFontAssets.Count > 0)
				{
					for (int j = 0; j < TMP_Settings.fallbackFontAssets.Count; j++)
					{
						if (TMP_Settings.fallbackFontAssets[j].characterDictionary != null && TMP_Settings.fallbackFontAssets[j].characterDictionary.ContainsKey(character))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		public bool HasCharacters(string text, out List<char> missingCharacters)
		{
			if (m_characterDictionary == null)
			{
				missingCharacters = null;
				return false;
			}
			missingCharacters = new List<char>();
			for (int i = 0; i < text.Length; i++)
			{
				if (!m_characterDictionary.ContainsKey(text[i]))
				{
					missingCharacters.Add(text[i]);
				}
			}
			if (missingCharacters.Count == 0)
			{
				return true;
			}
			return false;
		}

		public static string GetCharacters(TMP_FontAsset fontAsset)
		{
			string text = string.Empty;
			for (int i = 0; i < fontAsset.m_glyphInfoList.Count; i++)
			{
				text += (char)fontAsset.m_glyphInfoList[i].id;
			}
			return text;
		}

		public static int[] GetCharactersArray(TMP_FontAsset fontAsset)
		{
			int[] array = new int[fontAsset.m_glyphInfoList.Count];
			for (int i = 0; i < fontAsset.m_glyphInfoList.Count; i++)
			{
				array[i] = fontAsset.m_glyphInfoList[i].id;
			}
			return array;
		}
	}
}

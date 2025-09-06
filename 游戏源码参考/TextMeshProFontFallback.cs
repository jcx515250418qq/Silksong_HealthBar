using System;
using System.Collections.Generic;
using GlobalEnums;
using TMProOld;
using TeamCherry.Localization;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "TMP Font Fallback Helper", menuName = "Font/Font Fallback Helper (TMP)")]
public class TextMeshProFontFallback : FontFallbackBase
{
	[Serializable]
	private sealed class Fallback
	{
		public SupportedLanguages language = SupportedLanguages.EN;

		public List<TMP_FontAsset> fallbackFonts = new List<TMP_FontAsset>();
	}

	public TMP_FontAsset font;

	[SerializeField]
	private List<Fallback> languageFallbacks = new List<Fallback>();

	[NonSerialized]
	private bool init;

	private Dictionary<LanguageCode, Fallback> fallbacks = new Dictionary<LanguageCode, Fallback>();

	private void Init()
	{
		if (init)
		{
			return;
		}
		fallbacks.Clear();
		for (int i = 0; i < languageFallbacks.Count; i++)
		{
			Fallback fallback = languageFallbacks[i];
			if (fallback != null)
			{
				LanguageCode language = (LanguageCode)fallback.language;
				fallbacks[language] = fallback;
			}
		}
	}

	private void OnEnable()
	{
		Init();
	}

	public override void OnChangedLanguage(LanguageCode newLanguage)
	{
		Init();
		if (fallbacks.TryGetValue(newLanguage, out var value))
		{
			if (font.fallbackFontAssets == null)
			{
				font.fallbackFontAssets = new List<TMP_FontAsset>(value.fallbackFonts);
				return;
			}
			font.fallbackFontAssets.Clear();
			font.fallbackFontAssets.AddRange(value.fallbackFonts);
		}
	}
}

using System;
using System.Collections.Generic;
using GlobalEnums;
using TeamCherry.Localization;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Font Fallback Helper", menuName = "Font/Font Fallback Helper (Unity)")]
public class UnityFontFallback : FontFallbackBase
{
	[Serializable]
	private sealed class Fallback
	{
		public SupportedLanguages language = SupportedLanguages.EN;

		public string[] fontNames;
	}

	public Font font;

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
			font.fontNames = value.fontNames;
		}
	}
}

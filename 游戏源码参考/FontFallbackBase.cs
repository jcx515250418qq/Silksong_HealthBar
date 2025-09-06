using System;
using TeamCherry.Localization;
using UnityEngine;

[Serializable]
public abstract class FontFallbackBase : ScriptableObject
{
	public abstract void OnChangedLanguage(LanguageCode newLanguage);
}

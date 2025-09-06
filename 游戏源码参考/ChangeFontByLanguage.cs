using System;
using TMProOld;
using TeamCherry.Localization;
using UnityEngine;

public class ChangeFontByLanguage : MonoBehaviour
{
	public enum FontScaleLangTypes
	{
		None = 0,
		AreaName = 1,
		SubAreaName = 2,
		WideMap = 3,
		CreditsTitle = 4,
		ExcerptAuthor = 5,
		QuestType = 6,
		QuestName = 7
	}

	private class FontScaleLang
	{
		public float? fontSizeJA;

		public float? fontSizeRU;

		public float? fontSizeZH;

		public float? fontSizeKO;

		public float GetFontScale(string lang, float defaultScale)
		{
			switch (lang)
			{
			case "JA":
				if (!fontSizeJA.HasValue)
				{
					return defaultScale;
				}
				return fontSizeJA.Value;
			case "RU":
				if (!fontSizeRU.HasValue)
				{
					return defaultScale;
				}
				return fontSizeRU.Value;
			case "ZH":
				if (!fontSizeZH.HasValue)
				{
					return defaultScale;
				}
				return fontSizeZH.Value;
			case "KO":
				if (!fontSizeKO.HasValue)
				{
					return defaultScale;
				}
				return fontSizeKO.Value;
			default:
				return defaultScale;
			}
		}

		public float GetFontScale(LanguageCode lang, float defaultScale)
		{
			switch (lang)
			{
			case LanguageCode.JA:
				if (!fontSizeJA.HasValue)
				{
					return defaultScale;
				}
				return fontSizeJA.Value;
			case LanguageCode.RU:
				if (!fontSizeRU.HasValue)
				{
					return defaultScale;
				}
				return fontSizeRU.Value;
			case LanguageCode.ZH:
				if (!fontSizeZH.HasValue)
				{
					return defaultScale;
				}
				return fontSizeZH.Value;
			case LanguageCode.KO:
				if (!fontSizeKO.HasValue)
				{
					return defaultScale;
				}
				return fontSizeKO.Value;
			default:
				return defaultScale;
			}
		}
	}

	public TMP_FontAsset defaultFont;

	public TMP_FontAsset fontJA;

	public bool copyMaterialSettingsJA = true;

	public TMP_FontAsset fontRU;

	public bool copyMaterialSettingsRU = true;

	public TMP_FontAsset fontZH;

	public bool copyMaterialSettingsZH = true;

	public TMP_FontAsset fontKO;

	public bool copyMaterialSettingsKO = true;

	public bool onlyOnStart;

	[SerializeField]
	private ChangeTextFontScaleOnHandHeld changeTextFontScaleOnHandHeld;

	private new bool didAwake;

	private TextMeshPro tmpro;

	private float startFontSize;

	public FontScaleLangTypes fontScaleLangType;

	private FontScaleLang fontScaleAreaName = new FontScaleLang
	{
		fontSizeJA = 3.3f,
		fontSizeRU = 2.2f,
		fontSizeZH = 4.2f,
		fontSizeKO = 3.4f
	};

	private FontScaleLang fontScaleSubAreaName = new FontScaleLang
	{
		fontSizeJA = null,
		fontSizeRU = 2.8f,
		fontSizeZH = 4.1f,
		fontSizeKO = 3.6f
	};

	private FontScaleLang fontScaleWideMap = new FontScaleLang
	{
		fontSizeJA = 4.7f,
		fontSizeRU = 3.25f,
		fontSizeZH = 6.3f,
		fontSizeKO = 5.4f
	};

	private FontScaleLang fontScaleCreditsTitle = new FontScaleLang
	{
		fontSizeJA = null,
		fontSizeRU = 5.5f,
		fontSizeZH = null,
		fontSizeKO = null
	};

	private FontScaleLang fontScaleExcerptAuthor = new FontScaleLang
	{
		fontSizeJA = 4.5f,
		fontSizeRU = 4.5f,
		fontSizeZH = 4.5f,
		fontSizeKO = 4.5f
	};

	private FontScaleLang fontScaleQuestType = new FontScaleLang
	{
		fontSizeJA = 4f,
		fontSizeRU = 4f,
		fontSizeZH = 5f,
		fontSizeKO = 4f
	};

	private FontScaleLang fontScaleQuestName = new FontScaleLang
	{
		fontSizeJA = 6f,
		fontSizeRU = 6f,
		fontSizeZH = 5.5f,
		fontSizeKO = 6f
	};

	private Material defaultMaterial;

	private Material fallbackMaterialReference;

	public Material FallbackMaterialReference
	{
		get
		{
			return fallbackMaterialReference;
		}
		set
		{
			if (!(fallbackMaterialReference == value))
			{
				if (fallbackMaterialReference != null && fallbackMaterialReference != value)
				{
					TMP_MaterialManager.ReleaseFallbackMaterial(fallbackMaterialReference);
				}
				fallbackMaterialReference = value;
				TMP_MaterialManager.AddFallbackMaterialReference(fallbackMaterialReference);
			}
		}
	}

	private void Awake()
	{
		if (didAwake)
		{
			return;
		}
		didAwake = true;
		tmpro = GetComponent<TextMeshPro>();
		if ((bool)tmpro)
		{
			if (defaultFont == null)
			{
				defaultFont = tmpro.font;
			}
			defaultMaterial = tmpro.fontSharedMaterial;
			startFontSize = tmpro.fontSize;
		}
	}

	private void Start()
	{
		SetFont();
	}

	private void OnEnable()
	{
		if (!onlyOnStart || CheatManager.ForceLanguageComponentUpdates)
		{
			SetFont();
		}
	}

	private void OnDestroy()
	{
		FallbackMaterialReference = null;
	}

	public void SetFont()
	{
		if (!didAwake)
		{
			Awake();
		}
		if (tmpro == null)
		{
			return;
		}
		LanguageCode languageCode = Language.CurrentLanguage();
		switch (languageCode)
		{
		case LanguageCode.JA:
			tmpro.fontSize = GetFontScale(languageCode);
			SetFont(fontJA, copyMaterialSettingsJA);
			break;
		case LanguageCode.RU:
			tmpro.fontSize = GetFontScale(languageCode);
			SetFont(fontRU, copyMaterialSettingsRU);
			break;
		case LanguageCode.ZH:
			tmpro.fontSize = GetFontScale(languageCode);
			SetFont(fontZH, copyMaterialSettingsZH);
			break;
		case LanguageCode.KO:
			tmpro.fontSize = GetFontScale(languageCode);
			SetFont(fontKO, copyMaterialSettingsKO);
			break;
		default:
			tmpro.fontSize = startFontSize;
			if (defaultFont != null)
			{
				tmpro.font = defaultFont;
			}
			break;
		}
		if ((bool)changeTextFontScaleOnHandHeld)
		{
			changeTextFontScaleOnHandHeld.DoUpdate();
		}
	}

	private void SetFont(TMP_FontAsset fontAsset, bool copyMaterial)
	{
		if (fontAsset != null)
		{
			if (fontAsset != tmpro.font)
			{
				tmpro.font = fontAsset;
				if (copyMaterial)
				{
					Material fontSharedMaterial = (FallbackMaterialReference = TMP_MaterialManager.GetFallbackMaterial(defaultMaterial, tmpro.fontSharedMaterial));
					tmpro.fontSharedMaterial = fontSharedMaterial;
				}
			}
		}
		else if (tmpro.font != defaultFont)
		{
			FallbackMaterialReference = null;
			tmpro.font = defaultFont;
			tmpro.fontSharedMaterial = defaultMaterial;
		}
	}

	[Obsolete("Use GetFontScale(LanguageCode)")]
	private float GetFontScale(string lang)
	{
		return fontScaleLangType switch
		{
			FontScaleLangTypes.AreaName => fontScaleAreaName.GetFontScale(lang, startFontSize), 
			FontScaleLangTypes.SubAreaName => fontScaleSubAreaName.GetFontScale(lang, startFontSize), 
			FontScaleLangTypes.WideMap => fontScaleWideMap.GetFontScale(lang, startFontSize), 
			FontScaleLangTypes.CreditsTitle => fontScaleCreditsTitle.GetFontScale(lang, startFontSize), 
			FontScaleLangTypes.QuestType => fontScaleQuestType.GetFontScale(lang, startFontSize), 
			FontScaleLangTypes.ExcerptAuthor => fontScaleExcerptAuthor.GetFontScale(lang, startFontSize), 
			_ => startFontSize, 
		};
	}

	private float GetFontScale(LanguageCode lang)
	{
		return fontScaleLangType switch
		{
			FontScaleLangTypes.AreaName => fontScaleAreaName.GetFontScale(lang, startFontSize), 
			FontScaleLangTypes.SubAreaName => fontScaleSubAreaName.GetFontScale(lang, startFontSize), 
			FontScaleLangTypes.WideMap => fontScaleWideMap.GetFontScale(lang, startFontSize), 
			FontScaleLangTypes.CreditsTitle => fontScaleCreditsTitle.GetFontScale(lang, startFontSize), 
			FontScaleLangTypes.QuestType => fontScaleQuestType.GetFontScale(lang, startFontSize), 
			FontScaleLangTypes.ExcerptAuthor => fontScaleExcerptAuthor.GetFontScale(lang, startFontSize), 
			FontScaleLangTypes.QuestName => fontScaleQuestName.GetFontScale(lang, startFontSize), 
			_ => startFontSize, 
		};
	}
}

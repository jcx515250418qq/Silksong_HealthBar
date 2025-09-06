using System.Collections.Generic;
using TeamCherry.Localization;
using UnityEngine;

public sealed class FontManager : MonoBehaviour
{
	public delegate void LanguageChangedHandler(LanguageCode lang);

	private static FontManager instance;

	private static LanguageCode _currentLanguage;

	[SerializeField]
	private List<FontFallbackBase> fallbacks = new List<FontFallbackBase>();

	private bool started;

	private bool hasSetLanguage;

	public static LanguageCode CurrentLanguage
	{
		get
		{
			return _currentLanguage;
		}
		set
		{
			if (_currentLanguage != value)
			{
				_currentLanguage = value;
				FontManager.OnLanguageChanged?.Invoke(value);
			}
		}
	}

	public static event LanguageChangedHandler OnLanguageChanged;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		started = true;
		CurrentLanguage = Language.CurrentLanguage();
		ChangedLanguage(CurrentLanguage);
		hasSetLanguage = true;
	}

	private void OnEnable()
	{
		if (started && !hasSetLanguage)
		{
			CurrentLanguage = Language.CurrentLanguage();
			ChangedLanguage(CurrentLanguage);
			hasSetLanguage = true;
		}
	}

	private void OnDestroy()
	{
		if (instance == this)
		{
			instance = null;
		}
	}

	public void ChangedLanguage(LanguageCode newLanguage)
	{
		CurrentLanguage = newLanguage;
		foreach (FontFallbackBase fallback in fallbacks)
		{
			fallback.OnChangedLanguage(newLanguage);
		}
	}

	private void ForceUpdate()
	{
		ChangedLanguage(CurrentLanguage);
	}

	public static void ForceUpdateLanguage()
	{
		if (instance != null)
		{
			instance.ForceUpdate();
		}
	}
}

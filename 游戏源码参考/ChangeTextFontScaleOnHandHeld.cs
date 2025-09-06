using System;
using System.Collections.Generic;
using GlobalEnums;
using TMProOld;
using TeamCherry.Localization;
using UnityEngine;
using UnityEngine.UI;

public sealed class ChangeTextFontScaleOnHandHeld : MonoBehaviour
{
	[Serializable]
	private struct Override
	{
		public SupportedLanguages languageCode;

		public float normalSize;

		public float handHeldSize;
	}

	[SerializeField]
	private Text text;

	[SerializeField]
	private TMP_Text tmpText;

	[SerializeField]
	private float normalSize;

	[SerializeField]
	private float handHeldSize;

	[SerializeField]
	private List<Override> languageOverrides = new List<Override>();

	private Dictionary<LanguageCode, Override> languageCodeOverrides = new Dictionary<LanguageCode, Override>();

	private bool hasStarted;

	private bool hasText;

	private bool hasTmpText;

	private void Awake()
	{
		hasText = text != null;
		if (!hasText)
		{
			text = GetComponent<Text>();
			hasText = text != null;
		}
		if (!hasText)
		{
			hasTmpText = tmpText != null;
			if (!hasTmpText)
			{
				tmpText = GetComponent<TMP_Text>();
				hasTmpText = tmpText != null;
			}
		}
		if (!hasTmpText && !hasText)
		{
			base.enabled = false;
		}
		else
		{
			CreateLookup();
		}
	}

	private void Start()
	{
		hasStarted = true;
		DoUpdate();
	}

	private void OnValidate()
	{
		if (text == null)
		{
			text = GetComponent<Text>();
		}
		if (tmpText == null)
		{
			tmpText = GetComponent<TMP_Text>();
		}
		if (Application.isPlaying && hasStarted)
		{
			CreateLookup(log: false);
		}
	}

	private void OnEnable()
	{
		if (hasStarted)
		{
			DoUpdate();
		}
		Platform.Current.OnScreenModeChanged += OnScreenModeChanged;
		GameManager instance = GameManager.instance;
		if (instance != null)
		{
			instance.RefreshLanguageText += DoUpdate;
		}
	}

	private void OnDisable()
	{
		Platform.Current.OnScreenModeChanged -= OnScreenModeChanged;
		GameManager instance = GameManager.instance;
		if (instance != null)
		{
			instance.RefreshLanguageText -= DoUpdate;
		}
	}

	private void CreateLookup(bool log = true)
	{
		foreach (Override languageOverride in languageOverrides)
		{
			LanguageCode languageCode = (LanguageCode)languageOverride.languageCode;
			if (!languageCodeOverrides.ContainsKey(languageCode))
			{
				languageCodeOverrides[languageCode] = languageOverride;
			}
		}
	}

	public void DoUpdate()
	{
		if (hasStarted)
		{
			bool scale = false;
			if (Platform.Current.IsRunningOnHandHeld)
			{
				scale = true;
			}
			SetScale(scale);
		}
	}

	private void SetScale(bool isHandHeld)
	{
		float num = (isHandHeld ? handHeldSize : normalSize);
		if (languageCodeOverrides.TryGetValue(Language.CurrentLanguage(), out var value))
		{
			num = (isHandHeld ? value.handHeldSize : value.normalSize);
		}
		if (hasText)
		{
			text.fontSize = (int)num;
		}
		if (hasTmpText)
		{
			tmpText.fontSize = num;
		}
	}

	public void ChangeLanguage(LanguageCode languageCode)
	{
		DoUpdate();
	}

	private void OnScreenModeChanged(Platform.ScreenModeState screenMode)
	{
		SetScale(screenMode >= Platform.ScreenModeState.HandHeld);
	}
}

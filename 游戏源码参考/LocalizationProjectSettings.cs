using System;
using TeamCherry.Localization;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class LocalizationProjectSettings : LocalizationProjectSettingsBase
{
	[Serializable]
	public struct BoxTest
	{
		public string[] IncludeTitles;

		public AssetReferenceGameObject TextBoxPrefab;
	}

	[SerializeField]
	private string[] excludeSheets;

	[SerializeField]
	private string[] excludeKeys;

	[SerializeField]
	private BoxTest[] boxTests;

	private const string LAST_LANGUAGE_KEY = "M2H_lastLanguage";

	public override bool TryGetSavedLanguageCode(out string languageCode)
	{
		if ((bool)Platform.Current && Platform.Current.LocalSharedData.HasKey("M2H_lastLanguage"))
		{
			languageCode = Platform.Current.LocalSharedData.GetString("M2H_lastLanguage", "");
			return true;
		}
		languageCode = LanguageCode.EN.ToString();
		return false;
	}

	public override SystemLanguage GetSystemLanguage()
	{
		return Platform.Current.GetSystemLanguage();
	}

	public override void OnSwitchedLanguage(LanguageCode newLang)
	{
		if ((bool)Platform.Current)
		{
			Platform.Current.LocalSharedData.SetString("M2H_lastLanguage", newLang.ToString() ?? "");
			Platform.Current.LocalSharedData.Save();
		}
	}

	public override bool ShouldCheckText(string sheetTitle, string key)
	{
		string[] array = excludeSheets;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == sheetTitle)
			{
				return false;
			}
		}
		array = excludeKeys;
		foreach (string value in array)
		{
			if (key.Contains(value))
			{
				return false;
			}
		}
		return true;
	}

	public override bool IsTextOverflowing(string sheetTitle, string text)
	{
		return false;
	}
}

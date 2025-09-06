using System;
using GlobalEnums;
using TeamCherry.Localization;
using UnityEngine;

public class ActivatePerLanguage : MonoBehaviour
{
	[Serializable]
	public struct LangBoolPair
	{
		public SupportedLanguages language;

		public bool activate;
	}

	public GameObject target;

	public GameObject alt;

	[Space]
	public LangBoolPair[] languages;

	[Space]
	public bool defaultActivation = true;

	private void Start()
	{
		UpdateLanguage();
	}

	public void UpdateLanguage()
	{
		SupportedLanguages supportedLanguages = (SupportedLanguages)Language.CurrentLanguage();
		bool activate = defaultActivation;
		LangBoolPair[] array = languages;
		for (int i = 0; i < array.Length; i++)
		{
			LangBoolPair langBoolPair = array[i];
			if (langBoolPair.language == supportedLanguages)
			{
				activate = langBoolPair.activate;
				break;
			}
		}
		if ((bool)target)
		{
			target.SetActive(activate);
		}
		if ((bool)alt)
		{
			alt.SetActive(!activate);
		}
	}
}

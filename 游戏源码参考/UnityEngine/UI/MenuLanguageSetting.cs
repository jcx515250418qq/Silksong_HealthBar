using System;
using System.Collections.Generic;
using GlobalEnums;
using HKMenu;
using TeamCherry.Localization;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	public class MenuLanguageSetting : MenuOptionHorizontal, IMoveHandler, IEventSystemHandler, IMenuOptionListSetting, IPointerClickHandler, ISubmitHandler
	{
		private static Dictionary<LanguageCode, SupportedLanguages> languageCodeToSupportedLanguages = new Dictionary<LanguageCode, SupportedLanguages>();

		private static int languageState;

		private static SupportedLanguages[] langs;

		private new static string[] optionList;

		private GameSettings gs;

		public FixVerticalAlign textAligner;

		private new void OnEnable()
		{
			RefreshControls();
			UpdateAlpha();
		}

		public void UpdateAlpha()
		{
			CanvasGroup component = GetComponent<CanvasGroup>();
			if ((bool)component)
			{
				if (!base.interactable)
				{
					component.alpha = 0.5f;
				}
				else
				{
					component.alpha = 1f;
				}
			}
		}

		public new void OnMove(AxisEventData move)
		{
			if (base.interactable)
			{
				if (MoveOption(move.moveDir))
				{
					UpdateLanguageSetting();
				}
				else
				{
					base.OnMove(move);
				}
			}
		}

		public new void OnPointerClick(PointerEventData eventData)
		{
			if (base.interactable)
			{
				PointerClickCheckArrows(eventData);
				UpdateLanguageSetting();
			}
		}

		public new void OnSubmit(BaseEventData eventData)
		{
			MoveOption(MoveDirection.Right);
			UpdateLanguageSetting();
		}

		public static Rect RectTransformToScreenSpace(RectTransform transform)
		{
			Vector2 vector = Vector2.Scale(transform.rect.size, transform.lossyScale);
			return new Rect(transform.position.x, (float)Screen.height - transform.position.y, vector.x, vector.y);
		}

		public void RefreshControls()
		{
			RefreshAvailableLanguages();
			RefreshCurrentIndex();
			PushUpdateOptionList();
			UpdateText();
		}

		private void UpdateLanguageSetting()
		{
			GameManager.instance.gameSettings.gameLanguage = langs[selectedOptionIndex];
			Language.SwitchLanguage((LanguageCode)langs[selectedOptionIndex]);
			gm.RefreshLocalization();
			UpdateText();
		}

		private static void RefreshAvailableLanguages()
		{
			if (GameManager.instance.gameConfig.hideLanguageOption)
			{
				if (languageState != 1)
				{
					langs = Enum.GetValues(typeof(SupportedLanguages)) as SupportedLanguages[];
					if (langs != null && langs.Length != 0)
					{
						languageState = 1;
						CreateLanguageMap();
						UpdateLangsArray();
					}
					else
					{
						languageState = 0;
					}
				}
			}
			else if (languageState != 2)
			{
				langs = Enum.GetValues(typeof(SupportedLanguages)) as SupportedLanguages[];
				if (langs != null && langs.Length != 0)
				{
					languageState = 2;
					CreateLanguageMap();
					UpdateLangsArray();
				}
				else
				{
					languageState = 0;
				}
			}
		}

		private static void CreateLanguageMap()
		{
			languageCodeToSupportedLanguages.Clear();
			SupportedLanguages[] array = langs;
			for (int i = 0; i < array.Length; i++)
			{
				SupportedLanguages value = array[i];
				if (Enum.TryParse<LanguageCode>(value.ToString(), out var result))
				{
					languageCodeToSupportedLanguages[result] = value;
				}
			}
		}

		public override void RefreshCurrentIndex()
		{
			bool flag = false;
			if (languageCodeToSupportedLanguages.TryGetValue(Language.CurrentLanguage(), out var value))
			{
				for (int i = 0; i < langs.Length; i++)
				{
					if (value == langs[i])
					{
						selectedOptionIndex = i;
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				Debug.LogError("Couldn't find currently active language");
			}
			base.RefreshCurrentIndex();
		}

		public void PushUpdateOptionList()
		{
			SetOptionList(optionList);
		}

		private static void UpdateLangsArray()
		{
			optionList = new string[langs.Length];
			for (int i = 0; i < langs.Length; i++)
			{
				optionList[i] = langs[i].ToString();
			}
		}

		protected override void UpdateText()
		{
			if (optionList != null && optionText != null)
			{
				try
				{
					optionText.text = Language.Get("LANG_CURRENT", sheetTitle);
				}
				catch (Exception ex)
				{
					Debug.LogError(optionText.text + " : " + optionList?.ToString() + " : " + selectedOptionIndex + " " + ex);
				}
				optionText.GetComponent<FixVerticalAlign>().AlignText();
			}
		}
	}
}

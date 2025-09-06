using System;
using System.Collections.Generic;
using GlobalEnums;
using UnityEngine;

namespace TeamCherry.Localization.Platform
{
	[CreateAssetMenu(fileName = "LocalizationData", menuName = "Localization/Xbox Localization Data")]
	public sealed class XboxLocalizationData : ScriptableObject
	{
		[Serializable]
		public class LocaleLink
		{
			public SupportedLanguages language;

			public string locale;
		}

		public string devDisplayLocale;

		[NamedArray("GetElementName")]
		public List<LocalizedStringEntry> localizedStrings = new List<LocalizedStringEntry>();

		public List<LocaleLink> languages = new List<LocaleLink>();

		[NonSerialized]
		private Dictionary<string, LocalizedStringEntry> localizedStringsLookup = new Dictionary<string, LocalizedStringEntry>();

		[NonSerialized]
		private bool isValid;

		private void OnEnable()
		{
			isValid = false;
		}

		private void OnValidate()
		{
			isValid = false;
		}

		private void UpdateLookup()
		{
			if (!isValid)
			{
				bool flag = localizedStringsLookup.Count == 0;
				for (int i = 0; i < localizedStrings.Count; i++)
				{
					LocalizedStringEntry localizedStringEntry = localizedStrings[i];
					localizedStringsLookup[localizedStringEntry.id] = localizedStringEntry;
				}
				isValid = true;
			}
		}

		public void MergeEntry(LocalizedStringEntry incoming)
		{
			UpdateLookup();
			if (!localizedStringsLookup.TryGetValue(incoming.id, out var value))
			{
				localizedStringsLookup[incoming.id] = incoming;
				localizedStrings.Add(incoming);
			}
			else
			{
				value.MergeValues(incoming);
			}
		}

		public LocalizedStringEntry GetById(string id)
		{
			UpdateLookup();
			if (!localizedStringsLookup.TryGetValue(id, out var value))
			{
				Dictionary<string, LocalizedStringEntry> dictionary = localizedStringsLookup;
				LocalizedStringEntry obj = new LocalizedStringEntry
				{
					id = id
				};
				value = obj;
				dictionary[id] = obj;
				localizedStrings.Add(value);
			}
			return value;
		}

		public bool TryGetLocalizedStringEntry(string id, out LocalizedStringEntry value)
		{
			UpdateLookup();
			return localizedStringsLookup.TryGetValue(id, out value);
		}

		private string GetElementName(int index)
		{
			try
			{
				LocalizedStringEntry localizedStringEntry = localizedStrings[index];
				return $"{index}: !!{localizedStringEntry.localisedString.Sheet}/{localizedStringEntry.localisedString.Key}!! : {localizedStringEntry.id}";
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
			return $"Element {index}";
		}

		private bool AuditLanguages()
		{
			HashSet<SupportedLanguages> hashSet = new HashSet<SupportedLanguages>();
			HashSet<string> hashSet2 = new HashSet<string>();
			bool result = true;
			for (int i = 0; i < languages.Count; i++)
			{
				LocaleLink localeLink = languages[i];
				if (!hashSet.Add(localeLink.language))
				{
					Debug.LogError($"#{i} contains duplicated language code {localeLink.language}");
					result = false;
				}
				if (!hashSet2.Add(localeLink.locale))
				{
					Debug.LogError($"#{i} contains duplicated locale {localeLink.locale}");
					result = false;
				}
			}
			return result;
		}

		[ContextMenu("Update Localised Strings")]
		public void UpdateLocalisedStrings()
		{
			if (!AuditLanguages())
			{
				Debug.LogError("Languages list contains errors, Please fix before trying again.", this);
				return;
			}
			LanguageCode code = Language.CurrentLanguage();
			try
			{
				foreach (LocaleLink language in languages)
				{
					try
					{
						if (Language.SwitchLanguage((LanguageCode)language.language))
						{
							string locale = language.locale;
							for (int i = 0; i < localizedStrings.Count; i++)
							{
								LocalizedStringEntry localizedStringEntry = localizedStrings[i];
								if (localizedStringEntry.localisedString.IsEmpty)
								{
									Debug.LogError($"#{i} Unable to update {localizedStringEntry.id} missing localised string");
									continue;
								}
								localizedStringEntry.AddOrUpdate(new LocalizedValue
								{
									locale = locale,
									text = localizedStringEntry.localisedString.ToString()
								});
							}
						}
						else
						{
							Debug.LogError($"Failed to switch language to {language.language}. Skipping.", this);
						}
					}
					catch (Exception arg)
					{
						Debug.LogError($"Encountered error while updating localisation for {language.language} - {language.locale} : {arg}", this);
					}
				}
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
			finally
			{
				Language.SwitchLanguage(code);
			}
		}
	}
}

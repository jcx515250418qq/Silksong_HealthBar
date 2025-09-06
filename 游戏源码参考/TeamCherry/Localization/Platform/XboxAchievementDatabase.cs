using System;
using System.Collections.Generic;
using UnityEngine;

namespace TeamCherry.Localization.Platform
{
	[CreateAssetMenu(menuName = "Localization/Xbox Achievement Database")]
	public sealed class XboxAchievementDatabase : ScriptableObject
	{
		private const string SHEET = "Achievements";

		[NamedArray("GetElementName")]
		public List<AchievementEntry> achievements = new List<AchievementEntry>();

		public AssetLinker<XboxLocalizationData> xboxLocalizationData;

		public AssetLinker<AchievementIDMap> xboxAchievementIDMap;

		[NonSerialized]
		private Dictionary<string, AchievementEntry> achievementLookup = new Dictionary<string, AchievementEntry>();

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
				achievementLookup.Clear();
				for (int i = 0; i < achievements.Count; i++)
				{
					AchievementEntry achievementEntry = achievements[i];
					achievementLookup[achievementEntry.stableGuid] = achievementEntry;
				}
				isValid = true;
			}
		}

		public AchievementEntry GetOrCreateEntryByGuid(string guid)
		{
			UpdateLookup();
			if (achievementLookup.TryGetValue(guid, out var value))
			{
				return value;
			}
			AchievementEntry achievementEntry = new AchievementEntry
			{
				stableGuid = guid
			};
			achievementLookup[guid] = achievementEntry;
			achievements.Add(achievementEntry);
			return achievementEntry;
		}

		private string GetElementName(int index)
		{
			try
			{
				AchievementEntry achievementEntry = achievements[index];
				return $"{index + 1}: {achievementEntry.internalAchievementID} : {achievementEntry.stableGuid}";
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
			return $"Element {index}";
		}

		private static string PrintLocalisedString(LocalisedString localisedString)
		{
			return "!!" + localisedString.Sheet + "/" + localisedString.Key + "!!";
		}

		[ContextMenu("Export Internal ID to Localisation Asset")]
		private void ExportInternalIDToLocalisation()
		{
			XboxLocalizationData localisation = xboxLocalizationData.Asset;
			if (!localisation)
			{
				return;
			}
			foreach (AchievementEntry achievement in achievements)
			{
				AchievementEntry achievementEntry = achievement;
				if (!string.IsNullOrEmpty(achievementEntry.internalAchievementID))
				{
					AddLocalisation(achievementEntry.achievementNameId, "_NAME");
					AddLocalisation(achievementEntry.unlockedDescriptionId, "_DESC");
					AddLocalisation(achievementEntry.lockedDescriptionId, "_DESC");
				}
				void AddLocalisation(string xboxLocalisationID, string suffix)
				{
					string text = achievementEntry.internalAchievementID + suffix;
					if (localisation.TryGetLocalizedStringEntry(xboxLocalisationID, out var value))
					{
						LocalisedString localisedString = new LocalisedString("Achievements", text);
						if ((string)value.localisedString != (string)localisedString)
						{
							Debug.Log(xboxLocalisationID + " localised string changed from " + PrintLocalisedString(value.localisedString) + " to " + PrintLocalisedString(localisedString), localisation);
							value.localisedString = localisedString;
						}
					}
					else
					{
						Debug.LogError($"#{achievementEntry.achievementId} : {achievementEntry.internalAchievementID} : Did not find {xboxLocalisationID} for {text} in {localisation}", localisation);
					}
				}
			}
		}

		[ContextMenu("Import Internal ID")]
		private void ImportInternalID()
		{
			if (!(xboxAchievementIDMap.Asset != null))
			{
				Debug.Log("Xbox id map not assigned");
			}
		}
	}
}

using System;
using System.Collections.Generic;
using TeamCherry.Localization;
using UnityEngine;

namespace MyGame.Editor.TrophyExport
{
	[CreateAssetMenu(menuName = "Localization/PS4 Trophy Localisation Data")]
	public sealed class PS4TrophyLocalisation : ScriptableObject
	{
		[Serializable]
		public class TrophyEntrySource
		{
			public int id;

			public LocalisedString name;

			public LocalisedString description;
		}

		public string sheet = "Achievements";

		public LocalisedString title;

		public LocalisedString description;

		public List<TrophyEntrySource> trophyEntries = new List<TrophyEntrySource>();

		public AssetLinker<AchievementIDMap> achievementIdMap;

		[ContextMenu("Link Internal ID")]
		private void LinkInternalID()
		{
		}
	}
}

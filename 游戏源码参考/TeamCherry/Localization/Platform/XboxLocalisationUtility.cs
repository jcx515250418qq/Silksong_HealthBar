using System.IO;
using UnityEngine;

namespace TeamCherry.Localization.Platform
{
	public static class XboxLocalisationUtility
	{
		public static readonly string MAIN_DIRECTORY = Path.Combine(Application.dataPath, "Data Assets/Localisation");

		public static readonly string XML_DIRECTORY = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Xbox", "Online Config Backup");

		public static readonly string ACHIEVEMENT_FILE_NAME = "achievements2017";

		public static readonly string LOCALISATION_FILE_NAME = "localization";

		public static readonly string ACHIEVEMENT_ASSET_PATH = "Assets/Data Assets/Localisation/Xbox Achievement Database.asset";

		public static readonly string LOCALISATION_ASSET_PATH = "Assets/Data Assets/Localisation/Xbox Localisation Database.asset";
	}
}

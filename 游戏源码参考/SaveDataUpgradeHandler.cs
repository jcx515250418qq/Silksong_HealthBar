using System;
using System.Collections.Generic;

public static class SaveDataUpgradeHandler
{
	private class SceneSplit
	{
		public string SceneName { get; private set; }

		public string Version { get; private set; }

		public string[] NewSceneNames { get; private set; }

		public SceneSplit(string sceneName, string version, params string[] newSceneNames)
		{
			SceneName = sceneName;
			Version = version;
			NewSceneNames = newSceneNames;
		}

		public bool ShouldHandleSplit(string otherVersion)
		{
			otherVersion = SaveDataUtility.CleanupVersionText(otherVersion);
			Version version = new Version(Version);
			Version value = new Version(otherVersion);
			return version.CompareTo(value) > 0;
		}
	}

	private struct SystemDataUpgrade
	{
		public Version TargetVersion;

		public Action<object> UpgradeAction;
	}

	private static readonly SceneSplit[] _splitScenes = new SceneSplit[0];

	private static readonly Dictionary<Type, SystemDataUpgrade> systemDataUpgrades = new Dictionary<Type, SystemDataUpgrade>();

	private static void ClearDreamGate(SceneSplit sceneSplit, ref string dreamGateScene)
	{
		if (sceneSplit.SceneName == dreamGateScene)
		{
			dreamGateScene = "";
		}
	}

	private static void UpdateMap(SceneSplit sceneSplit, ref HashSet<string> scenesMapped)
	{
		if (scenesMapped.Contains(sceneSplit.SceneName))
		{
			string[] newSceneNames = sceneSplit.NewSceneNames;
			foreach (string item in newSceneNames)
			{
				scenesMapped.Add(item);
			}
		}
	}

	public static void UpgradeSaveData(ref PlayerData playerData)
	{
		SceneSplit[] splitScenes = _splitScenes;
		foreach (SceneSplit sceneSplit in splitScenes)
		{
			if (sceneSplit.ShouldHandleSplit(playerData.version))
			{
				UpdateMap(sceneSplit, ref playerData.scenesMapped);
			}
		}
	}

	public static void UpgradeSystemData<T>(T system)
	{
		Type typeFromHandle = typeof(T);
		if (systemDataUpgrades.ContainsKey(typeFromHandle))
		{
			string key = $"lastSystemVersion_{typeFromHandle}";
			Version version = new Version(Platform.Current.LocalSharedData.GetString(key, "0.0.0.0"));
			SystemDataUpgrade systemDataUpgrade = systemDataUpgrades[typeFromHandle];
			if (!(version >= systemDataUpgrade.TargetVersion))
			{
				systemDataUpgrade.UpgradeAction(system);
				Platform.Current.LocalSharedData.SetString(key, "1.0.28324");
			}
		}
	}
}

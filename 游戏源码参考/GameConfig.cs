using System;
using UnityEngine;

[Serializable]
public class GameConfig : ScriptableObject
{
	public bool showTargetSceneNamesOnGates;

	public bool enableDebugButtons;

	public bool enableCheats;

	public bool disableSaveGame;

	public bool useSaveEncryption;

	public bool enableDemoMode;

	public bool steamEnabled;

	public bool galaxyEnabled;

	public bool clearRecordsOnStart;

	public bool unlockPermadeathMode;

	public bool unlockBossRushMode;

	public bool clearPreferredLanguageSetting;

	public bool unlockAllMenuStyles;

	public bool hideAchievementsMenu;

	public bool hideExtrasMenu;

	public bool hideKeyboardMenu;

	public bool hideLanguageOption;

	public bool nativeAchievementsSettingAlwaysOn;

	public bool hideVsyncSetting;

	public bool enableTFRSetting;

	public bool hideMonitorSetting;
}

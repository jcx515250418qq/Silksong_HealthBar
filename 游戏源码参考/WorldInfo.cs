using System;

public static class WorldInfo
{
	public static readonly string[] MenuScenes = new string[3] { "Pre_Menu_Intro", "Menu_Title", "BetaEnd" };

	public static readonly string[] NonGameplayScenes = new string[20]
	{
		"Opening_Sequence", "Opening_Sequence_Act3", "Prologue_Excerpt", "Intro_Cutscene_Prologue", "Intro_Cutscene", "Cinematic_Stag_travel", "Cinematic_Ending_A", "Cinematic_Ending_B", "Cinematic_Ending_C", "End_Credits",
		"End_Credits_Scroll", "Cinematic_MrMushroom", "Menu_Credits", "End_Game_Completion", "PermaDeath", "PermaDeath_Unlock", "Cinematic_Ending_D", "Cinematic_Ending_E", "Demo Start", "Cinematic_Submarine_travel"
	};

	public static readonly string[] SubSceneNameSuffixes = new string[12]
	{
		"_boss_defeated", "_boss", "_preload", "_bellway", "_mapper", "_boss_golem", "_boss_golem_rest", "_boss_beastfly", "_additive", "_pre",
		"_caravan", "_festival"
	};

	public static bool NameLooksLikeAdditiveLoadScene(string sceneName)
	{
		string[] subSceneNameSuffixes = SubSceneNameSuffixes;
		foreach (string value in subSceneNameSuffixes)
		{
			if (sceneName.EndsWith(value, StringComparison.InvariantCultureIgnoreCase))
			{
				return true;
			}
		}
		return false;
	}
}

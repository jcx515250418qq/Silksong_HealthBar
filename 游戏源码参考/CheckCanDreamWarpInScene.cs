using System;
using System.Collections.Generic;
using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight")]
public class CheckCanDreamWarpInScene : FSMUtility.CheckFsmStateAction
{
	private static Func<bool> bossRushCheck = () => !GameManager.instance.playerData.bossRushMode;

	private Dictionary<string, Func<bool>> sceneCheckFunctions = new Dictionary<string, Func<bool>>
	{
		{ "GG_Atrium", bossRushCheck },
		{ "GG_Atrium_Roof", bossRushCheck },
		{ "GG_Workshop", bossRushCheck },
		{ "GG_Blue_Room", bossRushCheck },
		{
			"GG_Land_of_Storms",
			() => false
		},
		{
			"GG_Unlock_Wastes",
			() => false
		}
	};

	public override bool IsTrue
	{
		get
		{
			string sceneNameString = GameManager.instance.GetSceneNameString();
			if (sceneCheckFunctions.ContainsKey(sceneNameString))
			{
				return sceneCheckFunctions[sceneNameString]();
			}
			return true;
		}
	}
}

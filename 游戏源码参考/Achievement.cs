using System;
using GlobalEnums;
using UnityEngine;

[Serializable]
public class Achievement
{
	public string PlatformKey;

	public AchievementType Type;

	public Sprite Icon;

	public string TitleCell => PlatformKey + "_NAME";

	public string DescriptionCell => PlatformKey + "_DESC";
}

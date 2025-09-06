using System;
using GlobalEnums;

[Serializable]
public class SaveStats
{
	private PlayTime playTimeStruct;

	public readonly SaveGameData saveGameData;

	public string Version { get; private set; }

	public int RevisionBreak { get; private set; }

	public int MaxHealth { get; private set; }

	public int MaxSilk { get; private set; }

	public bool IsSpoolBroken { get; private set; }

	public int Geo { get; private set; }

	public int Shards { get; private set; }

	public MapZone MapZone { get; private set; }

	public ExtraRestZones ExtraRestZone { get; private set; }

	public BellhomePaintColours BellhomePaintColour { get; private set; }

	public float PlayTime { get; private set; }

	public PermadeathModes PermadeathMode { get; private set; }

	public bool BossRushMode { get; private set; }

	public float CompletionPercentage { get; private set; }

	public bool UnlockedCompletionRate { get; private set; }

	public bool IsBlackThreadInfected { get; private set; }

	public bool HasClearedBlackThreads { get; set; }

	public bool IsAct3 { get; private set; }

	public bool IsAct3IntroCompleted { get; private set; }

	public string CrestId { get; private set; }

	public SaveSlotCompletionIcons.CompletionState CompletedEndings { get; private set; }

	public SaveSlotCompletionIcons.CompletionState LastCompletedEnding { get; private set; }

	public bool IsBlank { get; private set; }

	public static SaveStats Blank => new SaveStats
	{
		IsBlank = true
	};

	public SaveStats(PlayerData playerData, SaveGameData saveGameData)
	{
		Version = playerData.version;
		RevisionBreak = playerData.RevisionBreak;
		MaxHealth = playerData.maxHealthBase;
		IsSpoolBroken = playerData.IsSilkSpoolBroken;
		MaxSilk = playerData.CurrentSilkMaxBasic;
		Geo = playerData.geo;
		Shards = playerData.ShellShards;
		MapZone = playerData.mapZone;
		ExtraRestZone = playerData.extraRestZone;
		BellhomePaintColour = playerData.BelltownHouseColour;
		PlayTime = playerData.playTime;
		playTimeStruct.RawTime = playerData.playTime;
		PermadeathMode = playerData.permadeathMode;
		BossRushMode = playerData.bossRushMode;
		CompletionPercentage = playerData.completionPercentage;
		UnlockedCompletionRate = playerData.ConstructedFarsight;
		IsBlackThreadInfected = playerData.IsAct3IntroQueued;
		IsAct3 = playerData.blackThreadWorld;
		IsAct3IntroCompleted = playerData.blackThreadWorld && playerData.act3_enclaveWakeSceneCompleted;
		CrestId = playerData.CurrentCrestID;
		this.saveGameData = saveGameData;
		CompletedEndings = playerData.CompletedEndings;
		LastCompletedEnding = playerData.LastCompletedEnding;
	}

	private SaveStats()
	{
	}

	public string GetPlaytimeHHMM()
	{
		if (playTimeStruct.HasHours)
		{
			return $"{(int)playTimeStruct.Hours:0}h {(int)playTimeStruct.Minutes:00}m";
		}
		return $"{(int)playTimeStruct.Minutes:0}m";
	}

	public string GetPlaytimeHHMMSS()
	{
		if (!playTimeStruct.HasHours)
		{
			return $"{(int)playTimeStruct.Minutes:0}m {(int)playTimeStruct.Seconds:00}s";
		}
		if (!playTimeStruct.HasMinutes)
		{
			return $"{(int)playTimeStruct.Seconds:0}s";
		}
		return $"{(int)playTimeStruct.Hours:0}h {(int)playTimeStruct.Minutes:00}m {(int)playTimeStruct.Seconds:00}s";
	}

	public string GetCompletionPercentage()
	{
		return CompletionPercentage + "%";
	}
}

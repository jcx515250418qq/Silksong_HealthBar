using System;
using TeamCherry.Localization;

[Serializable]
public sealed class RestorePointData
{
	public SaveGameData saveGameData;

	public string date;

	public string version;

	public AutoSaveName autoSaveName;

	private static readonly LocalisedString INCOMPATIBLE = new LocalisedString("MainMenu", "PROFILE_INCOMPATIBLE");

	private bool isVersionBackup;

	public RestorePointData(SaveGameData saveGameData, AutoSaveName autoSaveName)
	{
		this.autoSaveName = autoSaveName;
		this.saveGameData = saveGameData;
	}

	private RestorePointData(SaveGameData saveGameData)
	{
		this.saveGameData = saveGameData;
		version = saveGameData.playerData.version;
		date = saveGameData.playerData.date;
		isVersionBackup = true;
	}

	public static RestorePointData CreateVersionBackup(SaveGameData saveGameData)
	{
		if (saveGameData == null || saveGameData.playerData == null)
		{
			return null;
		}
		return new RestorePointData(saveGameData)
		{
			date = saveGameData.playerData.date,
			version = saveGameData.playerData.version
		};
	}

	public RestorePointData()
	{
	}

	public void SetDateString()
	{
		date = GetDateString();
	}

	public void SetVersion()
	{
		version = "1.0.28324";
	}

	private static string GetDateString()
	{
		return DateTime.Now.ToString("yyyy/MM/dd");
	}

	public bool IsValid()
	{
		if (saveGameData != null)
		{
			return saveGameData.playerData != null;
		}
		return false;
	}

	public string GetName()
	{
		if (!IsValid())
		{
			return INCOMPATIBLE;
		}
		if (isVersionBackup)
		{
			return version;
		}
		if (autoSaveName != 0)
		{
			return GameManager.GetFormattedAutoSaveNameString(autoSaveName);
		}
		return GameManager.GetFormattedMapZoneStringV2(saveGameData.playerData.mapZone);
	}

	public string GetDateTime()
	{
		if (!IsValid())
		{
			return date;
		}
		return date + " - " + new PlayTime
		{
			RawTime = saveGameData.playerData.playTime
		}.ToString();
	}
}

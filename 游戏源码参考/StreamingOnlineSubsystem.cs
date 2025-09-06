using System.IO;

public class StreamingOnlineSubsystem : DesktopOnlineSubsystem
{
	public override bool AreAchievementsFetched => true;

	public override bool LimitedGraphicsSettings => true;

	public static bool IsPackaged(DesktopPlatform desktopPlatform)
	{
		return desktopPlatform.IncludesPlugin(Path.Combine("x86_64", "IsStreamingBuild"));
	}

	public override bool? IsAchievementUnlocked(string achievementId)
	{
		return null;
	}

	public override void PushAchievementUnlock(string achievementId)
	{
	}

	public override void UpdateAchievementProgress(string achievementId, int value, int max)
	{
	}

	public override void ResetAchievements()
	{
	}
}

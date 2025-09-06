public struct AchievementState
{
	public bool isValid;

	public bool isUnlocked;

	public bool isProgressive;

	public long progressValue;

	public long maxValue;

	public static AchievementState Invalid
	{
		get
		{
			AchievementState result = default(AchievementState);
			result.isValid = false;
			return result;
		}
	}
}

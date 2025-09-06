public class MenuButtonAchievementListCondition : MenuButtonListCondition
{
	public override bool IsFulfilled()
	{
		if (!Platform.Current.HasNativeAchievementsDialog)
		{
			return !DemoHelper.IsDemoMode;
		}
		return false;
	}
}

public class MenuButtonKeyboardListCondition : MenuButtonListCondition
{
	public override bool IsFulfilled()
	{
		if (!DemoHelper.IsDemoMode)
		{
			return Platform.Current.WillDisplayKeyboardSettings;
		}
		return false;
	}
}

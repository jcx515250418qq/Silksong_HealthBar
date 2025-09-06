public sealed class MenuButtonLimitedGraphicsCondition : MenuButtonListCondition
{
	public override bool IsFulfilled()
	{
		return !Platform.Current.LimitedGraphicsSettings;
	}
}

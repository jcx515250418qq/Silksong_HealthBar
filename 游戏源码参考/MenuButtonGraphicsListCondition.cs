public class MenuButtonGraphicsListCondition : MenuButtonListCondition
{
	public override bool IsFulfilled()
	{
		return Platform.Current.WillDisplayGraphicsSettings;
	}
}

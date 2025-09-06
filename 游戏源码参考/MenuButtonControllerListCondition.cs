public class MenuButtonControllerListCondition : MenuButtonListCondition
{
	public override bool IsFulfilled()
	{
		return Platform.Current.WillDisplayControllerSettings;
	}
}

public class MenuButtonSwitchUserListCondition : MenuButtonListCondition
{
	public override bool IsFulfilled()
	{
		return Platform.Current.CanReEngage;
	}
}

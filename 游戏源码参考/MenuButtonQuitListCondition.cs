public class MenuButtonQuitListCondition : MenuButtonListCondition
{
	public override bool IsFulfilled()
	{
		return Platform.Current.WillDisplayQuitButton;
	}
}

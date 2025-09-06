public sealed class MenuButtonSaveImportCondition : MenuButtonListCondition
{
	public override bool IsFulfilled()
	{
		if ((bool)Platform.Current)
		{
			return Platform.Current.ShowSaveDataImport;
		}
		return false;
	}
}

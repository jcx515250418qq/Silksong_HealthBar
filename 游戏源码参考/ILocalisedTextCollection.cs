using TeamCherry.Localization;

public interface ILocalisedTextCollection
{
	bool IsActive { get; }

	LocalisedString GetRandom(LocalisedString skipString);

	NeedolinTextConfig GetConfig()
	{
		return null;
	}
}

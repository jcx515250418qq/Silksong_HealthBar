using TeamCherry.Localization;
using UnityEngine;

[CreateAssetMenu(menuName = "Hornet/Localised Text Collection")]
public class LocalisedTextCollection : ScriptableObject, ILocalisedTextCollection
{
	[SerializeField]
	private LocalisedTextCollectionData data;

	public bool IsActive => data.IsActive;

	public LocalisedString GetRandom(LocalisedString skipString)
	{
		return data.GetRandom(skipString);
	}

	public NeedolinTextConfig GetConfig()
	{
		return data.GetConfig();
	}

	public LocalisedTextCollectionData ResolveAlternatives()
	{
		return data.ResolveAlternatives();
	}
}

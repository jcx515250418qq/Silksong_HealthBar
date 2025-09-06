using System;
using UnityEngine;

[Serializable]
public class LocalisedTextCollectionField
{
	[SerializeField]
	private LocalisedTextCollection collection;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("collection", false, false, false)]
	private LocalisedTextCollectionData customData;

	public ILocalisedTextCollection GetCollection()
	{
		if (!collection)
		{
			return customData;
		}
		return collection;
	}

	public void SetCollection(LocalisedTextCollection newCollection)
	{
		collection = newCollection;
	}
}

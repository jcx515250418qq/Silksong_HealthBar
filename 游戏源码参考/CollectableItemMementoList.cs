using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Hornet/Collectable Items/Collectable Item Memento List")]
public class CollectableItemMementoList : NamedScriptableObjectList<CollectableItemMemento>, ICollectionViewerItemList
{
	public IEnumerable<ICollectionViewerItem> GetCollectionViewerItems()
	{
		foreach (CollectableItemMemento item in base.List)
		{
			yield return item;
		}
	}
}

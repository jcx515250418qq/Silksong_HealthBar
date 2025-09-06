using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Hornet/Collectable Items/Collectable Item List")]
public class CollectableItemList : NamedScriptableObjectList<CollectableItem>, ICollectionViewerItemList
{
	public IEnumerable<ICollectionViewerItem> GetCollectionViewerItems()
	{
		foreach (CollectableItem item in base.List)
		{
			yield return item;
		}
	}
}

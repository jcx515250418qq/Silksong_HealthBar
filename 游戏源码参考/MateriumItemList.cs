using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Hornet/Materium/Materium Item List")]
public class MateriumItemList : NamedScriptableObjectList<MateriumItem>, ICollectionViewerItemList
{
	public IEnumerable<ICollectionViewerItem> GetCollectionViewerItems()
	{
		foreach (MateriumItem item in base.List)
		{
			yield return item;
		}
	}
}

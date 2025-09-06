using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tool List", menuName = "Hornet/Tool Item List")]
public class ToolItemList : NamedScriptableObjectList<ToolItem>
{
	[ContextMenu("Sort By Type")]
	public void SortByType()
	{
		IEnumerable<ToolItemType> enumerable = typeof(ToolItemType).GetValuesWithOrder().Cast<ToolItemType>();
		Dictionary<ToolItemType, List<ToolItem>> dictionary = new Dictionary<ToolItemType, List<ToolItem>>(enumerable.Count());
		foreach (ToolItemType item in enumerable)
		{
			dictionary[item] = new List<ToolItem>();
		}
		foreach (ToolItem item2 in base.List)
		{
			if (!(item2 == null))
			{
				dictionary[item2.Type].Add(item2);
			}
		}
		base.List.Clear();
		foreach (ToolItemType item3 in enumerable)
		{
			base.List.AddRange(dictionary[item3]);
		}
	}

	[ContextMenu("Unlock All", true)]
	public bool CanUnlockAll()
	{
		return Application.isPlaying;
	}

	[ContextMenu("Unlock All")]
	public void UnlockAll()
	{
		PlayerData instance = PlayerData.instance;
		if (instance != null)
		{
			instance.SeenToolGetPrompt = true;
			instance.SeenToolWeaponGetPrompt = true;
		}
		using IEnumerator<ToolItem> enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			ToolItem current = enumerator.Current;
			if ((bool)current)
			{
				current.SetUnlockedTestsComplete();
				current.Unlock();
			}
		}
	}
}

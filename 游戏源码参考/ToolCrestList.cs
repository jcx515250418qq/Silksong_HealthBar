using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Crest List", menuName = "Hornet/Tool Crest List")]
public class ToolCrestList : NamedScriptableObjectList<ToolCrest>
{
	[ContextMenu("Unlock All", true)]
	public bool CanUnlockAll()
	{
		return Application.isPlaying;
	}

	[ContextMenu("Unlock All")]
	public void UnlockAll()
	{
		using IEnumerator<ToolCrest> enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			enumerator.Current.Unlock();
		}
	}
}

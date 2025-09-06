using System.Collections.Generic;
using UnityEngine;

public sealed class BlackThreadStrandGroup : MonoBehaviour
{
	[SerializeField]
	private List<BlackThreadStrand> strands = new List<BlackThreadStrand>();

	private List<BlackThreadStrand> visibleStrands = new List<BlackThreadStrand>();

	public int TotalStrands => strands.Count;

	public int TotalVisibleStrands => visibleStrands.Count;

	private void Awake()
	{
		strands.RemoveAll((BlackThreadStrand o) => o == null);
		visibleStrands.AddRange(strands);
		visibleStrands.RemoveAll((BlackThreadStrand o) => !o.gameObject.activeSelf);
		visibleStrands.Shuffle();
	}

	public void ResetVisibleStrands()
	{
		foreach (BlackThreadStrand strand in strands)
		{
			strand.gameObject.SetActive(value: true);
		}
		visibleStrands.Clear();
		visibleStrands.AddRange(strands);
		visibleStrands.Shuffle();
	}

	public void HideStrands(int count)
	{
		if (count <= 0)
		{
			return;
		}
		int num = visibleStrands.Count - 1;
		while (num >= 0)
		{
			visibleStrands[num].gameObject.SetActive(value: false);
			visibleStrands.RemoveAt(num);
			count--;
			if (count > 0)
			{
				num--;
				continue;
			}
			break;
		}
	}
}

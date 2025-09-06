using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScoreBoardUI : MonoBehaviour
{
	private enum FleaFestivalGames
	{
		Juggling = 0,
		Bouncing = 1,
		Dodging = 2
	}

	[SerializeField]
	[ArrayForEnum(typeof(FleaFestivalGames))]
	private Transform[] columns;

	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize(ref columns, typeof(FleaFestivalGames));
	}

	private void Awake()
	{
		OnValidate();
	}

	private void OnEnable()
	{
		Refresh();
	}

	public void Refresh()
	{
		Transform[] array = columns;
		foreach (Transform obj in array)
		{
			List<ScoreBoardUIBadgeBase> list = new List<ScoreBoardUIBadgeBase>(obj.childCount);
			foreach (Transform item in obj)
			{
				ScoreBoardUIBadgeBase component = item.GetComponent<ScoreBoardUIBadgeBase>();
				if ((bool)component)
				{
					list.Add(component);
				}
			}
			IOrderedEnumerable<ScoreBoardUIBadgeBase> orderedEnumerable = from s in list
				orderby s.Score descending, s is ScoreBoardUIBadgeHero
				select s;
			int num = 0;
			foreach (ScoreBoardUIBadgeBase item2 in orderedEnumerable)
			{
				item2.transform.SetSiblingIndex(num);
				num++;
				if (Application.isPlaying)
				{
					item2.Evaluate();
				}
			}
		}
	}
}

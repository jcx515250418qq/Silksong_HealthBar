using System.Collections.Generic;
using UnityEngine;

public class LimitBehaviour : MonoBehaviour
{
	public static Dictionary<string, List<GameObject>> behaviourLists = new Dictionary<string, List<GameObject>>();

	public string id = "";

	public int limit = 5;

	public string forceRemoveEvent = "REMOVE";

	private void OnDisable()
	{
		RemoveSelf();
		if (behaviourLists.Count <= 0)
		{
			return;
		}
		bool flag = true;
		foreach (KeyValuePair<string, List<GameObject>> behaviourList in behaviourLists)
		{
			if (behaviourList.Value.Count > 0)
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			behaviourLists.Clear();
		}
	}

	public void Add()
	{
		if (!(id != ""))
		{
			return;
		}
		List<GameObject> list = null;
		if (!behaviourLists.ContainsKey(id))
		{
			list = new List<GameObject>();
			behaviourLists.Add(id, list);
		}
		else
		{
			list = behaviourLists[id];
		}
		if (!list.Contains(base.gameObject))
		{
			list.Add(base.gameObject);
			if (list.Count > 5)
			{
				RemoveFirst();
			}
		}
	}

	public void RemoveFirst()
	{
		if (id != "" && behaviourLists.ContainsKey(id))
		{
			List<GameObject> list = behaviourLists[id];
			GameObject go = list[0];
			list.RemoveAt(0);
			FSMUtility.SendEventToGameObject(go, forceRemoveEvent);
		}
	}

	public void RemoveSelf()
	{
		if (id != "" && behaviourLists.ContainsKey(id) && behaviourLists[id].Contains(base.gameObject))
		{
			behaviourLists[id].Remove(base.gameObject);
		}
	}
}

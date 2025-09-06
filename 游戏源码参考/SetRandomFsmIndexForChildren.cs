using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;

public class SetRandomFsmIndexForChildren : MonoBehaviour
{
	[SerializeField]
	private Transform root;

	[SerializeField]
	private string fsmIntName;

	[SerializeField]
	private int startIndex;

	private void Awake()
	{
		if (!root || string.IsNullOrEmpty(fsmIntName))
		{
			return;
		}
		List<FsmInt> list = new List<FsmInt>();
		foreach (Transform item in root)
		{
			PlayMakerFSM component = item.GetComponent<PlayMakerFSM>();
			if ((bool)component)
			{
				FsmInt fsmInt = component.FsmVariables.FindFsmInt(fsmIntName);
				if (fsmInt != null)
				{
					list.Add(fsmInt);
				}
			}
		}
		list.Shuffle();
		for (int i = 0; i < list.Count; i++)
		{
			list[i].Value = startIndex + i;
		}
	}
}

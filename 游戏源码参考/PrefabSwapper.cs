using System;
using UnityEngine;

[Serializable]
public class PrefabSwapper : MonoBehaviour
{
	public GameObject objToSwapout;

	public string[] ignoreList;

	public bool preserveZDepth = true;

	public bool ignorePrefabs = true;

	public bool contains(string testGo)
	{
		string[] array = ignoreList;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == testGo)
			{
				return true;
			}
		}
		return false;
	}
}

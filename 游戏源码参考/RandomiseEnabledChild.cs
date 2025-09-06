using System.Collections.Generic;
using UnityEngine;

public sealed class RandomiseEnabledChild : MonoBehaviour
{
	[SerializeField]
	private List<GameObject> targets = new List<GameObject>();

	private void Awake()
	{
		targets.RemoveAll((GameObject o) => o == null);
	}

	private void OnEnable()
	{
		int num = Random.Range(0, targets.Count);
		for (int i = 0; i < targets.Count; i++)
		{
			targets[i].SetActive(i == num);
		}
	}
}

using System;
using UnityEngine;

public class ActivateRandomChildren : MonoBehaviour
{
	[SerializeField]
	private int amountMin = 1;

	[SerializeField]
	private int amountMax = 1;

	[SerializeField]
	private bool deparentAfterActivated = true;

	private void OnEnable()
	{
		GameManager instance = GameManager.instance;
		System.Random random = null;
		if ((bool)instance)
		{
			random = instance.SceneSeededRandom;
		}
		if (random == null)
		{
			random = new System.Random();
		}
		foreach (Transform item in base.transform)
		{
			item.gameObject.SetActive(value: false);
		}
		for (float num = random.Next(amountMin, amountMax); num > 0f; num -= 1f)
		{
			int index = random.Next(0, base.transform.childCount);
			Transform child = base.transform.GetChild(index);
			child.gameObject.SetActive(value: true);
			if (deparentAfterActivated)
			{
				child.parent = null;
			}
		}
	}
}

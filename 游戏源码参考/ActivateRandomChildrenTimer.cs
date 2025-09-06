using System.Collections;
using System.Collections.Generic;
using TeamCherry.SharedUtils;
using UnityEngine;

public class ActivateRandomChildrenTimer : MonoBehaviour
{
	[SerializeField]
	private MinMaxFloat delayBetween;

	[SerializeField]
	private MinMaxInt activateCount;

	private bool isEnabled = true;

	private readonly List<Transform> temp = new List<Transform>();

	private void OnEnable()
	{
		foreach (Transform item in base.transform)
		{
			item.gameObject.SetActive(value: false);
		}
		StartCoroutine(Routine());
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	private IEnumerator Routine()
	{
		while (true)
		{
			if (!isEnabled)
			{
				continue;
			}
			yield return new WaitForSeconds(delayBetween.GetRandomValue());
			temp.Clear();
			temp.Capacity = base.transform.childCount;
			foreach (Transform item in base.transform)
			{
				temp.Add(item);
			}
			temp.Shuffle();
			int randomValue = activateCount.GetRandomValue();
			for (int i = 0; i < Mathf.Min(randomValue, temp.Count); i++)
			{
				temp[i].gameObject.SetActive(value: true);
			}
		}
	}

	public void Disable()
	{
		StopAllCoroutines();
		isEnabled = false;
	}
}

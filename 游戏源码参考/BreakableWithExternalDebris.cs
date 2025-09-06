using System;
using System.Collections.Generic;
using UnityEngine;

public class BreakableWithExternalDebris : Breakable
{
	[Serializable]
	public struct ExternalDebris
	{
		public GameObject Prefab;

		public int Count;
	}

	[Serializable]
	public class WeightedExternalDebrisItem : WeightedItem
	{
		public ExternalDebris Value;
	}

	[SerializeField]
	private float debrisPrefabPositionVariance;

	[SerializeField]
	private ExternalDebris[] externalDebris;

	[SerializeField]
	private WeightedExternalDebrisItem[] externalDebrisVariants;

	private static readonly List<IExternalDebris> _externalDebrisResponders = new List<IExternalDebris>();

	protected override void CreateAdditionalDebrisParts(List<GameObject> sourceDebrisParts)
	{
		base.CreateAdditionalDebrisParts(sourceDebrisParts);
		ExternalDebris[] array = externalDebris;
		foreach (ExternalDebris externalDebrisPart in array)
		{
			Spawn(externalDebrisPart, sourceDebrisParts);
		}
		WeightedExternalDebrisItem weightedExternalDebrisItem = externalDebrisVariants.SelectValue();
		if (weightedExternalDebrisItem != null)
		{
			Spawn(weightedExternalDebrisItem.Value, sourceDebrisParts);
		}
	}

	private void Spawn(ExternalDebris externalDebrisPart, List<GameObject> debrisParts)
	{
		for (int i = 0; i < externalDebrisPart.Count; i++)
		{
			if (externalDebrisPart.Prefab == null)
			{
				continue;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(externalDebrisPart.Prefab);
			gameObject.GetComponents(_externalDebrisResponders);
			foreach (IExternalDebris externalDebrisResponder in _externalDebrisResponders)
			{
				externalDebrisResponder.InitExternalDebris();
			}
			_externalDebrisResponders.Clear();
			gameObject.transform.position = base.transform.position + new Vector3(UnityEngine.Random.Range(0f - debrisPrefabPositionVariance, debrisPrefabPositionVariance), UnityEngine.Random.Range(0f - debrisPrefabPositionVariance, debrisPrefabPositionVariance), 0f);
			gameObject.SetActive(value: false);
			debrisParts.Add(gameObject);
		}
	}
}

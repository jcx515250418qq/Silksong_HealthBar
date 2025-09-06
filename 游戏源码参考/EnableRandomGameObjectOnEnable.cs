using System;
using System.Collections.Generic;
using UnityEngine;

public class EnableRandomGameObjectOnEnable : MonoBehaviour
{
	[Serializable]
	private class ProbabilityGameObject : Probability.ProbabilityBase<GameObject>
	{
		public GameObject Child;

		public override GameObject Item => Child;
	}

	[SerializeField]
	private ProbabilityGameObject[] gameObjects;

	[SerializeField]
	[Tooltip("Others using the same fairness key, with the same amount of gameObjects, will share a fairness tracking array.")]
	private string fairnessKey;

	private static Dictionary<string, float[]> fairness = new Dictionary<string, float[]>();

	private void Awake()
	{
		if (fairnessKey != string.Empty)
		{
			fairnessKey = $"{fairnessKey}_{gameObjects.Length}";
		}
	}

	private void OnEnable()
	{
		ProbabilityGameObject[] array = gameObjects;
		foreach (ProbabilityGameObject probabilityGameObject in array)
		{
			if ((bool)probabilityGameObject.Child)
			{
				probabilityGameObject.Child.SetActive(value: false);
			}
		}
		GameObject gameObject;
		if (fairnessKey == string.Empty)
		{
			gameObject = Probability.GetRandomItemByProbability<ProbabilityGameObject, GameObject>(gameObjects);
		}
		else
		{
			float[] storeProbabilities = null;
			if (fairness.TryGetValue(fairnessKey, out var value))
			{
				storeProbabilities = value;
			}
			gameObject = Probability.GetRandomItemByProbabilityFair<ProbabilityGameObject, GameObject>(gameObjects, ref storeProbabilities);
			fairness[fairnessKey] = storeProbabilities;
		}
		if ((bool)gameObject)
		{
			gameObject.SetActive(value: true);
		}
	}
}

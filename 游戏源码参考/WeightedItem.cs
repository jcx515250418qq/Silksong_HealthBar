using UnityEngine;

public class WeightedItem
{
	[SerializeField]
	[Range(0.001f, 10f)]
	private float weight;

	public float Weight => weight;
}

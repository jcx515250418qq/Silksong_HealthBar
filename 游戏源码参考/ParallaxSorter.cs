using UnityEngine;

public class ParallaxSorter : MonoBehaviour
{
	public string[] sortingLayers;

	public int[] sortingLayerIDs;

	public float[] layerDepths;

	public bool stripSortingLayers;

	private void Awake()
	{
		stripSortingLayers = true;
	}
}

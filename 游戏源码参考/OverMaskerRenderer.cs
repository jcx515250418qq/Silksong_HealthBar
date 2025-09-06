using UnityEngine;

[DisallowMultipleComponent]
public sealed class OverMaskerRenderer : OverMaskerBase
{
	[SerializeField]
	private Renderer renderer;

	private void OnValidate()
	{
		if (renderer == null)
		{
			renderer = GetComponent<Renderer>();
		}
	}

	protected override void ApplySettings(int sortingLayer, short order)
	{
		if (renderer != null)
		{
			renderer.sortingLayerID = sortingLayer;
			renderer.sortingOrder = order;
		}
	}
}

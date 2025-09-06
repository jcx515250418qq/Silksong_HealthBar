using System;
using UnityEngine;

public sealed class RectTransformDimensionChangedEvent : MonoBehaviour
{
	public event Action DimensionsChanged;

	private void OnRectTransformDimensionsChange()
	{
		this.DimensionsChanged?.Invoke();
	}
}

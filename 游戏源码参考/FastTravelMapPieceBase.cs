using System;
using UnityEngine;

public class FastTravelMapPieceBase<TButtonType, TMapType, TLocation> : MonoBehaviour, IFastTravelMapPiece where TButtonType : FastTravelMapButtonBase<TLocation> where TMapType : FastTravelMapBase<TLocation> where TLocation : struct, IComparable
{
	[SerializeField]
	private TButtonType pairedButton;

	[SerializeField]
	private Vector2 indicatorOffset;

	private TMapType parentMap;

	public bool IsVisible
	{
		get
		{
			if ((bool)pairedButton)
			{
				return pairedButton.IsUnlocked();
			}
			return false;
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(indicatorOffset, 0.1f);
	}

	private void Awake()
	{
		parentMap = GetComponentInParent<TMapType>();
		if (parentMap != null)
		{
			parentMap.Opened += OnOpened;
		}
		if ((bool)pairedButton)
		{
			TButtonType val = pairedButton;
			val.Selected = (Action)Delegate.Combine(val.Selected, new Action(Select));
		}
	}

	private void OnOpened()
	{
		if (!pairedButton || !pairedButton.IsUnlocked())
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		base.gameObject.SetActive(value: true);
		if (pairedButton.IsCurrentLocation())
		{
			Vector3 vector = base.transform.TransformPoint(indicatorOffset);
			parentMap.SetMapIndicatorPosition(vector);
			parentMap.SetMapSelectorPosition(vector, isInstant: true);
		}
	}

	private void Select()
	{
		if (parentMap != null)
		{
			parentMap.SetMapSelectorPosition(base.transform.TransformPoint(indicatorOffset), isInstant: false);
		}
	}
}

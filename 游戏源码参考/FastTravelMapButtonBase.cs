using System;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Serialization;

public class FastTravelMapButtonBase<T> : MonoBehaviour where T : struct, IComparable
{
	public Action Selected;

	public Action Deselected;

	[FormerlySerializedAs("targetlocation")]
	[SerializeField]
	private T targetLocation;

	[SerializeField]
	[PlayerDataField(typeof(bool), false)]
	private string playerDataBool;

	private FastTravelMapBase<T> parentMap;

	private UISelectionListItem item;

	private void Awake()
	{
		parentMap = GetComponentInParent<FastTravelMapBase<T>>();
		if (parentMap != null)
		{
			parentMap.Opening += OnOpening;
			parentMap.Opened += OnOpened;
		}
		item = GetComponent<UISelectionListItem>();
		if ((bool)item)
		{
			item.AutoSelect = IsCurrentLocation;
			item.SubmitPressed.AddListener(Submit);
			item.CancelPressed.AddListener(Cancel);
			item.Selected.AddListener(Select);
			item.Deselected.AddListener(Deselect);
		}
	}

	private void OnOpening()
	{
		if (!IsUnlocked())
		{
			base.gameObject.SetActive(value: false);
		}
		else
		{
			base.gameObject.SetActive(value: true);
		}
	}

	private void OnOpened()
	{
		if (IsCurrentLocation())
		{
			parentMap.SetCurrentLocationIndicatorPosition(base.transform.position.y);
		}
	}

	public void Submit()
	{
		if (!(parentMap == null))
		{
			parentMap.ConfirmLocation(targetLocation);
		}
	}

	public void Cancel()
	{
		if (!(parentMap == null))
		{
			parentMap.ConfirmLocation(default(T));
		}
	}

	private void Select()
	{
		if (Selected != null)
		{
			Selected();
		}
	}

	private void Deselect()
	{
		if (Deselected != null)
		{
			Deselected();
		}
	}

	public bool IsCurrentLocation()
	{
		if (!IsUnlocked())
		{
			return false;
		}
		if (parentMap != null)
		{
			return parentMap.AutoSelectLocation.CompareTo(targetLocation) == 0;
		}
		return false;
	}

	public bool IsUnlocked()
	{
		if (!string.IsNullOrEmpty(playerDataBool))
		{
			return PlayerData.instance.GetVariable<bool>(playerDataBool);
		}
		return true;
	}
}

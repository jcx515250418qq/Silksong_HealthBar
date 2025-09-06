using GlobalSettings;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

public class InventoryPaneListItem : MonoBehaviour
{
	[SerializeField]
	private NestedFadeGroupBase group;

	[SerializeField]
	private SpriteRenderer icon;

	[SerializeField]
	private Color selectedColor;

	[SerializeField]
	private Color normalColor;

	[SerializeField]
	private float selectedScale;

	[SerializeField]
	private float scaleDuration;

	[SerializeField]
	private GameObject newOrb;

	private InventoryPane currentPane;

	private Vector3 newOrbInitialScale;

	public float Alpha
	{
		get
		{
			if (!group)
			{
				return 0f;
			}
			return group.AlphaSelf;
		}
		set
		{
			if ((bool)group)
			{
				group.AlphaSelf = value;
			}
		}
	}

	private void Awake()
	{
		if ((bool)newOrb)
		{
			newOrbInitialScale = newOrb.transform.localScale;
		}
	}

	public void UpdateValues(InventoryPane pane, bool isSelected)
	{
		if ((bool)currentPane)
		{
			currentPane.NewItemsUpdated -= OnNewItemsUpdated;
		}
		currentPane = pane;
		icon.sprite = pane.ListIcon;
		icon.color = (isSelected ? selectedColor : normalColor);
		float num = (isSelected ? selectedScale : 1f);
		base.transform.ScaleTo(this, new Vector3(num, num, 1f), scaleDuration, 0f, dontTrack: false, isRealtime: true);
		pane.NewItemsUpdated += OnNewItemsUpdated;
		if ((bool)newOrb)
		{
			if (pane.IsAnyUpdates)
			{
				newOrb.SetActive(value: true);
				newOrb.transform.localScale = newOrbInitialScale;
			}
			else
			{
				newOrb.SetActive(value: false);
			}
		}
	}

	private void OnNewItemsUpdated(bool isAnyNewItems)
	{
		if ((bool)newOrb)
		{
			if (isAnyNewItems)
			{
				newOrb.SetActive(value: true);
			}
			else
			{
				newOrb.transform.ScaleTo(this, Vector3.zero, UI.NewDotScaleTime, UI.NewDotScaleDelay, dontTrack: false, isRealtime: true);
			}
		}
	}
}

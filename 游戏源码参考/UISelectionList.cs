using System.Collections.Generic;
using UnityEngine;

public class UISelectionList : MonoBehaviour
{
	private enum ListType
	{
		Vertical = 0,
		Horizontal = 1
	}

	[SerializeField]
	private ListType listType;

	[SerializeField]
	private Transform listParent;

	[SerializeField]
	private List<UISelectionListItem> listItems = new List<UISelectionListItem>();

	[SerializeField]
	[Tooltip("If within listItems bounds will always select the item of this index before calling cancel.")]
	private int cancelItemIndex = -1;

	private int selectedIndex;

	private bool isInactive;

	private float activeCooldown;

	private InventoryItemManager manager;

	private void Awake()
	{
		if ((bool)listParent)
		{
			listItems.Clear();
			foreach (Transform item in listParent)
			{
				UISelectionListItem component = item.GetComponent<UISelectionListItem>();
				if ((bool)component)
				{
					listItems.Add(component);
				}
			}
		}
		InventoryPaneBase componentInParent = GetComponentInParent<InventoryPaneBase>();
		if ((bool)componentInParent)
		{
			switch (listType)
			{
			case ListType.Vertical:
				componentInParent.OnInputDown += delegate
				{
					MoveSelection(1);
				};
				componentInParent.OnInputUp += delegate
				{
					MoveSelection(-1);
				};
				break;
			case ListType.Horizontal:
				componentInParent.OnInputLeft += delegate
				{
					MoveSelection(-1);
				};
				componentInParent.OnInputRight += delegate
				{
					MoveSelection(1);
				};
				break;
			}
		}
		manager = GetComponentInParent<InventoryItemManager>();
	}

	private void Update()
	{
		if (isInactive || ((bool)manager && manager.IsActionsBlocked))
		{
			return;
		}
		if (activeCooldown > 0f)
		{
			activeCooldown -= Time.deltaTime;
		}
		else
		{
			if (listItems.Count <= 0 || selectedIndex < 0 || selectedIndex >= listItems.Count)
			{
				return;
			}
			HeroActions inputActions = GameManager.instance.inputHandler.inputActions;
			switch (Platform.Current.GetMenuAction(inputActions))
			{
			case Platform.MenuActions.Submit:
				listItems[selectedIndex].Submit();
				break;
			default:
				if (!InventoryPaneInput.IsInventoryButtonPressed(inputActions))
				{
					break;
				}
				goto case Platform.MenuActions.Cancel;
			case Platform.MenuActions.Cancel:
				if (cancelItemIndex >= 0 && cancelItemIndex < listItems.Count)
				{
					SetSelected(cancelItemIndex, isInstant: false, force: false, skipSelectSound: true);
				}
				listItems[selectedIndex].Cancel();
				break;
			}
		}
	}

	private void MoveSelection(int direction)
	{
		if (!isInactive && base.isActiveAndEnabled)
		{
			if (direction != 0)
			{
				direction = (int)Mathf.Sign(direction);
			}
			int num = selectedIndex + direction;
			if (num >= listItems.Count)
			{
				num = 0;
			}
			else if (num < 0)
			{
				num = listItems.Count - 1;
			}
			if (listItems[num].gameObject.activeSelf)
			{
				SetSelected(num, isInstant: false);
				return;
			}
			selectedIndex = num;
			MoveSelection(direction);
		}
	}

	private void SetSelected(int index, bool isInstant, bool force = false, bool skipSelectSound = false)
	{
		if (listItems.Count == 0)
		{
			return;
		}
		for (int i = 0; i < listItems.Count; i++)
		{
			if (i != index || force)
			{
				if (skipSelectSound)
				{
					listItems[i].SkipNextSelectSound();
				}
				listItems[i].SetSelected(value: false, isInstant);
			}
		}
		if (index >= 0)
		{
			if (skipSelectSound)
			{
				listItems[index].SkipNextSelectSound();
			}
			listItems[index].SetSelected(value: true, isInstant);
		}
		selectedIndex = index;
	}

	public void SetActive(bool value)
	{
		isInactive = !value;
		if (!value)
		{
			SetSelected(-1, isInstant: true, force: true);
			return;
		}
		int index = 0;
		for (int i = 0; i < listItems.Count; i++)
		{
			if (listItems[i].gameObject.activeSelf && listItems[i].AutoSelect != null && listItems[i].AutoSelect())
			{
				index = i;
				break;
			}
		}
		SetSelected(index, isInstant: false, force: true);
		activeCooldown = 0.2f;
	}
}

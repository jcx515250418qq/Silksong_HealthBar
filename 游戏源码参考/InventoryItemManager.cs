using System;
using System.Collections.Generic;
using System.Linq;
using GlobalSettings;
using TMProOld;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class InventoryItemManager : MonoBehaviour
{
	public enum SelectedActionType
	{
		Default = 0,
		LeftMost = 1,
		RightMost = 2,
		Previous = 3
	}

	public enum SelectionDirection
	{
		Up = 0,
		Down = 1,
		Left = 2,
		Right = 3
	}

	[SerializeField]
	[FormerlySerializedAs("defaultSelected")]
	[HideInInspector]
	[Obsolete]
	private InventoryItemSelectable old_defaultSelected;

	[SerializeField]
	private InventoryItemSelectable[] defaultSelectables;

	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private InventoryItemSelectable leftMostSelected;

	[SerializeField]
	private InventoryItemSelectable[] leftMostSelectables;

	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private InventoryItemSelectable rightMostSelected;

	[SerializeField]
	private InventoryItemSelectable[] rightMostSelectables;

	[SerializeField]
	protected TextMeshPro nameText;

	[SerializeField]
	protected TextMeshPro descriptionText;

	[SerializeField]
	private LayoutGroup descriptionLayout;

	[SerializeField]
	private InventoryCursor cursorPrefab;

	protected InventoryCursor cursor;

	[SerializeField]
	private Transform cursorParentOverride;

	private bool isSubmitHeld;

	private bool isExtraHeld;

	private static readonly int _selectionDirectionLength = Enum.GetNames(typeof(SelectionDirection)).Length;

	public InventoryItemSelectable CurrentSelected { get; private set; }

	public InventoryItemSelectable NextSelected { get; private set; }

	protected virtual IEnumerable<InventoryItemSelectable> DefaultSelectables => defaultSelectables;

	public bool IsActionsBlocked { get; set; }

	protected virtual void OnValidate()
	{
		if ((bool)old_defaultSelected)
		{
			defaultSelectables = new InventoryItemSelectable[1] { old_defaultSelected };
			old_defaultSelected = null;
		}
		if ((bool)leftMostSelected)
		{
			leftMostSelectables = new InventoryItemSelectable[1] { leftMostSelected };
			leftMostSelected = null;
		}
		if ((bool)rightMostSelected)
		{
			rightMostSelectables = new InventoryItemSelectable[1] { rightMostSelected };
			rightMostSelected = null;
		}
	}

	protected virtual void Awake()
	{
		OnValidate();
		if ((bool)cursorPrefab)
		{
			cursor = UnityEngine.Object.Instantiate(cursorPrefab, cursorParentOverride ? cursorParentOverride : base.transform);
			cursor.gameObject.name = "Cursor";
			cursor.transform.SetLocalPosition2D(new Vector2(-100f, -100f));
		}
		InventoryPaneBase component = GetComponent<InventoryPaneBase>();
		if (!component)
		{
			return;
		}
		component.OnPrePaneStart += delegate
		{
			InstantScroll();
		};
		component.OnPaneEnd += delegate
		{
			if (isSubmitHeld)
			{
				isSubmitHeld = false;
				if ((bool)CurrentSelected)
				{
					CurrentSelected.SubmitReleased();
				}
			}
			if (isExtraHeld)
			{
				isExtraHeld = false;
				if ((bool)CurrentSelected)
				{
					CurrentSelected.ExtraReleased();
				}
			}
		};
	}

	public bool SetSelected(SelectedActionType selectedAction, bool justDisplay = false)
	{
		InventoryItemSelectable inventoryItemSelectable = GetStartSelectable();
		SelectionDirection? direction = null;
		IEnumerable<InventoryItemSelectable> collection = DefaultSelectables;
		switch (selectedAction)
		{
		case SelectedActionType.LeftMost:
			direction = SelectionDirection.Right;
			if (TrySelectOrdered(GetLeftMostSelectables(), direction, justDisplay))
			{
				return true;
			}
			break;
		case SelectedActionType.RightMost:
			direction = SelectionDirection.Left;
			if (TrySelectOrdered(GetRightMostSelectables(), direction, justDisplay))
			{
				return true;
			}
			break;
		case SelectedActionType.Previous:
			if (!inventoryItemSelectable)
			{
				if (!CurrentSelected || !CurrentSelected.isActiveAndEnabled)
				{
					return TrySelectOrdered(collection, null, justDisplay);
				}
				inventoryItemSelectable = CurrentSelected;
			}
			break;
		}
		if (!inventoryItemSelectable)
		{
			return TrySelectOrdered(collection, direction, justDisplay);
		}
		if (SetSelected(inventoryItemSelectable, direction, justDisplay))
		{
			return true;
		}
		return TrySelectOrdered(collection, direction, justDisplay);
	}

	private bool TrySelectOrdered(IEnumerable<InventoryItemSelectable> collection, SelectionDirection? direction, bool justDisplay)
	{
		if (collection == null)
		{
			return false;
		}
		foreach (InventoryItemSelectable item in collection.Where((InventoryItemSelectable s) => s != null && s.gameObject.activeSelf))
		{
			if (SetSelected(item, direction, justDisplay))
			{
				return true;
			}
		}
		return false;
	}

	public bool TryGetFurthestSelectableInDirection(SelectionDirection direction, out InventoryItemSelectable furthestSelectable)
	{
		switch (direction)
		{
		case SelectionDirection.Up:
			foreach (InventoryItemSelectable topMostSelectable in GetTopMostSelectables())
			{
				if (!(topMostSelectable == null) && topMostSelectable.isActiveAndEnabled && !(topMostSelectable.Get(null) == null))
				{
					furthestSelectable = topMostSelectable;
					return true;
				}
			}
			break;
		case SelectionDirection.Down:
			foreach (InventoryItemSelectable bottomMostSelectable in GetBottomMostSelectables())
			{
				if (!(bottomMostSelectable == null) && bottomMostSelectable.isActiveAndEnabled && !(bottomMostSelectable.Get(null) == null))
				{
					furthestSelectable = bottomMostSelectable;
					return true;
				}
			}
			break;
		case SelectionDirection.Left:
			foreach (InventoryItemSelectable leftMostSelectable in GetLeftMostSelectables())
			{
				if (!(leftMostSelectable == null) && leftMostSelectable.isActiveAndEnabled)
				{
					InventoryItemSelectable inventoryItemSelectable2 = leftMostSelectable.Get(null);
					if (!(inventoryItemSelectable2 == null))
					{
						furthestSelectable = inventoryItemSelectable2;
						return true;
					}
				}
			}
			break;
		case SelectionDirection.Right:
			foreach (InventoryItemSelectable rightMostSelectable in GetRightMostSelectables())
			{
				if (!(rightMostSelectable == null) && rightMostSelectable.isActiveAndEnabled)
				{
					InventoryItemSelectable inventoryItemSelectable = rightMostSelectable.Get(null);
					if (!(inventoryItemSelectable == null))
					{
						furthestSelectable = inventoryItemSelectable;
						return true;
					}
				}
			}
			break;
		}
		furthestSelectable = null;
		return false;
	}

	public void SetSelected(GameObject selectedGameObject, bool justDisplay = false)
	{
		if ((bool)CurrentSelected && selectedGameObject != CurrentSelected.gameObject)
		{
			CurrentSelected.Deselect();
			CurrentSelected.OnUpdateDisplay -= SetDisplay;
		}
		if (selectedGameObject == null || justDisplay)
		{
			return;
		}
		if ((bool)cursor)
		{
			ScrollView componentInParent = selectedGameObject.GetComponentInParent<ScrollView>();
			if ((bool)componentInParent)
			{
				Vector3 position = componentInParent.transform.parent.position;
				Bounds viewBounds = componentInParent.ViewBounds;
				Vector3 extents = viewBounds.extents;
				if (extents.x == 0f)
				{
					extents.x = float.MaxValue;
				}
				if (extents.y == 0f)
				{
					extents.y = float.MaxValue;
				}
				viewBounds.extents = extents;
				Vector3 vector = position + viewBounds.min;
				Vector3 vector2 = position + viewBounds.max;
				cursor.SetClampedPos(vector, vector2);
			}
			else
			{
				cursor.ResetClampedPos();
			}
			cursor.SetTarget(selectedGameObject ? selectedGameObject.transform : null);
		}
		else
		{
			PlayMakerFSM playMakerFSM = PlayMakerFSM.FindFsmOnGameObject(base.gameObject, "Update Cursor");
			if ((bool)playMakerFSM)
			{
				playMakerFSM.FsmVariables.FindFsmGameObject("Item").Value = selectedGameObject;
				playMakerFSM.SendEvent("UPDATE CURSOR");
			}
		}
	}

	public virtual void InstantScroll()
	{
	}

	public bool SetSelected(InventoryItemSelectable selectable, SelectionDirection? direction, bool justDisplay = false)
	{
		InventoryItemSelectable currentSelected = CurrentSelected;
		if ((bool)selectable)
		{
			InventoryItemSelectable inventoryItemSelectable = selectable.Get(direction);
			if (inventoryItemSelectable == null)
			{
				return false;
			}
			selectable = inventoryItemSelectable;
		}
		if (!selectable)
		{
			return false;
		}
		NextSelected = selectable;
		if ((bool)currentSelected)
		{
			currentSelected.Deselect();
			currentSelected.OnUpdateDisplay -= SetDisplay;
		}
		CurrentSelected = selectable;
		NextSelected = null;
		selectable.OnUpdateDisplay += SetDisplay;
		if (justDisplay)
		{
			SetDisplay(selectable);
		}
		else
		{
			selectable.Select(direction);
			SetSelected(selectable.gameObject);
		}
		if ((bool)descriptionLayout)
		{
			descriptionLayout.ForceUpdateLayoutNoCanvas();
		}
		return true;
	}

	public virtual bool MoveSelection(SelectionDirection direction)
	{
		if (IsActionsBlocked)
		{
			return true;
		}
		if (CurrentSelected == null)
		{
			return false;
		}
		InventoryItemSelectable nextSelectable = CurrentSelected.GetNextSelectable(direction);
		if (nextSelectable == null)
		{
			return false;
		}
		if (nextSelectable == CurrentSelected)
		{
			return true;
		}
		PlayMoveSound();
		return SetSelected(nextSelectable, direction);
	}

	public virtual bool MoveSelectionPage(SelectionDirection direction)
	{
		if (IsActionsBlocked)
		{
			return true;
		}
		if (CurrentSelected == null)
		{
			return false;
		}
		InventoryItemSelectable nextSelectablePage = CurrentSelected.GetNextSelectablePage(CurrentSelected, direction);
		if (nextSelectablePage == null)
		{
			return false;
		}
		if (nextSelectablePage == CurrentSelected)
		{
			return true;
		}
		PlayMoveSound();
		return SetSelected(nextSelectablePage, direction);
	}

	public void PlayMoveSound()
	{
		Audio.InventorySelectionMoveSound.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position);
	}

	public virtual void SetDisplay(GameObject selectedGameObject)
	{
		if ((bool)nameText)
		{
			nameText.text = string.Empty;
		}
		if ((bool)descriptionText)
		{
			descriptionText.text = string.Empty;
		}
	}

	protected virtual string FormatDisplayName(string displayName)
	{
		return displayName;
	}

	protected virtual string FormatDescription(string description)
	{
		return description;
	}

	public virtual void SetDisplay(InventoryItemSelectable selectable)
	{
		SetDisplay(selectable.gameObject);
		if ((bool)nameText)
		{
			nameText.text = FormatDisplayName(selectable.DisplayName);
		}
		if ((bool)descriptionText)
		{
			descriptionText.text = FormatDescription(selectable.Description);
		}
	}

	public virtual bool SubmitButtonSelected()
	{
		if (IsActionsBlocked)
		{
			return false;
		}
		if ((bool)CurrentSelected && CurrentSelected.Submit())
		{
			isSubmitHeld = true;
			return true;
		}
		return false;
	}

	public virtual bool SubmitButtonReleaseSelected()
	{
		if (IsActionsBlocked)
		{
			return false;
		}
		if (isSubmitHeld)
		{
			isSubmitHeld = false;
			if ((bool)CurrentSelected && CurrentSelected.SubmitReleased())
			{
				return true;
			}
		}
		return false;
	}

	public virtual bool CancelButtonSelected()
	{
		if (IsActionsBlocked)
		{
			return false;
		}
		if ((bool)CurrentSelected)
		{
			return CurrentSelected.Cancel();
		}
		return false;
	}

	public virtual bool ExtraButtonSelected()
	{
		if (IsActionsBlocked)
		{
			return false;
		}
		if ((bool)CurrentSelected && CurrentSelected.Extra())
		{
			isExtraHeld = true;
			return true;
		}
		return false;
	}

	public virtual bool ExtraButtonReleaseSelected()
	{
		if (IsActionsBlocked)
		{
			return false;
		}
		if (isExtraHeld)
		{
			isExtraHeld = false;
			if ((bool)CurrentSelected && CurrentSelected.ExtraReleased())
			{
				return true;
			}
		}
		return false;
	}

	public virtual bool SuperButtonSelected()
	{
		if (IsActionsBlocked)
		{
			return false;
		}
		if ((bool)CurrentSelected)
		{
			return CurrentSelected.Super();
		}
		return false;
	}

	public static void PropagateSelectables(InventoryItemSelectableDirectional source, InventoryItemSelectableDirectional target)
	{
		for (int i = 0; i < _selectionDirectionLength; i++)
		{
			if (target.Selectables[i] == null)
			{
				target.Selectables[i] = source.Selectables[i];
			}
		}
	}

	public void SetProxyActive(bool value, SelectedActionType select = SelectedActionType.Default)
	{
		PlayMakerFSM playMakerFSM = PlayMakerFSM.FindFsmOnGameObject(base.gameObject, "Inventory Proxy");
		if ((bool)playMakerFSM)
		{
			playMakerFSM.SendEvent(value ? "ACTIVATE" : "PANE RESET");
			playMakerFSM.FsmVariables.FindFsmEnum("Start Selection").Value = select;
		}
	}

	protected virtual InventoryItemSelectable GetStartSelectable()
	{
		return null;
	}

	protected virtual IEnumerable<InventoryItemSelectable> GetRightMostSelectables()
	{
		return rightMostSelectables;
	}

	protected virtual IEnumerable<InventoryItemSelectable> GetLeftMostSelectables()
	{
		return leftMostSelectables;
	}

	protected virtual IEnumerable<InventoryItemSelectable> GetTopMostSelectables()
	{
		return Array.Empty<InventoryItemSelectable>();
	}

	protected virtual IEnumerable<InventoryItemSelectable> GetBottomMostSelectables()
	{
		return Array.Empty<InventoryItemSelectable>();
	}
}

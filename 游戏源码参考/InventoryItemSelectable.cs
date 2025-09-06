using System;
using UnityEngine;

public abstract class InventoryItemSelectable : MonoBehaviour, InventoryCursor.ICursorTarget
{
	[SerializeField]
	private Vector2 cursorGlowScale = Vector2.one;

	[SerializeField]
	private Vector2 navigationOffset;

	public virtual string DisplayName => string.Empty;

	public virtual string Description => string.Empty;

	public virtual Color? CursorColor => null;

	public Vector2 CursorGlowScale
	{
		get
		{
			return cursorGlowScale;
		}
		set
		{
			cursorGlowScale = value;
		}
	}

	public Vector2 NavigationOffset => navigationOffset;

	public virtual bool ShowCursor => true;

	public event Action<InventoryItemSelectable> OnSelected;

	public event Action<InventoryItemSelectable, InventoryItemManager.SelectionDirection?> OnSelectedDirection;

	public event Action<InventoryItemSelectable> OnDeselected;

	public event Action<InventoryItemSelectable> OnUpdateDisplay;

	public virtual InventoryItemSelectable Get(InventoryItemManager.SelectionDirection? direction)
	{
		return this;
	}

	public abstract InventoryItemSelectable GetNextSelectable(InventoryItemManager.SelectionDirection direction);

	public virtual InventoryItemSelectable GetNextSelectablePage(InventoryItemSelectable currentSelected, InventoryItemManager.SelectionDirection direction)
	{
		if (!base.transform.parent)
		{
			return null;
		}
		InventoryItemSelectable componentInParent = base.transform.parent.GetComponentInParent<InventoryItemSelectable>();
		if (!componentInParent)
		{
			return null;
		}
		return componentInParent.GetNextSelectablePage(currentSelected, direction);
	}

	public virtual void Select(InventoryItemManager.SelectionDirection? direction)
	{
		UpdateDisplay();
		if (this.OnSelected != null)
		{
			this.OnSelected(this);
		}
		if (this.OnSelectedDirection != null)
		{
			this.OnSelectedDirection(this, direction);
		}
	}

	public virtual void Deselect()
	{
		if (this.OnDeselected != null)
		{
			this.OnDeselected(this);
		}
	}

	public virtual bool Submit()
	{
		return false;
	}

	public virtual bool SubmitReleased()
	{
		return false;
	}

	public virtual bool Cancel()
	{
		return false;
	}

	public virtual bool Extra()
	{
		return false;
	}

	public virtual bool ExtraReleased()
	{
		return false;
	}

	public virtual bool Super()
	{
		return false;
	}

	protected virtual void UpdateDisplay()
	{
		if (this.OnUpdateDisplay != null)
		{
			this.OnUpdateDisplay(this);
		}
	}
}

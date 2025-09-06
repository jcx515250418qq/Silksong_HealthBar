using System;
using UnityEngine;

public abstract class InventoryPaneBase : MonoBehaviour
{
	public enum InputEventType
	{
		Left = 0,
		Right = 1,
		Up = 2,
		Down = 3
	}

	public bool IsPaneActive { get; private set; }

	public event Action OnPrePaneStart;

	public event Action OnPaneStart;

	public event Action OnPostPaneStart;

	public event Action OnPaneEnd;

	public event Action OnPrePaneEnd;

	public event Action OnInputLeft;

	public event Action OnInputRight;

	public event Action OnInputUp;

	public event Action OnInputDown;

	public virtual void PaneStart()
	{
		IsPaneActive = true;
		base.gameObject.SetActive(value: true);
		if (this.OnPrePaneStart != null)
		{
			this.OnPrePaneStart();
		}
		if (this.OnPaneStart != null)
		{
			this.OnPaneStart();
		}
		if (this.OnPostPaneStart != null)
		{
			this.OnPostPaneStart();
		}
	}

	public virtual void PaneEnd()
	{
		IsPaneActive = false;
		if (this.OnPrePaneEnd != null)
		{
			this.OnPrePaneEnd();
		}
		if (this.OnPaneEnd != null)
		{
			this.OnPaneEnd();
		}
	}

	public void SendInputEvent(InputEventType eventType)
	{
		Action action = null;
		switch (eventType)
		{
		case InputEventType.Left:
			action = this.OnInputLeft;
			break;
		case InputEventType.Right:
			action = this.OnInputRight;
			break;
		case InputEventType.Up:
			action = this.OnInputUp;
			break;
		case InputEventType.Down:
			action = this.OnInputDown;
			break;
		}
		action?.Invoke();
	}
}

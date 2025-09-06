using System;
using UnityEngine;

public abstract class VisibilityEvent : MonoBehaviour
{
	public delegate void VisibilityChanged(bool visible);

	[Serializable]
	private enum ParentMode
	{
		Manual = 0,
		Auto = 1
	}

	[SerializeField]
	private ParentMode parentMode;

	private bool isVisible;

	private VisibilityGroup parent;

	public bool IsVisible
	{
		get
		{
			return isVisible;
		}
		protected set
		{
			if (isVisible != value)
			{
				isVisible = value;
				this.OnVisibilityChanged?.Invoke(value);
			}
		}
	}

	public event VisibilityChanged OnVisibilityChanged;

	protected virtual void OnDestroy()
	{
		if ((bool)parent)
		{
			parent.UnregisterObject(this);
			parent = null;
		}
	}

	protected void FindParent()
	{
		if (parentMode == ParentMode.Auto)
		{
			parent = GetComponentInParent<VisibilityGroup>();
		}
	}

	public void SetParent(VisibilityGroup visibilityGroup)
	{
		if (!(parent == visibilityGroup))
		{
			if (parent != null)
			{
				parent.UnregisterObject(this);
			}
			parent = visibilityGroup;
			if (visibilityGroup != null)
			{
				parentMode = ParentMode.Manual;
				parent.UnsafeRegisterObject(this);
			}
		}
	}
}

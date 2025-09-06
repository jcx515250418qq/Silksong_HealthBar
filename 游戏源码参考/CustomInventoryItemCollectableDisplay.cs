using System;
using UnityEngine;

public class CustomInventoryItemCollectableDisplay : MonoBehaviour
{
	[SerializeField]
	private float jitterMagnitudeMultiplier = 1f;

	private bool isPaneActive = true;

	private bool isObjActive;

	private bool activated;

	public InventoryItemSelectable Owner { get; set; }

	public float JitterMagnitudeMultiplier => jitterMagnitudeMultiplier;

	public event Action<CustomInventoryItemCollectableDisplay> OnDestroyed;

	protected virtual void OnEnable()
	{
		isObjActive = true;
		if (isPaneActive && !activated)
		{
			OnActivate();
			activated = true;
		}
	}

	protected virtual void OnDisable()
	{
		isObjActive = false;
		if (isPaneActive && activated)
		{
			OnDeactivate();
			activated = false;
		}
	}

	protected virtual void OnDestroy()
	{
		this.OnDestroyed?.Invoke(this);
		this.OnDestroyed = null;
	}

	public virtual void OnPaneStart()
	{
		isPaneActive = true;
		if (isObjActive && !activated)
		{
			OnActivate();
			activated = true;
		}
	}

	public virtual void OnPaneEnd()
	{
		isPaneActive = false;
		if (isObjActive && activated)
		{
			OnDeactivate();
			activated = false;
		}
	}

	protected virtual void OnActivate()
	{
	}

	protected virtual void OnDeactivate()
	{
	}

	public virtual void OnPrePaneEnd()
	{
	}

	public virtual void OnSelect()
	{
	}

	public virtual void OnDeselect()
	{
	}

	public virtual void OnConsumeStart()
	{
	}

	public virtual void OnConsumeEnd()
	{
	}

	public virtual void OnConsumeComplete()
	{
	}

	public virtual void OnConsumeBlocked()
	{
	}
}

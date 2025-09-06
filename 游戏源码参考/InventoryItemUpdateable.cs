using System;
using System.Collections.Generic;
using GlobalSettings;
using UnityEngine;

public abstract class InventoryItemUpdateable : InventoryItemSelectableDirectional
{
	public interface IIsSeenProvider
	{
		bool IsSeen { get; set; }
	}

	[Space]
	[SerializeField]
	private GameObject newDot;

	private Vector3 newDotInitialScale;

	private bool isNew;

	private bool isScaling;

	private List<IIsSeenProvider> isSeenProviders;

	protected InventoryPane Pane;

	protected abstract bool IsSeen { get; set; }

	private bool IsSeenAll
	{
		get
		{
			if (isSeenProviders == null)
			{
				return IsSeen;
			}
			foreach (IIsSeenProvider isSeenProvider in isSeenProviders)
			{
				if (!isSeenProvider.IsSeen)
				{
					return false;
				}
			}
			return IsSeen;
		}
		set
		{
			IsSeen = value;
			if (isSeenProviders == null)
			{
				return;
			}
			foreach (IIsSeenProvider isSeenProvider in isSeenProviders)
			{
				isSeenProvider.IsSeen = value;
			}
		}
	}

	protected override void Awake()
	{
		base.Awake();
		if ((bool)newDot)
		{
			newDotInitialScale = newDot.transform.localScale;
		}
		Pane = GetComponentInParent<InventoryPane>();
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		if ((bool)Pane)
		{
			Pane.OnPaneStart += ResetIsScaling;
			Pane.OnPostPaneStart += UpdateDisplay;
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		if ((bool)Pane)
		{
			Pane.OnPaneStart -= ResetIsScaling;
			Pane.OnPostPaneStart -= UpdateDisplay;
		}
		if (isScaling)
		{
			isScaling = false;
			newDot.SetActive(value: false);
		}
	}

	protected virtual void OnDestroy()
	{
		if ((bool)Pane)
		{
			Pane = null;
		}
	}

	private void ResetIsScaling()
	{
		isScaling = false;
	}

	protected override void UpdateDisplay()
	{
		base.UpdateDisplay();
		if ((bool)newDot && !isScaling)
		{
			if (!IsSeenAll)
			{
				isNew = true;
			}
			newDot.SetActive(isNew);
			newDot.transform.localScale = newDotInitialScale;
		}
	}

	public override void Select(InventoryItemManager.SelectionDirection? direction)
	{
		base.Select(direction);
		if (!isNew)
		{
			return;
		}
		isNew = false;
		if ((bool)newDot)
		{
			newDot.transform.ScaleTo(this, Vector3.zero, UI.NewDotScaleTime, UI.NewDotScaleDelay, dontTrack: false, isRealtime: true);
			IsSeenAll = true;
			isScaling = true;
			return;
		}
		try
		{
			IsSeenAll = true;
		}
		catch (NotImplementedException)
		{
		}
	}

	public void RegisterIsSeenProvider(IIsSeenProvider provider)
	{
		if (isSeenProviders == null)
		{
			isSeenProviders = new List<IIsSeenProvider>();
		}
		isSeenProviders.Add(provider);
	}

	public void DeregisterIsSeenProvider(IIsSeenProvider provider)
	{
		isSeenProviders?.Remove(provider);
	}
}

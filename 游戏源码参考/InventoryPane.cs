using System;
using TeamCherry.Localization;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Serialization;

public class InventoryPane : InventoryPaneBase
{
	[SerializeField]
	[FormerlySerializedAs("DisplayName")]
	private LocalisedString displayName;

	[SerializeField]
	private Sprite listIcon;

	[SerializeField]
	[FormerlySerializedAs("PlayerDataTest")]
	private PlayerDataTest playerDataTest;

	[SerializeField]
	[PlayerDataField(typeof(bool), false)]
	private string hasNewPDBool;

	private int newItemsCount;

	private IInventoryPaneAvailabilityProvider availabilityProvider;

	public string DisplayName => displayName;

	public Sprite ListIcon => listIcon;

	public virtual bool IsAvailable
	{
		get
		{
			if (playerDataTest.IsFulfilled)
			{
				if (availabilityProvider == null)
				{
					availabilityProvider = GetComponent<IInventoryPaneAvailabilityProvider>();
				}
				if (availabilityProvider != null)
				{
					return availabilityProvider.IsAvailable();
				}
				return true;
			}
			return false;
		}
	}

	public bool IsAnyUpdates
	{
		get
		{
			if (string.IsNullOrEmpty(hasNewPDBool))
			{
				return false;
			}
			return PlayerData.instance.GetVariable<bool>(hasNewPDBool);
		}
		set
		{
			if (!string.IsNullOrEmpty(hasNewPDBool))
			{
				PlayerData.instance.SetVariable(hasNewPDBool, value);
			}
		}
	}

	public InventoryPane RootPane { get; set; }

	public event Action<bool> NewItemsUpdated;

	protected virtual void Awake()
	{
	}

	public override void PaneStart()
	{
		base.PaneStart();
		if (IsAnyUpdates)
		{
			IsAnyUpdates = false;
			if (this.NewItemsUpdated != null)
			{
				this.NewItemsUpdated(obj: false);
			}
		}
	}

	public virtual InventoryPane Get()
	{
		return this;
	}
}

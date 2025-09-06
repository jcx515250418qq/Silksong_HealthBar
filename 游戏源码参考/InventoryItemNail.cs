using System;
using TeamCherry.Localization;
using UnityEngine;

public class InventoryItemNail : InventoryItemUpdateable
{
	[Serializable]
	private class DisplayState
	{
		public GameObject DisplayObject;

		public LocalisedString DisplayName;

		public LocalisedString Description;
	}

	[Space]
	[SerializeField]
	private DisplayState[] displayStates;

	private DisplayState currentState;

	public override string DisplayName => currentState.DisplayName;

	public override string Description => currentState.Description;

	protected override bool IsSeen
	{
		get
		{
			return !PlayerData.instance.InvNailHasNew;
		}
		set
		{
			PlayerData.instance.InvNailHasNew = !value;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		InventoryPane componentInParent = GetComponentInParent<InventoryPane>();
		if ((bool)componentInParent)
		{
			componentInParent.OnPaneStart += UpdateState;
		}
	}

	protected override void Start()
	{
		base.Start();
		UpdateState();
	}

	private void UpdateState()
	{
		if (CollectableItemManager.IsInHiddenMode())
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: true);
		}
		DisplayState[] array = displayStates;
		foreach (DisplayState displayState in array)
		{
			if ((bool)displayState.DisplayObject)
			{
				displayState.DisplayObject.SetActive(value: false);
			}
		}
		int num = PlayerData.instance.nailUpgrades;
		int num2 = displayStates.Length - 1;
		if (num > num2)
		{
			num = num2;
		}
		currentState = displayStates[num];
		if ((bool)currentState.DisplayObject)
		{
			currentState.DisplayObject.SetActive(value: true);
		}
	}
}

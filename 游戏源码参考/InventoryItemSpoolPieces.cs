using System;
using TeamCherry.Localization;
using UnityEngine;

public class InventoryItemSpoolPieces : InventoryItemSelectableDirectional
{
	[Serializable]
	private struct DisplayState
	{
		public GameObject DisplayObject;

		public LocalisedString DisplayName;

		public LocalisedString Description;
	}

	[Space]
	[SerializeField]
	private DisplayState emptyState;

	[SerializeField]
	private DisplayState halfState;

	[SerializeField]
	private DisplayState fullState;

	private DisplayState currentState;

	public override string DisplayName => currentState.DisplayName;

	public override string Description => currentState.Description;

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
		PlayerData instance = PlayerData.instance;
		if (instance.silkSpoolParts <= 0 && instance.silkMax <= 9)
		{
			if (base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(value: false);
			}
			return;
		}
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: true);
		}
		if (instance.silkMax >= 18)
		{
			currentState = fullState;
		}
		else
		{
			switch (instance.silkSpoolParts)
			{
			case 0:
				currentState = emptyState;
				break;
			case 1:
				currentState = (CollectableItemManager.IsInHiddenMode() ? emptyState : halfState);
				break;
			default:
				currentState = fullState;
				break;
			}
		}
		if ((bool)emptyState.DisplayObject)
		{
			emptyState.DisplayObject.SetActive(value: false);
		}
		if ((bool)halfState.DisplayObject)
		{
			halfState.DisplayObject.SetActive(value: false);
		}
		if ((bool)fullState.DisplayObject)
		{
			fullState.DisplayObject.SetActive(value: false);
		}
		if ((bool)currentState.DisplayObject)
		{
			currentState.DisplayObject.SetActive(value: true);
		}
	}
}

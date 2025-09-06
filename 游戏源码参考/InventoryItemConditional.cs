using UnityEngine;

public class InventoryItemConditional : InventoryItemBasic
{
	[Space]
	public PlayerDataTest Test;

	[SerializeField]
	private bool hideWhenInventoryBare;

	[SerializeField]
	private GameObject overrideSetActive;

	private InventoryItemButtonPrompt buttonPrompt;

	public override string DisplayName
	{
		get
		{
			if (!GetObj().activeSelf)
			{
				return string.Empty;
			}
			return base.DisplayName;
		}
	}

	public override string Description
	{
		get
		{
			if (!GetObj().activeSelf)
			{
				return string.Empty;
			}
			return base.Description;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		InventoryPaneBase componentInParent = GetComponentInParent<InventoryPaneBase>();
		if ((bool)componentInParent)
		{
			componentInParent.OnPaneStart += Evaluate;
		}
		buttonPrompt = GetComponent<InventoryItemButtonPrompt>();
		Evaluate();
	}

	private void Evaluate()
	{
		bool active = (!hideWhenInventoryBare || !CollectableItemManager.IsInHiddenMode()) && Test.IsFulfilled;
		GetObj().SetActive(active);
		if ((bool)buttonPrompt)
		{
			buttonPrompt.enabled = active;
		}
	}

	private GameObject GetObj()
	{
		if (!overrideSetActive)
		{
			return base.gameObject;
		}
		return overrideSetActive;
	}
}

using System;
using TeamCherry.Localization;
using UnityEngine;

public class InventoryItemSelectableButtonEvent : InventoryItemSelectableDirectional
{
	public Action ButtonActivated;

	[SerializeField]
	private SetTextMeshProGameText interactionDisplayText;

	[SerializeField]
	private LocalisedString interactionText;

	private LocalisedString previousInteractionText;

	private bool isSelected;

	public LocalisedString InteractionText
	{
		get
		{
			return interactionText;
		}
		set
		{
			interactionText = value;
			if (isSelected)
			{
				UpdateInteractionText();
			}
		}
	}

	public override bool Submit()
	{
		if (ButtonActivated != null)
		{
			ButtonActivated();
			return true;
		}
		return base.Submit();
	}

	public override void Select(InventoryItemManager.SelectionDirection? direction)
	{
		base.Select(direction);
		isSelected = true;
		UpdateInteractionText();
	}

	public override void Deselect()
	{
		base.Deselect();
		isSelected = false;
		if ((bool)interactionDisplayText)
		{
			interactionDisplayText.Text = previousInteractionText;
		}
	}

	private void UpdateInteractionText()
	{
		if ((bool)interactionDisplayText)
		{
			previousInteractionText = interactionDisplayText.Text;
			interactionDisplayText.Text = interactionText;
		}
	}
}

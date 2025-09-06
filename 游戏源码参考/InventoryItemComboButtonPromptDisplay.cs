using System;
using GlobalEnums;
using TMProOld;
using TeamCherry.Localization;
using UnityEngine;

public class InventoryItemComboButtonPromptDisplay : MonoBehaviour, InventoryItemButtonPromptDisplayList.IPromptDisplayListOrder
{
	[Serializable]
	public struct Display
	{
		public HeroActionButton ActionButton;

		public AttackToolBinding DirectionModifier;

		public LocalisedString PromptText;

		public bool ShowHold;
	}

	[SerializeField]
	private GameObject parentWithoutModifier;

	[SerializeField]
	private ActionButtonIcon actionButtonWithoutModifier;

	[SerializeField]
	private GameObject parentWithModifier;

	[SerializeField]
	private ActionButtonIcon actionButtonWithModifier;

	[SerializeField]
	private GameObject modifierHoldPrompt;

	[SerializeField]
	private GameObject modifierUp;

	[SerializeField]
	private GameObject modifierDown;

	[SerializeField]
	private TMP_Text promptText;

	public int order { get; set; }

	Transform InventoryItemButtonPromptDisplayList.IPromptDisplayListOrder.transform => base.transform;

	public void Show(Display display)
	{
		base.gameObject.SetActive(value: true);
		ActionButtonIcon actionButtonIcon;
		switch (display.DirectionModifier)
		{
		case AttackToolBinding.Neutral:
			parentWithModifier.SetActive(value: false);
			parentWithoutModifier.SetActive(value: true);
			actionButtonIcon = actionButtonWithoutModifier;
			break;
		case AttackToolBinding.Up:
			parentWithModifier.SetActive(value: true);
			parentWithoutModifier.SetActive(value: false);
			modifierUp.SetActive(value: true);
			modifierDown.SetActive(value: false);
			actionButtonIcon = actionButtonWithModifier;
			break;
		case AttackToolBinding.Down:
			parentWithModifier.SetActive(value: true);
			parentWithoutModifier.SetActive(value: false);
			modifierUp.SetActive(value: false);
			modifierDown.SetActive(value: true);
			actionButtonIcon = actionButtonWithModifier;
			break;
		default:
			throw new NotImplementedException();
		}
		actionButtonIcon.SetAction(display.ActionButton);
		promptText.text = display.PromptText;
		SetTextMeshProGameText component = promptText.GetComponent<SetTextMeshProGameText>();
		if (component != null)
		{
			component.Text = display.PromptText;
		}
		if ((bool)modifierHoldPrompt)
		{
			modifierHoldPrompt.SetActive(display.ShowHold);
		}
	}

	public void Hide()
	{
		parentWithModifier.SetActive(value: false);
		parentWithoutModifier.SetActive(value: false);
		base.gameObject.SetActive(value: false);
	}
}

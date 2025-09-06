using System;
using JetBrains.Annotations;
using TMProOld;
using TeamCherry.Localization;
using UnityEngine;

public class InteractEvents : NPCControlBase
{
	[SerializeField]
	private bool doMovePlayer;

	[Space]
	[SerializeField]
	private PlayMakerFSM targetFSM;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("IsFsmEventValid")]
	private string interactEvent = "INTERACT";

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("IsFsmEventValidOptional")]
	private string canInteractEvent;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("IsFsmEventValidOptional")]
	private string canNotInteractEvent;

	[Space]
	[SerializeField]
	[LocalisedString.NotRequired]
	private LocalisedString inspectText;

	[SerializeField]
	[LocalisedString.NotRequired]
	private LocalisedString yesNoPrompt;

	private bool showingYesNo;

	private bool isInteracting;

	public override bool AutoEnd => !showingYesNo;

	protected override bool AllowMovePlayer => doMovePlayer;

	public event Action Interacted;

	[UsedImplicitly]
	private bool? IsFsmEventValid(string eventName)
	{
		if (string.IsNullOrEmpty(eventName))
		{
			return null;
		}
		if (!targetFSM)
		{
			return false;
		}
		return targetFSM.IsEventValid(eventName, isRequired: true);
	}

	[UsedImplicitly]
	private bool? IsFsmEventValidOptional(string eventName)
	{
		return targetFSM.IsEventValid(eventName, isRequired: false);
	}

	protected override void OnStartDialogue()
	{
		showingYesNo = false;
		isInteracting = true;
		if (!InteractableBase.TrySendStateChangeEvent(targetFSM, interactEvent) && this.Interacted == null)
		{
			isInteracting = false;
		}
		else if (isInteracting)
		{
			DisableInteraction();
			if (inspectText.IsEmpty)
			{
				ShowYesNo();
				return;
			}
			DialogueBox.StartConversation(inspectText, this, overrideContinue: false, new DialogueBox.DisplayOptions
			{
				Alignment = TextAlignmentOptions.Top,
				ShowDecorators = true,
				TextColor = Color.white
			}, ShowYesNo);
		}
	}

	private void ShowYesNo()
	{
		if (yesNoPrompt.IsEmpty)
		{
			this.Interacted?.Invoke();
			return;
		}
		showingYesNo = true;
		DialogueYesNoBox.Open(delegate
		{
			this.Interacted?.Invoke();
			showingYesNo = false;
		}, EndInteraction, returnHud: true, yesNoPrompt);
	}

	protected override void OnEndDialogue()
	{
		isInteracting = false;
		if (!showingYesNo)
		{
			EnableInteraction();
		}
	}

	public void EndInteraction()
	{
		showingYesNo = false;
		EndDialogue();
	}

	public override void CanInteract()
	{
		if (!string.IsNullOrEmpty(canInteractEvent))
		{
			targetFSM.SendEventSafe(canInteractEvent);
		}
	}

	public override void CanNotInteract()
	{
		if (!string.IsNullOrEmpty(canNotInteractEvent))
		{
			targetFSM.SendEventSafe(canNotInteractEvent);
		}
	}
}

using TeamCherry.Localization;
using UnityEngine;

public abstract class BasicNPCBase : NPCControlBase
{
	[Space]
	[SerializeField]
	private DialogueBox.DisplayOptions displayOptions = DialogueBox.DisplayOptions.Default;

	[Space]
	[SerializeField]
	private PlayMakerFSM eventTarget;

	[Tooltip("Sent as soon as NPC is interacted with.")]
	[SerializeField]
	[ModifiableProperty]
	[Conditional("eventTarget", true, false, false)]
	[InspectorValidation("IsEventValid")]
	private string dialogueStartingEvent;

	[Tooltip("Sent once player has moved into position and dialogue has started.")]
	[SerializeField]
	[ModifiableProperty]
	[Conditional("eventTarget", true, false, false)]
	[InspectorValidation("IsEventValid")]
	private string dialogueStartedEvent;

	[Tooltip("Sent after dialogue has ended.")]
	[SerializeField]
	[ModifiableProperty]
	[Conditional("eventTarget", true, false, false)]
	[InspectorValidation("IsEventValid")]
	private string dialogueEndedEvent;

	private bool? IsEventValid(string eventName)
	{
		return eventTarget.IsEventValid(eventName, isRequired: false);
	}

	protected override void Awake()
	{
		base.Awake();
		base.StartingDialogue += delegate
		{
			SendDialogueEvent(dialogueStartingEvent);
		};
	}

	private void SendDialogueEvent(string eventName)
	{
		if ((bool)eventTarget && !string.IsNullOrEmpty(eventName))
		{
			eventTarget.SendEvent(eventName);
		}
	}

	protected override void OnStartDialogue()
	{
		DisableInteraction();
		LocalisedString dialogue = GetDialogue();
		if (!dialogue.IsEmpty)
		{
			DialogueBox.StartConversation(dialogue, this, overrideContinue: false, displayOptions);
		}
		else
		{
			Debug.LogError("NPC Dialogue Text is empty! Canceling...", this);
			OnEndDialogue();
		}
		SendDialogueEvent(dialogueStartedEvent);
	}

	protected override void OnEndDialogue()
	{
		EnableInteraction();
		SendDialogueEvent(dialogueEndedEvent);
	}

	protected abstract LocalisedString GetDialogue();
}

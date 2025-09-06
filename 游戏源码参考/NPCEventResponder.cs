using UnityEngine;
using UnityEngine.Events;

public class NPCEventResponder : MonoBehaviour
{
	[SerializeField]
	private NPCControlBase control;

	[Space]
	[SerializeField]
	private PlayMakerFSM fsmTarget;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("ValidateFsmEvent")]
	[Conditional("fsmTarget", true, false, false)]
	private string convoStartEvent;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("ValidateFsmEvent")]
	[Conditional("fsmTarget", true, false, false)]
	private string convoEndEvent;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("ValidateFsmEvent")]
	[Conditional("fsmTarget", true, false, false)]
	private string newLineEvent;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("ValidateFsmEvent")]
	[Conditional("fsmTarget", true, false, false)]
	private string newLineNPCEvent;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("ValidateFsmEvent")]
	[Conditional("fsmTarget", true, false, false)]
	private string newLinePlayerEvent;

	[Space]
	public UnityEvent OnNewLineNpc;

	private bool? ValidateFsmEvent(string value)
	{
		return fsmTarget.IsEventValid(value, isRequired: false);
	}

	private void Awake()
	{
		if (!control)
		{
			return;
		}
		if ((bool)fsmTarget)
		{
			if (!string.IsNullOrEmpty(convoStartEvent))
			{
				control.StartedDialogue += delegate
				{
					fsmTarget.SendEvent(convoStartEvent);
				};
			}
			if (!string.IsNullOrEmpty(convoEndEvent))
			{
				control.EndingDialogue += delegate
				{
					fsmTarget.SendEvent(convoEndEvent);
				};
			}
			if (!string.IsNullOrEmpty(newLinePlayerEvent))
			{
				control.StartedNewLine += delegate(DialogueBox.DialogueLine line)
				{
					if (line.IsPlayer)
					{
						fsmTarget.SendEvent(newLinePlayerEvent);
					}
				};
			}
			if (!string.IsNullOrEmpty(newLineEvent))
			{
				control.StartedNewLine += delegate
				{
					fsmTarget.SendEvent(newLineEvent);
				};
			}
		}
		control.StartedNewLine += delegate(DialogueBox.DialogueLine line)
		{
			if (!line.IsPlayer)
			{
				if ((bool)fsmTarget && !string.IsNullOrEmpty(newLineNPCEvent))
				{
					fsmTarget.SendEvent(newLineNPCEvent);
				}
				OnNewLineNpc.Invoke();
			}
		};
	}
}

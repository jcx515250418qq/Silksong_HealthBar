using System;
using UnityEngine;

public class PlayMakerNPC : NPCControlBase
{
	public const string DIALOGUE_END_EVENT = "CONVO_END";

	public const string LINE_END_EVENT = "LINE_END";

	public const string DIALOGUE_END_FORCED_EVENT = "CONVO_END_FORCED";

	[Space]
	[SerializeField]
	private PlayMakerFSM dialogueFsm;

	[SerializeField]
	private PlayMakerFSM[] secondaryFsms;

	[Space]
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("IsFsmEventValidNotRequired")]
	private string preInteractEvent = string.Empty;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("IsFsmEventValidRequired")]
	private string interactEvent = "INTERACT";

	private bool isFirstLine;

	private bool wasAutoStarted;

	private bool overridingTalkTable;

	private int id;

	public override bool AutoEnd => false;

	protected override bool AutoCallEndAction => false;

	protected override bool AllowMovePlayer => !IsTemporary;

	public bool IsRunningDialogue { get; private set; }

	public bool IsTemporary { get; private set; }

	public PlayMakerFSM CustomEventTarget { get; set; }

	protected override void OnDisable()
	{
		base.OnDisable();
		RemoveTalkTableOverride();
	}

	private bool? IsFsmEventValidRequired(string eventName)
	{
		return dialogueFsm.IsEventValid(eventName, isRequired: true);
	}

	private bool? IsFsmEventValidNotRequired(string eventName)
	{
		return dialogueFsm.IsEventValid(eventName, isRequired: false);
	}

	public override void OnDialogueBoxEnded()
	{
		isFirstLine = true;
		SendEvent("LINE_END");
		SendEvent("CONVO_END");
	}

	public void CloseDialogueBox(bool returnControl, bool returnHud, Action onBoxHidden)
	{
		if (!IsRunningDialogue)
		{
			if (onBoxHidden != null)
			{
				onBoxHidden();
			}
			return;
		}
		Action action = null;
		if (returnControl)
		{
			RecordControlVersion();
			action = delegate
			{
				if (onBoxHidden != null)
				{
					onBoxHidden();
				}
				IsRunningDialogue = false;
				EnableInteraction();
				CallEndAction();
				if (IsTemporary)
				{
					UnityEngine.Object.Destroy(this);
				}
			};
		}
		DialogueBox.EndConversation(returnHud, action ?? onBoxHidden);
	}

	protected override void OnNewLineStarted(DialogueBox.DialogueLine line)
	{
		if (isFirstLine)
		{
			isFirstLine = false;
		}
		else
		{
			SendEvent("LINE_END");
		}
		if (!string.IsNullOrEmpty(line.Event))
		{
			SendEvent(line.Event);
		}
	}

	protected override void OnStartingDialogue()
	{
		if (!string.IsNullOrEmpty(preInteractEvent))
		{
			SendEvent(preInteractEvent);
		}
	}

	protected override void OnStartDialogue()
	{
		DisableInteraction();
		IsRunningDialogue = true;
		isFirstLine = true;
		if (string.IsNullOrEmpty(interactEvent) || !SendEvent(interactEvent))
		{
			if (wasAutoStarted)
			{
				wasAutoStarted = false;
			}
			else if (!IsTemporary)
			{
				EnableInteraction();
				CallEndAction();
				IsRunningDialogue = false;
			}
		}
	}

	private bool SendEvent(string eventName)
	{
		if (secondaryFsms != null)
		{
			PlayMakerFSM[] array = secondaryFsms;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SendEventRecursive(eventName);
			}
		}
		PlayMakerFSM playMakerFSM = (CustomEventTarget ? CustomEventTarget : dialogueFsm);
		if (!playMakerFSM.enabled)
		{
			playMakerFSM.enabled = true;
		}
		return playMakerFSM.SendEventRecursive(eventName);
	}

	public void ForceEndDialogue()
	{
		CancelHeroMove();
		SendEvent("CONVO_END_FORCED");
		CloseDialogueBox(returnControl: true, returnHud: true, null);
	}

	public static PlayMakerNPC GetNewTemp(PlayMakerFSM eventTarget)
	{
		if (!eventTarget)
		{
			return null;
		}
		PlayMakerNPC playMakerNPC = eventTarget.gameObject.AddComponent<PlayMakerNPC>();
		playMakerNPC.IsTemporary = true;
		playMakerNPC.interactEvent = null;
		playMakerNPC.dialogueFsm = eventTarget;
		return playMakerNPC;
	}

	public void SetAutoStarting()
	{
		wasAutoStarted = true;
	}

	public void SetTalkTableOverride(RandomAudioClipTable table)
	{
		overridingTalkTable = true;
		id = HeroTalkAnimation.SetTalkTableOverride(table);
	}

	public void RemoveTalkTableOverride()
	{
		if (overridingTalkTable)
		{
			HeroTalkAnimation.RemoveTalkTableOverride(id);
			overridingTalkTable = false;
		}
	}
}

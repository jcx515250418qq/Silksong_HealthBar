using System;
using HutongGames.PlayMaker;
using UnityEngine;

public class NpcDialogueTitle : MonoBehaviour
{
	[Serializable]
	private class SpeakerTitle
	{
		public string Title;

		public string SpeakerEvent;
	}

	[SerializeField]
	[Obsolete]
	[HideInInspector]
	private NPCControlBase npcControl;

	[SerializeField]
	private NPCControlBase[] npcControls;

	[Space]
	[SerializeField]
	private bool displayRight;

	[SerializeField]
	[Obsolete]
	[HideInInspector]
	private string npcTitle;

	[SerializeField]
	private SpeakerTitle[] titles;

	private bool isShowing;

	private DialogueBox.DialogueLine previousFirstLine;

	private bool skipNextHide;

	private void Reset()
	{
		npcControls = GetComponents<NPCControlBase>();
	}

	private void OnValidate()
	{
		if ((bool)npcControl)
		{
			npcControls = new NPCControlBase[1] { npcControl };
			npcControl = null;
		}
		if (!string.IsNullOrEmpty(npcTitle))
		{
			titles = new SpeakerTitle[1]
			{
				new SpeakerTitle
				{
					Title = npcTitle
				}
			};
			npcTitle = null;
		}
	}

	private void Awake()
	{
		OnValidate();
	}

	private void OnEnable()
	{
		NPCControlBase[] array = npcControls;
		foreach (NPCControlBase nPCControlBase in array)
		{
			if ((bool)nPCControlBase)
			{
				nPCControlBase.OpeningDialogueBox += Show;
				nPCControlBase.EndingDialogue += Hide;
			}
		}
	}

	private void OnDisable()
	{
		NPCControlBase[] array = npcControls;
		foreach (NPCControlBase nPCControlBase in array)
		{
			if ((bool)nPCControlBase)
			{
				nPCControlBase.OpeningDialogueBox -= Show;
				nPCControlBase.EndingDialogue -= Hide;
			}
		}
		skipNextHide = false;
		if (isShowing)
		{
			Hide();
		}
	}

	public void EnableAndShow()
	{
		if (!base.enabled)
		{
			base.enabled = true;
			Show(previousFirstLine);
		}
	}

	private void Show(DialogueBox.DialogueLine firstLine)
	{
		previousFirstLine = firstLine;
		if (isShowing)
		{
			return;
		}
		isShowing = true;
		SpeakerTitle speakerTitle = null;
		SpeakerTitle[] array = titles;
		foreach (SpeakerTitle speakerTitle2 in array)
		{
			if (speakerTitle == null || speakerTitle2.SpeakerEvent == firstLine.Event)
			{
				speakerTitle = speakerTitle2;
			}
		}
		if (speakerTitle == null)
		{
			Debug.LogError("No NPC title found for speaker event: " + firstLine.Event, this);
			return;
		}
		AreaTitle instance = ManagerSingleton<AreaTitle>.Instance;
		if ((bool)instance)
		{
			GameObject gameObject = instance.gameObject;
			PlayMakerFSM gameObjectFsm = ActionHelpers.GetGameObjectFsm(gameObject, "Area Title Control");
			bool value = gameObjectFsm.FsmVariables.FindFsmBool("NPC Title Waiting").Value;
			FsmString fsmString = gameObjectFsm.FsmVariables.FindFsmString("Area Event");
			if (value && fsmString.Value == speakerTitle.Title)
			{
				gameObjectFsm.SendEventSafe("NPC TITLE DOWN CANCEL");
				return;
			}
			gameObject.SetActive(value: false);
			gameObjectFsm.FsmVariables.FindFsmBool("Visited").Value = true;
			gameObjectFsm.FsmVariables.FindFsmBool("NPC Title").Value = true;
			gameObjectFsm.FsmVariables.FindFsmBool("Display Right").Value = displayRight;
			fsmString.Value = speakerTitle.Title;
			gameObject.SetActive(value: true);
		}
	}

	public void Hide()
	{
		if (skipNextHide)
		{
			skipNextHide = false;
			return;
		}
		isShowing = false;
		AreaTitle instance = ManagerSingleton<AreaTitle>.Instance;
		if ((bool)instance)
		{
			ActionHelpers.GetGameObjectFsm(instance.gameObject, "Area Title Control").SendEventSafe("NPC TITLE DOWN");
		}
	}

	public void SkipNextHide()
	{
		skipNextHide = true;
	}
}

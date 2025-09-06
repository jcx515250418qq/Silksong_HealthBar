using System;
using System.Collections.Generic;
using TeamCherry.Localization;
using UnityEngine;
using UnityEngine.Events;

public class BasicNPC : BasicNPCBase
{
	[Space]
	[SerializeField]
	private GameObject activateOnInteract;

	[SerializeField]
	private PersistentIntItem stateTracker;

	[SerializeField]
	private LocalisedString[] talkText;

	[SerializeField]
	[LocalisedString.NotRequired]
	private LocalisedString repeatText;

	[SerializeField]
	[LocalisedString.NotRequired]
	private LocalisedString returnText;

	[SerializeField]
	[Obsolete("Use giveOnFirstTalkItems instead.")]
	[HideInInspector]
	private SavedItem giveOnFirstTalk;

	[Space]
	[SerializeField]
	private List<SavedItem> giveOnFirstTalkItems = new List<SavedItem>();

	[Space]
	public UnityEvent OnEnd;

	private int talkState;

	private int endTalkState;

	private bool startedAlreadySpoken;

	public List<SavedItem> GiveOnFirstTalk => giveOnFirstTalkItems;

	public bool HasFinishedTalks => talkState >= talkText.Length;

	public bool HasRepeated
	{
		get
		{
			if (repeatText.IsEmpty)
			{
				return HasFinishedTalks;
			}
			return talkState >= talkText.Length + 1;
		}
	}

	protected override void Awake()
	{
		Upgrade();
		base.Awake();
		if ((bool)stateTracker)
		{
			stateTracker.OnGetSaveState += delegate(out int value)
			{
				value = talkState;
			};
			stateTracker.OnSetSaveState += delegate(int value)
			{
				talkState = value;
				endTalkState = value;
				startedAlreadySpoken = HasFinishedTalks;
			};
		}
	}

	protected override void OnValidate()
	{
		base.OnValidate();
		Upgrade();
	}

	private void Upgrade()
	{
		if (giveOnFirstTalk != null)
		{
			giveOnFirstTalkItems.RemoveAll((SavedItem o) => o == null);
			if (!giveOnFirstTalkItems.Contains(giveOnFirstTalk))
			{
				giveOnFirstTalkItems.Add(giveOnFirstTalk);
			}
			giveOnFirstTalk = null;
		}
	}

	protected override void OnStartDialogue()
	{
		base.OnStartDialogue();
		if ((bool)activateOnInteract)
		{
			activateOnInteract.SetActive(value: true);
		}
	}

	protected override void OnEndDialogue()
	{
		base.OnEndDialogue();
		if (talkState == 0 && giveOnFirstTalkItems.Count > 0)
		{
			giveOnFirstTalkItems.RemoveAll((SavedItem o) => o == null);
			foreach (SavedItem giveOnFirstTalkItem in giveOnFirstTalkItems)
			{
				giveOnFirstTalkItem.Get();
			}
		}
		talkState = endTalkState;
		if ((bool)activateOnInteract)
		{
			activateOnInteract.SetActive(value: false);
		}
		OnEnd?.Invoke();
	}

	protected override LocalisedString GetDialogue()
	{
		if (startedAlreadySpoken && !returnText.IsEmpty)
		{
			return returnText;
		}
		if (HasFinishedTalks && !repeatText.IsEmpty)
		{
			endTalkState = talkText.Length + 1;
			return repeatText;
		}
		if (talkText.Length == 0)
		{
			return default(LocalisedString);
		}
		talkState = Mathf.Clamp(talkState, 0, talkText.Length - 1);
		LocalisedString result = talkText[talkState];
		endTalkState = talkState + 1;
		return result;
	}
}

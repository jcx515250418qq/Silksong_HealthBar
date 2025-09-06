using System;
using TeamCherry.Localization;
using TeamCherry.SharedUtils;
using UnityEngine;

public class BasicNPCBoolTest : BasicNPCBase
{
	[Serializable]
	private class ConditionalTalk
	{
		[PlayerDataField(typeof(bool), false)]
		public string BoolTest;

		public bool ExpectedBoolValue;

		public LocalisedString Text;

		public bool IsTestFulfilled
		{
			get
			{
				if (string.IsNullOrEmpty(BoolTest))
				{
					return true;
				}
				return PlayerData.instance.GetVariable<bool>(BoolTest) == ExpectedBoolValue;
			}
		}
	}

	[Space]
	[SerializeField]
	private GameObject activateOnInteract;

	[SerializeField]
	private PersistentIntItem stateTracker;

	[SerializeField]
	private ConditionalTalk[] talks;

	[SerializeField]
	[LocalisedString.NotRequired]
	private LocalisedString repeatText;

	[SerializeField]
	[LocalisedString.NotRequired]
	private LocalisedString returnText;

	protected int talkState;

	protected int endTalkState;

	protected bool startedAlreadySpoken;

	private bool HasFinishedAllTalks => GetNextTalk(canSettleOnLast: false) == null;

	protected override void Awake()
	{
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
				startedAlreadySpoken = HasFinishedAllTalks;
			};
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
		talkState = endTalkState;
		if ((bool)activateOnInteract)
		{
			activateOnInteract.SetActive(value: false);
		}
	}

	protected override LocalisedString GetDialogue()
	{
		if (startedAlreadySpoken && !returnText.IsEmpty)
		{
			return returnText;
		}
		if (HasFinishedAllTalks && !repeatText.IsEmpty)
		{
			return repeatText;
		}
		if (talks.Length == 0)
		{
			return default(LocalisedString);
		}
		return GetNextTalk(canSettleOnLast: true, out endTalkState)?.Text ?? default(LocalisedString);
	}

	private ConditionalTalk GetNextTalk(bool canSettleOnLast)
	{
		int newTalkMask;
		return GetNextTalk(canSettleOnLast, out newTalkMask);
	}

	private ConditionalTalk GetNextTalk(bool canSettleOnLast, out int newTalkMask)
	{
		ConditionalTalk result = null;
		for (int i = 0; i < talks.Length; i++)
		{
			ConditionalTalk conditionalTalk = talks[i];
			if (conditionalTalk.IsTestFulfilled)
			{
				int num = 1 << i;
				if ((talkState & num) != num)
				{
					result = conditionalTalk;
					newTalkMask = talkState | num;
					return conditionalTalk;
				}
				if (canSettleOnLast)
				{
					result = conditionalTalk;
				}
			}
		}
		newTalkMask = talkState;
		return result;
	}
}

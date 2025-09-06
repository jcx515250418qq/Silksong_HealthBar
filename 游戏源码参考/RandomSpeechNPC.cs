using System;
using System.Linq;
using TeamCherry.Localization;
using UnityEngine;

public class RandomSpeechNPC : BasicNPCBase
{
	[Serializable]
	private class RandomDialogue : Probability.ProbabilityBase<LocalisedString>
	{
		public LocalisedString Dialogue;

		public override LocalisedString Item => Dialogue;
	}

	[Space]
	[SerializeField]
	private PersistentIntItem stateTracker;

	[SerializeField]
	private LocalisedString[] meetText;

	[SerializeField]
	private RandomDialogue[] randomDialogues;

	[SerializeField]
	private float notChosenMultiplier = 2f;

	private float[] runningProbabilities;

	private int talkState;

	private int endTalkState;

	protected override void Awake()
	{
		base.Awake();
		runningProbabilities = randomDialogues.Select((RandomDialogue dlg) => dlg.Probability).ToArray();
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
			};
		}
	}

	protected override LocalisedString GetDialogue()
	{
		if (meetText.Length != 0 && talkState < meetText.Length)
		{
			if (talkState < 0)
			{
				talkState = 0;
			}
			LocalisedString result = meetText[talkState];
			endTalkState = talkState + 1;
			return result;
		}
		int chosenIndex;
		LocalisedString randomItemByProbability = Probability.GetRandomItemByProbability<RandomDialogue, LocalisedString>(randomDialogues, out chosenIndex, runningProbabilities);
		for (int i = 0; i < randomDialogues.Length; i++)
		{
			runningProbabilities[i] = ((i == chosenIndex) ? randomDialogues[i].Probability : (runningProbabilities[i] * notChosenMultiplier));
		}
		return randomItemByProbability;
	}

	protected override void OnEndDialogue()
	{
		base.OnEndDialogue();
		talkState = endTalkState;
	}
}

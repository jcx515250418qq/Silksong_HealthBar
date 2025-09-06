using System;
using UnityEngine;

[Serializable]
public struct QuestTest
{
	public FullQuestBase Quest;

	[Space]
	public bool CheckAvailable;

	[ModifiableProperty]
	[Conditional("CheckAvailable", true, false, false)]
	public bool IsAvailable;

	[Space]
	public bool CheckAccepted;

	[ModifiableProperty]
	[Conditional("CheckAccepted", true, false, false)]
	public bool IsAccepted;

	[Space]
	public bool CheckCompletedAmount;

	[ModifiableProperty]
	[Conditional("CheckCompletedAmount", true, false, false)]
	public int CompletedAmount;

	[Space]
	public bool CheckCompletable;

	[ModifiableProperty]
	[Conditional("CheckCompletable", true, false, false)]
	public bool IsCompletable;

	[Space]
	public bool CheckCompleted;

	[ModifiableProperty]
	[Conditional("CheckCompleted", true, false, false)]
	public bool IsCompleted;

	[Space]
	public bool CheckWasEverCompleted;

	[ModifiableProperty]
	[Conditional("CheckWasEverCompleted", true, false, false)]
	public bool WasEverCompleted;

	public bool IsFulfilled
	{
		get
		{
			if (!Quest)
			{
				Debug.LogError("Quest test has a null quest!");
				return false;
			}
			if (CheckAvailable && Quest.IsAvailable != IsAvailable)
			{
				return false;
			}
			if (CheckAccepted && Quest.IsAccepted != IsAccepted)
			{
				return false;
			}
			if (CheckCompletedAmount)
			{
				Debug.LogWarning("Checking completed amount for " + Quest.name + ". NOTE: this will check all quest targets and fail if any are below the desired amount (legacy behaviour)");
				int completedAmount = CompletedAmount;
				foreach (int counter in Quest.Counters)
				{
					if (counter < completedAmount)
					{
						return false;
					}
				}
			}
			if (CheckCompletable && Quest.CanComplete != IsCompletable)
			{
				return false;
			}
			if (CheckCompleted && Quest.IsCompleted != IsCompleted)
			{
				return false;
			}
			if (CheckWasEverCompleted && Quest.WasEverCompleted != WasEverCompleted)
			{
				return false;
			}
			return true;
		}
	}
}

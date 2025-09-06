using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class SaveSlotCompletionIcons : MonoBehaviour
{
	[Serializable]
	private struct CompletionIcon
	{
		public CompletionState state;

		public GameObject icon;
	}

	[Serializable]
	[Flags]
	public enum CompletionState
	{
		None = 0,
		Act2Regular = 1,
		Act2Cursed = 2,
		Act2SoulSnare = 4,
		Act3Ending = 8
	}

	[SerializeField]
	private List<CompletionIcon> completionIcons = new List<CompletionIcon>();

	private bool hasAwaken;

	private bool hasStarted;

	public bool OnAwake()
	{
		if (hasAwaken)
		{
			return false;
		}
		hasAwaken = true;
		completionIcons.RemoveAll((CompletionIcon o) => o.icon == null);
		return true;
	}

	public bool OnStart()
	{
		OnAwake();
		if (hasStarted)
		{
			return false;
		}
		hasStarted = true;
		return true;
	}

	private void Awake()
	{
		OnAwake();
	}

	public void SetCompletionIconState(SaveStats SaveStats)
	{
		if (SaveStats == null)
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		OnAwake();
		CompletionState completionState = SaveStats.LastCompletedEnding;
		if (CheatManager.IsCheatsEnabled && CheatManager.ShowAllCompletionIcons)
		{
			completionState = SaveStats.CompletedEndings;
		}
		if (completionState == CompletionState.None)
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		base.gameObject.SetActive(value: true);
		foreach (CompletionIcon completionIcon in completionIcons)
		{
			completionIcon.icon.gameObject.SetActive(completionIcon.state != 0 && completionState.HasFlag(completionIcon.state));
		}
	}
}

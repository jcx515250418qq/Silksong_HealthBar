using System;
using System.Reflection;
using UnityEngine;

public class BossDoorCompletionStates : MonoBehaviour
{
	[Serializable]
	public class CompletionState
	{
		public GameObject stateObject;

		public string sendEvent;
	}

	public CompletionState[] completionStates;

	[Space]
	[Tooltip("OPTIONAL - using an int, will ensure each state is seen at least once. (Requires a 2D trigger on this GameObject)")]
	public string stateSeenPlayerData;

	private int completedIndex;

	private void Start()
	{
		completedIndex = 0;
		FieldInfo[] fields = typeof(PlayerData).GetFields();
		foreach (FieldInfo fieldInfo in fields)
		{
			if (fieldInfo.FieldType == typeof(BossSequenceDoor.Completion) && ((BossSequenceDoor.Completion)fieldInfo.GetValue(GameManager.instance.playerData)).completed)
			{
				completedIndex++;
			}
		}
		if (!string.IsNullOrEmpty(stateSeenPlayerData))
		{
			int num = GameManager.instance.GetPlayerDataInt(stateSeenPlayerData) + 1;
			if (completedIndex > num)
			{
				completedIndex = num;
			}
		}
		if (completedIndex >= completionStates.Length)
		{
			completedIndex = completionStates.Length - 1;
		}
		for (int j = 0; j < completionStates.Length; j++)
		{
			if ((bool)completionStates[j].stateObject)
			{
				completionStates[j].stateObject.SetActive(value: false);
			}
		}
		CompletionState completionState = completionStates[completedIndex];
		if ((bool)completionState.stateObject)
		{
			completionState.stateObject.SetActive(value: true);
			if (!string.IsNullOrEmpty(completionState.sendEvent))
			{
				FSMUtility.SendEventToGameObject(completionState.stateObject, completionState.sendEvent);
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!string.IsNullOrEmpty(stateSeenPlayerData))
		{
			GameManager.instance.SetPlayerDataInt(stateSeenPlayerData, completedIndex);
		}
	}
}

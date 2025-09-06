using System;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

public class StateChangeSequence : MonoBehaviour
{
	[Serializable]
	private class State
	{
		public UnityEvent OnEnter;

		public string OnEnterEventRegister;

		[Space]
		public bool UseOnReturn;

		public UnityEvent OnReturn;

		public string OnReturnEventRegister;
	}

	[SerializeField]
	private PersistentIntItem persistent;

	[SerializeField]
	private int initialState;

	[Space]
	[SerializeField]
	private State[] states = new State[1];

	[Space]
	[SerializeField]
	[PlayerDataField(typeof(bool), false)]
	private string isCompleteBool;

	[SerializeField]
	private string awardAchievement;

	[SerializeField]
	private bool queueAchievement;

	private int stateValue = -1;

	private void Awake()
	{
		if (!persistent)
		{
			return;
		}
		persistent.OnGetSaveState += delegate(out int value)
		{
			value = stateValue;
		};
		persistent.OnSetSaveState += delegate(int value)
		{
			if (!CheckCompleteBool())
			{
				SetState(value, isReturning: true);
			}
		};
	}

	private void Start()
	{
		if (!CheckCompleteBool() && stateValue < 0)
		{
			SetState(initialState, isReturning: true);
		}
	}

	private bool CheckCompleteBool()
	{
		if (!string.IsNullOrEmpty(isCompleteBool) && PlayerData.instance.GetVariable<bool>(isCompleteBool))
		{
			SetState(states.Length - 1, isReturning: true);
			return true;
		}
		return false;
	}

	private void SetState(int index, bool isReturning)
	{
		if (index >= states.Length)
		{
			return;
		}
		int num = stateValue;
		stateValue = index;
		if (stateValue <= num)
		{
			return;
		}
		if (stateValue == states.Length - 1)
		{
			SetIsCompleteBool();
		}
		for (int i = Mathf.Max(0, num + 1); i <= stateValue; i++)
		{
			State state = states[i];
			UnityEvent unityEvent;
			string text;
			if (isReturning && state.UseOnReturn)
			{
				unityEvent = state.OnReturn;
				text = state.OnReturnEventRegister;
			}
			else
			{
				unityEvent = state.OnEnter;
				text = state.OnEnterEventRegister;
			}
			unityEvent?.Invoke();
			if (!string.IsNullOrEmpty(text))
			{
				EventRegister.SendEvent(text);
			}
		}
	}

	public void SetIsCompleteBool()
	{
		if (!string.IsNullOrEmpty(isCompleteBool))
		{
			PlayerData.instance.SetVariable(isCompleteBool, value: true);
		}
		if (!string.IsNullOrWhiteSpace(awardAchievement))
		{
			if (queueAchievement)
			{
				GameManager.instance.QueueAchievement(awardAchievement);
			}
			else
			{
				GameManager.instance.AwardAchievement(awardAchievement);
			}
		}
	}

	public void SetState(int index)
	{
		SetState(index, isReturning: false);
	}

	public void SetStateReturn(int index)
	{
		SetState(index, isReturning: true);
	}

	public void IncrementState()
	{
		SetState(stateValue + 1);
	}

	public void SetLastState()
	{
		SetState(states.Length - 1);
	}
}

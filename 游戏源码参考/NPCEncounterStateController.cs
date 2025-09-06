using System;
using System.Collections.Generic;
using GlobalEnums;
using TeamCherry.SharedUtils;
using UnityEngine;

public sealed class NPCEncounterStateController : MonoBehaviour
{
	[Serializable]
	private struct ObjectState
	{
		public GameObject gameObject;

		public bool isActive;

		public void Apply()
		{
			if ((bool)gameObject)
			{
				gameObject.SetActive(isActive);
			}
		}
	}

	[SerializeField]
	[PlayerDataField(typeof(NPCEncounterState), true)]
	private string encounterStateName;

	private NPCEncounterState localState;

	[SerializeField]
	private bool requireLeaveAuthorisation;

	[Range(0f, 1f)]
	[SerializeField]
	private float leaveProbability = 1f;

	[SerializeField]
	private bool disableSelf = true;

	[Tooltip("Applied when npc present")]
	[SerializeField]
	private List<ObjectState> presentStates = new List<ObjectState>();

	[Tooltip("Applied when npc away")]
	[SerializeField]
	private List<ObjectState> awayStates = new List<ObjectState>();

	private void Awake()
	{
		localState = GetState();
		if (localState == NPCEncounterState.HasLeft)
		{
			Leave();
		}
		else if (localState == NPCEncounterState.AuthorisedToLeave && leaveProbability >= UnityEngine.Random.Range(0f, 1f))
		{
			SetState(NPCEncounterState.HasLeft);
			Leave();
		}
		else
		{
			Here();
		}
	}

	private NPCEncounterState GetState()
	{
		return PlayerData.instance.GetVariable<NPCEncounterState>(encounterStateName);
	}

	public NPCEncounterState GetCurrentState()
	{
		localState = GetState();
		return localState;
	}

	public void UpdateState()
	{
		localState = GetState();
	}

	private void SetPlayerDataValue(NPCEncounterState state)
	{
		localState = state;
		PlayerData.instance.SetVariable(encounterStateName, state);
	}

	public void SetState(NPCEncounterState newState)
	{
		if (localState < newState)
		{
			if (newState == NPCEncounterState.HasLeft && requireLeaveAuthorisation && localState != NPCEncounterState.AuthorisedToLeave)
			{
				SetPlayerDataValue(NPCEncounterState.ReadyToLeave);
			}
			else
			{
				SetPlayerDataValue(newState);
			}
		}
	}

	public void SetMet()
	{
		SetState(NPCEncounterState.Met);
	}

	public void SetReadyToLeave()
	{
		SetState(NPCEncounterState.ReadyToLeave);
	}

	public void SetAuthorisedToLeave()
	{
		SetState(NPCEncounterState.AuthorisedToLeave);
	}

	public void SetHasLeft()
	{
		SetState(NPCEncounterState.HasLeft);
	}

	private void Here()
	{
		foreach (ObjectState presentState in presentStates)
		{
			presentState.Apply();
		}
	}

	private void Leave()
	{
		foreach (ObjectState awayState in awayStates)
		{
			awayState.Apply();
		}
		if (disableSelf)
		{
			base.gameObject.SetActive(value: false);
		}
	}
}

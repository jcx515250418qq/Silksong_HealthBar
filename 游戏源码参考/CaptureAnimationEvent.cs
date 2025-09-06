using System;
using UnityEngine;
using UnityEngine.Events;

public class CaptureAnimationEvent : MonoBehaviour, IMutable
{
	[SerializeField]
	private UnityEvent[] indexedEvents;

	private PlayerData playerData;

	private HeroController hc;

	private bool muted;

	public bool Muted => muted;

	public event Action EventFired;

	public event Action EventFiredTemp;

	private void Start()
	{
		hc = HeroController.instance;
		playerData = hc.playerData;
	}

	public void FireEvent()
	{
		if (!muted)
		{
			if (this.EventFired != null)
			{
				this.EventFired();
			}
			if (this.EventFiredTemp != null)
			{
				this.EventFiredTemp();
				ClearTempEvent();
			}
		}
	}

	public void ClearTempEvent()
	{
		this.EventFiredTemp = null;
	}

	public void FireIndexedEvent(int index)
	{
		if (!muted && indexedEvents != null && indexedEvents.Length > index)
		{
			indexedEvents[index].Invoke();
		}
	}

	public void SetMute(bool muted)
	{
		this.muted = muted;
	}

	public void SetPlayerDataBoolTrue(string boolName)
	{
		playerData.SetBool(boolName, value: true);
	}

	public void SetPlayerDataBoolFalse(string boolName)
	{
		playerData.SetBool(boolName, value: false);
	}

	public void IncrementPlayerDataInt(string intName)
	{
		playerData.IncrementInt(intName);
	}

	public void DecrementPlayerDataInt(string intName)
	{
		playerData.DecrementInt(intName);
	}

	public bool GetPlayerDataBool(string boolName)
	{
		return playerData.GetBool(boolName);
	}

	public int GetPlayerDataInt(string intName)
	{
		return playerData.GetInt(intName);
	}

	public float GetPlayerDataFloat(string floatName)
	{
		return playerData.GetFloat(floatName);
	}

	public string GetPlayerDataString(string stringName)
	{
		return playerData.GetString(stringName);
	}

	public void EquipCharm(int charmNum)
	{
		playerData.EquipCharm(charmNum);
	}

	public void UnequipCharm(int charmNum)
	{
		playerData.UnequipCharm(charmNum);
	}

	public void UpdateBlueHealth()
	{
		hc.UpdateBlueHealth();
	}

	public void SendEventRegister(string eventName)
	{
		EventRegister.SendEvent(eventName);
	}
}

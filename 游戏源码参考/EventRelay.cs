using System;
using UnityEngine;

public sealed class EventRelay : MonoBehaviour
{
	public event Action<string> OnSendEvent;

	public event Action<string> TemporaryEvent;

	private void OnDisable()
	{
		this.TemporaryEvent = null;
	}

	public void SendEvent(string eventName)
	{
		this.OnSendEvent?.Invoke(eventName);
		this.TemporaryEvent?.Invoke(eventName);
	}
}

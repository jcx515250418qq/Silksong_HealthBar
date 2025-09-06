using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public sealed class EventRelayResponder : MonoBehaviour
{
	[Serializable]
	public class EventResponse
	{
		public string eventName;

		public UnityEvent response;
	}

	[SerializeField]
	private List<EventResponse> responses = new List<EventResponse>();

	[SerializeField]
	private bool sendToChildren;

	private List<EventRelayResponder> childRepsonders = new List<EventRelayResponder>();

	private Dictionary<string, UnityEvent> eventResponses = new Dictionary<string, UnityEvent>();

	private void Awake()
	{
		responses.RemoveAll((EventResponse o) => o.response == null);
		foreach (EventResponse response in responses)
		{
			eventResponses.Add(response.eventName, response.response);
		}
	}

	public void ReceiveEvent(string eventName)
	{
		if (eventResponses.TryGetValue(eventName, out var value))
		{
			value?.Invoke();
		}
	}
}

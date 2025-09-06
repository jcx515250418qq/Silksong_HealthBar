using System;
using System.Collections;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class EventResponder : MonoBehaviour
{
	[Serializable]
	private struct PlaymakerEventTarget
	{
		public PlayMakerFSM EventTarget;

		[ModifiableProperty]
		[InspectorValidation("IsEventValid")]
		public string EventName;

		private bool? IsEventValid(string eventName)
		{
			return EventTarget.IsEventValid(eventName, isRequired: true);
		}
	}

	[Header("Receive")]
	[SerializeField]
	[FormerlySerializedAs("eventRegister")]
	private EventBase eventReceiver;

	[SerializeField]
	private bool requireActive;

	[Header("Response")]
	[SerializeField]
	private MinMaxFloat delay;

	[SerializeField]
	private string sendEventRegister;

	[Space]
	public UnityEvent OnRespond;

	[Space]
	[SerializeField]
	private PlaymakerEventTarget[] playmakerEventTargets;

	private Coroutine delayedRoutine;

	public EventBase Event
	{
		get
		{
			return eventReceiver;
		}
		set
		{
			if ((bool)eventReceiver)
			{
				eventReceiver.ReceivedEvent -= Respond;
			}
			eventReceiver = value;
			if ((bool)eventReceiver)
			{
				eventReceiver.ReceivedEvent += Respond;
			}
		}
	}

	private void Awake()
	{
		Event = eventReceiver;
	}

	public void Respond()
	{
		if (!requireActive || base.isActiveAndEnabled)
		{
			CancelRespond();
			float randomValue = delay.GetRandomValue();
			if (randomValue > 0f)
			{
				delayedRoutine = StartCoroutine(RespondDelayed(randomValue));
			}
			else
			{
				DoResponses();
			}
		}
	}

	public void CancelRespond()
	{
		if (delayedRoutine != null)
		{
			StopCoroutine(delayedRoutine);
			delayedRoutine = null;
		}
	}

	private IEnumerator RespondDelayed(float delayTime)
	{
		yield return new WaitForSeconds(delayTime);
		DoResponses();
	}

	private void DoResponses()
	{
		OnRespond.Invoke();
		if (!string.IsNullOrEmpty(sendEventRegister))
		{
			EventRegister.SendEvent(sendEventRegister);
		}
		PlaymakerEventTarget[] array = playmakerEventTargets;
		for (int i = 0; i < array.Length; i++)
		{
			PlaymakerEventTarget playmakerEventTarget = array[i];
			if (!playmakerEventTarget.EventTarget || string.IsNullOrEmpty(playmakerEventTarget.EventName))
			{
				break;
			}
			playmakerEventTarget.EventTarget.SendEvent(playmakerEventTarget.EventName);
		}
	}
}

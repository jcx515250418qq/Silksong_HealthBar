using System;
using System.Collections;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class TimedEventCaller : EventBase
{
	[SerializeField]
	private bool runOnEnable = true;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("runOnEnable", false, false, false)]
	private TriggerEnterEvent runOnTrigger;

	[SerializeField]
	private bool repeat = true;

	[HideInInspector]
	[SerializeField]
	[FormerlySerializedAs("repeatDelay")]
	[Obsolete]
	private float delay = 1f;

	[SerializeField]
	private MinMaxFloat randomDelay;

	[Space]
	public UnityEvent OnCall;

	private Coroutine callRoutine;

	public override string InspectorInfo => $"{randomDelay.Start}-{randomDelay.End} seconds";

	private void OnValidate()
	{
		if (delay > 0f)
		{
			randomDelay = new MinMaxFloat(delay, delay);
			delay = 0f;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		OnValidate();
		if (!runOnEnable && (bool)runOnTrigger)
		{
			runOnTrigger.OnTriggerEntered += delegate
			{
				StartCallRoutine();
			};
			runOnTrigger.OnTriggerExited += delegate
			{
				Stop();
			};
		}
	}

	private void OnEnable()
	{
		if (runOnEnable)
		{
			Stop();
			StartCallRoutine();
		}
	}

	public void StartCallRoutine()
	{
		if (callRoutine == null)
		{
			callRoutine = StartCoroutine(CallRoutine());
		}
	}

	public void Stop()
	{
		if (callRoutine != null)
		{
			StopCoroutine(callRoutine);
			callRoutine = null;
		}
	}

	private IEnumerator CallRoutine()
	{
		WaitForSeconds wait = null;
		float oldDelay = 0f;
		do
		{
			float randomValue = randomDelay.GetRandomValue();
			if (wait == null || Math.Abs(randomValue - oldDelay) > Mathf.Epsilon)
			{
				wait = ((randomValue > 0f) ? new WaitForSeconds(randomValue) : null);
				oldDelay = randomValue;
			}
			if (wait != null)
			{
				yield return wait;
			}
			if (OnCall != null)
			{
				OnCall.Invoke();
			}
			CallReceivedEvent();
		}
		while (repeat);
		callRoutine = null;
	}
}

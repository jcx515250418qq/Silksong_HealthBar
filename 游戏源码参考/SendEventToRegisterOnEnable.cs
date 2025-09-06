using System;
using System.Collections;
using UnityEngine;

public class SendEventToRegisterOnEnable : MonoBehaviour
{
	private enum DelayTypes
	{
		None = 0,
		Frame = 1,
		HalfSecond = 2
	}

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private string sendEvent;

	[SerializeField]
	private DelayTypes delayType;

	private void OnEnable()
	{
		if (!string.IsNullOrEmpty(sendEvent))
		{
			switch (delayType)
			{
			case DelayTypes.None:
				EventRegister.SendEvent(sendEvent);
				break;
			case DelayTypes.Frame:
				StartCoroutine(SendEventDelayedFrame());
				break;
			case DelayTypes.HalfSecond:
				StartCoroutine(SendEventDelayedTime(0.5f));
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	private IEnumerator SendEventDelayedFrame()
	{
		yield return null;
		EventRegister.SendEvent(sendEvent);
	}

	private IEnumerator SendEventDelayedTime(float delay)
	{
		yield return new WaitForSeconds(delay);
		EventRegister.SendEvent(sendEvent);
	}
}

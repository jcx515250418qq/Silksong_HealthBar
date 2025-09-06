using System;
using UnityEngine;
using UnityEngine.Events;

public class SetVector3PerSwitchMode : SwitchPlatformModeUpdateHandler
{
	[Serializable]
	private class Vector3UnityEvent : UnityEvent<Vector3>
	{
	}

	[SerializeField]
	private Vector3 isHandheldValue;

	[SerializeField]
	private Vector3UnityEvent isHandheldEvent;

	[Space]
	[SerializeField]
	private Vector3 notHandheldValue;

	[SerializeField]
	private Vector3UnityEvent notHandheldEvent;

	protected override void OnOperationModeChanged(bool isHandheld)
	{
		if (isHandheld)
		{
			isHandheldEvent.Invoke(isHandheldValue);
		}
		else
		{
			notHandheldEvent.Invoke(notHandheldValue);
		}
	}
}

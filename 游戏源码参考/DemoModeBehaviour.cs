using System;
using UnityEngine;
using UnityEngine.Events;

public class DemoModeBehaviour : MonoBehaviour
{
	[SerializeField]
	private EventBase waitForEvent;

	[SerializeField]
	[Obsolete]
	[HideInInspector]
	private bool exhibitionMode;

	public UnityEvent OnIsExhibitionMode;

	public UnityEvent OnIsDemoMode;

	public UnityEvent OnIsNotDemoMode;

	private void OnValidate()
	{
		if (exhibitionMode)
		{
			OnIsExhibitionMode = OnIsDemoMode.Clone();
			OnIsDemoMode = OnIsNotDemoMode.Clone();
			exhibitionMode = false;
		}
	}

	private void Awake()
	{
		OnValidate();
	}

	private void OnEnable()
	{
		if (!waitForEvent)
		{
			DoBehaviour();
		}
		else
		{
			waitForEvent.ReceivedEvent += DoBehaviour;
		}
	}

	private void OnDisable()
	{
		if ((bool)waitForEvent)
		{
			waitForEvent.ReceivedEvent -= DoBehaviour;
		}
	}

	private void DoBehaviour()
	{
		if ((bool)waitForEvent)
		{
			waitForEvent.ReceivedEvent -= DoBehaviour;
		}
		if (DemoHelper.IsExhibitionMode)
		{
			OnIsExhibitionMode.Invoke();
		}
		else if (DemoHelper.IsDemoMode)
		{
			OnIsDemoMode.Invoke();
		}
		else
		{
			OnIsNotDemoMode.Invoke();
		}
	}
}

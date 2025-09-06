using UnityEngine;
using UnityEngine.Events;

public sealed class HeroInPositionDelayedEvent : MonoBehaviour
{
	public UnityEvent onHeroInPosition;

	private HeroController heroController;

	private bool registeredEvent;

	private void Start()
	{
		heroController = HeroController.instance;
		if ((bool)heroController)
		{
			if (heroController.isHeroInPosition)
			{
				TriggerEvents();
			}
			else
			{
				RegisterEvents();
			}
		}
	}

	private void OnDisable()
	{
		UnregisterEvents();
	}

	private void RegisterEvents()
	{
		if (!registeredEvent)
		{
			registeredEvent = true;
			heroController.heroInPositionDelayed += OnHeroSetPosition;
		}
	}

	private void UnregisterEvents()
	{
		if (registeredEvent)
		{
			registeredEvent = false;
			heroController.heroInPositionDelayed -= OnHeroSetPosition;
		}
	}

	private void OnHeroSetPosition(bool forceDirect)
	{
		TriggerEvents();
		UnregisterEvents();
	}

	private void TriggerEvents()
	{
		onHeroInPosition?.Invoke();
	}
}

using UnityEngine;

public abstract class SwitchPlatformModeUpdateHandler : MonoBehaviour
{
	[SerializeField]
	protected Platform.HandHeldTypes handHeldTarget;

	private bool registeredEvents;

	protected virtual void OnEnable()
	{
		RegisterEvents();
		OnScreenModeUpdated(Platform.Current.ScreenMode);
	}

	protected virtual void OnDisable()
	{
		UnregisterEvents();
	}

	private void RegisterEvents()
	{
		if (!registeredEvents)
		{
			registeredEvents = true;
			Platform.Current.OnScreenModeChanged += OnScreenModeUpdated;
		}
	}

	private void UnregisterEvents()
	{
		if (registeredEvents)
		{
			registeredEvents = false;
			Platform.Current.OnScreenModeChanged -= OnScreenModeUpdated;
		}
	}

	private void OnScreenModeUpdated(Platform.ScreenModeState screenMode)
	{
		bool flag = (screenMode & (Platform.ScreenModeState.HandHeld | Platform.ScreenModeState.HandHeldSmall)) != 0;
		if (flag && !Platform.Current.IsTargetHandHeld(handHeldTarget))
		{
			flag = false;
		}
		OnOperationModeChanged(flag);
	}

	protected abstract void OnOperationModeChanged(bool isHandheld);
}

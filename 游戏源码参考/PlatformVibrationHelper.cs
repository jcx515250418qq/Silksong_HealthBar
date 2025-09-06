using InControl;
using UnityEngine;

public class PlatformVibrationHelper
{
	private readonly GamepadVibrationMixer vibrationMixer;

	private InputDevice lastVibratingInputDevice;

	private bool lastVibrationWasEmpty;

	public PlatformVibrationHelper()
	{
		vibrationMixer = new GamepadVibrationMixer();
	}

	public void Destroy()
	{
		if (lastVibratingInputDevice != null)
		{
			lastVibratingInputDevice.StopVibration();
			lastVibratingInputDevice = null;
		}
	}

	public void UpdateVibration()
	{
		vibrationMixer.Update(Time.unscaledDeltaTime);
		GamepadVibrationMixer.GamepadVibrationEmission.Values currentValues = vibrationMixer.CurrentValues;
		InputDevice activeDevice = InputManager.ActiveDevice;
		if (lastVibratingInputDevice != activeDevice)
		{
			if (lastVibratingInputDevice != null)
			{
				lastVibratingInputDevice.StopVibration();
				lastVibratingInputDevice = null;
			}
			lastVibratingInputDevice = activeDevice;
			if (lastVibratingInputDevice != null)
			{
				lastVibratingInputDevice.StopVibration();
			}
			lastVibrationWasEmpty = false;
		}
		if (lastVibratingInputDevice != null)
		{
			if (!lastVibrationWasEmpty || !currentValues.IsNearlyZero)
			{
				lastVibratingInputDevice.Vibrate(currentValues.Small, currentValues.Large);
			}
			lastVibrationWasEmpty = currentValues.IsNearlyZero;
		}
	}

	public VibrationMixer GetMixer()
	{
		return vibrationMixer;
	}
}

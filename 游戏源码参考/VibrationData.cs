using System;
using TeamCherry.PS5;
using TeamCherry.SharedUtils;
using UnityEngine;

[Serializable]
public struct VibrationData
{
	[SerializeField]
	private LowFidelityVibrations lowFidelityVibration;

	[SerializeField]
	private TextAsset highFidelityVibration;

	[SerializeField]
	private GamepadVibration gamepadVibration;

	[SerializeField]
	private PS5VibrationData ps5Vibration;

	[SerializeField]
	private OverrideFloat strength;

	public LowFidelityVibrations LowFidelityVibration => lowFidelityVibration;

	public TextAsset HighFidelityVibration => highFidelityVibration;

	public GamepadVibration GamepadVibration => gamepadVibration;

	public float Strength
	{
		get
		{
			OverrideFloat overrideFloat = strength;
			if (overrideFloat == null || !overrideFloat.IsEnabled)
			{
				return 1f;
			}
			return strength.Value;
		}
	}

	public AudioClip PS5Vibration => GetPS5Vibration();

	public PS5VibrationData PS5VibrationAsset => ps5Vibration;

	public static VibrationData Create(LowFidelityVibrations lowFidelityVibration = LowFidelityVibrations.None, TextAsset highFidelityVibration = null, GamepadVibration gamepadVibration = null, PS5VibrationData ps5Vibration = null)
	{
		VibrationData result = default(VibrationData);
		result.lowFidelityVibration = lowFidelityVibration;
		result.highFidelityVibration = highFidelityVibration;
		result.gamepadVibration = gamepadVibration;
		result.ps5Vibration = ps5Vibration;
		return result;
	}

	public AudioClip GetPS5Vibration()
	{
		if ((bool)ps5Vibration)
		{
			return ps5Vibration;
		}
		return null;
	}

	public void SetVibrationData(PS5VibrationData ps5VibrationData)
	{
		ps5Vibration = ps5VibrationData;
	}
}

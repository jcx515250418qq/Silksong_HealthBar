using System;
using System.Collections.Generic;
using InControl;
using UnityEngine;

public static class VibrationManager
{
	public enum VibrationSettings
	{
		On = 0,
		Reduced = 1,
		Off = 2
	}

	public interface IVibrationMixerProvider
	{
		VibrationMixer GetVibrationMixer();
	}

	private static VibrationSettings _vibrationSetting;

	private static float internalStrength = 1f;

	private static float targetStrength = 0f;

	private static float transitionRate;

	private static bool initialised;

	private static VibrationManagerUpdater vibrationManagerUpdater;

	public static VibrationSettings VibrationSetting
	{
		get
		{
			return _vibrationSetting;
		}
		set
		{
			if (_vibrationSetting != value)
			{
				_vibrationSetting = value;
				if (_vibrationSetting == VibrationSettings.Off)
				{
					StopAllVibration();
				}
			}
		}
	}

	public static float StrengthMultiplier => _vibrationSetting switch
	{
		VibrationSettings.On => 1f * internalStrength, 
		VibrationSettings.Reduced => ConfigManager.ReducedControllerRumble * internalStrength, 
		VibrationSettings.Off => 0f, 
		_ => throw new ArgumentOutOfRangeException(), 
	};

	public static float InternalStrength => internalStrength;

	private static void Init()
	{
		if (!initialised)
		{
			vibrationManagerUpdater = new GameObject("Vibration Manager Updater").AddComponent<VibrationManagerUpdater>();
			initialised = true;
		}
	}

	public static void FadeVibration(float strength, float duration)
	{
		if (duration <= 0f)
		{
			internalStrength = strength;
			return;
		}
		Init();
		targetStrength = strength;
		transitionRate = (targetStrength - internalStrength) / duration;
		vibrationManagerUpdater.enabled = true;
	}

	public static bool Update()
	{
		if (transitionRate == 0f)
		{
			return false;
		}
		internalStrength += transitionRate * Time.unscaledDeltaTime;
		if (transitionRate > 0f)
		{
			if (internalStrength >= targetStrength)
			{
				internalStrength = targetStrength;
				transitionRate = 0f;
			}
		}
		else if (internalStrength <= targetStrength)
		{
			internalStrength = targetStrength;
			transitionRate = 0f;
		}
		return transitionRate != 0f;
	}

	public static VibrationMixer GetMixer()
	{
		Platform current = Platform.Current;
		if (current != null && current is IVibrationMixerProvider vibrationMixerProvider)
		{
			VibrationMixer vibrationMixer = vibrationMixerProvider.GetVibrationMixer();
			if (vibrationMixer != null)
			{
				return vibrationMixer;
			}
		}
		InputDevice activeDevice = InputManager.ActiveDevice;
		if (activeDevice != null && activeDevice.IsAttached && activeDevice is IVibrationMixerProvider vibrationMixerProvider2)
		{
			VibrationMixer vibrationMixer2 = vibrationMixerProvider2.GetVibrationMixer();
			if (vibrationMixer2 != null)
			{
				return vibrationMixer2;
			}
		}
		return null;
	}

	public static VibrationEmission PlayVibrationClipOneShot(VibrationData vibrationData, VibrationTarget? vibrationTarget = null, bool isLooping = false, string tag = "", bool isRealtime = false)
	{
		if (_vibrationSetting == VibrationSettings.Off)
		{
			return null;
		}
		VibrationMixer mixer = GetMixer();
		if (mixer == null)
		{
			return null;
		}
		VibrationEmission vibrationEmission = mixer.PlayEmission(vibrationData, vibrationTarget ?? new VibrationTarget(VibrationMotors.All), isLooping, tag, isRealtime);
		if (vibrationEmission != null)
		{
			vibrationEmission.BaseStrength = vibrationData.Strength;
		}
		return vibrationEmission;
	}

	public static VibrationEmission PlayVibrationClipOneShot(VibrationEmission emission)
	{
		if (_vibrationSetting == VibrationSettings.Off)
		{
			return null;
		}
		if (emission == null)
		{
			return null;
		}
		emission.SetPlaybackTime(0f);
		return GetMixer()?.PlayEmission(emission);
	}

	public static void StopAllVibration()
	{
		if (_vibrationSetting != VibrationSettings.Off)
		{
			GetMixer()?.StopAllEmissions();
		}
	}

	public static void StopAllVibrationsWithTag(string tag)
	{
		if (_vibrationSetting != VibrationSettings.Off)
		{
			GetMixer()?.StopAllEmissionsWithTag(tag);
		}
	}

	public static void GetVibrationsWithTag(string tag, List<VibrationEmission> emissions)
	{
		if (_vibrationSetting == VibrationSettings.Off || emissions == null)
		{
			return;
		}
		VibrationMixer mixer = GetMixer();
		if (mixer == null)
		{
			return;
		}
		bool flag = string.IsNullOrEmpty(tag);
		if (!flag)
		{
			tag = tag.Trim();
		}
		for (int i = 0; i < mixer.PlayingEmissionCount; i++)
		{
			VibrationEmission playingEmission = mixer.GetPlayingEmission(i);
			if (flag || !(playingEmission.Tag != tag))
			{
				emissions.Add(playingEmission);
			}
		}
	}
}

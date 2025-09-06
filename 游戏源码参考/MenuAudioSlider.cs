using System;
using System.Globalization;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MenuAudioSlider : MonoBehaviour
{
	private enum AudioSettingType
	{
		MasterVolume = 0,
		MusicVolume = 1,
		SoundVolume = 2
	}

	[SerializeField]
	[Tooltip("The slider being controlled.")]
	private Slider slider;

	[SerializeField]
	[Tooltip("The Text UI object that displays the value of the slider.")]
	private Text textUI;

	[SerializeField]
	[Tooltip("The master audio mixer containing the variables we want to set.")]
	private AudioMixer masterMixer;

	[SerializeField]
	private AudioMixer uiMixer;

	[SerializeField]
	private AudioMixer cinematicMixer;

	[SerializeField]
	[Tooltip("The setting to load when this control is loaded.")]
	private AudioSettingType audioSetting;

	private GameSettings gs;

	private FixVerticalAlign fixVerticalAlign;

	private bool hasVerticalAlign;

	private void Start()
	{
		gs = GameManager.instance.gameSettings;
		if ((bool)textUI)
		{
			fixVerticalAlign = textUI.GetComponent<FixVerticalAlign>();
			hasVerticalAlign = fixVerticalAlign;
		}
		UpdateValue();
		base.gameObject.AddComponentIfNotPresent<SliderRightStickInput>();
	}

	public void UpdateValue()
	{
		textUI.text = slider.value.ToString(CultureInfo.InvariantCulture);
		if (hasVerticalAlign)
		{
			fixVerticalAlign.AlignAuto();
		}
	}

	public void RefreshValueFromSettings()
	{
		if (gs == null)
		{
			gs = GameManager.instance.gameSettings;
		}
		switch (audioSetting)
		{
		case AudioSettingType.MasterVolume:
			slider.value = gs.masterVolume;
			UpdateValue();
			break;
		case AudioSettingType.MusicVolume:
			slider.value = gs.musicVolume;
			UpdateValue();
			break;
		case AudioSettingType.SoundVolume:
			slider.value = gs.soundVolume;
			UpdateValue();
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	public void UpdateTextValue(float newValue)
	{
		textUI.text = newValue.ToString(CultureInfo.InvariantCulture);
	}

	private float GetVolumeLevel(float sourceLevel)
	{
		return new MinMaxFloat(slider.minValue, slider.maxValue).GetTBetween(sourceLevel);
	}

	public void SetMasterLevel(float masterLevel)
	{
		float value = Helper.LinearToDecibel(GetVolumeLevel(masterLevel));
		masterMixer.SetFloat("MasterVolume", value);
		gs.masterVolume = masterLevel;
	}

	public void SetMusicLevel(float musicLevel)
	{
		float value = Helper.LinearToDecibel(GetVolumeLevel(musicLevel));
		masterMixer.SetFloat("MusicVolume", value);
		gs.musicVolume = musicLevel;
	}

	public void SetSoundLevel(float soundLevel)
	{
		float value = Helper.LinearToDecibel(GetVolumeLevel(soundLevel));
		masterMixer.SetFloat("SFXVolume", value);
		uiMixer.SetFloat("UIVolume", value);
		cinematicMixer.SetFloat("CinematicVolume", value);
		gs.soundVolume = soundLevel;
	}
}

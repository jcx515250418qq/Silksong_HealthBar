using UnityEngine;
using UnityEngine.UI;

public class MenuSetting : MonoBehaviour
{
	public enum MenuSettingType
	{
		Resolution = 10,
		FullScreen = 11,
		VSync = 12,
		MonitorSelect = 14,
		SwitchFrameCap = 15,
		ParticleLevel = 16,
		ShaderQuality = 17,
		GameLanguage = 33,
		GameBackerCredits = 34,
		NativeAchievements = 35,
		ControllerRumble = 37,
		PlayerVoice = 38,
		HudVisibility = 39,
		CameraShake = 40
	}

	public MenuSettingType settingType;

	public MenuOptionHorizontal optionList;

	private VibrationPlayer vibration;

	private GameManager gm;

	protected GameSettings gs;

	private bool verboseMode;

	private void Awake()
	{
		vibration = GetComponent<VibrationPlayer>();
	}

	private void Start()
	{
		gm = GameManager.instance;
		gs = gm.gameSettings;
	}

	public void RefreshValueFromGameSettings(bool alsoApplySetting = false)
	{
		if (gs == null)
		{
			gs = GameManager.instance.gameSettings;
		}
		switch (settingType)
		{
		case MenuSettingType.FullScreen:
			optionList.SetOptionTo(gs.fullScreen);
			break;
		case MenuSettingType.VSync:
			optionList.SetOptionTo(gs.vSync);
			break;
		case MenuSettingType.ParticleLevel:
			optionList.SetOptionTo(gs.particleEffectsLevel);
			break;
		case MenuSettingType.ShaderQuality:
			optionList.SetOptionTo((int)gs.shaderQuality);
			break;
		case MenuSettingType.GameBackerCredits:
			optionList.SetOptionTo(gs.backerCredits);
			break;
		case MenuSettingType.NativeAchievements:
			optionList.SetOptionTo(gs.showNativeAchievementPopups);
			break;
		case MenuSettingType.HudVisibility:
			optionList.SetOptionTo(gs.hudScaleSetting);
			break;
		case MenuSettingType.ControllerRumble:
			optionList.SetOptionTo(gs.vibrationSetting);
			break;
		case MenuSettingType.CameraShake:
			optionList.SetOptionTo(gs.cameraShakeSetting);
			break;
		case MenuSettingType.PlayerVoice:
			optionList.SetOptionTo(gs.playerVoiceEnabled ? 1 : 0);
			break;
		}
		if (alsoApplySetting)
		{
			UpdateSetting(optionList.selectedOptionIndex);
		}
	}

	public void ChangeSetting(int settingIndex)
	{
		UpdateSetting(settingIndex);
		if (settingType == MenuSettingType.ControllerRumble && (bool)vibration)
		{
			vibration.Play();
		}
	}

	public void UpdateSetting(int settingIndex)
	{
		if (gs == null)
		{
			gs = GameManager.instance.gameSettings;
		}
		switch (settingType)
		{
		case MenuSettingType.GameBackerCredits:
			gs.backerCredits = ((settingIndex != 0) ? 1 : 0);
			break;
		case MenuSettingType.NativeAchievements:
			gs.showNativeAchievementPopups = ((settingIndex != 0) ? 1 : 0);
			break;
		case MenuSettingType.HudVisibility:
		{
			HudGlobalHide.IsHidden = settingIndex == 2;
			HudGlobalHide.IsReduced = (HudScalePositioner.IsReduced = settingIndex == 1);
			int val = settingIndex;
			if (val == 2)
			{
				GameSettings.LoadInt("HudScaleSetting", ref val, 0);
			}
			gs.hudScaleSetting = val;
			break;
		}
		case MenuSettingType.FullScreen:
			gs.fullScreen = settingIndex;
			switch (settingIndex)
			{
			case 0:
				Screen.fullScreenMode = FullScreenMode.Windowed;
				break;
			case 2:
				Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
				break;
			default:
				Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
				break;
			}
			break;
		case MenuSettingType.VSync:
			if (settingIndex == 0)
			{
				gs.vSync = 0;
				QualitySettings.vSyncCount = 0;
				break;
			}
			gs.vSync = 1;
			QualitySettings.vSyncCount = 1;
			Application.targetFrameRate = -1;
			UIManager.instance.DisableFrameCapSetting();
			break;
		case MenuSettingType.ParticleLevel:
			if (settingIndex == 0)
			{
				gs.particleEffectsLevel = 0;
				gm.RefreshParticleSystems();
			}
			else
			{
				gs.particleEffectsLevel = 1;
				gm.RefreshParticleSystems();
			}
			break;
		case MenuSettingType.ShaderQuality:
			gs.shaderQuality = ((settingIndex != 0) ? ShaderQualities.High : ShaderQualities.Low);
			break;
		case MenuSettingType.ControllerRumble:
			VibrationManager.VibrationSetting = (VibrationManager.VibrationSettings)settingIndex;
			gs.vibrationSetting = settingIndex;
			break;
		case MenuSettingType.CameraShake:
			CameraShakeManager.ShakeSetting = (CameraShakeManager.ShakeSettings)settingIndex;
			gs.cameraShakeSetting = settingIndex;
			break;
		case MenuSettingType.PlayerVoice:
			gs.playerVoiceEnabled = settingIndex == 1;
			break;
		}
	}
}

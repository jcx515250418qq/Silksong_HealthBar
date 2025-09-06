using System.IO;
using UnityEngine;

public static class ConfigManager
{
	private static bool _isInit;

	private static readonly string _path = (IsConfigFileSupported ? Path.Combine(Application.persistentDataPath, "AppConfig.ini") : null);

	public static bool IsNativeInputEnabled { get; private set; }

	public static float ReducedCameraShake { get; private set; }

	public static float ReducedControllerRumble { get; private set; }

	public static bool IsNoiseEffectEnabled { get; private set; }

	private static bool IsConfigFileSupported
	{
		get
		{
			if (Application.platform != RuntimePlatform.WindowsPlayer && Application.platform != RuntimePlatform.OSXPlayer)
			{
				return Application.platform == RuntimePlatform.LinuxPlayer;
			}
			return true;
		}
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void Init()
	{
		_isInit = true;
		IsNativeInputEnabled = true;
		ReducedCameraShake = 0.2f;
		ReducedControllerRumble = 0.4f;
		IsNoiseEffectEnabled = false;
		if (IsConfigFileSupported)
		{
			if ((bool)Platform.Current)
			{
				LoadConfig();
			}
			else
			{
				Platform.PlatformBecameCurrent += LoadConfig;
			}
		}
	}

	private static void LoadConfig()
	{
		if (IsConfigFileSupported && _isInit)
		{
			if (File.Exists(_path))
			{
				INIParser iNIParser = new INIParser();
				iNIParser.Open(_path);
				IsNoiseEffectEnabled = iNIParser.ReadValue("VideoSettings", "NoiseEffect", IsNoiseEffectEnabled);
				IsNativeInputEnabled = iNIParser.ReadValue("Input", "NativeInput", IsNativeInputEnabled);
				ReducedCameraShake = iNIParser.ReadValue("Accessibility", "ReducedCameraShake", ReducedCameraShake);
				ReducedControllerRumble = iNIParser.ReadValue("Accessibility", "ReducedControllerRumble", ReducedControllerRumble);
				iNIParser.Close();
			}
			SaveConfig();
		}
	}

	public static void SaveConfig()
	{
		if (IsConfigFileSupported && _isInit)
		{
			INIParser iNIParser = new INIParser();
			iNIParser.Open(_path);
			iNIParser.WriteValue("VideoSettings", "NoiseEffect", IsNoiseEffectEnabled);
			iNIParser.WriteValue("Input", "NativeInput", IsNativeInputEnabled);
			iNIParser.WriteValue("Accessibility", "ReducedCameraShake", ReducedCameraShake);
			iNIParser.WriteValue("Accessibility", "ReducedControllerRumble", ReducedControllerRumble);
			iNIParser.Close();
		}
	}
}

using System;
using System.Collections;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

public class MenuStyles : MonoBehaviour
{
	[Serializable]
	public class MenuStyle
	{
		[Serializable]
		public class CameraCurves
		{
			[Range(0f, 5f)]
			public float Saturation = 1f;

			public AnimationCurve RedChannel = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

			public AnimationCurve GreenChannel = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

			public AnimationCurve BlueChannel = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));
		}

		public bool Enabled = true;

		public string DisplayName;

		public GameObject StyleObject;

		public NestedFadeGroupBase Foreground;

		public CameraCurves CameraColorCorrection;

		public Color AmbientColor;

		public float AmbientIntensity;

		public float BlurPlaneVibranceOffset;

		public string UnlockKey;

		[NonSerialized]
		public float[] InitialAudioVolumes;

		public bool IsAvailable
		{
			get
			{
				if (!Enabled)
				{
					return false;
				}
				_ = GameManager.instance;
				if (string.IsNullOrEmpty(UnlockKey))
				{
					return true;
				}
				if (DemoHelper.IsDemoMode)
				{
					return false;
				}
				return Platform.Current.RoamingSharedData.GetBool(UnlockKey, def: false);
			}
		}

		public void SetInitialAudioVolumes(AudioSource[] sources)
		{
			if (InitialAudioVolumes == null || InitialAudioVolumes.Length == 0)
			{
				InitialAudioVolumes = new float[sources.Length];
				for (int i = 0; i < InitialAudioVolumes.Length; i++)
				{
					InitialAudioVolumes[i] = sources[i].volume;
				}
			}
		}
	}

	[Serializable]
	public struct StyleSettings
	{
		public int StyleIndex;

		public string AutoChangeVersion;
	}

	[Serializable]
	public struct StyleSettingsPlatform
	{
		public RuntimePlatform Platform;

		public StyleSettings Settings;
	}

	private enum FadeType
	{
		Up = 0,
		Down = 1
	}

	public static MenuStyles Instance;

	public MenuStyle[] Styles;

	[Space]
	public StyleSettings StyleDefault;

	public StyleSettingsPlatform[] StylePlatforms;

	[Space]
	public SpriteRenderer BlackSolid;

	public float FadeTime = 0.25f;

	public float ForegroundFadeTime = 0.15f;

	private Coroutine fadeRoutine;

	private StyleSettings currentSettings;

	private bool isInSubMenu;

	private bool started;

	public int CurrentStyle
	{
		get
		{
			if (currentSettings.StyleIndex < 0 || currentSettings.StyleIndex >= Styles.Length)
			{
				currentSettings.StyleIndex = Mathf.Clamp(currentSettings.StyleIndex, 0, Styles.Length - 1);
			}
			return currentSettings.StyleIndex;
		}
	}

	private void Awake()
	{
		Instance = this;
		MenuStyle[] styles = Styles;
		foreach (MenuStyle menuStyle in styles)
		{
			if ((bool)menuStyle.StyleObject)
			{
				menuStyle.StyleObject.SetActive(value: false);
			}
		}
		Platform.OnSaveStoreStateChanged += OnSaveStoreStateChanged;
	}

	private void Start()
	{
		started = true;
		LoadRecentMenuStyle(fade: false);
	}

	private void OnDestroy()
	{
		Instance = null;
		Platform.OnSaveStoreStateChanged -= OnSaveStoreStateChanged;
	}

	private void OnSaveStoreStateChanged(bool mounted)
	{
		if (mounted)
		{
			LoadRecentMenuStyle(started);
		}
	}

	private void LoadRecentMenuStyle(bool fade)
	{
		currentSettings = StyleDefault;
		if (Platform.Current.IsSaveStoreMounted)
		{
			StyleSettingsPlatform[] stylePlatforms = StylePlatforms;
			for (int i = 0; i < stylePlatforms.Length; i++)
			{
				StyleSettingsPlatform styleSettingsPlatform = stylePlatforms[i];
				if (Application.platform == styleSettingsPlatform.Platform)
				{
					currentSettings = styleSettingsPlatform.Settings;
					break;
				}
			}
			if (Platform.Current.LocalSharedData.HasKey("lastVersion") && Platform.Current.LocalSharedData.GetString("lastVersion", "0.0.0.0") == currentSettings.AutoChangeVersion)
			{
				int num = Mathf.Clamp(Platform.Current.LocalSharedData.GetInt("menuStyle", currentSettings.StyleIndex), 0, Styles.Length - 1);
				if (num >= 0 && num < Styles.Length && Styles[num].IsAvailable)
				{
					currentSettings.StyleIndex = num;
				}
			}
			string key = "unlockedMenuStyle";
			if (Platform.Current.RoamingSharedData.HasKey(key))
			{
				string @string = Platform.Current.RoamingSharedData.GetString(key, "");
				Platform.Current.RoamingSharedData.DeleteKey(key);
				Platform.Current.RoamingSharedData.Save();
				for (int j = 0; j < Styles.Length; j++)
				{
					if (Styles[j].UnlockKey == @string && Styles[j].IsAvailable)
					{
						currentSettings.StyleIndex = j;
						break;
					}
				}
			}
		}
		SetStyle(currentSettings.StyleIndex, fade);
	}

	public void SetStyle(int index, bool fade, bool save = true)
	{
		if (index >= 0 && index < Styles.Length)
		{
			if (fadeRoutine != null)
			{
				StopCoroutine(fadeRoutine);
			}
			fadeRoutine = StartCoroutine(SwitchStyle(index, fade, currentSettings.StyleIndex));
			currentSettings.StyleIndex = index;
			if (save)
			{
				Platform.Current.LocalSharedData.SetString("lastVersion", currentSettings.AutoChangeVersion);
				Platform.Current.LocalSharedData.SetInt("menuStyle", currentSettings.StyleIndex);
				Platform.Current.LocalSharedData.Save();
			}
		}
	}

	private IEnumerator SwitchStyle(int index, bool fade, int oldIndex)
	{
		yield return null;
		MenuStyle obj = Styles[oldIndex];
		MenuStyle newStyle = Styles[index];
		AudioSource[] componentsInChildren = obj.StyleObject.GetComponentsInChildren<AudioSource>();
		obj.SetInitialAudioVolumes(componentsInChildren);
		yield return StartCoroutine(Fade(oldIndex, FadeType.Down, fade, componentsInChildren));
		for (int i = 0; i < Styles.Length; i++)
		{
			MenuStyle menuStyle = Styles[i];
			bool flag = index == i;
			menuStyle.StyleObject.SetActive(flag);
			if (flag && (bool)menuStyle.Foreground)
			{
				menuStyle.Foreground.AlphaSelf = ((!isInSubMenu) ? 1 : 0);
			}
		}
		GameCameras instance = GameCameras.instance;
		if ((bool)instance && (bool)instance.colorCorrectionCurves)
		{
			MenuStyle.CameraCurves cameraColorCorrection = newStyle.CameraColorCorrection;
			instance.colorCorrectionCurves.saturation = cameraColorCorrection.Saturation;
			instance.colorCorrectionCurves.redChannel = cameraColorCorrection.RedChannel;
			instance.colorCorrectionCurves.greenChannel = cameraColorCorrection.GreenChannel;
			instance.colorCorrectionCurves.blueChannel = cameraColorCorrection.BlueChannel;
		}
		CustomSceneManager.SetLighting(newStyle.AmbientColor, newStyle.AmbientIntensity);
		BlurPlane.SetVibranceOffset(newStyle.BlurPlaneVibranceOffset);
		componentsInChildren = newStyle.StyleObject.GetComponentsInChildren<AudioSource>();
		newStyle.SetInitialAudioVolumes(componentsInChildren);
		yield return StartCoroutine(Fade(index, FadeType.Up, fade, componentsInChildren));
		fadeRoutine = null;
	}

	private IEnumerator Fade(int styleIndex, FadeType fadeType, bool fade, AudioSource[] audioSources)
	{
		float toAlpha = ((fadeType == FadeType.Down) ? 1 : 0);
		if (!BlackSolid)
		{
			yield break;
		}
		Color color = BlackSolid.color;
		float startAlpha = color.a;
		if (fade)
		{
			for (float elapsed = 0f; elapsed < FadeTime; elapsed += Time.deltaTime)
			{
				float t = elapsed / FadeTime;
				color.a = Mathf.Lerp(startAlpha, toAlpha, t);
				BlackSolid.color = color;
				for (int i = 0; i < audioSources.Length; i++)
				{
					float num = Styles[styleIndex].InitialAudioVolumes[i];
					audioSources[i].volume = ((fadeType == FadeType.Down) ? Mathf.Lerp(num, 0f, t) : Mathf.Lerp(0f, num, t));
				}
				yield return null;
			}
			for (int j = 0; j < audioSources.Length; j++)
			{
				float volume = Styles[styleIndex].InitialAudioVolumes[j];
				if (fadeType == FadeType.Down)
				{
					audioSources[j].volume = 0f;
				}
				else
				{
					audioSources[j].volume = volume;
				}
			}
		}
		color.a = toAlpha;
		BlackSolid.color = color;
	}

	public void StopAudio()
	{
		AudioSource[] componentsInChildren = Styles[CurrentStyle].StyleObject.GetComponentsInChildren<AudioSource>();
		StartCoroutine(FadeOutAudio(componentsInChildren));
	}

	private IEnumerator FadeOutAudio(AudioSource[] audioSources)
	{
		AudioSource[] array;
		for (float elapsed = 0f; elapsed <= FadeTime; elapsed += Time.deltaTime)
		{
			array = audioSources;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].volume = Mathf.Lerp(0f, 1f, elapsed / FadeTime);
			}
			yield return null;
		}
		array = audioSources;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].volume = 0f;
		}
	}

	public void SetInSubMenu(bool value)
	{
		isInSubMenu = value;
		MenuStyle menuStyle = Styles[CurrentStyle];
		if ((bool)menuStyle.Foreground)
		{
			menuStyle.Foreground.FadeTo((!value) ? 1 : 0, ForegroundFadeTime);
		}
	}
}

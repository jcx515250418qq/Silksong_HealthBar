using System;
using System.Collections.Generic;
using GlobalEnums;
using TeamCherry;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

[Serializable]
public class CustomSceneManager : MonoBehaviour
{
	public enum WorldRumbleSettings
	{
		MapZone = 0,
		Enabled = 1,
		Disabled = 2
	}

	public enum SceneBorderPositions
	{
		Top = 0,
		Bottom = 1,
		Left = 2,
		Right = 3
	}

	public enum BoolFriendly
	{
		Off = 0,
		On = 1
	}

	[Serializable]
	public class OverrideBoolFriendly : OverrideValue<BoolFriendly>
	{
	}

	private const float HERO_SATURATION_GLOBAL = 0.5f;

	private const float ACTOR_MIXER_FADE_UP_TIME = 0.25f;

	[Space]
	[Tooltip("This denotes the type of this scene, mainly if it is a gameplay scene or not.")]
	public SceneType sceneType;

	[Header("Gameplay Scene Settings")]
	[Tooltip("The area of the map this scene belongs to.")]
	[Space]
	public MapZone mapZone;

	[SerializeField]
	private bool forceNotMemory;

	public ExtraRestZones extraRestZone;

	[Tooltip("Determines if this area is currently windy.")]
	public bool isWindy;

	[SerializeField]
	[AssetPickerDropdown]
	private FrostSpeedProfile frostSpeed;

	[Tooltip("Determines if this level experiences tremors.")]
	public bool isTremorZone;

	[Tooltip("Set environment type on scene entry. 0 = Dust, 1 = Grass, 2 = Bone, 3 = Spa, 4 = Metal, 5 = No Effect, 6 = Wet")]
	public EnvironmentTypes environmentType;

	public int darknessLevel;

	public bool noLantern;

	[SerializeField]
	[Obsolete]
	[HideInInspector]
	private bool noTeleport;

	[SerializeField]
	private NoTeleportRegion.TeleportAllowState teleportAllowState;

	[SerializeField]
	private bool heroKeepHealth;

	[Header("Camera Color Correction Curves")]
	[SerializeField]
	private bool overrideColorSettings;

	[ModifiableProperty]
	[Conditional("IsOverridingMapZoneColorSettings", true, true, false)]
	public float saturation;

	[ModifiableProperty]
	[Conditional("IsOverridingMapZoneColorSettings", true, true, false)]
	public AnimationCurve redChannel;

	[ModifiableProperty]
	[Conditional("IsOverridingMapZoneColorSettings", true, true, false)]
	public AnimationCurve greenChannel;

	[ModifiableProperty]
	[Conditional("IsOverridingMapZoneColorSettings", true, true, false)]
	public AnimationCurve blueChannel;

	[Space]
	public bool ignorePlatformSaturationModifiers;

	public float heroSaturationOffset;

	[Header("Ambient Light")]
	[ModifiableProperty]
	[Conditional("IsOverridingMapZoneColorSettings", true, true, false)]
	public Color defaultColor;

	[ModifiableProperty]
	[Conditional("IsOverridingMapZoneColorSettings", true, true, false)]
	public float defaultIntensity;

	[Header("Hero Light")]
	[ModifiableProperty]
	[Conditional("IsOverridingMapZoneColorSettings", true, true, false)]
	public Color heroLightColor;

	[Header("Blur Plane")]
	[ModifiableProperty]
	[Conditional("IsOverridingMapZoneColorSettings", true, true, false)]
	public float blurPlaneVibranceOffset;

	[Header("Scene Particles")]
	public bool noParticles;

	public MapZone overrideParticlesWith;

	public CustomSceneParticles overrideParticlesWithCustom;

	public OverrideBoolFriendly act3ParticlesOverride;

	[Header("Audio Snapshots")]
	[SerializeField]
	private AtmosCue atmosCue;

	[Space]
	[SerializeField]
	private MusicCue musicCue;

	[SerializeField]
	private AudioMixerSnapshot musicSnapshot;

	[SerializeField]
	private float musicDelayTime;

	[SerializeField]
	private float musicTransitionTime;

	[SerializeField]
	private bool useAltsIfAlreadyPlaying;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("useAltsIfAlreadyPlaying", true, false, false)]
	private float altMusicDelayTime;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("useAltsIfAlreadyPlaying", true, false, false)]
	private float altMusicTransitionTime;

	[Space]
	public AudioMixerSnapshot atmosSnapshot;

	public AudioMixerSnapshot enviroSnapshot;

	public AudioMixerSnapshot actorSnapshot;

	public AudioMixerSnapshot shadeSnapshot;

	public float transitionTime;

	[Header("Scene Border")]
	public GameObject borderPrefab;

	[EnumPickerBitmask(typeof(SceneBorderPositions))]
	public int sceneBordersMask = -1;

	[Header("Mapping")]
	public bool manualMapTrigger;

	[Header("Object Spawns")]
	public GameObject heroCorpsePrefab;

	public SceneObjectPool[] scenePools;

	[Header("World Rumble")]
	public WorldRumbleSettings WorldRumble;

	private GameManager gm;

	private GameCameras gc;

	private HeroController heroCtrl;

	private PlayerData pd;

	private List<SceneAppearanceRegion> appearanceRegions = new List<SceneAppearanceRegion>();

	private Coroutine ambientLightFadeRoutine;

	private float ambientLightLastFadeTime;

	private bool didAffectAmbientLight;

	private static Color _currentAmbientLightColor;

	private static float _currentAmbientLightIntensity;

	private Coroutine saturationFadeRoutine;

	private float saturationLastFadeTime;

	private bool didAffectSaturation;

	private static readonly int _desaturationPropId = Shader.PropertyToID("_HeroDesaturation");

	private float enviroTimer;

	private bool enviroSent;

	private bool heroInfoSent;

	private bool setSaturation;

	private bool isGameplayScene;

	private bool addedWind;

	private bool cancelEnviroSnapshot;

	private bool isValid;

	public static float AmbientIntesityMix = 0.5f;

	private const float REGULAR_CONSTANT = 0.4f;

	public bool ForceNotMemory => forceNotMemory;

	public NoTeleportRegion.TeleportAllowState TeleportAllowState => teleportAllowState;

	public bool HeroKeepHealth => heroKeepHealth;

	public float FrostSpeed
	{
		get
		{
			if (!frostSpeed)
			{
				return 0f;
			}
			return frostSpeed.FrostSpeed;
		}
	}

	public float AngleToSilkThread { get; private set; }

	public bool IsAudioSnapshotsApplied { get; private set; }

	public static int Version { get; private set; }

	public bool IsGradeOverridden { get; set; }

	public event Action AudioSnapshotsApplied;

	private bool IsOverridingMapZoneColorSettings()
	{
		if (mapZone == MapZone.NONE)
		{
			return true;
		}
		return overrideColorSettings;
	}

	public static void IncrementVersion()
	{
		Version++;
	}

	private void Awake()
	{
		OnValidate();
		IncrementVersion();
		SceneObjectPool[] array = scenePools;
		foreach (SceneObjectPool sceneObjectPool in array)
		{
			if ((bool)sceneObjectPool)
			{
				sceneObjectPool.SpawnPool(base.gameObject);
			}
		}
	}

	private void Start()
	{
		gm = GameManager.instance;
		gc = GameCameras.instance;
		pd = PlayerData.instance;
		if ((bool)musicCue)
		{
			musicCue.Preload(base.gameObject);
		}
		heroCtrl = HeroController.SilentInstance;
		isGameplayScene = gm.IsGameplayScene();
		gm.SceneInit += OnSceneInit;
		OnSceneInit();
		if (!IsOverridingMapZoneColorSettings())
		{
			AsyncOperationHandle<References> handle = Addressables.LoadAssetAsync<References>("ReferencesData");
			References references = handle.WaitForCompletion();
			if ((bool)references && (bool)references.sceneDefaultSettings)
			{
				UpdateSceneSettings(references.sceneDefaultSettings.GetMapZoneSettingsRuntime(mapZone, pd.blackThreadWorld ? SceneManagerSettings.Conditions.BlackThread : SceneManagerSettings.Conditions.None));
			}
			Addressables.Release(handle);
		}
		UpdateScene();
		pd.environmentType = environmentType;
		Action afterCameraPositioned = null;
		isValid = true;
		afterCameraPositioned = delegate
		{
			if (!isValid)
			{
				isValid = false;
				GameCameras.instance.cameraController.PositionedAtHero -= afterCameraPositioned;
			}
			else
			{
				float num;
				float num2;
				if (useAltsIfAlreadyPlaying && musicCue != null && gm.AudioManager.CurrentMusicCue == musicCue)
				{
					num = altMusicDelayTime;
					num2 = altMusicTransitionTime;
				}
				else
				{
					num = musicDelayTime;
					num2 = musicTransitionTime;
				}
				if (gm.entryGateName == "door_tubeEnter")
				{
					num += 2.5f;
				}
				if (musicCue != null)
				{
					gm.AudioManager.ApplyMusicCue(musicCue, num, num2, applySnapshot: false);
				}
				if (musicSnapshot != null)
				{
					gm.AudioManager.ApplyMusicSnapshot(musicSnapshot, num, num2);
				}
				if (atmosCue != null)
				{
					gm.AudioManager.ClearAtmosOverrides();
					gm.AudioManager.ApplyAtmosCue(atmosCue, transitionTime, markWaitForAtmos: true);
				}
				if (atmosSnapshot != null)
				{
					AudioManager.TransitionToAtmosOverride(atmosSnapshot, transitionTime);
				}
				if (enviroSnapshot != null && !cancelEnviroSnapshot)
				{
					enviroSnapshot.TransitionTo(transitionTime);
				}
				if (actorSnapshot != null)
				{
					AudioManager.CustomSceneManagerSnapshotReady(delegate
					{
						if (actorSnapshot != null && (!(gm != null) || !gm.SkipNormalActorFadeIn()))
						{
							actorSnapshot.TransitionTo(0.25f);
						}
					});
				}
				else
				{
					AudioManager.CustomSceneManagerSnapshotReady();
				}
				if (shadeSnapshot != null)
				{
					shadeSnapshot.TransitionTo(transitionTime);
				}
				IsAudioSnapshotsApplied = true;
				this.AudioSnapshotsApplied?.Invoke();
				GameCameras.instance.cameraController.PositionedAtHero -= afterCameraPositioned;
				AudioManager.CustomSceneManagerReady();
			}
		};
		GameCameras.instance.cameraController.PositionedAtHero += afterCameraPositioned;
		AudioManager.SetIsWaitingForCustomSceneManager();
		if (!isGameplayScene)
		{
			afterCameraPositioned();
		}
		if (sceneType == SceneType.GAMEPLAY)
		{
			GameObject gameObject = GameObject.FindGameObjectWithTag("Vignette");
			if ((bool)gameObject)
			{
				PlayMakerFSM playMakerFSM = FSMUtility.LocateFSM(gameObject, "Darkness Control");
				if ((bool)playMakerFSM)
				{
					FSMUtility.SetInt(playMakerFSM, "Darkness Level", darknessLevel);
				}
				if (!noLantern)
				{
					if ((bool)playMakerFSM)
					{
						playMakerFSM.SendEvent("RESET");
					}
				}
				else if ((bool)playMakerFSM)
				{
					playMakerFSM.SendEvent("SCENE RESET NO LANTERN");
				}
			}
		}
		if (isWindy)
		{
			WindRegion.AddWind();
			addedWind = true;
		}
		if (isGameplayScene)
		{
			DrawBlackBorders();
		}
		if ((!MazeController.NewestInstance || MazeController.NewestInstance.IsCapScene) && !string.IsNullOrEmpty(pd.HeroCorpseScene) && isGameplayScene && pd.HeroCorpseScene == base.gameObject.scene.name)
		{
			Vector2 vector = pd.HeroDeathScenePos;
			HeroCorpseMarker byGuid = HeroCorpseMarker.GetByGuid(pd.HeroCorpseMarkerGuid);
			if ((bool)byGuid)
			{
				vector = byGuid.Position;
			}
			GameObject gameObject2 = UnityEngine.Object.Instantiate(heroCorpsePrefab, new Vector3(vector.x, vector.y, heroCorpsePrefab.transform.position.z), Quaternion.identity);
			if ((bool)byGuid)
			{
				RepositionFromWalls component = gameObject2.GetComponent<RepositionFromWalls>();
				if ((bool)component)
				{
					component.enabled = false;
					gameObject2.transform.position = vector;
				}
			}
			gameObject2.transform.SetParent(base.transform, worldPositionStays: true);
			gameObject2.transform.SetParent(null);
		}
		if (sceneType == SceneType.MENU)
		{
			Platform.Current.SetSceneLoadState(isInProgress: false);
		}
	}

	private void OnSceneInit()
	{
		if ((bool)gm.gameMap)
		{
			AngleToSilkThread = gm.gameMap.GetAngleBetweenScenes(gm.sceneName, pd.blackThreadWorld ? "Thread_Target_Down" : "Thread_Target_Up");
		}
		if (isGameplayScene)
		{
			AddSceneMapped();
		}
	}

	private void OnDestroy()
	{
		isValid = false;
		if (addedWind)
		{
			WindRegion.RemoveWind();
			addedWind = false;
		}
		if ((bool)gm)
		{
			gm.SceneInit -= OnSceneInit;
			gm = null;
		}
		IncrementVersion();
	}

	public void AddInsideAppearanceRegion(SceneAppearanceRegion region, bool forceImmediate)
	{
		appearanceRegions.AddIfNotPresent(region);
		UpdateAppearanceRegion(forceImmediate);
	}

	public void RemoveInsideAppearanceRegion(SceneAppearanceRegion region, bool forceImmediate)
	{
		appearanceRegions.Remove(region);
		UpdateAppearanceRegion(forceImmediate);
	}

	private void UpdateAppearanceRegion(bool forceImmediate)
	{
		Color ambientLightColor = defaultColor;
		float ambientLightIntensity = defaultIntensity;
		float newSaturation = saturation;
		bool flag = false;
		bool flag2 = false;
		float duration = ambientLightLastFadeTime;
		float duration2 = saturationLastFadeTime;
		foreach (SceneAppearanceRegion appearanceRegion in appearanceRegions)
		{
			if (appearanceRegion.AffectAmbientLight)
			{
				ambientLightColor = appearanceRegion.AmbientLightColor;
				ambientLightIntensity = appearanceRegion.AmbientLightIntensity;
				duration = appearanceRegion.FadeDuration;
				ambientLightLastFadeTime = appearanceRegion.ExitFadeDuration;
				flag = true;
			}
			if (appearanceRegion.AffectSaturation)
			{
				newSaturation = appearanceRegion.Saturation;
				duration2 = appearanceRegion.FadeDuration;
				saturationLastFadeTime = appearanceRegion.ExitFadeDuration;
				flag2 = true;
			}
		}
		if (forceImmediate)
		{
			duration = 0f;
			duration2 = 0f;
		}
		if (flag || didAffectAmbientLight)
		{
			if (ambientLightFadeRoutine != null)
			{
				StopCoroutine(ambientLightFadeRoutine);
			}
			Color startColor = _currentAmbientLightColor;
			float startIntensity = _currentAmbientLightIntensity;
			ambientLightFadeRoutine = this.StartTimerRoutine(0f, duration, delegate(float time)
			{
				gc.sceneColorManager.AmbientColorA = Color.Lerp(startColor, ambientLightColor, time);
				gc.sceneColorManager.AmbientIntensityA = Mathf.Lerp(startIntensity, ambientLightIntensity, time);
				gc.sceneColorManager.UpdateScript();
			});
		}
		if (flag2 || didAffectSaturation)
		{
			if (!setSaturation)
			{
				gc.colorCorrectionCurves.saturation = AdjustSaturation(saturation);
				setSaturation = true;
			}
			if (saturationFadeRoutine != null)
			{
				StopCoroutine(saturationFadeRoutine);
			}
			float startSaturation = gc.colorCorrectionCurves.saturation;
			saturationFadeRoutine = this.StartTimerRoutine(0f, duration2, delegate(float time)
			{
				float originalSaturation = Mathf.Lerp(startSaturation, newSaturation, time);
				originalSaturation = AdjustSaturation(originalSaturation);
				gc.colorCorrectionCurves.saturation = originalSaturation;
				gc.sceneColorManager.SaturationA = originalSaturation;
			});
		}
		didAffectAmbientLight = flag;
		didAffectSaturation = flag2;
	}

	public static void SetLighting(Color ambientLightColor, float ambientLightIntensity)
	{
		_currentAmbientLightColor = ambientLightColor;
		_currentAmbientLightIntensity = ambientLightIntensity;
		float num = Mathf.Lerp(1f, ambientLightIntensity, AmbientIntesityMix);
		RenderSettings.ambientLight = new Color(ambientLightColor.r * num, ambientLightColor.g * num, ambientLightColor.b * num, 1f);
		RenderSettings.ambientIntensity = 1f;
	}

	private void Update()
	{
		if (!isGameplayScene)
		{
			return;
		}
		if (enviroTimer < 0.25f)
		{
			enviroTimer += Time.deltaTime;
		}
		else if (!enviroSent && heroCtrl != null)
		{
			heroCtrl.checkEnvironment();
			enviroSent = true;
		}
		if (!heroInfoSent && heroCtrl != null)
		{
			heroCtrl.heroLight.MaterialColor = Color.white;
			heroCtrl.SetDarkness(darknessLevel);
			heroInfoSent = true;
		}
		if (!setSaturation)
		{
			if (Math.Abs(AdjustSaturation(saturation) - gc.colorCorrectionCurves.saturation) > Mathf.Epsilon)
			{
				gc.colorCorrectionCurves.saturation = AdjustSaturation(saturation);
			}
			setSaturation = true;
		}
	}

	public int GetDarknessLevel()
	{
		return darknessLevel;
	}

	public void SetWindy(bool setting)
	{
		isWindy = setting;
	}

	public float AdjustSaturation(float originalSaturation)
	{
		if (!ignorePlatformSaturationModifiers)
		{
			return AdjustSaturationForPlatform(originalSaturation, mapZone);
		}
		return originalSaturation;
	}

	public static float AdjustSaturationForPlatform(float originalSaturation, MapZone? mapZone = null)
	{
		return originalSaturation + 0.4f;
	}

	public void CancelEnviroSnapshot()
	{
		cancelEnviroSnapshot = true;
	}

	public void SetExtraRestZone(int zone)
	{
		extraRestZone = (ExtraRestZones)zone;
	}

	private void PrintDebugInfo()
	{
		string text = "SM Setting Curves to ";
		text += "R: (";
		Keyframe[] keys = redChannel.keys;
		foreach (Keyframe keyframe in keys)
		{
			text = text + keyframe.value + ", ";
		}
		text += ") G: (";
		keys = greenChannel.keys;
		foreach (Keyframe keyframe2 in keys)
		{
			text = text + keyframe2.value + ", ";
		}
		text += " ) B: (";
		keys = blueChannel.keys;
		foreach (Keyframe keyframe3 in keys)
		{
			text = text + keyframe3.value + ", ";
		}
		text = text + ") S: " + saturation;
		Debug.Log(text);
	}

	private void DrawBlackBorders()
	{
		float x = borderPrefab.transform.localScale.x;
		if (sceneBordersMask.IsBitSet(3))
		{
			GameObject obj = UnityEngine.Object.Instantiate(borderPrefab);
			obj.transform.SetPosition2D(gm.sceneWidth + x / 2f, gm.sceneHeight / 2f);
			obj.transform.localScale = new Vector2(x, gm.sceneHeight + x * 2f);
			obj.transform.eulerAngles = new Vector3(0f, 0f, 180f);
			SceneManager.MoveGameObjectToScene(obj, base.gameObject.scene);
		}
		if (sceneBordersMask.IsBitSet(2))
		{
			GameObject obj2 = UnityEngine.Object.Instantiate(borderPrefab);
			obj2.transform.SetPosition2D(0f - x / 2f, gm.sceneHeight / 2f);
			obj2.transform.localScale = new Vector2(x, gm.sceneHeight + x * 2f);
			SceneManager.MoveGameObjectToScene(obj2, base.gameObject.scene);
		}
		if (sceneBordersMask.IsBitSet(0))
		{
			GameObject obj3 = UnityEngine.Object.Instantiate(borderPrefab);
			obj3.transform.SetPosition2D(gm.sceneWidth / 2f, gm.sceneHeight + x / 2f);
			obj3.transform.localScale = new Vector2(x, x * 2f + gm.sceneWidth);
			obj3.transform.eulerAngles = new Vector3(0f, 0f, -90f);
			SceneManager.MoveGameObjectToScene(obj3, base.gameObject.scene);
		}
		if (sceneBordersMask.IsBitSet(1))
		{
			GameObject obj4 = UnityEngine.Object.Instantiate(borderPrefab);
			obj4.transform.SetPosition2D(gm.sceneWidth / 2f, 0f - x / 2f);
			obj4.transform.localScale = new Vector2(x, x * 2f + gm.sceneWidth);
			obj4.transform.eulerAngles = new Vector3(0f, 0f, 90f);
			SceneManager.MoveGameObjectToScene(obj4, base.gameObject.scene);
		}
	}

	private void AddSceneMapped()
	{
		string sceneNameString = gm.GetSceneNameString();
		if (pd.scenesVisited.Contains(sceneNameString) || manualMapTrigger)
		{
			return;
		}
		pd.scenesVisited.Add(sceneNameString);
		GameManager instance = GameManager.instance;
		if ((bool)instance)
		{
			GameMap gameMap = instance.gameMap;
			if ((bool)gameMap)
			{
				gameMap.SetupMap(pinsOnly: true);
			}
		}
	}

	public void UpdateSceneSettings(SceneManagerSettings sms)
	{
		if (sms != null)
		{
			mapZone = sms.mapZone;
			defaultColor = new Color(sms.defaultColor.r, sms.defaultColor.g, sms.defaultColor.b, sms.defaultColor.a);
			defaultIntensity = sms.defaultIntensity;
			saturation = sms.saturation;
			redChannel = new AnimationCurve(sms.redChannel.keys.Clone() as Keyframe[]);
			greenChannel = new AnimationCurve(sms.greenChannel.keys.Clone() as Keyframe[]);
			blueChannel = new AnimationCurve(sms.blueChannel.keys.Clone() as Keyframe[]);
			heroLightColor = new Color(sms.heroLightColor.r, sms.heroLightColor.g, sms.heroLightColor.b, sms.heroLightColor.a);
			blurPlaneVibranceOffset = sms.blurPlaneVibranceOffset;
			heroSaturationOffset = sms.heroSaturationOffset;
		}
	}

	public void UpdateScene()
	{
		if (!Application.isPlaying || IsGradeOverridden)
		{
			return;
		}
		if ((bool)gc)
		{
			gc.colorCorrectionCurves.saturation = AdjustSaturation(saturation);
			gc.colorCorrectionCurves.redChannel = redChannel;
			gc.colorCorrectionCurves.greenChannel = greenChannel;
			gc.colorCorrectionCurves.blueChannel = blueChannel;
			gc.colorCorrectionCurves.UpdateParameters();
			gc.sceneColorManager.SaturationA = AdjustSaturation(saturation);
			gc.sceneColorManager.RedA = redChannel;
			gc.sceneColorManager.GreenA = greenChannel;
			gc.sceneColorManager.BlueA = blueChannel;
			SetLighting(defaultColor, defaultIntensity);
			gc.sceneColorManager.AmbientColorA = defaultColor;
			gc.sceneColorManager.AmbientIntensityA = defaultIntensity;
			if (isGameplayScene)
			{
				if (heroCtrl != null)
				{
					heroCtrl.heroLight.BaseColor = heroLightColor;
					heroCtrl.heroLight.Alpha = 1f;
					heroCtrl.heroLight.UpdateColor(forceImmediate: true);
				}
				gc.sceneColorManager.HeroLightColorA = heroLightColor;
				float num = heroSaturationOffset + 0.5f;
				Shader.SetGlobalFloat(_desaturationPropId, num * -1f);
			}
			gc.sceneColorManager.UpdateScript(forceUpdate: true);
		}
		BlurPlane.SetVibranceOffset(blurPlaneVibranceOffset);
	}

	public void SetMusicCue(MusicCue newMusicCue)
	{
		musicCue = newMusicCue;
		if ((bool)newMusicCue)
		{
			newMusicCue.Preload(base.gameObject);
		}
	}

	private void OnValidate()
	{
		if (noTeleport)
		{
			teleportAllowState = NoTeleportRegion.TeleportAllowState.Blocked;
			noTeleport = false;
		}
	}

	private void OnEnable()
	{
	}

	private void OnDisable()
	{
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using GenericVariableExtension;
using GlobalEnums;
using GlobalSettings;
using InControl;
using TeamCherry.Localization;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public delegate void PausedEvent(bool isPaused);

	public delegate void GameStateEvent(GameState gameState);

	private class SceneSeedTracker
	{
		public int SceneNameHash;

		public int Seed;

		public int TransitionsLeft;
	}

	public delegate void BossLoad();

	public delegate void EnterSceneEvent();

	public delegate void SceneTransitionFinishEvent();

	public enum SceneLoadVisualizations
	{
		Custom = -1,
		Default = 0,
		ContinueFromSave = 1,
		ThreadMemory = 2
	}

	public class SceneLoadInfo
	{
		public bool IsFirstLevelForPlayer;

		public string SceneName;

		public IResourceLocation SceneResourceLocation;

		public int AsyncPriority = -1;

		public GatePosition? HeroLeaveDirection;

		public string EntryGateName;

		public float EntryDelay;

		public bool EntrySkip;

		public bool PreventCameraFadeOut;

		public bool WaitForSceneTransitionCameraFade;

		public SceneLoadVisualizations Visualization;

		public bool AlwaysUnloadUnusedAssets;

		public bool ForceWaitFetch;

		public int TransitionID;

		protected static int transitionCounter;

		public SceneLoadInfo()
		{
			TransitionID = transitionCounter++;
		}

		public override string ToString()
		{
			return $"Scene Load #{TransitionID} : Scene Name: {SceneName} : Entry Gate Name {EntryGateName} : Entry Skip {EntrySkip} : Prevent Camera Fade {PreventCameraFadeOut} : Wait For Scene Transition {WaitForSceneTransitionCameraFade} : Force Fetch Wait {ForceWaitFetch}";
		}

		public virtual void NotifyFadedOut()
		{
		}

		public virtual void NotifyFetchComplete()
		{
		}

		public virtual bool IsReadyToActivate()
		{
			return true;
		}

		public virtual void NotifyFinished()
		{
		}
	}

	public delegate void SceneTransitionBeganDelegate(SceneLoad sceneLoad);

	private struct Rb2dState
	{
		public Rigidbody2D Body;

		public bool Simulated;
	}

	public interface ISceneManualSimulatePhysics
	{
		void OnManualPhysics(float deltaTime);

		void PrepareManualSimulate();

		void OnManualSimulateFinished();
	}

	public interface ISkippable
	{
		IEnumerator Skip();
	}

	private const float TIME_PASSES_SCENE_LIMIT = 300f;

	public bool isPaused;

	private int timeSlowedCount;

	public string sceneName;

	public int sceneNameHash;

	public string nextSceneName;

	public string entryGateName;

	private TransitionPoint callingGate;

	private Vector3 entrySpawnPoint;

	private float entryDelay;

	public float sceneWidth;

	public float sceneHeight;

	public string lastSceneName;

	private List<SceneSeedTracker> sceneSeedTrackers;

	public GameConfig gameConfig;

	public GameCameras gameCams;

	private List<string> queuedMenuStyles = new List<string>();

	[SerializeField]
	private AudioManager audioManager;

	[SerializeField]
	private InControlManager inControlManagerPrefab;

	[SerializeField]
	public GameSettings gameSettings;

	public TimeScaleIndependentUpdate timeTool;

	public GameMap gameMap;

	[SerializeField]
	public PlayerData playerData;

	[SerializeField]
	public SceneData sceneData;

	public int profileID;

	private bool needsFlush;

	private float sessionStartTime;

	private float sessionPlayTimer;

	private float timeInScene;

	private float timeSinceLastTimePasses;

	public string lastTimePassesMapZone;

	public bool startedOnThisScene = true;

	private bool hazardRespawningHero;

	private string targetScene;

	private bool needFirstFadeIn;

	private bool waitForManualLevelStart;

	private AsyncOperationHandle<GameObject> globalPoolPrefabHandle;

	private AsyncOperationHandle<GameObject> heroPrefabHandle;

	private int heroDeathCount;

	private bool startedSteamEnabled;

	private bool startedGOGEnabled;

	private bool startedLanguageDisabled;

	private bool isSaveGameQueued;

	private bool isAutoSaveQueued;

	private AutoSaveName queuedAutoSaveName;

	private bool forceCurrentSceneMemory;

	public AudioMixerSnapshot actorSnapshotUnpaused;

	public AudioMixerSnapshot actorSnapshotPaused;

	[SerializeField]
	private float sceneTransitionActorFadeDown;

	[SerializeField]
	private float sceneTransitionActorFadeUp;

	public AudioMixerSnapshot silentSnapshot;

	public AudioMixerSnapshot noMusicSnapshot;

	public MusicCue noMusicCue;

	public AudioMixerSnapshot noAtmosSnapshot;

	[NonSerialized]
	private int nextLevelEntryNumber;

	[NonSerialized]
	private int skipActorEntryFade = -1;

	private bool hasFinishedEnteringScene;

	private bool didEmergencyQuit;

	private bool isLoading;

	private SceneLoadVisualizations loadVisualization;

	private float currentLoadDuration;

	private int sceneLoadsWithoutGarbageCollect;

	private int queuedBlueHealth;

	[SerializeField]
	private StandaloneLoadingSpinner standaloneLoadingSpinnerPrefab;

	[SerializeField]
	private StandaloneLoadingSpinner standaloneLoadingSpinnerEndGamePrefab;

	private bool shouldFadeInScene;

	public static GameManager _instance;

	private static bool isFirstStartup = true;

	private SceneLoad sceneLoad;

	private bool hasSetup;

	private bool registerEvents;

	private CameraManagerReference subbedCamShake;

	private List<ISkippable> skippables;

	private int mapZoneStringVersion = -1;

	private string mapZoneString;

	private int mapZoneVersion = -1;

	private MapZone currentMapZone;

	private string rawSceneName;

	private static string lastFullSceneName;

	private static string fixedSceneName;

	private int saveIconShowCounter;

	private static object _inventoryInputBlocker;

	public GameState GameState { get; private set; }

	public bool TimeSlowed => timeSlowedCount > 0;

	public System.Random SceneSeededRandom { get; private set; }

	public InputHandler inputHandler { get; private set; }

	public AchievementHandler achievementHandler { get; private set; }

	public AudioManager AudioManager => audioManager;

	public static bool IsCollectingGarbage { get; set; }

	public CameraController cameraCtrl { get; private set; }

	public HeroController hero_ctrl { get; private set; }

	public SpriteRenderer heroLight { get; private set; }

	public CustomSceneManager sm { get; private set; }

	public UIManager ui { get; private set; }

	public tk2dTileMap tilemap { get; private set; }

	public PlayMakerFSM soulOrb_fsm { get; private set; }

	public PlayMakerFSM soulVessel_fsm { get; private set; }

	public PlayMakerFSM inventoryFSM { get; private set; }

	public PlayMakerFSM screenFader_fsm { get; private set; }

	public float PlayTime => sessionStartTime + sessionPlayTimer;

	public bool RespawningHero { get; set; }

	public bool IsInSceneTransition { get; private set; }

	public static bool SuppressRegainControl { get; set; }

	public bool HasFinishedEnteringScene => hasFinishedEnteringScene;

	public bool IsLoadingSceneTransition
	{
		get
		{
			if (sceneLoad != null && !sceneLoad.SceneLoadInfo.IsReadyToActivate())
			{
				return false;
			}
			return isLoading;
		}
	}

	public SceneLoadVisualizations LoadVisualization => loadVisualization;

	public float CurrentLoadDuration
	{
		get
		{
			if (!isLoading)
			{
				return 0f;
			}
			return currentLoadDuration;
		}
	}

	public int QueuedBlueHealth
	{
		get
		{
			return queuedBlueHealth;
		}
		set
		{
			queuedBlueHealth = Mathf.Max(value, 0);
		}
	}

	public bool IsFirstLevelForPlayer { get; private set; }

	public static bool IsWaitingForSceneReady { get; private set; }

	public static GameManager instance
	{
		get
		{
			GameManager silentInstance = SilentInstance;
			if (!silentInstance)
			{
				Debug.LogError("Couldn't find a Game Manager, make sure one exists in the scene.");
			}
			return silentInstance;
		}
	}

	public static GameManager SilentInstance
	{
		get
		{
			if (_instance != null)
			{
				return _instance;
			}
			_instance = UnityEngine.Object.FindObjectOfType<GameManager>();
			if ((bool)_instance && Application.isPlaying)
			{
				UnityEngine.Object.DontDestroyOnLoad(_instance.gameObject);
			}
			return _instance;
		}
	}

	public static GameManager UnsafeInstance => _instance;

	public SceneLoad LastSceneLoad { get; set; }

	public bool BlockNextVibrationFadeIn { get; set; }

	public bool DidPurchasePin { get; set; }

	public bool DidPurchaseMap { get; set; }

	public event PausedEvent GamePausedChange;

	public event GameStateEvent GameStateChange;

	public event Action SceneInit;

	public event Action SavePersistentObjects;

	public event Action ResetSemiPersistentObjects;

	public event Action NextSceneWillActivate;

	public event Action UnloadingLevel;

	public event Action RefreshLanguageText;

	public event Action RefreshParticleLevel;

	public event BossLoad OnLoadedBoss;

	public event EnterSceneEvent OnFinishedEnteringScene;

	public event SceneTransitionFinishEvent OnBeforeFinishedSceneTransition;

	public event SceneTransitionFinishEvent OnFinishedSceneTransition;

	public static event SceneTransitionBeganDelegate SceneTransitionBegan;

	public void SpawnInControlManager()
	{
		bool flag = false;
		try
		{
			flag = SingletonMonoBehavior<InControlManager>.Instance != null;
		}
		catch
		{
		}
		if (!flag)
		{
			UnityEngine.Object.DontDestroyOnLoad(UnityEngine.Object.Instantiate(inControlManagerPrefab).gameObject);
		}
	}

	private void Awake()
	{
		PlayMakerPrefs.LogPerformanceWarnings = false;
		if (_instance == null)
		{
			_instance = this;
			UnityEngine.Object.DontDestroyOnLoad(this);
			SetupGameRefs();
		}
		else
		{
			if (this != _instance)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			SetupGameRefs();
		}
		PlayMakerGlobals.Instance.Variables.FindFsmGameObject("GameManager").Value = base.gameObject;
	}

	private void Start()
	{
		if (this == _instance)
		{
			SetupStatusModifiers();
			if ((bool)Platform.Current)
			{
				Platform.Current.SetGameManagerReady();
			}
		}
	}

	private void OnDestroy()
	{
		if (_instance == this)
		{
			_instance = null;
			PlayerData.ClearOptimisers();
			VariableExtensions.ClearCache();
			VariableExtensionsGeneric.ClearCache();
			IsCollectingGarbage = false;
		}
		UnregisterEvents();
	}

	protected void Update()
	{
		AudioGroupManager.UpdateAudioGroups();
		if (isLoading)
		{
			currentLoadDuration += Time.unscaledDeltaTime;
		}
		else
		{
			currentLoadDuration = 0f;
		}
		IncreaseGameTimer(ref sessionPlayTimer);
		IncreaseGameTimer(ref timeInScene);
		if (timeInScene < 300f)
		{
			IncreaseGameTimer(ref timeSinceLastTimePasses);
		}
		CrossSceneWalker.Tick();
		UpdateEngagement();
	}

	private void UpdateEngagement()
	{
		if (GameState == GameState.MAIN_MENU)
		{
			if (!ui.didLeaveEngageMenu)
			{
				if (ui.menuState != MainMenuState.ENGAGE_MENU)
				{
					return;
				}
				Platform.EngagementStates engagementState = Platform.Current.EngagementState;
				if (engagementState != Platform.EngagementStates.Engaged)
				{
					if (engagementState != Platform.EngagementStates.EngagePending && Input.anyKeyDown)
					{
						Platform.Current.BeginEngagement();
					}
					Platform.Current.UpdateWaitingForEngagement();
				}
				else if (!Platform.Current.IsSavingAllowedByEngagement || Platform.Current.IsSaveStoreMounted)
				{
					ui.didLeaveEngageMenu = true;
					ui.UIGoToMainMenu();
				}
			}
			else if (Platform.Current.EngagementState != Platform.EngagementStates.Engaged && inputHandler.acceptingInput && ui.menuState != MainMenuState.ENGAGE_MENU)
			{
				ui.UIGoToEngageMenu();
				ui.slotOne.ClearCache();
				ui.slotTwo.ClearCache();
				ui.slotThree.ClearCache();
				ui.slotFour.ClearCache();
			}
		}
		else if ((GameState == GameState.PLAYING || GameState == GameState.PAUSED) && Platform.Current.EngagementState == Platform.EngagementStates.NotEngaged && !didEmergencyQuit)
		{
			didEmergencyQuit = true;
			EmergencyReturnToMenu();
		}
	}

	private void LevelActivated(Scene sceneFrom, Scene sceneTo)
	{
		if (!(this != _instance))
		{
			PersistentAudioManager.OnLevelLoaded();
			if (!waitForManualLevelStart)
			{
				SetupSceneRefs(refreshTilemapInfo: true);
				BeginScene();
				OnNextLevelReady();
			}
		}
	}

	private void OnDisable()
	{
		SceneManager.activeSceneChanged -= LevelActivated;
	}

	private void OnApplicationQuit()
	{
		if (startedLanguageDisabled)
		{
			gameConfig.hideLanguageOption = true;
		}
	}

	public void BeginSceneTransition(SceneLoadInfo info)
	{
		inventoryFSM.SendEvent("INVENTORY CANCEL");
		if (info.IsFirstLevelForPlayer)
		{
			ResetGameTimer();
			LoadedFromMenu();
		}
		StartCoroutine(BeginSceneTransitionRoutine(info));
	}

	private IEnumerator BeginSceneTransitionRoutine(SceneLoadInfo info)
	{
		SuppressRegainControl = false;
		if (sceneLoad != null)
		{
			yield break;
		}
		cameraCtrl.ResetPositionedAtHero();
		IsInSceneTransition = true;
		hasFinishedEnteringScene = false;
		StaticVariableList.ReportSceneTransition();
		sceneLoad = new SceneLoad(this, info);
		isLoading = true;
		loadVisualization = info.Visualization;
		SceneLoad unloadingSceneLoad = LastSceneLoad;
		LastSceneLoad = sceneLoad;
		NonTinter.ClearNonTinters();
		if (hero_ctrl != null)
		{
			FSMUtility.SendEventToGameObject(hero_ctrl.gameObject, "ROAR EXIT");
			hero_ctrl.LeavingScene();
			hero_ctrl.SetHeroParent(null);
		}
		if (!info.IsFirstLevelForPlayer)
		{
			NoLongerFirstGame();
		}
		else
		{
			IsFirstLevelForPlayer = true;
		}
		SaveLevelState();
		if (GameState != GameState.CUTSCENE)
		{
			SetState(GameState.EXITING_LEVEL);
		}
		entryGateName = info.EntryGateName ?? "";
		targetScene = info.SceneName;
		if (hero_ctrl != null)
		{
			hero_ctrl.LeaveScene(info.HeroLeaveDirection);
		}
		if (!info.PreventCameraFadeOut)
		{
			cameraCtrl.FreezeInPlace(freezeTarget: true);
			screenFader_fsm.SendEvent("SCENE FADE OUT");
		}
		EventRegister.SendEvent(EventRegisterEvents.SceneTransitionBegan);
		startedOnThisScene = false;
		nextSceneName = info.SceneName;
		waitForManualLevelStart = true;
		IsWaitingForSceneReady = true;
		MapZone previousMapZone = sm.mapZone;
		bool forcedNotMemory = sm.ForceNotMemory;
		actorSnapshotPaused.TransitionToSafe(sceneTransitionActorFadeDown);
		SilkSpool.EndSilkAudio();
		this.UnloadingLevel?.Invoke();
		string unloadingSceneName = SceneManager.GetActiveScene().name;
		bool didBlankScreen = false;
		sceneLoad.FetchComplete += FetchCompleteAction;
		sceneLoad.WillActivate += WillActivateAction;
		sceneLoad.ActivationComplete += ActivationCompleteAction;
		sceneLoad.Complete += CompleteAction;
		sceneLoad.Finish += FinishAction;
		if (GameManager.SceneTransitionBegan != null)
		{
			try
			{
				GameManager.SceneTransitionBegan(sceneLoad);
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in responders to GameManager.SceneTransitionBegan. Attempting to continue load regardless.");
				CheatManager.LastErrorText = ex.ToString();
				Debug.LogException(ex);
			}
		}
		sceneLoad.IsFetchAllowed = !info.ForceWaitFetch && (Platform.Current.FetchScenesBeforeFade || info.PreventCameraFadeOut);
		sceneLoad.IsActivationAllowed = false;
		sceneLoad.WaitForFade = info.WaitForSceneTransitionCameraFade;
		float cameraFadeTimer;
		if (info.WaitForSceneTransitionCameraFade)
		{
			cameraFadeTimer = 0.34f;
		}
		else
		{
			cameraFadeTimer = 0f;
			info.NotifyFadedOut();
			sceneLoad.WaitForFade = false;
		}
		Platform.Current.SetSceneLoadState(isInProgress: true);
		if (info.SceneName == unloadingSceneName)
		{
			Debug.Log("Reloading same scene! Destroying GuidComponents to prevent collisions...");
			GuidComponent[] array = UnityEngine.Object.FindObjectsByType<GuidComponent>(FindObjectsInactive.Include, FindObjectsSortMode.None);
			foreach (GuidComponent guidComponent in array)
			{
				if (guidComponent.gameObject.scene.name == info.SceneName)
				{
					UnityEngine.Object.Destroy(guidComponent);
				}
			}
		}
		sceneLoad.Begin();
		while (true)
		{
			bool flag = false;
			if (cameraFadeTimer > 0f)
			{
				cameraFadeTimer -= Time.deltaTime;
				if (cameraFadeTimer > 0f)
				{
					flag = true;
				}
				else
				{
					info.NotifyFadedOut();
					sceneLoad.WaitForFade = false;
				}
			}
			if (!info.IsReadyToActivate())
			{
				flag = true;
			}
			if (!flag)
			{
				break;
			}
			yield return null;
		}
		VibrationManager.StopAllVibration();
		Platform.Current.SetSceneLoadState(isInProgress: true, isHighPriority: true);
		Platform.Current.OnScreenFaded();
		sceneLoad.IsFetchAllowed = true;
		sceneLoad.IsActivationAllowed = true;
		void ActivationCompleteAction()
		{
			sceneLoad.ActivationComplete -= ActivationCompleteAction;
			UnloadScene(unloadingSceneName, unloadingSceneLoad);
			ObjectPool.AuditSpawnedDictionary();
			RefreshTilemapInfo(info.SceneName);
			sceneLoad.IsUnloadAssetsRequired = info.AlwaysUnloadUnusedAssets;
			bool flag2 = true;
			if (!sceneLoad.IsUnloadAssetsRequired)
			{
				float? beginTime = sceneLoad.BeginTime;
				if (beginTime.HasValue && Time.realtimeSinceStartup - beginTime.Value > Platform.Current.MaximumLoadDurationForNonCriticalGarbageCollection && sceneLoadsWithoutGarbageCollect < Platform.Current.MaximumSceneTransitionsWithoutNonCriticalGarbageCollection)
				{
					flag2 = false;
				}
			}
			if (flag2)
			{
				sceneLoadsWithoutGarbageCollect = 0;
			}
			else
			{
				sceneLoadsWithoutGarbageCollect++;
			}
			sceneLoad.IsGarbageCollectRequired = flag2;
			if (flag2)
			{
				IsCollectingGarbage = true;
			}
			if (needFirstFadeIn)
			{
				ui.BlankScreen(value: true);
				didBlankScreen = true;
			}
		}
		void CompleteAction()
		{
			IsCollectingGarbage = false;
			sceneLoad.Complete -= CompleteAction;
			SetupSceneRefs(refreshTilemapInfo: false);
			BeginScene();
			if (sm.mapZone != previousMapZone || sm.ForceNotMemory || forcedNotMemory)
			{
				EnteredNewMapZone(previousMapZone, sm.mapZone, sm.ForceNotMemory);
			}
			if (gameMap != null)
			{
				gameMap.LevelReady();
			}
		}
		void FetchCompleteAction()
		{
			sceneLoad.FetchComplete -= FetchCompleteAction;
			info.NotifyFetchComplete();
		}
		void FinishAction()
		{
			sceneLoad.Finish -= FinishAction;
			if (didBlankScreen)
			{
				ui.BlankScreen(value: false);
			}
			Platform.Current.SetSceneLoadState(isInProgress: false);
			isLoading = false;
			this.OnBeforeFinishedSceneTransition?.Invoke();
			info.NotifyFinished();
			waitForManualLevelStart = false;
			OnNextLevelReady();
			sceneLoad = null;
			IsInSceneTransition = false;
			this.OnFinishedSceneTransition?.Invoke();
		}
		void StopEmitter(string name)
		{
			if (!(gameCams == null))
			{
				Transform transform = gameCams.transform.Find(name);
				if (!(transform == null))
				{
					FSMUtility.SendEventToGameObject(transform.gameObject, "END");
				}
			}
		}
		void WillActivateAction()
		{
			sceneLoad.WillActivate -= WillActivateAction;
			SetState(GameState.EXITING_LEVEL);
			skippables?.Clear();
			this.NextSceneWillActivate?.Invoke();
			entryDelay = info.EntryDelay;
			AudioManager.PauseActorSnapshot();
			actorSnapshotPaused.TransitionToSafe(0f);
			HeroController silentInstance = HeroController.SilentInstance;
			if ((bool)silentInstance && silentInstance.cState.downSpiking)
			{
				hero_ctrl.FinishDownspike();
			}
			TweenExtensions.ClenaupInactiveCoroutines();
			PlayMakerValidator.FixGlobalVariables(null);
			StopEmitter("Roar Wave Emitter");
			StopEmitter("Roar Wave Emitter Small");
		}
	}

	private static void ReportUnload(string sn)
	{
		PersonalObjectPool.PreUnloadingScene(sn);
		PersonalObjectPool.UnloadingScene(sn);
	}

	private void UnloadScene(string unloadingSceneName, SceneLoad unloadingSceneLoad)
	{
		ReportUnload(unloadingSceneName);
		if (unloadingSceneLoad == null && LastSceneLoad != null && LastSceneLoad.TargetSceneName == unloadingSceneName)
		{
			unloadingSceneLoad = LastSceneLoad;
		}
		SceneManager.UnloadScene(unloadingSceneName);
	}

	public IEnumerator TransitionScene(TransitionPoint gate)
	{
		Debug.LogError("TransitionScene(TransitionPoint) is no longer supported");
		callingGate = gate;
		hero_ctrl.LeavingScene();
		NoLongerFirstGame();
		SaveLevelState();
		SetState(GameState.EXITING_LEVEL);
		entryGateName = gate.entryPoint;
		targetScene = gate.targetScene;
		hero_ctrl.LeaveScene(gate.GetGatePosition());
		cameraCtrl.FreezeInPlace(freezeTarget: true);
		screenFader_fsm.SendEvent("SCENE FADE OUT");
		hasFinishedEnteringScene = false;
		yield return new WaitForSeconds(0.34f);
		LeftScene(doAdditiveLoad: true);
	}

	public void ChangeToScene(string targetScene, string entryGateName, float pauseBeforeEnter)
	{
		if (hero_ctrl != null)
		{
			hero_ctrl.LeavingScene();
			hero_ctrl.transform.SetParent(null);
		}
		NoLongerFirstGame();
		SaveLevelState();
		SetState(GameState.EXITING_LEVEL);
		this.entryGateName = entryGateName;
		this.targetScene = targetScene;
		entryDelay = pauseBeforeEnter;
		cameraCtrl.FreezeInPlace();
		if (hero_ctrl != null)
		{
			hero_ctrl.ResetState();
		}
		LeftScene();
	}

	public void LeftScene(bool doAdditiveLoad = false)
	{
		if (doAdditiveLoad)
		{
			StartCoroutine(LoadSceneAdditive(targetScene));
		}
		else
		{
			LoadScene(targetScene);
		}
	}

	public IEnumerator PlayerDead(float waitTime)
	{
		cameraCtrl.FreezeInPlace(freezeTarget: true);
		NoLongerFirstGame();
		ResetSemiPersistentItems();
		MazeController.ResetSaveData();
		bool isPermaDead = playerData.permadeathMode == PermadeathModes.Dead;
		bool willDemoEnd;
		bool finishedSaving;
		if (DemoHelper.IsDemoMode)
		{
			heroDeathCount++;
			int maxDeathCount = Demo.MaxDeathCount;
			willDemoEnd = heroDeathCount >= maxDeathCount && maxDeathCount > 0;
			finishedSaving = true;
		}
		else
		{
			willDemoEnd = false;
			finishedSaving = false;
			SaveGame(profileID, delegate
			{
				finishedSaving = true;
			});
			if (!isPermaDead)
			{
				GetRespawnInfo(out var scene, out var _);
				ScenePreloader.SpawnPreloader(scene, LoadSceneMode.Additive);
			}
		}
		yield return new WaitForSeconds(waitTime);
		while (!finishedSaving)
		{
			yield return null;
		}
		if (willDemoEnd)
		{
			BeginSceneTransition(new SceneLoadInfo
			{
				SceneName = "Demo End",
				PreventCameraFadeOut = true,
				WaitForSceneTransitionCameraFade = false,
				Visualization = SceneLoadVisualizations.Default
			});
		}
		else
		{
			TimePasses();
			if (!isPermaDead)
			{
				ReadyForRespawn(isFirstLevelForPlayer: false);
			}
			else
			{
				LoadScene("PermaDeath");
			}
			ResetSemiPersistentItems();
		}
	}

	public IEnumerator PlayerDeadFromHazard(float waitTime)
	{
		cameraCtrl.FreezeInPlace(freezeTarget: true);
		NoLongerFirstGame();
		SaveLevelState();
		yield return new WaitForSeconds(waitTime);
		screenFader_fsm.SendEventSafe("HAZARD FADE");
		EventRegister.SendEvent(EventRegisterEvents.HazardFade);
		yield return new WaitForSeconds(0.65f);
		PlayMakerFSM.BroadcastEvent("HAZARD RELOAD");
		EventRegister.SendEvent(EventRegisterEvents.HazardReload);
		if (!hero_ctrl.cState.dead)
		{
			HazardRespawn();
		}
	}

	private void GetRespawnInfo(out string scene, out string marker)
	{
		string savedRespawnScene;
		string savedRespawnMarker;
		if (!string.IsNullOrEmpty(playerData.tempRespawnScene))
		{
			savedRespawnScene = playerData.tempRespawnScene;
			savedRespawnMarker = playerData.tempRespawnMarker;
		}
		else
		{
			savedRespawnScene = playerData.respawnScene;
			savedRespawnMarker = playerData.respawnMarkerName;
		}
		Dictionary<string, SceneTeleportMap.SceneInfo> teleportMap = SceneTeleportMap.GetTeleportMap();
		if (Application.isEditor)
		{
			scene = savedRespawnScene;
			marker = savedRespawnMarker;
			if (teleportMap.TryGetValue(savedRespawnScene, out var value) && !value.RespawnPoints.Contains(savedRespawnMarker))
			{
				teleportMap.Where((KeyValuePair<string, SceneTeleportMap.SceneInfo> kvp) => kvp.Key.StartsWith(savedRespawnScene)).Any((KeyValuePair<string, SceneTeleportMap.SceneInfo> kvp) => kvp.Value.RespawnPoints.Contains(savedRespawnMarker));
			}
			return;
		}
		if (teleportMap.TryGetValue(savedRespawnScene, out var value2))
		{
			if (value2.RespawnPoints.Contains(savedRespawnMarker))
			{
				scene = savedRespawnScene;
				marker = savedRespawnMarker;
				return;
			}
			if (teleportMap.Where((KeyValuePair<string, SceneTeleportMap.SceneInfo> kvp) => kvp.Key.StartsWith(savedRespawnScene)).Any((KeyValuePair<string, SceneTeleportMap.SceneInfo> kvp) => kvp.Value.RespawnPoints.Contains(savedRespawnMarker)))
			{
				scene = savedRespawnScene;
				marker = savedRespawnMarker;
				return;
			}
		}
		scene = "Tut_01";
		marker = "Death Respawn Marker Init";
		playerData.ResetTempRespawn();
	}

	public void ReadyForRespawn(bool isFirstLevelForPlayer)
	{
		RespawningHero = true;
		GetRespawnInfo(out var scene, out var marker);
		BeginSceneTransition(new SceneLoadInfo
		{
			PreventCameraFadeOut = true,
			WaitForSceneTransitionCameraFade = false,
			EntryGateName = marker,
			SceneName = scene,
			Visualization = (isFirstLevelForPlayer ? SceneLoadVisualizations.ContinueFromSave : SceneLoadVisualizations.Default),
			AlwaysUnloadUnusedAssets = true,
			IsFirstLevelForPlayer = isFirstLevelForPlayer
		});
	}

	public void HazardRespawn()
	{
		hazardRespawningHero = true;
		cameraCtrl.ResetStartTimer();
		if (!cameraCtrl.camTarget.IsFreeModeManual)
		{
			cameraCtrl.camTarget.mode = CameraTarget.TargetMode.FOLLOW_HERO;
		}
		EnterHero();
	}

	public void TimePasses()
	{
		StaticVariableList.ClearSceneTransitions();
		string sceneNameString = GetSceneNameString();
		MapZone currentMapZoneEnum = GetCurrentMapZoneEnum();
		if (playerData.seenBellBeast && playerData.shermaPos == 0)
		{
			playerData.shermaPos = 1;
		}
		if (playerData.shermaPos == 1 && sceneNameString != "Bone_East_10_Room" && (playerData.scenesVisited.Contains("Belltown") || playerData.scenesVisited.Contains("Halfway_01")))
		{
			playerData.shermaPos = 2;
		}
		if (playerData.spinnerDefeated && (playerData.encounteredLastJudge || (playerData.activatedStepsUpperBellbench && playerData.defeatedCoralDrillers) || (playerData.visitedCoral && currentMapZoneEnum == MapZone.DUSTPENS) || currentMapZoneEnum == MapZone.SWAMP))
		{
			playerData.shermaAtSteps = true;
		}
		if (playerData.SeenLastJudgeGateOpen && !playerData.shermaCitadelEntrance_Visiting && playerData.enteredCoral_10 && playerData.enteredSong_19 && playerData.citadelWoken)
		{
			playerData.shermaCitadelEntrance_Visiting = true;
		}
		if ((playerData.shermaCitadelEntrance_Seen || playerData.citadelHalfwayComplete) && !playerData.shermaCitadelEntrance_Left)
		{
			playerData.shermaCitadelEntrance_Left = true;
		}
		if (playerData.shermaCitadelSpa_Seen && !playerData.shermaCitadelSpa_Left)
		{
			playerData.shermaCitadelSpa_Left = true;
			playerData.shermaCitadelEntrance_Left = true;
		}
		if (playerData.enclaveLevel >= 2 && sceneNameString != "Song_Enclave" && sceneNameString != "Bellshrine_Enclave")
		{
			playerData.shermaInEnclave = true;
		}
		if (!playerData.shermaHealerActive && QuestManager.GetQuest("Save Sherma").IsCompleted && sceneNameString != "Song_Enclave" && sceneNameString != "Bellshrine_Enclave")
		{
			playerData.shermaHealerActive = true;
		}
		if (playerData.shermaHealerActive && sceneNameString != "Song_Enclave" && sceneNameString != "Bellshrine_Enclave")
		{
			playerData.shermaWoundedPilgrim = UnityEngine.Random.Range(1, 4);
		}
		if (sceneNameString != "Bonetown" && sceneNameString != "Belltown")
		{
			playerData.mapperAway = UnityEngine.Random.Range(1, 100) > 50;
		}
		if (playerData.killedRoostingCrowman)
		{
			playerData.killedRoostingCrowman = false;
		}
		if (playerData.spinnerDefeated)
		{
			playerData.MapperAppearInBellhart = true;
		}
		if (playerData.defeatedCoralDrillers)
		{
			playerData.coralDrillerSoloReady = true;
		}
		if (playerData.dust01_battleCompleted)
		{
			playerData.dust01_returnReady = true;
		}
		if (sceneNameString != "Bonetown" && playerData.visitedBellhart)
		{
			playerData.BonebottomBellwayPilgrimLeft = true;
		}
		if (playerData.BoneBottomShopKeepWillLeave && !playerData.BoneBottomShopKeepLeft && sceneNameString != "Bonetown")
		{
			playerData.BoneBottomShopKeepLeft = true;
		}
		if (currentMapZoneEnum != MapZone.BONETOWN && currentMapZoneEnum != MapZone.PATH_OF_BONE && currentMapZoneEnum != MapZone.MOSS_CAVE)
		{
			if (playerData.seenPilbyLeft && !playerData.bonetownPilgrimRoundActive)
			{
				playerData.bonetownPilgrimRoundActive = true;
			}
			if (playerData.seenPebbLeft && !playerData.bonetownPilgrimHornedActive)
			{
				playerData.bonetownPilgrimHornedActive = true;
			}
		}
		if (playerData.hasChargeSlash && sceneNameString != "Room_Pinstress")
		{
			playerData.pinstressStoppedResting = true;
			if (UnityEngine.Random.Range(0, 100) >= 50)
			{
				playerData.pinstressInsideSitting = true;
			}
			else
			{
				playerData.pinstressInsideSitting = false;
			}
			if (playerData.blackThreadWorld)
			{
				playerData.pinstressQuestReady = true;
			}
		}
		if (!playerData.IsPinGallerySetup && playerData.spinnerDefeated && sceneNameString != "Bone_12")
		{
			playerData.IsPinGallerySetup = true;
		}
		if (!playerData.PinGalleryLastChallengeOpen && playerData.pinGalleriesCompleted == 2 && playerData.visitedCitadel && sceneNameString != "Bone_12")
		{
			playerData.PinGalleryLastChallengeOpen = true;
		}
		if (playerData.bone03_openedTrapdoor && !QuestManager.GetQuest("Rock Rollers").IsAccepted)
		{
			playerData.bone03_openedTrapdoorForRockRoller = true;
		}
		if (playerData.rhinoChurchUnlocked)
		{
			playerData.rhinoRampageCompleted = true;
		}
		if (playerData.rhinoChurchUnlocked && !playerData.churchRhinoKilled && playerData.visitedCitadel && playerData.PilgrimsRestDoorBroken && currentMapZoneEnum != MapZone.WILDS && playerData.respawnScene != "Bone_East_10_Room")
		{
			playerData.rhinoRuckus = true;
			playerData.didRhinoRuckus = true;
		}
		if (sceneNameString != "Bone_East_10_Room")
		{
			playerData.pilgrimRestCrowd = UnityEngine.Random.Range(1, 6);
		}
		if (sceneNameString != "Halfway_01" && playerData.MetHalfwayHunterFan)
		{
			if (UnityEngine.Random.Range(1, 100) > 50)
			{
				playerData.nuuIsHome = true;
			}
			else
			{
				playerData.nuuIsHome = false;
			}
		}
		if (playerData.MetHalfwayHunterFan && playerData.defeatedSplinterQueen && currentMapZoneEnum != MapZone.SHELLWOOD_THICKET && !playerData.nuuVisiting_splinterQueen)
		{
			playerData.nuuVisiting_splinterQueen = true;
		}
		if (playerData.MetHalfwayHunterFan && playerData.defeatedCoralDrillers && currentMapZoneEnum != MapZone.JUDGE_STEPS && !playerData.nuuVisiting_coralDrillers)
		{
			playerData.nuuVisiting_coralDrillers = true;
		}
		if (playerData.MetHalfwayHunterFan && playerData.skullKingDefeated && currentMapZoneEnum != MapZone.PATH_OF_BONE && !playerData.nuuVisiting_skullKing)
		{
			playerData.nuuVisiting_skullKing = true;
		}
		if (playerData.MetHalfwayHunterFan && playerData.defeatedZapCoreEnemy && currentMapZoneEnum != MapZone.RED_CORAL_GORGE && !playerData.nuuVisiting_zapNest)
		{
			playerData.nuuVisiting_zapNest = true;
		}
		if (sceneNameString != "Halfway_01")
		{
			playerData.halfwayCrowd = UnityEngine.Random.Range(1, 5);
			if (playerData.MetHalfwayBartender)
			{
				playerData.HalfwayPatronsCanVisit = true;
			}
			if (playerData.SeenHalfwayPatronLeft)
			{
				playerData.HalfwayPatronLeftGone = true;
			}
			if (playerData.SeenHalfwayPatronRight)
			{
				playerData.HalfwayPatronRightGone = true;
			}
		}
		if (!playerData.greymoor_05_centipedeArrives && (playerData.greymoor_04_battleCompleted || playerData.greymoor_10_entered))
		{
			playerData.greymoor_05_centipedeArrives = true;
		}
		if (playerData.garmondMoorwingConvo)
		{
			playerData.garmondMoorwingConvoReady = true;
		}
		if (playerData.metGarmond && playerData.vampireGnatDeaths > 1)
		{
			playerData.garmondMoorwingConvoReady = true;
		}
		if (playerData.openedDust05Gate)
		{
			playerData.garmondInDust05 = true;
		}
		if (playerData.dust05EnemyClearedOut)
		{
			playerData.dust05EnemyClearedOut = true;
		}
		if (playerData.bellShrineEnclave || playerData.defeatedCogworkDancers || playerData.hasHarpoonDash)
		{
			if (!playerData.garmondInSong01 && !playerData.garmondSeenInSong01 && playerData.enteredSong_01)
			{
				playerData.garmondInSong01 = true;
			}
			if (!playerData.garmondInSong02 && !playerData.garmondSeenInSong02 && playerData.enteredSong_02)
			{
				playerData.garmondInSong02 = true;
			}
			if (!playerData.garmondInSong13 && !playerData.garmondSeenInSong13 && playerData.enteredSong_13)
			{
				playerData.garmondInSong13 = true;
			}
			if (!playerData.garmondInSong17 && !playerData.garmondSeenInSong17 && playerData.enteredSong_17 && playerData.marionettesMet)
			{
				playerData.garmondInSong17 = true;
			}
		}
		if (!playerData.garmondInLibrary && playerData.libraryRoofShortcut && playerData.metGarmond)
		{
			playerData.garmondInLibrary = true;
		}
		if (playerData.HasMelodyArchitect && playerData.HasMelodyConductor && playerData.HasMelodyLibrarian && QuestManager.GetQuest("Fine Pins").IsCompleted && (playerData.metGarmond || (playerData.garmondSeenInSong13 && playerData.garmondSeenInSong17)))
		{
			playerData.garmondInEnclave = true;
		}
		if ((playerData.garmondEncounters_act3 >= 3 || playerData.HasWhiteFlower) && !playerData.garmondFinalQuestReady)
		{
			playerData.garmondFinalQuestReady = true;
		}
		playerData.pilgrimGroupBonegrave = UnityEngine.Random.Range(1, 4);
		if (playerData.savedPlinney)
		{
			playerData.shellGravePopulated = true;
		}
		playerData.pilgrimGroupShellgrave = UnityEngine.Random.Range(1, 4);
		playerData.pilgrimGroupGreymoorField = UnityEngine.Random.Range(1, 4);
		playerData.enemyGroupAnt04 = UnityEngine.Random.Range(1, 4);
		float num = UnityEngine.Random.Range(1, 100);
		if (num <= 60f)
		{
			playerData.halfwayCrowEnemyGroup = 2;
		}
		else if (num <= 90f)
		{
			playerData.halfwayCrowEnemyGroup = 1;
		}
		else
		{
			playerData.halfwayCrowEnemyGroup = 3;
		}
		if (playerData.bonegraveRosaryPilgrimDefeated)
		{
			playerData.bonegravePilgrimCrowdsCanReturn = true;
		}
		if (playerData.wokeSongChevalier && !playerData.songChevalierActiveInSong_25)
		{
			playerData.songChevalierActiveInSong_25 = true;
		}
		if (playerData.wokeSongChevalier && !playerData.songChevalierActiveInSong_27)
		{
			playerData.songChevalierActiveInSong_27 = true;
		}
		if (playerData.wokeSongChevalier && !playerData.songChevalierActiveInSong_04 && playerData.song_04_battleCompleted)
		{
			playerData.songChevalierActiveInSong_04 = true;
		}
		if (playerData.blackThreadWorld && playerData.wokeSongChevalier)
		{
			if (!playerData.songChevalierActiveInSong_02)
			{
				playerData.songChevalierActiveInSong_02 = true;
			}
			if (!playerData.songChevalierActiveInSong_24)
			{
				playerData.songChevalierActiveInSong_24 = true;
			}
			if (!playerData.songChevalierActiveInHang_02)
			{
				playerData.songChevalierActiveInHang_02 = true;
			}
			if (!playerData.songChevalierActiveInSong_07 && QuestManager.GetQuest("Save City Merchant").IsCompleted)
			{
				playerData.songChevalierActiveInSong_07 = true;
			}
		}
		if (playerData.songChevalierEncounterCooldown)
		{
			playerData.songChevalierEncounterCooldown = false;
		}
		if (playerData.songChevalierEncounters >= 2 && playerData.HasMelodyConductor && playerData.enclaveLevel >= 2 && !playerData.songChevalierQuestReady && !QuestManager.GetQuest("Song Knight").IsCompleted)
		{
			playerData.songChevalierQuestReady = true;
		}
		if (playerData.songChevalierEncounters >= 1 && playerData.HasWhiteFlower && !playerData.songChevalierQuestReady && !QuestManager.GetQuest("Song Knight").IsCompleted)
		{
			playerData.songChevalierQuestReady = true;
		}
		if (playerData.blackThreadWorld && QuestManager.GetQuest("Song Knight").IsCompleted)
		{
			playerData.songChevalierActiveInSong_25 = false;
			playerData.songChevalierSeenInSong_25 = false;
			playerData.songChevalierActiveInSong_27 = false;
			playerData.songChevalierSeenInSong_27 = false;
			playerData.songChevalierActiveInSong_02 = false;
			playerData.songChevalierSeenInSong_02 = false;
			playerData.songChevalierActiveInSong_24 = false;
			playerData.songChevalierSeenInSong_24 = false;
			playerData.songChevalierActiveInSong_07 = false;
			playerData.songChevalierSeenInSong_07 = false;
			playerData.songChevalierActiveInHang_02 = false;
			playerData.songChevalierSeenInHang_02 = false;
			switch (UnityEngine.Random.Range(1, 7))
			{
			case 1:
				playerData.songChevalierActiveInSong_25 = true;
				break;
			case 2:
				playerData.songChevalierActiveInSong_27 = true;
				break;
			case 3:
				playerData.songChevalierActiveInSong_02 = true;
				break;
			case 4:
				playerData.songChevalierActiveInSong_24 = true;
				break;
			case 5:
				if (QuestManager.GetQuest("Save City Merchant").IsCompleted)
				{
					playerData.songChevalierActiveInSong_07 = true;
				}
				else
				{
					playerData.songChevalierActiveInSong_25 = true;
				}
				break;
			case 6:
				playerData.songChevalierActiveInHang_02 = true;
				break;
			}
		}
		if (!playerData.enclaveNPC_songKnightFan && playerData.blackThreadWorld && playerData.wokeSongChevalier && playerData.hasSuperJump && currentMapZoneEnum != MapZone.CITY_OF_SONG)
		{
			playerData.enclaveNPC_songKnightFan = true;
		}
		if (playerData.boneEastJailerKilled || playerData.CurseKilledFlyBoneEast)
		{
			playerData.boneEastJailerClearedOut = true;
		}
		if (playerData.enteredGreymoor05)
		{
			playerData.previouslyVisitedGreymoor_05 = true;
		}
		if (playerData.greymoor05_clearedOut)
		{
			playerData.greymoor05_clearedOut = false;
		}
		if (playerData.seenEmptyShellwood16)
		{
			playerData.slabFlyInShellwood16 = true;
		}
		if (playerData.completedLibraryEntryBattle)
		{
			playerData.scholarAmbushReady = true;
		}
		if (playerData.ant04_battleCompleted)
		{
			playerData.ant04_enemiesReturn = true;
		}
		if (playerData.dicePilgrimDefeated && playerData.dicePilgrimState == 0 && (playerData.defeatedCogworkDancers || playerData.hasHarpoonDash) && currentMapZoneEnum != MapZone.JUDGE_STEPS)
		{
			playerData.dicePilgrimState = 2;
		}
		else if (playerData.dicePilgrimState == 0 && (playerData.defeatedCogworkDancers || playerData.hasHarpoonDash) && currentMapZoneEnum != MapZone.JUDGE_STEPS)
		{
			playerData.dicePilgrimState = 1;
		}
		if (playerData.coral19_clearedOut)
		{
			playerData.coral19_clearedOut = false;
		}
		if (playerData.defeatedCoralDrillerSolo && currentMapZoneEnum != MapZone.RED_CORAL_GORGE && currentMapZoneEnum != MapZone.JUDGE_STEPS)
		{
			playerData.coralDrillerSoloEnemiesReturned = true;
		}
		if (playerData.defeatedWispPyreEffigy)
		{
			playerData.wisp02_enemiesReturned = true;
		}
		if (!playerData.HalfwayScarecrawAppeared && sceneNameString != "Halfway_01" && QuestManager.GetQuest("Crow Feathers").IsCompleted)
		{
			playerData.HalfwayScarecrawAppeared = true;
		}
		if (playerData.metGrubFarmer && !playerData.farmer_grewFirstGrub && sceneNameString != "Dust_11")
		{
			playerData.farmer_grubGrown_1 = true;
			playerData.farmer_grewFirstGrub = true;
		}
		if (playerData.grubFarmLevel >= 1 && playerData.farmer_grewFirstGrub && (!playerData.blackThreadWorld || playerData.silkFarmAbyssCoresCleared))
		{
			if (timeSinceLastTimePasses > 0f)
			{
				playerData.grubFarmerTimer += timeSinceLastTimePasses;
			}
			while (playerData.grubFarmerTimer >= 1800f)
			{
				playerData.grubFarmerTimer -= 1800f;
				if (!playerData.farmer_grubGrowing_1 && !playerData.farmer_grubGrown_1)
				{
					playerData.farmer_grubGrowing_1 = true;
					continue;
				}
				if (!playerData.farmer_grubGrown_1)
				{
					playerData.farmer_grubGrown_1 = true;
					playerData.farmer_grubGrowing_1 = false;
					continue;
				}
				if (playerData.grubFarmLevel >= 2 && playerData.farmer_grubGrown_1)
				{
					if (!playerData.farmer_grubGrowing_2 && !playerData.farmer_grubGrown_2)
					{
						playerData.farmer_grubGrowing_2 = true;
						continue;
					}
					if (!playerData.farmer_grubGrown_2)
					{
						playerData.farmer_grubGrown_2 = true;
						playerData.farmer_grubGrowing_2 = false;
						continue;
					}
				}
				if (playerData.grubFarmLevel >= 3 && playerData.farmer_grubGrown_2)
				{
					if (!playerData.farmer_grubGrowing_3 && !playerData.farmer_grubGrown_3)
					{
						playerData.farmer_grubGrowing_3 = true;
					}
					else if (!playerData.farmer_grubGrown_3)
					{
						playerData.farmer_grubGrown_3 = true;
						playerData.farmer_grubGrowing_3 = false;
					}
				}
			}
		}
		if (playerData.hitCrowCourtSwitch && !playerData.CrowCourtInSession && !playerData.defeatedCrowCourt && currentMapZoneEnum != MapZone.GREYMOOR && playerData.blackThreadWorld)
		{
			playerData.CrowCourtInSession = true;
		}
		if (playerData.defeatedCrowCourt && playerData.CrowCourtInSession && currentMapZoneEnum != MapZone.GREYMOOR)
		{
			playerData.CrowCourtInSession = false;
		}
		if (playerData.CrawbellInstalled)
		{
			if (timeSinceLastTimePasses > 0f)
			{
				playerData.CrawbellTimer += timeSinceLastTimePasses;
			}
			switch (sceneNameString)
			{
			default:
			{
				playerData.CrawbellCrawsInside = UnityEngine.Random.Range(0, 2) == 0;
				ArrayForEnumAttribute.EnsureArraySize(ref playerData.CrawbellCurrency, typeof(CurrencyType));
				ArrayForEnumAttribute.EnsureArraySize(ref playerData.CrawbellCurrencyCaps, typeof(CurrencyType));
				for (int i = 0; i < playerData.CrawbellCurrencyCaps.Length; i++)
				{
					if (playerData.CrawbellCurrencyCaps[i] <= 0)
					{
						playerData.CrawbellCurrencyCaps[i] = UnityEngine.Random.Range(300, 500);
					}
				}
				while (playerData.CrawbellTimer >= 300f)
				{
					playerData.CrawbellTimer -= 300f;
					for (int j = 0; j < playerData.CrawbellCurrency.Length; j++)
					{
						int num2 = playerData.CrawbellCurrency[j];
						if (num2 < playerData.CrawbellCurrencyCaps[j])
						{
							playerData.CrawbellCurrency[j] = num2 + UnityEngine.Random.Range(5, 20);
						}
					}
				}
				break;
			}
			case "Belltown":
			case "Belltown_Room_Spare":
			case "Belltown_basement":
			case "Belltown_Room_pinsmith":
			case "Belltown_Room_Relic":
				break;
			}
		}
		if (playerData.Collectables.GetData("Growstone").Amount > 0)
		{
			if (timeSinceLastTimePasses > 0f)
			{
				playerData.GrowstoneTimer += timeSinceLastTimePasses;
			}
			while (playerData.GrowstoneTimer >= 1200f)
			{
				if (playerData.GrowstoneState >= 3)
				{
					playerData.GrowstoneTimer = 0f;
					break;
				}
				playerData.GrowstoneTimer -= 1200f;
				playerData.GrowstoneState++;
			}
		}
		if (playerData.blackThreadWorld && !playerData.HuntressRuntAppeared && !Gameplay.HuntressQuest.IsCompleted)
		{
			playerData.HuntressRuntAppeared = true;
		}
		if (playerData.defeatedSplinterQueen && !playerData.splinterQueenSproutCut && currentMapZoneEnum != MapZone.SHELLWOOD_THICKET && playerData.splinterQueenSproutTimer < 50)
		{
			playerData.splinterQueenSproutTimer++;
		}
		if (playerData.roofCrabDefeated && playerData.citadelWoken && currentMapZoneEnum != MapZone.CRAWLSPACE && currentMapZoneEnum != MapZone.BONETOWN && currentMapZoneEnum != MapZone.PATH_OF_BONE && currentMapZoneEnum != MapZone.MOSSTOWN)
		{
			playerData.littleCrabsAppeared = true;
		}
		if ((playerData.BonebottomBellwayPilgrimScared || playerData.skullKingInvaded) && !playerData.BonebottomBellwayPilgrimLeft && (!playerData.skullKingInvaded || playerData.skullKingKilled) && currentMapZoneEnum != MapZone.BONETOWN && currentMapZoneEnum != MapZone.PATH_OF_BONE)
		{
			playerData.BonebottomBellwayPilgrimLeft = true;
		}
		if (playerData.spinnerDefeated)
		{
			playerData.BonebottomBellwayPilgrimLeft = true;
		}
		if (playerData.skullKingDefeated && !playerData.skullKingWillInvade && currentMapZoneEnum != MapZone.BONETOWN && currentMapZoneEnum != MapZone.PATH_OF_BONE && currentMapZoneEnum != MapZone.MOSS_CAVE && !playerData.blackThreadWorld && (playerData.visitedCitadel || playerData.visitedCoral || playerData.visitedDustpens) && UnityEngine.Random.Range(1, 100) <= 20)
		{
			playerData.skullKingWillInvade = true;
		}
		if (playerData.skullKingKilled && !playerData.skullKingBenchMended)
		{
			playerData.skullKingBenchMended = true;
		}
		if (playerData.skullKingBenchMended && playerData.pilbyKilled && !playerData.boneBottomFuneral && !playerData.blackThreadWorld && currentMapZoneEnum != MapZone.BONETOWN && currentMapZoneEnum != MapZone.PATH_OF_BONE && currentMapZoneEnum != MapZone.MOSS_CAVE)
		{
			playerData.boneBottomFuneral = true;
			playerData.skullKingPlatMended = true;
		}
		if ((playerData.UnlockedMelodyLift || playerData.hasDoubleJump) && !playerData.pilbyKilled && playerData.pilbyMeetConvo && !playerData.pilbyAtPilgrimsRest && !playerData.rhinoRuckus)
		{
			playerData.pilbyAtPilgrimsRest = true;
			if (playerData.PilgrimsRestDoorBroken)
			{
				playerData.pilbyInsidePilgrimsRest = true;
			}
		}
		if (playerData.pilbySeenAtPilgrimsRest && !playerData.pilbyLeftPilgrimsRest && currentMapZoneEnum != MapZone.WILDS)
		{
			playerData.pilbyLeftPilgrimsRest = true;
		}
		if (currentMapZoneEnum != MapZone.BONETOWN && currentMapZoneEnum != MapZone.PATH_OF_BONE && currentMapZoneEnum != MapZone.MOSS_CAVE && playerData.visitedBoneForest)
		{
			playerData.bonetownCrowd = UnityEngine.Random.Range(1, 8);
		}
		if (sceneNameString != "Bellway_01" && sceneNameString != "Bonetown")
		{
			if (QuestManager.GetQuest("Building Materials (Statue)").IsCompleted)
			{
				playerData.fixerStatueConstructed = true;
			}
			if (QuestManager.GetQuest("Building Materials (Bridge)").IsCompleted)
			{
				playerData.fixerBridgeConstructed = true;
			}
			if (QuestManager.GetQuest("Pilgrim Rags").IsCompleted)
			{
				playerData.boneBottomAddition_RagLine = true;
			}
		}
		if (!playerData.ChurchKeeperLeftBasement && sceneName != "Tut_03" && sceneName != "Bonetown" && sceneData.PersistentInts.TryGetValue("Tut_03", "Churchkeeper Basement", out var value) && value.Value > 0)
		{
			playerData.ChurchKeeperLeftBasement = true;
		}
		if (playerData.CaravanLechSaved && !playerData.CaravanLechReturnedToCaravan && sceneName != "Bone_10" && sceneName != "Greymoor_08" && sceneName != "Coral_Judge_Arena" && sceneName != "Aqueduct_05")
		{
			playerData.CaravanLechReturnedToCaravan = true;
		}
		if ((playerData.encounteredVampireGnat_05 || playerData.encounteredVampireGnatBoss) && currentMapZoneEnum != MapZone.GREYMOOR)
		{
			playerData.allowVampireGnatInAltLoc = true;
		}
		if (!playerData.VampireGnatCorpseOnCaravan && playerData.VampireGnatDefeatedBeforeCaravanArrived && !playerData.VampireGnatCorpseInWater && playerData.CaravanTroupeLocation > CaravanTroupeLocations.Bone && sceneName != "Greymoor_08")
		{
			playerData.VampireGnatCorpseOnCaravan = true;
		}
		if (playerData.CaravanTroupeLocation > CaravanTroupeLocations.Bone && !playerData.creaturesReturnedToBone10 && playerData.respawnScene != "Bone_10")
		{
			playerData.creaturesReturnedToBone10 = true;
		}
		switch (playerData.CaravanTroupeLocation)
		{
		case CaravanTroupeLocations.Greymoor:
			if (playerData.MetCaravanTroupeLeaderGreymoor && !playerData.CaravanTroupeLeaderCanLeaveGreymoor && sceneName != "Greymoor_08")
			{
				playerData.CaravanTroupeLeaderCanLeaveGreymoor = true;
			}
			break;
		case CaravanTroupeLocations.CoralJudge:
			if (playerData.MetCaravanTroupeLeaderJudge && !playerData.CaravanTroupeLeaderCanLeaveJudge && sceneName != "Coral_Judge_Arena")
			{
				playerData.CaravanTroupeLeaderCanLeaveJudge = true;
			}
			break;
		}
		if (!playerData.SpinnerDefeatedTimePassed && sceneName != "Belltown" && sceneName != "Belltown_Boss")
		{
			playerData.SpinnerDefeatedTimePassed = true;
		}
		if (!playerData.antMerchantKilled && currentMapZoneEnum != MapZone.HUNTERS_NEST && (playerData.defeatedCogworkDancers || playerData.bellShrineEnclave || playerData.hasHarpoonDash))
		{
			playerData.antMerchantKilled = true;
			if (playerData.ant21_InitBattleCompleted && !Gameplay.CurveclawTool.IsUnlocked && !Gameplay.CurveclawUpgradedTool.IsUnlocked)
			{
				playerData.ant21_ExtraBattleAdded = true;
			}
		}
		if (playerData.MottledChildGivenTool && !playerData.MottledChildNewTool)
		{
			playerData.MottledChildNewTool = true;
		}
		if (playerData.bellShrineBellhart && currentMapZoneEnum != MapZone.BELLTOWN && currentMapZoneEnum != MapZone.MEMORY)
		{
			playerData.belltownCrowdsReady = true;
			playerData.belltownCrowd = UnityEngine.Random.Range(1, 7);
		}
		if (playerData.visitedBellhartSaved && currentMapZoneEnum != MapZone.BELLTOWN)
		{
			playerData.shermaInBellhart = true;
		}
		if (currentMapZoneEnum != MapZone.BELLTOWN)
		{
			switch (playerData.BelltownHouseState)
			{
			case BelltownHouseStates.None:
				if (QuestManager.GetQuest("Belltown House Start").IsCompleted)
				{
					playerData.BelltownHouseState = BelltownHouseStates.Half;
				}
				break;
			case BelltownHouseStates.Half:
				if (QuestManager.GetQuest("Belltown House Mid").IsCompleted)
				{
					playerData.BelltownHouseState = BelltownHouseStates.Full;
				}
				break;
			}
			if (playerData.BelltownGreeterConvo > 0)
			{
				playerData.BelltownGreeterMetTimePassed = true;
			}
		}
		if (playerData.gotPastDockSpearThrower)
		{
			playerData.gotPastDockSpearThrower = false;
		}
		if (playerData.wardBossDefeated && currentMapZoneEnum != MapZone.WARD && currentMapZoneEnum != MapZone.MEMORY && playerData.respawnScene != "Ward_02")
		{
			playerData.wardWoken = true;
		}
		if (sceneData.PersistentBools.TryGetValue("Dock_10", "dock_pressure_plate_lock", out var value2) && value2.Value && currentMapZoneEnum != MapZone.DOCKS && playerData.respawnScene != "Dock_10" && !playerData.blackThreadWorld && !playerData.BallowInSauna)
		{
			playerData.BallowInSauna = true;
		}
		if (playerData.BallowSeenInSauna && !playerData.BallowLeftSauna && sceneNameString != "Dock_10")
		{
			playerData.BallowLeftSauna = true;
		}
		if (playerData.defeatedRoachkeeperChef && currentMapZoneEnum != MapZone.DUSTPENS)
		{
			playerData.roachkeeperChefCorpsePrepared = true;
		}
		if (sceneNameString != "Song_Enclave" && sceneNameString != "Bellshrine_Enclave")
		{
			switch (playerData.enclaveLevel)
			{
			case 0:
				if (playerData.metCaretaker)
				{
					playerData.enclaveLevel = 1;
				}
				break;
			case 1:
				if (QuestManager.GetQuest("Songclave Donation 1").IsCompleted && Gameplay.EnclaveQuestBoard.CompletedQuestCount >= 2)
				{
					playerData.enclaveLevel = 2;
				}
				break;
			case 2:
				if (QuestManager.GetQuest("Songclave Donation 2").IsCompleted && Gameplay.EnclaveQuestBoard.CompletedQuestCount >= 5)
				{
					playerData.enclaveLevel = 3;
				}
				break;
			}
			if (QuestManager.GetQuest("Fine Pins").IsCompleted)
			{
				playerData.enclaveAddition_PinRack = true;
			}
			if (QuestManager.GetQuest("Song Pilgrim Cloaks").IsCompleted)
			{
				playerData.enclaveAddition_CloakLine = true;
			}
			if (Gameplay.EnclaveQuestBoard.CompletedQuestCount >= 4)
			{
				playerData.enclaveDonation2_Available = true;
			}
		}
		if (!playerData.citadelHalfwayComplete && (playerData.defeatedCogworkDancers || playerData.hasHarpoonDash || playerData.visitedEnclave))
		{
			playerData.citadelHalfwayComplete = true;
		}
		if (playerData.citadelWoken && currentMapZoneEnum != MapZone.WARD && currentMapZoneEnum != MapZone.CITY_OF_SONG)
		{
			playerData.song05MarchGroupReady = true;
		}
		if (playerData.under07_battleCompleted && playerData.hasHarpoonDash && currentMapZoneEnum != MapZone.UNDERSTORE && currentMapZoneEnum != MapZone.CITY_OF_SONG)
		{
			playerData.under07_heavyWorkerReturned = true;
		}
		if (playerData.grindleInSong_08)
		{
			playerData.grindleInSong_08 = false;
		}
		if (playerData.seenGrindleInSong_08 && currentMapZoneEnum != MapZone.CITY_OF_SONG)
		{
			playerData.savedGrindleInCitadel = true;
			playerData.grindleInSong_08 = false;
		}
		if (playerData.savedGrindleInCitadel && playerData.grindleChestLocation == 0)
		{
			playerData.grindleChestLocation = UnityEngine.Random.Range(1, 4);
		}
		if (playerData.hang04Battle)
		{
			playerData.leftTheGrandForum = true;
		}
		if (playerData.song_17_clearedOut)
		{
			playerData.song_17_clearedOut = false;
		}
		if (sceneData.PersistentBools.TryGetValue("Under_07c", "Bot Blocker", out var value3) && value3.Value && currentMapZoneEnum != MapZone.UNDERSTORE && currentMapZoneEnum != MapZone.CITY_OF_SONG && currentMapZoneEnum != MapZone.COG_CORE)
		{
			playerData.rosaryThievesInUnder07 = true;
		}
		if (playerData.bankOpened && currentMapZoneEnum != MapZone.HANG && currentMapZoneEnum != MapZone.CITY_OF_SONG && currentMapZoneEnum != MapZone.COG_CORE)
		{
			playerData.rosaryThievesInBank = true;
		}
		if (playerData.cog7_automaton_defeated)
		{
			playerData.cog7_automatonRepairing = true;
		}
		if (playerData.cog7_automatonRepairingComplete)
		{
			playerData.cog7_automaton_defeated = false;
			playerData.cog7_automatonRepairing = false;
			playerData.cog7_automatonRepairingComplete = false;
		}
		if (!playerData.cityMerchantCanLeaveForBridge && playerData.enclaveLevel > 2 && playerData.hasDoubleJump && playerData.MetCityMerchantEnclave)
		{
			playerData.cityMerchantCanLeaveForBridge = true;
		}
		if (playerData.MetCityMerchantEnclave)
		{
			playerData.cityMerchantInLibrary03 = true;
		}
		if (playerData.cityMerchantInGrandForumSeen)
		{
			playerData.cityMerchantInGrandForumLeft = true;
		}
		if (playerData.cityMerchantInLibrary03Seen)
		{
			playerData.cityMerchantInLibrary03Left = true;
		}
		if (playerData.cityMerchantRecentlySeenInEnclave && currentMapZoneEnum != MapZone.LIBRARY && currentMapZoneEnum != MapZone.CITY_OF_SONG && currentMapZoneEnum != MapZone.HANG)
		{
			playerData.cityMerchantRecentlySeenInEnclave = false;
		}
		if (playerData.ArchitectWillLeave && sceneName != "Under_17" && !playerData.ArchitectLeft)
		{
			playerData.ArchitectLeft = true;
		}
		if (!playerData.trobbioCleanedUp && playerData.defeatedTrobbio && currentMapZoneEnum != MapZone.LIBRARY && currentMapZoneEnum != MapZone.CITY_OF_SONG && currentMapZoneEnum != MapZone.UNDERSTORE)
		{
			playerData.trobbioCleanedUp = true;
		}
		if (playerData.visitedCradle && currentMapZoneEnum != MapZone.CITY_OF_SONG && currentMapZoneEnum != MapZone.MEMORY && currentMapZoneEnum != MapZone.CRADLE)
		{
			playerData.laceCorpseAddedEffects = true;
		}
		if (QuestManager.GetQuest("Broodmother Hunt").IsCompleted && !playerData.tinyBroodMotherAppeared && currentMapZoneEnum != MapZone.THE_SLAB)
		{
			playerData.tinyBroodMotherAppeared = true;
		}
		if (!playerData.BlueScientistDead && currentMapZoneEnum != MapZone.CRAWLSPACE && QuestManager.GetQuest("Extractor Blue").IsCompleted && QuestManager.GetQuest("Extractor Blue Worms").IsCompleted)
		{
			playerData.BlueScientistDead = true;
		}
		if (!playerData.BlueScientistSceneryPustulesGrown && currentMapZoneEnum != MapZone.CRAWLSPACE && QuestManager.GetQuest("Extractor Blue").IsCompleted)
		{
			playerData.BlueScientistSceneryPustulesGrown = true;
		}
		if (playerData.UnlockedDustCage)
		{
			if (playerData.GreenPrinceLocation == GreenPrinceLocations.DustCage && playerData.song_04_battleCompleted)
			{
				playerData.GreenPrinceLocation = GreenPrinceLocations.Song04;
			}
			else if (playerData.GreenPrinceLocation < GreenPrinceLocations.CogDancers && playerData.defeatedCogworkDancers && playerData.bellShrineEnclave && (playerData.GreenPrinceSeenSong04 || playerData.blackThreadWorld))
			{
				playerData.GreenPrinceLocation = GreenPrinceLocations.CogDancers;
			}
		}
		if (!playerData.ShakraFinalQuestAppear && sceneNameString != "Belltown")
		{
			ShopItemList mapperStock = Gameplay.MapperStock;
			bool flag = false;
			ShopItem[] shopItems = mapperStock.ShopItems;
			foreach (ShopItem shopItem in shopItems)
			{
				if ((shopItem.GetTypeFlags() & ShopItem.TypeFlags.Map) != 0 && !shopItem.IsPurchased)
				{
					flag = true;
					break;
				}
			}
			int num3 = 0;
			if (playerData.HasMelodyArchitect)
			{
				num3++;
			}
			if (playerData.HasMelodyConductor)
			{
				num3++;
			}
			if (playerData.HasMelodyLibrarian)
			{
				num3++;
			}
			bool flag2 = false;
			if (num3 >= 2)
			{
				flag2 = true;
			}
			if (!flag && (playerData.DefeatedSwampShaman || flag2))
			{
				playerData.ShakraFinalQuestAppear = true;
				playerData.MapperLeaveAll();
			}
		}
		if (!playerData.swampMuckmanTallInvades && playerData.DefeatedSwampShaman && currentMapZoneEnum != MapZone.SWAMP && currentMapZoneEnum != MapZone.AQUEDUCT)
		{
			playerData.swampMuckmanTallInvades = true;
		}
		if (playerData.blackThreadWorld && playerData.seenMapperAct3 && currentMapZoneEnum != MapZone.BELLTOWN)
		{
			playerData.mapperLocationAct3 = UnityEngine.Random.Range(1, 6);
			if (playerData.mapperLocationAct3 == 4 || playerData.mapperLocationAct3 == 5)
			{
				playerData.mapperLocationAct3 = 2;
			}
			if (UnityEngine.Random.Range(1, 100) <= 50)
			{
				playerData.mapperIsFightingAct3 = true;
			}
			else
			{
				playerData.mapperIsFightingAct3 = false;
			}
			playerData.mapperFightGroup = UnityEngine.Random.Range(1, 3);
		}
		if (!playerData.thievesReturnedToShadow28 && playerData.SavedFlea_Shadow_28)
		{
			playerData.thievesReturnedToShadow28 = true;
		}
		if (!playerData.FleaGamesCanStart && playerData.blackThreadWorld && playerData.CaravanTroupeLocation == CaravanTroupeLocations.Aqueduct && Gameplay.FleaCharmTool.IsUnlocked && currentMapZoneEnum != MapZone.AQUEDUCT && playerData.respawnScene != "Aqueduct_05")
		{
			playerData.FleaGamesCanStart = true;
		}
		if (playerData.SethNpcLocation == SethNpcLocations.Absent && playerData.HasWhiteFlower)
		{
			playerData.SethNpcLocation = SethNpcLocations.Greymoor;
		}
		if (!playerData.SethJoinedFleatopia && playerData.FleaGamesEnded && (playerData.SethNpcLocation == SethNpcLocations.Fleatopia || (playerData.MetSethNPC & playerData.HasWhiteFlower)) && sceneNameString != "Aqueduct_05")
		{
			playerData.SethNpcLocation = SethNpcLocations.Fleatopia;
			playerData.SethJoinedFleatopia = true;
		}
		if (playerData.gillyQueueMovingOn && playerData.blackThreadWorld)
		{
			playerData.gillyLocationAct3++;
			playerData.gillyQueueMovingOn = false;
		}
		if (playerData.gillyLocationAct3 > 0 && playerData.Collectables.GetData("Hunter Heart").Amount > 0 && currentMapZoneEnum != MapZone.HUNTERS_NEST && currentMapZoneEnum != MapZone.WILDS)
		{
			playerData.gillyLocationAct3 = 3;
		}
		if (GetCurrentMapZone() != lastTimePassesMapZone)
		{
			TimePassesElsewhere();
		}
		lastTimePassesMapZone = GetCurrentMapZone();
		timeSinceLastTimePasses = 0f;
	}

	public void TimePassesElsewhere()
	{
		string sceneNameString = GetSceneNameString();
		if (playerData.muchTimePassed)
		{
			sceneData.MimicShuffle();
		}
		if (playerData.hang04Battle && playerData.MetCityMerchantEnclave)
		{
			playerData.cityMerchantInGrandForum = true;
		}
		if (playerData.scholarAcolytesReleased)
		{
			playerData.scholarAcolytesInLibrary_02 = true;
		}
		if (playerData.completedLavaChallenge)
		{
			playerData.lavaSpittersEmerge = true;
		}
		if (playerData.garmondEncounterCooldown)
		{
			playerData.garmondEncounterCooldown = false;
		}
		if (!(sceneNameString == "Belltown") && !(sceneNameString == "Belltown_Room_Spare"))
		{
			if (playerData.mapperBellhartConvo)
			{
				playerData.mapperBellhartConvoTimePassed = true;
			}
			if (!playerData.mapperSellingTubePins && playerData.mapperTubeConvo)
			{
				playerData.mapperSellingTubePins = true;
			}
			if (playerData.mapperConvo_Act3Intro)
			{
				playerData.mapperConvo_Act3IntroTimePassed = true;
			}
			if (!playerData.BelltownFurnishingSpaAvailable)
			{
				int num = 0;
				if (playerData.BelltownHouseColour != 0)
				{
					num++;
				}
				if (playerData.BelltownFurnishingDesk)
				{
					num++;
				}
				if (playerData.BelltownFurnishingFairyLights)
				{
					num++;
				}
				if (playerData.BelltownFurnishingGramaphone)
				{
					num++;
				}
				if (num >= 2)
				{
					playerData.BelltownFurnishingSpaAvailable = true;
				}
			}
			if (playerData.BelltownHouseColour != 0)
			{
				playerData.BelltownHousePaintComplete = true;
			}
		}
		if (playerData.BelltownHermitConvoCooldown)
		{
			playerData.BelltownHermitConvoCooldown = false;
		}
		if (playerData.HasMelodyConductor)
		{
			playerData.ConductorWeaverDlgQueued = true;
		}
		if (playerData.bonetownPilgrimHornedSeen && playerData.bonetownPilgrimHornedCount > 0)
		{
			playerData.bonetownPilgrimHornedCount--;
		}
		if (playerData.bonetownPilgrimRoundSeen && playerData.bonetownPilgrimRoundCount > 0)
		{
			playerData.bonetownPilgrimRoundCount--;
		}
		if (!playerData.ArchitectMelodyReturnSeen && playerData.ArchitectMentionedMelody)
		{
			playerData.ArchitectMelodyReturnQueued = true;
		}
		if (!playerData.MaskMakerTalkedUnmasked2 && playerData.MaskMakerTalkedUnmasked1 && !playerData.MaskMakerQueuedUnmasked2 && playerData.UnlockedMelodyLift)
		{
			playerData.MaskMakerQueuedUnmasked2 = true;
		}
		if (playerData.BonebottomBellwayPilgrimState == 1)
		{
			playerData.BonebottomBellwayPilgrimState = 2;
		}
		if (!(sceneNameString == "Song_Enclave"))
		{
			CheckReadyToLeave(ref playerData.EnclaveStatePilgrimSmall);
			CheckReadyToLeave(ref playerData.EnclaveStateNPCShortHorned);
			CheckReadyToLeave(ref playerData.EnclaveStateNPCTall);
			CheckReadyToLeave(ref playerData.EnclaveStateNPCStandard);
			CheckReadyToLeave(ref playerData.EnclaveState_songKnightFan);
		}
		if (playerData.metShermaEnclave && playerData.visitedWard && !playerData.shermaQuestActive)
		{
			playerData.shermaQuestActive = true;
		}
		if (playerData.gillyQueueMovingOn)
		{
			playerData.gillyLocation++;
			playerData.gillyQueueMovingOn = false;
		}
		if (!playerData.SprintMasterExtraRaceAvailable && QuestManager.GetQuest("Sprintmaster Race").IsCompleted && playerData.HasWhiteFlower && !(sceneNameString == "Sprintmaster_Cave"))
		{
			playerData.SprintMasterExtraRaceAvailable = true;
		}
		if (playerData.defeatedTormentedTrobbio)
		{
			playerData.tormentedTrobbioLurking = true;
		}
		if (!playerData.CrestPreUpgradeTalked && playerData.MetCrestUpgrader)
		{
			playerData.CrestPurposeQueued = true;
		}
		static void CheckReadyToLeave(ref NPCEncounterState state)
		{
			if (state == NPCEncounterState.ReadyToLeave)
			{
				state = NPCEncounterState.AuthorisedToLeave;
			}
		}
	}

	public void TimePassesLoadedIn()
	{
		string respawnScene = playerData.respawnScene;
		if ((respawnScene == "Belltown" || respawnScene == "Belltown_Room_Spare") && playerData.BelltownHouseColour != 0)
		{
			playerData.BelltownHousePaintComplete = true;
		}
	}

	public void MuchTimePasses()
	{
		if (!playerData.muchTimePassed)
		{
			playerData.muchTimePassed = true;
		}
	}

	public void StartBlackThreadWorld()
	{
		playerData.blackThreadWorld = true;
		playerData.act3_wokeUp = false;
	}

	private void StartAct3()
	{
		playerData.respawnScene = "Song_Tower_Destroyed";
		playerData.respawnMarkerName = "Death Respawn Marker Init";
		playerData.respawnType = 0;
		playerData.mapZone = MapZone.CRADLE;
		playerData.extraRestZone = ExtraRestZones.None;
		QuestManager.GetQuest("Silk Defeat Snare").TryEndQuest(null, consumeCurrency: false, forceEnd: true, showPrompt: false);
		playerData.bindCutscenePlayed = false;
		if (!playerData.churchRhinoKilled && !playerData.rhinoChurchUnlocked)
		{
			playerData.churchRhinoBlackThreadCorpse = true;
		}
		if (!Gameplay.WeightedAnkletTool.IsUnlocked)
		{
			playerData.mortKeptWeightedAnklet = true;
		}
		DeliveryQuestItem.BreakAllNoEffects();
		ToolCrest crest = (Gameplay.HunterCrest3.IsUnlocked ? Gameplay.HunterCrest3 : ((!Gameplay.HunterCrest2.IsUnlocked) ? Gameplay.HunterCrest : Gameplay.HunterCrest2));
		ToolItemManager.AutoEquip(crest, markTemp: false);
		playerData.ExtraToolEquips = new FloatingCrestSlotsData();
		ToolItemManager.AutoEquip(Gameplay.DefaultSkillTool);
		if (playerData.fixerBridgeConstructed)
		{
			playerData.fixerBridgeBroken = true;
		}
		playerData.scholarAcolytesReleased = true;
		playerData.spinnerDefeated = true;
		playerData.marionettesBurned = true;
		EnemyJournalRecord record = EnemyJournalManager.GetRecord("Song Handmaiden");
		EnemyJournalKillData.KillData killData = playerData.EnemyJournalKillData.GetKillData(record.name);
		if (killData.Kills < record.KillsRequired)
		{
			killData.Kills = record.KillsRequired;
			playerData.EnemyJournalKillData.RecordKillData(record.name, killData);
		}
		playerData.encounteredSongGolem = true;
		playerData.defeatedSongGolem = true;
		EnemyJournalRecord record2 = EnemyJournalManager.GetRecord("Song Golem");
		EnemyJournalKillData.KillData killData2 = playerData.EnemyJournalKillData.GetKillData(record2.name);
		if (killData2.Kills < record2.KillsRequired)
		{
			killData2.Kills = record2.KillsRequired;
			playerData.EnemyJournalKillData.RecordKillData(record2.name, killData2);
		}
		playerData.defeatedTrobbio = true;
		playerData.encounteredTrobbio = true;
		if (!playerData.PinsmithMetBelltown)
		{
			playerData.savedPlinney = true;
		}
		playerData.SeenLastJudgeGateOpen = true;
		QuestManager.GetQuest("Grand Gate Bellshrines").TryEndQuest(null, consumeCurrency: false, forceEnd: true, showPrompt: false);
		if (playerData.defeatedSplinterQueen && !playerData.splinterQueenSproutCut)
		{
			playerData.splinterQueenSproutGrewLarge = true;
		}
		playerData.garmondInEnclave = true;
		if (playerData.hitCrowCourtSwitch)
		{
			playerData.CrowCourtInSession = true;
		}
		RemoveIncompleteActiveQuest(playerData, "Pilgrim Rags", "Pilgrim Rag");
		RemoveIncompleteActiveQuest(playerData, "Skull King", "Skull King Fragment");
		RemoveIncompleteActiveQuest(playerData, "Roach Killing", "Roach Corpse Item");
		RemoveIncompleteActiveQuest(playerData, "Rock Rollers", "Rock Roller Item");
		playerData.grubFarmerTimer = 0f;
		playerData.farmer_grubGrowing_1 = false;
		playerData.farmer_grubGrown_1 = false;
		playerData.farmer_grubGrowing_2 = false;
		playerData.farmer_grubGrown_2 = false;
		playerData.farmer_grubGrowing_3 = false;
		playerData.farmer_grubGrown_3 = false;
		QuestManager.IncrementVersion();
		CollectableItemManager.IncrementVersion();
		if (!playerData.UnlockedEnclaveTube)
		{
			sceneData.PersistentBools.SetValue(new PersistentItemData<bool>
			{
				SceneName = "Song_Enclave_Tube",
				ID = "One Way Wall",
				Value = true
			});
		}
		playerData.mapperSellingTubePins = true;
	}

	private static void RemoveIncompleteActiveQuest(PlayerData playerData, string questName, string itemName)
	{
		QuestCompletionData.Completion data = playerData.QuestCompletionData.GetData(questName);
		if (data.IsAccepted && !data.IsCompleted)
		{
			playerData.QuestCompletionData.SetData(questName, default(QuestCompletionData.Completion));
		}
		CollectableItemsData.Data data2 = playerData.Collectables.GetData(itemName);
		if (data2.Amount > 0)
		{
			MateriumItemsData.Data data3 = playerData.MateriumCollected.GetData(itemName);
			if (!data3.IsCollected)
			{
				data3.IsCollected = true;
				playerData.MateriumCollected.SetData(itemName, data3);
			}
			data2.Amount = 0;
			playerData.Collectables.SetData(itemName, data2);
		}
	}

	public void SethTravelCheck()
	{
		if (playerData.SethNpcLocation == SethNpcLocations.Absent && playerData.defeatedFlowerQueen)
		{
			playerData.SethNpcLocation = SethNpcLocations.Greymoor;
		}
	}

	private void EnteredNewMapZone(MapZone previousMapZone, MapZone currentMapZone, bool forcedNotMemory)
	{
		string location = currentMapZone.ToString();
		if (playerData.hang04Battle)
		{
			playerData.leftTheGrandForum = true;
		}
		Platform.Current.UpdateLocation(location);
		Platform.Current.UpdatePlayTime(PlayTime);
		if (!forcedNotMemory && IsMemoryScene(currentMapZone))
		{
			if (!playerData.HasStoredMemoryState)
			{
				playerData.PreMemoryState = HeroItemsState.Record(hero_ctrl);
				playerData.HasStoredMemoryState = true;
				playerData.CaptureToolAmountsOverride();
				EventRegister.SendEvent("END FOLLOWERS INSTANT");
				hero_ctrl.MaxHealthKeepBlue();
			}
		}
		else if (playerData.HasStoredMemoryState && IsMemoryScene(previousMapZone))
		{
			playerData.PreMemoryState.Apply(hero_ctrl);
			playerData.HasStoredMemoryState = false;
			playerData.ClearToolAmountsOverride();
			EventRegister.SendEvent("END FOLLOWERS INSTANT");
		}
	}

	public void LoadedFromMenu()
	{
		if (playerData.vampireGnatRequestedAid)
		{
			playerData.vampireGnatRequestedAid = false;
		}
		if (playerData.garmondAidForumBattle)
		{
			playerData.garmondAidForumBattle = false;
		}
		if (playerData.shakraAidForumBattle)
		{
			playerData.shakraAidForumBattle = false;
		}
	}

	public void FixUpSaveState()
	{
	}

	public void FadeSceneIn()
	{
		if (IsWaitingForSceneReady)
		{
			shouldFadeInScene = true;
			return;
		}
		if (!BlockNextVibrationFadeIn)
		{
			VibrationManager.FadeVibration(1f, 0.25f);
		}
		else
		{
			BlockNextVibrationFadeIn = false;
		}
		cameraCtrl.FadeSceneIn();
		screenFader_fsm.SendEvent("SCENE FADE IN");
		shouldFadeInScene = false;
	}

	public IEnumerator FadeSceneInWithDelay(float delay)
	{
		if (delay >= 0f)
		{
			yield return new WaitForSeconds(delay);
		}
		else
		{
			yield return null;
		}
		FadeSceneIn();
	}

	public bool IsGamePaused()
	{
		if (GameState == GameState.PAUSED)
		{
			return true;
		}
		return false;
	}

	public void SetGameMap(GameObject go_gameMap)
	{
		gameMap = go_gameMap.GetComponent<GameMap>();
	}

	public void CalculateNotchesUsed()
	{
		playerData.CalculateNotchesUsed();
	}

	public string GetLanguageAsString()
	{
		return gameSettings.gameLanguage.ToString();
	}

	public string GetEntryGateName()
	{
		return entryGateName;
	}

	public void SetDeathRespawnSimple(string respawnMarkerName, int respawnType, bool respawnFacingRight)
	{
		playerData.respawnMarkerName = respawnMarkerName;
		playerData.respawnType = respawnType;
		playerData.respawnScene = sceneName;
		SetCurrentMapZoneAsRespawn();
	}

	public void SetNonlethalDeathRespawn(string respawnMarkerName, int respawnType, bool respawnFacingRight)
	{
		playerData.nonLethalRespawnMarker = respawnMarkerName;
		playerData.nonLethalRespawnType = respawnType;
		playerData.nonLethalRespawnScene = sceneName;
	}

	public void SetPlayerDataBool(string boolName, bool value)
	{
		playerData.SetBool(boolName, value);
	}

	public void SetPlayerDataInt(string intName, int value)
	{
		playerData.SetInt(intName, value);
	}

	public void SetPlayerDataFloat(string floatName, float value)
	{
		playerData.SetFloat(floatName, value);
	}

	public void SetPlayerDataString(string stringName, string value)
	{
		playerData.SetString(stringName, value);
	}

	public void IncrementPlayerDataInt(string intName)
	{
		playerData.IncrementInt(intName);
	}

	public void DecrementPlayerDataInt(string intName)
	{
		playerData.DecrementInt(intName);
	}

	public void IntAdd(string intName, int amount)
	{
		playerData.IntAdd(intName, amount);
	}

	public bool GetPlayerDataBool(string boolName)
	{
		return playerData.GetBool(boolName);
	}

	public int GetPlayerDataInt(string intName)
	{
		return playerData.GetInt(intName);
	}

	public float GetPlayerDataFloat(string floatName)
	{
		return playerData.GetFloat(floatName);
	}

	public string GetPlayerDataString(string stringName)
	{
		return playerData.GetString(stringName);
	}

	public void SetPlayerDataVector3(string vectorName, Vector3 value)
	{
		playerData.SetVector3(vectorName, value);
	}

	public Vector3 GetPlayerDataVector3(string vectorName)
	{
		return playerData.GetVector3(vectorName);
	}

	public T GetPlayerDataVariable<T>(string fieldName)
	{
		return playerData.GetVariable<T>(fieldName);
	}

	public void SetPlayerDataVariable<T>(string fieldName, T value)
	{
		playerData.SetVariable(fieldName, value);
	}

	public int GetNextMossberryValue()
	{
		return playerData.GetNextMossberryValue();
	}

	public int GetNextSilkGrubValue()
	{
		return playerData.GetNextSilkGrubValue();
	}

	public void EquipCharm(int charmNum)
	{
		playerData.EquipCharm(charmNum);
	}

	public void UnequipCharm(int charmNum)
	{
		playerData.UnequipCharm(charmNum);
	}

	public void RefreshOvercharm()
	{
	}

	public void UpdateBlueHealth()
	{
		hero_ctrl.UpdateBlueHealth();
	}

	public void AddBlueHealthQueued()
	{
		QueuedBlueHealth++;
		EventRegister.SendEvent(EventRegisterEvents.AddQueuedBlueHealth);
	}

	public void SetCurrentMapZoneAsRespawn()
	{
		playerData.mapZone = sm.mapZone;
		playerData.extraRestZone = sm.extraRestZone;
	}

	public void SetOverrideMapZoneAsRespawn(MapZone mapZone)
	{
		playerData.mapZone = mapZone;
		playerData.extraRestZone = sm.extraRestZone;
	}

	public void SetMapZoneToSpecific(string mapZone)
	{
		object obj = Enum.Parse(typeof(MapZone), mapZone);
		if (obj != null)
		{
			playerData.mapZone = (MapZone)obj;
		}
		else
		{
			Debug.LogError("Couldn't convert " + mapZone + " to a MapZone");
		}
		playerData.extraRestZone = ExtraRestZones.None;
	}

	public bool UpdateGameMapWithPopup(float delay = 0f)
	{
		GameObject mapUpdatedPopupPrefab = UI.MapUpdatedPopupPrefab;
		if (mapUpdatedPopupPrefab != null)
		{
			UnityEngine.Object.Instantiate(mapUpdatedPopupPrefab).GetComponent<PlayMakerFSM>().FsmVariables.FindFsmFloat("Delay").Value = delay;
		}
		return UpdateGameMap();
	}

	public bool UpdateGameMap()
	{
		bool result = gameMap.UpdateGameMap();
		gameMap.SetupMap();
		if (playerData.HasAnyPin && !CollectableItemManager.IsInHiddenMode() && MapPin.DidActivateNewPin)
		{
			MapPin.DidActivateNewPin = false;
			result = true;
		}
		playerData.mapUpdateQueued = false;
		return result;
	}

	public bool UpdateGameMapPins()
	{
		gameMap.SetupMap(pinsOnly: true);
		bool didActivateNewPin = MapPin.DidActivateNewPin;
		MapPin.DidActivateNewPin = false;
		if (didActivateNewPin)
		{
			DidPurchasePin = true;
		}
		return didActivateNewPin;
	}

	public bool DoShopCloseGameMapUpdate()
	{
		if (DidPurchasePin || DidPurchaseMap)
		{
			DidPurchasePin = false;
			DidPurchaseMap = false;
			UpdateGameMapWithPopup();
			return true;
		}
		return false;
	}

	public void AddToScenesVisited(string scene)
	{
		scene = scene.Trim();
		if (!string.IsNullOrWhiteSpace(scene) && !playerData.scenesVisited.Contains(scene))
		{
			playerData.scenesVisited.Add(scene);
		}
	}

	public void AddToBenchList()
	{
		if (!playerData.scenesEncounteredBench.Contains(GetSceneNameString()))
		{
			playerData.scenesEncounteredBench.Add(GetSceneNameString());
		}
	}

	public void AddToGrubList()
	{
	}

	public void AddToFlameList()
	{
	}

	public void AddToCocoonList()
	{
		if (!playerData.scenesEncounteredCocoon.Contains(GetSceneNameString()))
		{
			playerData.scenesEncounteredCocoon.Add(GetSceneNameString());
		}
	}

	public void AddToDreamPlantList()
	{
		Debug.LogError("DEPRECATED");
	}

	public void AddToDreamPlantCList()
	{
		Debug.LogError("DEPRECATED");
	}

	public void CountGameCompletion()
	{
		playerData.CountGameCompletion();
	}

	public void CountJournalEntries()
	{
	}

	public void ActivateTestingCheats()
	{
		playerData.ActivateTestingCheats();
	}

	public void GetAllPowerups()
	{
		playerData.GetAllPowerups();
	}

	public void MapperLeavePreviousLocations(string seenBool)
	{
		if (playerData.SeenMapperBoneForest && seenBool != "SeenMapperBoneForest")
		{
			playerData.MapperLeftBoneForest = true;
		}
		if (playerData.SeenMapperCoralCaverns && seenBool != "SeenMapperCoralCaverns")
		{
			playerData.MapperLeftCoralCaverns = true;
		}
		if (playerData.SeenMapperCrawl && seenBool != "SeenMapperCrawl")
		{
			playerData.MapperLeftCrawl = true;
		}
		if (playerData.SeenMapperDocks && seenBool != "SeenMapperDocks")
		{
			playerData.MapperLeftDocks = true;
		}
		if (playerData.SeenMapperDustpens && seenBool != "SeenMapperDustpens")
		{
			playerData.MapperLeftDustpens = true;
		}
		if (playerData.SeenMapperGreymoor && seenBool != "SeenMapperGreymoor")
		{
			playerData.MapperLeftGreymoor = true;
		}
		if (playerData.SeenMapperHuntersNest && seenBool != "SeenMapperHuntersNest")
		{
			playerData.MapperLeftHuntersNest = true;
		}
		if (playerData.SeenMapperJudgeSteps && seenBool != "SeenMapperJudgeSteps")
		{
			playerData.MapperLeftJudgeSteps = true;
		}
		if (playerData.SeenMapperPeak && seenBool != "SeenMapperPeak")
		{
			playerData.MapperLeftPeak = true;
		}
		if (playerData.SeenMapperShadow && seenBool != "SeenMapperShadow")
		{
			playerData.MapperLeftShadow = true;
		}
		if (playerData.SeenMapperShellwood && seenBool != "SeenMapperShellwood")
		{
			playerData.MapperLeftShellwood = true;
		}
		if (playerData.SeenMapperWilds && seenBool != "SeenMapperWilds")
		{
			playerData.MapperLeftWilds = true;
		}
	}

	public void AwardAchievement(string key)
	{
		achievementHandler.AwardAchievementToPlayer(key);
	}

	public void QueueAchievementProgress(string key, int current, int max)
	{
		achievementHandler.QueueAchievementProgress(key, current, max);
	}

	public void UpdateAchievementProgress(string key, int current, int max)
	{
		if (current >= max)
		{
			AwardAchievement(key);
		}
		achievementHandler.UpdateAchievementProgress(key, current, max);
	}

	public void QueueAchievement(string key)
	{
		achievementHandler.QueueAchievement(key);
	}

	public void AwardQueuedAchievements(float delay)
	{
		if (delay > 0f)
		{
			this.StartTimerRoutine(delay, 0f, null, AwardQueuedAchievements);
		}
		else
		{
			AwardQueuedAchievements();
		}
	}

	public void AwardQueuedAchievements()
	{
		achievementHandler.AwardQueuedAchievements();
		foreach (string queuedMenuStyle in queuedMenuStyles)
		{
			MenuStyleUnlock.Unlock(queuedMenuStyle, forceChange: true);
		}
		queuedMenuStyles.Clear();
	}

	public void QueuedMenuStyleUnlock(string key)
	{
		if (!queuedMenuStyles.Contains(key))
		{
			queuedMenuStyles.Add(key);
		}
	}

	public static bool CanUnlockMenuStyle(string key)
	{
		GameManager gameManager = instance;
		if (gameManager == null)
		{
			return true;
		}
		if (gameManager.achievementHandler != null && gameManager.achievementHandler.QueuedAchievements.Contains(key))
		{
			gameManager.QueuedMenuStyleUnlock(key);
			return false;
		}
		return true;
	}

	public bool IsAchievementAwarded(string key)
	{
		return achievementHandler.AchievementWasAwarded(key);
	}

	public void ClearAllAchievements()
	{
		achievementHandler.ResetAllAchievements();
	}

	public void CheckAllAchievements()
	{
		_ = Platform.Current.IsFiringAchievementsFromSavesAllowed;
	}

	public void CheckBellwayAchievements()
	{
		int num = 0;
		if (playerData.UnlockedDocksStation)
		{
			num++;
		}
		if (playerData.UnlockedBoneforestEastStation)
		{
			num++;
		}
		if (playerData.UnlockedGreymoorStation)
		{
			num++;
		}
		if (playerData.UnlockedBelltownStation)
		{
			num++;
		}
		if (playerData.UnlockedCoralTowerStation)
		{
			num++;
		}
		if (playerData.UnlockedCityStation)
		{
			num++;
		}
		if (playerData.UnlockedPeakStation)
		{
			num++;
		}
		if (playerData.UnlockedShellwoodStation)
		{
			num++;
		}
		if (playerData.UnlockedShadowStation)
		{
			num++;
		}
		if (playerData.UnlockedAqueductStation)
		{
			num++;
		}
		UpdateAchievementProgress("BELLWAYS_FULL", num, 10);
	}

	public void CheckTubeAchievements()
	{
		int num = 0;
		if (playerData.UnlockedSongTube)
		{
			num++;
		}
		if (playerData.UnlockedUnderTube)
		{
			num++;
		}
		if (playerData.UnlockedCityBellwayTube)
		{
			num++;
		}
		if (playerData.UnlockedHangTube)
		{
			num++;
		}
		if (playerData.UnlockedEnclaveTube)
		{
			num++;
		}
		if (playerData.UnlockedArboriumTube)
		{
			num++;
		}
		UpdateAchievementProgress("TUBES_FULL", num, 6);
	}

	public void CheckMapAchievements()
	{
		UpdateAchievementProgress("ALL_MAPS", playerData.MapCount, 28);
	}

	public void CheckSubQuestAchievements()
	{
		int num = 0;
		if (playerData.HasMelodyArchitect)
		{
			num++;
		}
		if (playerData.HasMelodyConductor)
		{
			num++;
		}
		if (playerData.HasMelodyLibrarian)
		{
			num++;
		}
		UpdateAchievementProgress("CITADEL_SONG", num, 3);
	}

	public void CheckHeartAchievements()
	{
		int num = (playerData.maxHealthBase - 5) * 4 + playerData.heartPieces;
		if (num < 4)
		{
			QueueAchievementProgress("FIRST_MASK", num, 4);
		}
		QueueAchievementProgress("ALL_MASKS", num, 20);
	}

	public void CheckSilkSpoolAchievements()
	{
		int num = (playerData.silkMax - 9) * 2 + playerData.silkSpoolParts;
		if (num < 2)
		{
			QueueAchievementProgress("FIRST_SILK_SPOOL", num, 2);
		}
		QueueAchievementProgress("ALL_SILK_SPOOLS", num, 18);
	}

	public void CheckCompletionAchievements()
	{
		CountGameCompletion();
		bool flag = playerData.permadeathMode > PermadeathModes.Off;
		bool flag2 = playerData.completionPercentage >= 100f;
		if (flag)
		{
			AwardAchievement("STEEL_SOUL");
			MenuStyleUnlock.Unlock("COMPLETED_STEEL", forceChange: false);
		}
		if (flag2)
		{
			AwardAchievement("COMPLETION");
			if (flag)
			{
				AwardAchievement("STEEL_SOUL_FULL");
			}
		}
		if (playerData.playTime <= 18000f)
		{
			AwardAchievement("SPEEDRUN_1");
		}
		if (flag2 && playerData.playTime <= 108000f)
		{
			AwardAchievement("SPEED_COMPLETION");
		}
	}

	public void RecordGameComplete()
	{
	}

	public void SetStatusRecordInt(string key, int value)
	{
		Platform.Current.RoamingSharedData.SetInt(key, value);
	}

	public int GetStatusRecordInt(string key)
	{
		return Platform.Current.RoamingSharedData.GetInt(key, 0);
	}

	public void ResetStatusRecords()
	{
		Platform.Current.RoamingSharedData.DeleteKey("RecPermadeathMode");
	}

	public void SaveStatusRecords()
	{
		Platform.Current.RoamingSharedData.Save();
	}

	public void SetState(GameState newState)
	{
		GameState = newState;
		this.GameStateChange?.Invoke(newState);
	}

	public void LoadScene(string destScene)
	{
		PersistentAudioManager.OnLeaveScene();
		PersistentAudioManager.QueueSceneEntry();
		startedOnThisScene = false;
		nextSceneName = destScene;
		this.NextSceneWillActivate?.Invoke();
		this.UnloadingLevel?.Invoke();
		AsyncOperationHandle<SceneInstance> fromOperationHandle = Addressables.LoadSceneAsync("Scenes/" + destScene);
		LastSceneLoad = new SceneLoad(fromOperationHandle, new SceneLoadInfo
		{
			SceneName = destScene
		});
	}

	public IEnumerator LoadSceneAdditive(string destScene)
	{
		startedOnThisScene = false;
		nextSceneName = destScene;
		waitForManualLevelStart = true;
		IsWaitingForSceneReady = true;
		this.NextSceneWillActivate?.Invoke();
		this.UnloadingLevel?.Invoke();
		string exitingScene = SceneManager.GetActiveScene().name;
		AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(destScene, LoadSceneMode.Additive);
		asyncOperation.allowSceneActivation = true;
		yield return asyncOperation;
		UnloadScene(exitingScene, null);
		RefreshTilemapInfo(destScene);
		if (SceneLoad.IsClearMemoryRequired())
		{
			yield return SceneLoad.TryClearMemory(revertGlobalPool: true);
		}
		else
		{
			GCManager.Collect();
		}
		SetupSceneRefs(refreshTilemapInfo: true);
		BeginScene();
		OnNextLevelReady();
		waitForManualLevelStart = false;
	}

	public void OnNextLevelReady()
	{
		nextLevelEntryNumber++;
		PersistentAudioManager.AttachToObject(cameraCtrl.transform);
		if (!IsGameplayScene())
		{
			IsWaitingForSceneReady = false;
			if (shouldFadeInScene)
			{
				FadeSceneIn();
			}
			AudioManager.UnpauseActorSnapshot();
			return;
		}
		SimulateSceneStartPhysics();
		IsWaitingForSceneReady = false;
		SetState(GameState.ENTERING_LEVEL);
		playerData.disablePause = false;
		inputHandler.AllowPause();
		inputHandler.StartAcceptingInput();
		HeroController heroController = HeroController.instance;
		if (SuppressRegainControl)
		{
			SuppressRegainControl = false;
		}
		else
		{
			heroController.RegainControl(allowInput: false);
		}
		heroController.StartAnimationControl();
		EnterHero();
		AudioManager.UnpauseActorSnapshot(delegate
		{
			if (!SkipNormalActorFadeIn())
			{
				actorSnapshotUnpaused.TransitionToSafe(sceneTransitionActorFadeUp);
			}
		});
		UpdateUIStateFromGameState();
		if (!IsMemoryScene(sm.mapZone))
		{
			playerData.nonLethalRespawnScene = null;
			playerData.nonLethalRespawnMarker = null;
			playerData.nonLethalRespawnType = -1;
		}
		if (shouldFadeInScene)
		{
			FadeSceneIn();
		}
		else if (!BlockNextVibrationFadeIn)
		{
			VibrationManager.FadeVibration(1f, 0.25f);
		}
		else
		{
			BlockNextVibrationFadeIn = false;
		}
	}

	public bool SkipNormalActorFadeIn()
	{
		return skipActorEntryFade >= nextLevelEntryNumber;
	}

	public void SetSkipNextLevelReadyActorFadeIn(bool skip)
	{
		if (skip)
		{
			skipActorEntryFade = nextLevelEntryNumber + 1;
		}
		else
		{
			skipActorEntryFade = -1;
		}
	}

	private void SimulateSceneStartPhysics()
	{
		hero_ctrl.AffectedByGravity(gravityApplies: false);
		hero_ctrl.ResetVelocity();
		Rb2dState[] array = (from body in UnityEngine.Object.FindObjectsOfType<Rigidbody2D>()
			where body.gameObject.layer == 11
			select body).Select(delegate(Rigidbody2D body)
		{
			Rb2dState result = default(Rb2dState);
			result.Body = body;
			result.Simulated = body.simulated;
			return result;
		}).ToArray();
		Rb2dState[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].Body.simulated = false;
		}
		ISceneManualSimulatePhysics[] array3 = UnityEngine.Object.FindObjectsOfType<MonoBehaviour>().OfType<ISceneManualSimulatePhysics>().ToArray();
		SimulationMode2D simulationMode = Physics2D.simulationMode;
		Physics2D.simulationMode = SimulationMode2D.Script;
		ISceneManualSimulatePhysics[] array4 = array3;
		for (int i = 0; i < array4.Length; i++)
		{
			array4[i].PrepareManualSimulate();
		}
		float num = 3f;
		while (num > 0f)
		{
			float fixedDeltaTime = Time.fixedDeltaTime;
			Physics2D.Simulate(fixedDeltaTime);
			array4 = array3;
			for (int i = 0; i < array4.Length; i++)
			{
				array4[i].OnManualPhysics(fixedDeltaTime);
			}
			num -= fixedDeltaTime;
		}
		Physics2D.simulationMode = simulationMode;
		array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			Rb2dState rb2dState = array2[i];
			rb2dState.Body.simulated = rb2dState.Simulated;
		}
		array4 = array3;
		for (int i = 0; i < array4.Length; i++)
		{
			array4[i].OnManualSimulateFinished();
		}
	}

	public void OnWillActivateFirstLevel()
	{
		HeroController.instance.isEnteringFirstLevel = true;
		entryGateName = "top1";
		SetState(GameState.PLAYING);
		ui.ConfigureMenu();
	}

	public IEnumerator LoadFirstScene()
	{
		yield return new WaitForEndOfFrame();
		OnWillActivateFirstLevel();
		LoadScene("Tut_01");
	}

	public void LoadPermadeathUnlockScene()
	{
		if (GetStatusRecordInt("RecPermadeathMode") == 0)
		{
			LoadScene("PermaDeath_Unlock");
		}
		else
		{
			StartCoroutine(ReturnToMainMenu(willSave: true));
		}
	}

	public void LoadOpeningCinematic()
	{
		SetState(GameState.CUTSCENE);
		LoadScene("Intro_Cutscene");
	}

	private void PositionHeroAtSceneEntrance()
	{
		Vector2 position = FindEntryPoint(entryGateName, default(Scene)) ?? new Vector2(-20000f, 20000f);
		if (hero_ctrl != null)
		{
			hero_ctrl.transform.SetPosition2D(position);
		}
	}

	private Vector2? FindEntryPoint(string entryPointName, Scene filterScene)
	{
		if (RespawningHero)
		{
			Transform transform = hero_ctrl.LocateSpawnPoint();
			if (transform != null)
			{
				return transform.transform.position;
			}
			return null;
		}
		if (hazardRespawningHero)
		{
			return playerData.hazardRespawnLocation;
		}
		TransitionPoint transitionPoint = FindTransitionPoint(entryPointName, filterScene, fallbackToAnyAvailable: true);
		if (transitionPoint != null)
		{
			return (Vector2)transitionPoint.transform.position + transitionPoint.entryOffset;
		}
		return null;
	}

	private TransitionPoint FindTransitionPoint(string entryPointName, Scene filterScene, bool fallbackToAnyAvailable)
	{
		List<TransitionPoint> transitionPoints = TransitionPoint.TransitionPoints;
		foreach (TransitionPoint item in transitionPoints)
		{
			if (!(item.name != entryPointName) && (!filterScene.IsValid() || item.gameObject.scene == filterScene))
			{
				return item;
			}
		}
		if (fallbackToAnyAvailable && transitionPoints.Count > 0)
		{
			TransitionPoint transitionPoint = transitionPoints[0];
			Debug.LogWarning("Couldn't find transition point \"" + entryPointName + "\", falling back to first available: \"" + transitionPoint.name + "\"");
			return transitionPoint;
		}
		return null;
	}

	private void EnterHero()
	{
		if (entryGateName == "door_dreamReturn" && !string.IsNullOrEmpty(playerData.bossReturnEntryGate))
		{
			entryGateName = playerData.bossReturnEntryGate;
			playerData.bossReturnEntryGate = string.Empty;
		}
		RespawnMarker respawnMarker;
		bool hasRespawnMarker;
		bool hasFaded;
		if (RespawningHero)
		{
			Transform transform = hero_ctrl.LocateSpawnPoint();
			respawnMarker = (transform ? transform.GetComponent<RespawnMarker>() : null);
			hasRespawnMarker = respawnMarker != null;
			if (hasRespawnMarker)
			{
				respawnMarker.PrepareRespawnHere();
			}
			hasFaded = false;
			if (needFirstFadeIn && (!hasRespawnMarker || !respawnMarker.customWakeUp) && (!hasRespawnMarker || !respawnMarker.customFadeDuration.IsEnabled))
			{
				screenFader_fsm.SendEventSafe("SCENE FADE OUT INSTANT");
				StartCoroutine(FadeSceneInWithDelay(0.3f));
				hasFaded = true;
			}
			needFirstFadeIn = false;
			if (hasFaded || cameraCtrl.HasBeenPositionedAtHero)
			{
				DoFadeIn();
			}
			else
			{
				cameraCtrl.PositionedAtHero += OnHeroInPosition;
			}
			StartCoroutine(hero_ctrl.Respawn(transform));
			RespawningHero = false;
		}
		else if (hazardRespawningHero)
		{
			StartCoroutine(hero_ctrl.HazardRespawn());
			FinishedEnteringScene();
			hazardRespawningHero = false;
		}
		else if (entryGateName == "dreamGate")
		{
			hero_ctrl.EnterSceneDreamGate();
		}
		else if (!startedOnThisScene)
		{
			SetState(GameState.ENTERING_LEVEL);
			bool enterSkip = sceneLoad != null && sceneLoad.SceneLoadInfo.EntrySkip;
			if (!string.IsNullOrEmpty(entryGateName))
			{
				TransitionPoint transitionPoint = FindTransitionPoint(entryGateName, default(Scene), fallbackToAnyAvailable: true);
				if ((bool)transitionPoint)
				{
					StartCoroutine(hero_ctrl.EnterScene(transitionPoint, entryDelay, forceCustomFade: false, null, enterSkip));
				}
				else if (ProjectBenchmark.IsRunning)
				{
					TransitionPoint transitionPoint2 = TransitionPoint.TransitionPoints.FirstOrDefault();
					if ((bool)transitionPoint2)
					{
						StartCoroutine(hero_ctrl.EnterScene(transitionPoint2, entryDelay, forceCustomFade: false, null, enterSkip));
					}
				}
			}
			else
			{
				FinishedEnteringScene();
			}
		}
		else if (IsGameplayScene())
		{
			FinishedEnteringScene();
			FadeSceneIn();
		}
		void DoFadeIn()
		{
			if (!hasFaded && (!hasRespawnMarker || !respawnMarker.customWakeUp))
			{
				if (hasRespawnMarker && respawnMarker.customFadeDuration.IsEnabled)
				{
					Color colour;
					Color startColour = (colour = ScreenFaderUtils.GetColour());
					colour.a = 0f;
					ScreenFaderUtils.Fade(startColour, colour, respawnMarker.customFadeDuration.Value);
				}
				else
				{
					screenFader_fsm.SendEventSafe("DEATH RESPAWN");
				}
			}
		}
		void OnHeroInPosition()
		{
			DoFadeIn();
			cameraCtrl.PositionedAtHero -= OnHeroInPosition;
		}
	}

	public void FinishedEnteringScene()
	{
		if (GameState != GameState.CUTSCENE)
		{
			SetState(GameState.PLAYING);
		}
		entryDelay = 0f;
		hasFinishedEnteringScene = true;
		EventRegister.SendEvent(EventRegisterEvents.HeroEnteredScene);
		this.OnFinishedEnteringScene?.Invoke();
	}

	private void SetupGameRefs()
	{
		if (!hasSetup)
		{
			hasSetup = true;
			playerData = PlayerData.instance;
			playerData.SetupExistingPlayerData();
			sceneData = SceneData.instance;
			gameCams = GameCameras.instance;
			cameraCtrl = gameCams.cameraController;
			gameSettings = new GameSettings();
			inputHandler = GetComponent<InputHandler>();
			achievementHandler = GetComponent<AchievementHandler>();
			SpawnInControlManager();
			GameObject gameplayChild = gameCams.hudCamera.GetComponent<HUDCamera>().GameplayChild;
			screenFader_fsm = gameplayChild.LocateMyFSM("Screen Fader");
			inventoryFSM = gameplayChild.transform.Find("Inventory").gameObject.GetComponent<PlayMakerFSM>();
			if ((bool)AchievementPopupHandler.Instance)
			{
				AchievementPopupHandler.Instance.Setup(achievementHandler);
			}
			Platform.Current.AdjustGraphicsSettings(gameSettings);
			if (inputHandler == null)
			{
				Debug.LogError("Couldn't find InputHandler component.");
			}
			if (achievementHandler == null)
			{
				Debug.LogError("Couldn't find AchievementHandler component.");
			}
			SceneManager.activeSceneChanged += LevelActivated;
			NextSceneWillActivate += AutoRecycleSelf.RecycleActiveRecyclers;
			NextSceneWillActivate += PlayAudioAndRecycle.RecycleActiveRecyclers;
			NextSceneWillActivate += ResetDynamicHierarchy.ForceReconnectAll;
			RegisterEvents();
		}
	}

	private void RegisterEvents()
	{
		if (!registerEvents)
		{
			registerEvents = true;
			subbedCamShake = GlobalSettings.Camera.MainCameraShakeManager;
			subbedCamShake.CameraShakedWorldForce += CurrencyObjectBase.SendOnCameraShakedWorldForce;
			cameraCtrl.PositionedAtHero += PersistentAudioManager.OnEnteredNextScene;
		}
	}

	private void UnregisterEvents()
	{
		if (registerEvents)
		{
			registerEvents = false;
			if ((bool)subbedCamShake)
			{
				subbedCamShake.CameraShakedWorldForce -= CurrencyObjectBase.SendOnCameraShakedWorldForce;
				subbedCamShake = null;
			}
			if ((bool)cameraCtrl)
			{
				cameraCtrl.PositionedAtHero -= PersistentAudioManager.OnEnteredNextScene;
			}
		}
	}

	private void FindSceneManager()
	{
		GameObject gameObject = GameObject.FindGameObjectWithTag("SceneManager");
		if (gameObject != null)
		{
			sm = gameObject.GetComponent<CustomSceneManager>();
			CustomSceneManager.IncrementVersion();
		}
		else
		{
			Debug.Log("Scene Manager missing from scene " + sceneName);
		}
	}

	public void EnsureGlobalPool()
	{
		if (!globalPoolPrefabHandle.IsValid() && IsGameplayScene())
		{
			UnityEngine.Object.Instantiate(LoadGlobalPoolPrefab().WaitForCompletion());
		}
	}

	public void SetupSceneRefs(bool refreshTilemapInfo)
	{
		forceCurrentSceneMemory = false;
		UpdateSceneName();
		if (ui == null)
		{
			ui = UIManager.instance;
		}
		FindSceneManager();
		if (IsGameplayScene())
		{
			EnsureGlobalPool();
			ObjectPool.CreateStartupPools();
			if (hero_ctrl == null)
			{
				SetupHeroRefs();
			}
			inputHandler.AttachHeroController(hero_ctrl);
			if (refreshTilemapInfo)
			{
				RefreshTilemapInfo(sceneName);
			}
			soulOrb_fsm = gameCams.soulOrbFSM;
			soulVessel_fsm = gameCams.soulVesselFSM;
		}
		if (sceneSeedTrackers == null)
		{
			sceneSeedTrackers = new List<SceneSeedTracker>();
		}
		SceneSeedTracker sceneSeedTracker = null;
		for (int num = sceneSeedTrackers.Count - 1; num >= 0; num--)
		{
			SceneSeedTracker sceneSeedTracker2 = sceneSeedTrackers[num];
			sceneSeedTracker2.TransitionsLeft--;
			if (sceneSeedTracker2.TransitionsLeft <= 0)
			{
				sceneSeedTrackers.RemoveAt(num);
			}
			else if (sceneSeedTracker2.SceneNameHash == sceneNameHash)
			{
				sceneSeedTracker = sceneSeedTracker2;
			}
		}
		if (sceneSeedTracker == null)
		{
			sceneSeedTracker = new SceneSeedTracker
			{
				SceneNameHash = sceneNameHash,
				Seed = UnityEngine.Random.Range(0, 99999),
				TransitionsLeft = 3
			};
			sceneSeedTrackers.Add(sceneSeedTracker);
		}
		else
		{
			sceneSeedTracker.TransitionsLeft = 3;
		}
		SceneSeededRandom = new System.Random(sceneSeedTracker.Seed);
	}

	public void SetupHeroRefs()
	{
		hero_ctrl = HeroController.instance;
		if ((bool)hero_ctrl)
		{
			heroLight = hero_ctrl.heroLight.GetComponent<SpriteRenderer>();
		}
	}

	public void BeginScene()
	{
		ObjectPool.PurgeRecentRecycled();
		timeInScene = 0f;
		inputHandler.SceneInit();
		ui.SceneInit();
		bool num = IsMenuScene();
		if (!num)
		{
			SetupHeroRefs();
			if ((bool)hero_ctrl)
			{
				hero_ctrl.SceneInit();
			}
		}
		gameCams.SceneInit();
		if (!num && this.SceneInit != null)
		{
			this.SceneInit();
		}
		if (num)
		{
			SetState(GameState.MAIN_MENU);
			UpdateUIStateFromGameState();
		}
		else if (IsGameplayScene())
		{
			if ((!Application.isEditor && !Debug.isDebugBuild) || Time.renderedFrameCount > 3)
			{
				PositionHeroAtSceneEntrance();
			}
		}
		else if (IsNonGameplayScene())
		{
			SetState(GameState.CUTSCENE);
			UpdateUIStateFromGameState();
		}
		else
		{
			UpdateUIStateFromGameState();
		}
	}

	private void UpdateUIStateFromGameState()
	{
		if (ui != null)
		{
			ui.SetUIStartState(GameState);
			return;
		}
		ui = UnityEngine.Object.FindObjectOfType<UIManager>();
		if (ui != null)
		{
			ui.SetUIStartState(GameState);
		}
	}

	public void SkipCutscene()
	{
		StartCoroutine(SkipCutsceneNoMash());
	}

	public void RegisterSkippable(ISkippable skippable)
	{
		if (skippables == null)
		{
			skippables = new List<ISkippable>();
		}
		skippables.Add(skippable);
	}

	public void DeregisterSkippable(ISkippable skippable)
	{
		skippables.Remove(skippable);
	}

	private IEnumerator SkipCutsceneNoMash()
	{
		if (GameState != GameState.CUTSCENE)
		{
			yield break;
		}
		ui.HideCutscenePrompt(isInstant: true);
		if (skippables != null)
		{
			using List<ISkippable>.Enumerator enumerator = skippables.GetEnumerator();
			if (enumerator.MoveNext())
			{
				ISkippable current = enumerator.Current;
				yield return StartCoroutine(current.Skip());
				inputHandler.skippingCutscene = false;
				yield break;
			}
		}
		EventRegister.SendEvent(EventRegisterEvents.CustomCutsceneSkip);
		inputHandler.skippingCutscene = false;
	}

	public void NoLongerFirstGame()
	{
		if (playerData.isFirstGame)
		{
			playerData.isFirstGame = false;
		}
		IsFirstLevelForPlayer = false;
	}

	private void SetupStatusModifiers()
	{
		if (gameConfig.clearRecordsOnStart)
		{
			ResetStatusRecords();
		}
		if (gameConfig.unlockPermadeathMode)
		{
			SetStatusRecordInt("RecPermadeathMode", 1);
		}
		if (gameConfig.unlockBossRushMode)
		{
			SetStatusRecordInt("RecBossRushMode", 1);
		}
		if (gameConfig.clearPreferredLanguageSetting)
		{
			Platform.Current.LocalSharedData.DeleteKey("GameLangSet");
		}
		if (gameSettings.CommandArgumentUsed("-forcelang"))
		{
			Debug.Log("== Language option forced on by command argument.");
			gameConfig.hideLanguageOption = true;
		}
	}

	public void RefreshLocalization()
	{
		this.RefreshLanguageText?.Invoke();
	}

	public void RefreshParticleSystems()
	{
		this.RefreshParticleLevel?.Invoke();
	}

	public void ApplyNativeInput()
	{
	}

	public void EnablePermadeathMode()
	{
		SetStatusRecordInt("RecPermadeathMode", 1);
	}

	public string GetCurrentMapZone()
	{
		if (mapZoneStringVersion == CustomSceneManager.Version)
		{
			return mapZoneString;
		}
		mapZoneStringVersion = CustomSceneManager.Version;
		mapZoneString = GetCurrentMapZoneEnum().ToString();
		return mapZoneString;
	}

	public MapZone GetCurrentMapZoneEnum()
	{
		if (mapZoneVersion == CustomSceneManager.Version)
		{
			return currentMapZone;
		}
		if (!sm)
		{
			FindSceneManager();
			if (!sm)
			{
				mapZoneVersion = CustomSceneManager.Version;
				currentMapZone = MapZone.NONE;
				return MapZone.NONE;
			}
		}
		mapZoneVersion = CustomSceneManager.Version;
		currentMapZone = sm.mapZone;
		return currentMapZone;
	}

	public float GetSceneWidth()
	{
		if (IsGameplayScene())
		{
			return sceneWidth;
		}
		return 0f;
	}

	public float GetSceneHeight()
	{
		if (IsGameplayScene())
		{
			return sceneHeight;
		}
		return 0f;
	}

	public GameObject GetSceneManager()
	{
		if (!sm)
		{
			FindSceneManager();
			if (!sm)
			{
				return null;
			}
		}
		return sm.gameObject;
	}

	public string GetFormattedMapZoneString(MapZone mapZone)
	{
		return Language.Get(mapZone.ToString(), "Map Zones");
	}

	public static string GetFormattedMapZoneStringV2(MapZone mapZone)
	{
		return Language.Get(mapZone.ToString(), "Map Zones");
	}

	public static string GetFormattedAutoSaveNameString(AutoSaveName autoSaveName)
	{
		return Language.Get(autoSaveName.ToString(), "AutoSaveNames");
	}

	public void UpdateSceneName()
	{
		string text = sceneName;
		string text2 = SceneManager.GetActiveScene().name;
		if (!(text2 == rawSceneName))
		{
			rawSceneName = text2;
			sceneName = GetBaseSceneName(text2);
			sceneNameHash = sceneName.GetHashCode();
			if (sceneName != text)
			{
				lastSceneName = text;
			}
		}
	}

	public static string GetBaseSceneName(string fullSceneName)
	{
		if (fullSceneName == lastFullSceneName)
		{
			return fixedSceneName;
		}
		lastFullSceneName = fullSceneName;
		fixedSceneName = InternalBaseSceneName(fullSceneName);
		return fixedSceneName;
	}

	private static string InternalBaseSceneName(string fullSceneName)
	{
		if (string.IsNullOrEmpty(fullSceneName))
		{
			return string.Empty;
		}
		string[] subSceneNameSuffixes = WorldInfo.SubSceneNameSuffixes;
		foreach (string text in subSceneNameSuffixes)
		{
			if (fullSceneName.EndsWith(text, StringComparison.InvariantCultureIgnoreCase))
			{
				int length = text.Length;
				return fullSceneName.Substring(0, fullSceneName.Length - length);
			}
		}
		return fullSceneName;
	}

	public string GetSceneNameString()
	{
		UpdateSceneName();
		return sceneName;
	}

	private static tk2dTileMap GetTileMap(GameObject gameObject)
	{
		if (gameObject.CompareTag("TileMap"))
		{
			return gameObject.GetComponent<tk2dTileMap>();
		}
		return null;
	}

	public void RefreshTilemapInfo(string targetScene)
	{
		tk2dTileMap tk2dTileMap2 = null;
		int num = 0;
		while (tk2dTileMap2 == null && num < SceneManager.sceneCount)
		{
			Scene sceneAt = SceneManager.GetSceneAt(num);
			if (string.IsNullOrEmpty(targetScene) || !(sceneAt.name != targetScene))
			{
				GameObject[] rootGameObjects = sceneAt.GetRootGameObjects();
				int num2 = 0;
				while (tk2dTileMap2 == null && num2 < rootGameObjects.Length)
				{
					tk2dTileMap2 = GetTileMap(rootGameObjects[num2]);
					num2++;
				}
			}
			num++;
		}
		if (tk2dTileMap2 == null)
		{
			Debug.LogWarningFormat("Using fallback 1 to find tilemap. Scene {0} requires manual fixing.", targetScene);
			GameObject[] array = GameObject.FindGameObjectsWithTag("TileMap");
			int num3 = 0;
			while (tk2dTileMap2 == null && num3 < array.Length)
			{
				GameObject gameObject = array[num3];
				if (string.IsNullOrEmpty(targetScene) || !(gameObject.scene.name != targetScene))
				{
					tk2dTileMap2 = gameObject.GetComponent<tk2dTileMap>();
				}
				num3++;
			}
		}
		if (tk2dTileMap2 == null)
		{
			Debug.LogErrorFormat("Failed to find tilemap in " + targetScene + " entirely.");
			return;
		}
		tilemap = tk2dTileMap2;
		sceneWidth = tilemap.width;
		sceneHeight = tilemap.height;
	}

	public void SaveLevelState()
	{
		this.SavePersistentObjects?.Invoke();
	}

	public void ResetSemiPersistentItems()
	{
		this.ResetSemiPersistentObjects?.Invoke();
		sceneData.ResetSemiPersistentItems();
	}

	public bool IsMenuScene()
	{
		UpdateSceneName();
		if (sceneName == "Menu_Title")
		{
			return true;
		}
		return false;
	}

	public bool IsTitleScreenScene()
	{
		UpdateSceneName();
		if (string.Compare(sceneName, "Title_Screens", ignoreCase: true) == 0)
		{
			return true;
		}
		return false;
	}

	public bool IsGameplayScene()
	{
		UpdateSceneName();
		if (IsNonGameplayScene())
		{
			return false;
		}
		return true;
	}

	public bool IsNonGameplayScene()
	{
		UpdateSceneName();
		if (IsMenuScene())
		{
			return true;
		}
		if (IsCinematicScene())
		{
			return true;
		}
		switch (sceneName)
		{
		case "Knight Pickup":
		case "Pre_Menu_Intro":
		case "Menu_Title":
		case "End_Credits":
		case "Cutscene_Boss_Door":
		case "Quit_To_Menu":
		case "PermaDeath_Unlock":
		case "GG_Unlock":
		case "GG_End_Sequence":
		case "End_Game_Completion":
		case "BetaEnd":
		case "PermaDeath":
		case "GG_Entrance_Cutscene":
		case "GG_Boss_Door_Entrance":
		case "Demo Start":
			return true;
		default:
			return InGameCutsceneInfo.IsInCutscene;
		}
	}

	public bool IsCinematicScene()
	{
		UpdateSceneName();
		switch (sceneName)
		{
		case "Intro_Cutscene_Prologue":
		case "Opening_Sequence":
		case "Opening_Sequence_Act3":
		case "Prologue_Excerpt":
		case "Intro_Cutscene":
		case "Cinematic_Stag_travel":
		case "Cinematic_Submarine_travel":
		case "PermaDeath":
		case "Cinematic_Ending_A":
		case "Cinematic_Ending_B":
		case "Cinematic_Ending_C":
		case "Cinematic_Ending_D":
		case "Cinematic_Ending_E":
		case "Cinematic_MrMushroom":
		case "BetaEnd":
		case "Demo Start":
		case "Menu_Credits":
		case "End_Credits_Scroll":
			return true;
		default:
			return false;
		}
	}

	public bool IsStagTravelScene()
	{
		UpdateSceneName();
		return sceneName == "Cinematic_Stag_travel";
	}

	public bool IsBossDoorScene()
	{
		UpdateSceneName();
		return sceneName == "Cutscene_Boss_Door";
	}

	public bool ShouldKeepHUDCameraActive()
	{
		UpdateSceneName();
		if (!(sceneName == "GG_Entrance_Cutscene") && !(sceneName == "GG_Boss_Door_Entrance") && !(sceneName == "GG_End_Sequence"))
		{
			return InGameCutsceneInfo.IsInCutscene;
		}
		return true;
	}

	public void HasSaveFile(int saveSlot, Action<bool> callback)
	{
		if (DemoHelper.IsDemoMode)
		{
			bool obj = DemoHelper.HasSaveFile(saveSlot);
			callback?.Invoke(obj);
		}
		else
		{
			Platform.Current.IsSaveSlotInUse(saveSlot, callback);
		}
	}

	public void SaveGame(Action<bool> callback)
	{
		if (isAutoSaveQueued)
		{
			SaveGameWithAutoSave(queuedAutoSaveName, callback);
			return;
		}
		FixUpSaveState();
		SaveGame(profileID, callback);
		isSaveGameQueued = false;
	}

	public void SaveGameWithAutoSave(AutoSaveName autoSaveName, Action<bool> callback)
	{
		FixUpSaveState();
		SaveGame(profileID, callback, withAutoSave: true, autoSaveName);
		isSaveGameQueued = false;
		isAutoSaveQueued = false;
		queuedAutoSaveName = AutoSaveName.NONE;
	}

	public void QueueSaveGame()
	{
		isSaveGameQueued = true;
	}

	public void QueueAutoSave(AutoSaveName autoSaveName)
	{
		queuedAutoSaveName = autoSaveName;
		isAutoSaveQueued = true;
	}

	public void DoQueuedSaveGame()
	{
		if (isSaveGameQueued)
		{
			SaveGame(null);
		}
		else if (isAutoSaveQueued)
		{
			isAutoSaveQueued = false;
			CreateRestorePoint(queuedAutoSaveName);
		}
	}

	public void CreateRestorePoint(AutoSaveName autoSaveName, Action<bool> callback = null)
	{
		FixUpSaveState();
		CreateRestorePoint(profileID, autoSaveName, callback);
	}

	public SaveGameData CreateSaveGameData(int saveSlot)
	{
		PreparePlayerDataForSave(saveSlot);
		return new SaveGameData(playerData, sceneData);
	}

	public void CreateRestorePoint(int saveSlot, AutoSaveName autoSaveName, Action<bool> callback = null)
	{
		if (!CheatManager.AllowSaving)
		{
			callback?.Invoke(obj: true);
		}
		else if (saveSlot >= 0)
		{
			SaveLevelState();
			isAutoSaveQueued = false;
			if (!gameConfig.disableSaveGame)
			{
				if (!DemoHelper.IsDemoMode)
				{
					try
					{
						SaveGameData saveData = CreateSaveGameData(saveSlot);
						SaveDataUtility.AddTaskToAsyncQueue(delegate
						{
							try
							{
								saveData.playerData.UpdateDate();
								DoSaveRestorePoint(saveSlot, autoSaveName, saveData, callback);
							}
							catch (Exception)
							{
								if (callback != null)
								{
									CoreLoop.InvokeSafe(delegate
									{
										callback(obj: false);
									});
								}
							}
						});
						return;
					}
					catch (Exception)
					{
						if (callback != null)
						{
							CoreLoop.InvokeSafe(delegate
							{
								callback(obj: false);
							});
						}
						return;
					}
				}
				callback?.Invoke(obj: true);
			}
			else if (callback != null)
			{
				CoreLoop.InvokeNext(delegate
				{
					callback(obj: false);
				});
			}
		}
		else if (callback != null)
		{
			CoreLoop.InvokeNext(delegate
			{
				callback(obj: false);
			});
		}
	}

	private void ShowSaveIcon()
	{
		UIManager uIManager = UIManager.instance;
		if (uIManager != null && saveIconShowCounter == 0)
		{
			CheckpointSprite checkpointSprite = uIManager.checkpointSprite;
			if (checkpointSprite != null)
			{
				checkpointSprite.Show();
			}
		}
		saveIconShowCounter++;
	}

	private void HideSaveIcon()
	{
		saveIconShowCounter--;
		UIManager uIManager = UIManager.instance;
		if (uIManager != null && saveIconShowCounter == 0)
		{
			CheckpointSprite checkpointSprite = uIManager.checkpointSprite;
			if (checkpointSprite != null)
			{
				checkpointSprite.Hide();
			}
		}
	}

	private void ResetGameTimer()
	{
		sessionPlayTimer = 0f;
		sessionStartTime = playerData.playTime;
	}

	public void IncreaseGameTimer(ref float timer)
	{
		if ((GameState == GameState.PLAYING || GameState == GameState.ENTERING_LEVEL || GameState == GameState.EXITING_LEVEL) && !PlayerData.instance.isInventoryOpen && Time.unscaledDeltaTime < 1f)
		{
			timer += Time.unscaledDeltaTime;
		}
	}

	public SaveGameData GetSaveGameData(int saveSlot)
	{
		FixUpSaveState();
		if (achievementHandler != null)
		{
			achievementHandler.FlushRecordsToDisk();
		}
		else
		{
			Debug.LogError("Error saving achievements (PlayerAchievements is null)");
		}
		if (playerData != null)
		{
			playerData.playTime += sessionPlayTimer;
			ResetGameTimer();
			playerData.version = "1.0.28324";
			playerData.RevisionBreak = 28104;
			playerData.profileID = saveSlot;
			playerData.CountGameCompletion();
			playerData.OnBeforeSave();
		}
		return new SaveGameData(playerData, sceneData);
	}

	private void SaveGame(int saveSlot, Action<bool> ogCallback, bool withAutoSave = false, AutoSaveName autoSaveName = AutoSaveName.NONE)
	{
		Action<bool, string> callback = delegate(bool didSave, string errorInfo)
		{
			if (didSave)
			{
				ogCallback?.Invoke(obj: true);
			}
			else
			{
				CheatManager.LastErrorText = errorInfo;
				GenericMessageCanvas.Show("SAVE_FAILED", delegate
				{
					ogCallback?.Invoke(obj: false);
				});
			}
		};
		if (!CheatManager.AllowSaving)
		{
			callback(arg1: true, null);
		}
		else if (saveSlot >= 0)
		{
			SaveLevelState();
			if (!gameConfig.disableSaveGame)
			{
				if (DemoHelper.IsDemoMode)
				{
					callback(arg1: true, null);
					return;
				}
				ShowSaveIcon();
				PreparePlayerDataForSave(saveSlot);
				SaveGameData saveData = new SaveGameData(playerData, sceneData);
				SaveDataUtility.AddTaskToAsyncQueue(delegate(TaskCompletionSource<string> tcs)
				{
					saveData.playerData.UpdateDate();
					string result2 = SaveDataUtility.SerializeSaveData(saveData);
					tcs.SetResult(result2);
				}, delegate(bool success, string result)
				{
					if (!success)
					{
						HideSaveIcon();
						CoreLoop.InvokeNext(delegate
						{
							callback(arg1: false, result);
						});
						return;
					}
					try
					{
						GetBytesForSaveJsonAsync(result, delegate(byte[] fileBytes)
						{
							Platform.Current.WriteSaveSlot(saveSlot, fileBytes, delegate(bool didSave)
							{
								HideSaveIcon();
								callback(didSave, null);
							});
							try
							{
								ShowSaveIcon();
								Platform.Current.WriteSaveBackup(saveSlot, fileBytes, delegate
								{
									HideSaveIcon();
								});
							}
							catch (Exception)
							{
								HideSaveIcon();
							}
							if (withAutoSave)
							{
								try
								{
									ShowSaveIcon();
									SaveDataUtility.AddTaskToAsyncQueue(delegate
									{
										DoSaveRestorePoint(saveSlot, autoSaveName, saveData, delegate
										{
											HideSaveIcon();
										});
									});
								}
								catch (Exception)
								{
									HideSaveIcon();
								}
							}
						});
					}
					catch (Exception ex3)
					{
						Exception ex4 = ex3;
						Exception e = ex4;
						HideSaveIcon();
						CoreLoop.InvokeNext(delegate
						{
							callback(arg1: false, e.ToString());
						});
					}
				});
			}
			else
			{
				CoreLoop.InvokeNext(delegate
				{
					callback(arg1: false, "Saving game disabled. No save file written.");
				});
			}
		}
		else
		{
			CoreLoop.InvokeNext(delegate
			{
				callback(arg1: false, $"Save game slot not valid: {saveSlot}");
			});
		}
	}

	public void SaveGameData(int saveSlot, SaveGameData saveData, bool showSaveIcon, Action<bool> ogCallback)
	{
		Action<bool, string> callback = delegate(bool didSave, string errorInfo)
		{
			if (didSave)
			{
				ogCallback?.Invoke(obj: true);
			}
			else
			{
				CheatManager.LastErrorText = errorInfo;
				GenericMessageCanvas.Show("SAVE_FAILED", delegate
				{
					ogCallback?.Invoke(obj: false);
				});
			}
		};
		if (showSaveIcon)
		{
			ShowSaveIcon();
		}
		SaveDataUtility.AddTaskToAsyncQueue(delegate(TaskCompletionSource<string> tcs)
		{
			saveData.playerData.UpdateDate();
			string result2 = SaveDataUtility.SerializeSaveData(saveData);
			tcs.SetResult(result2);
		}, delegate(bool success, string result)
		{
			if (!success)
			{
				if (showSaveIcon)
				{
					HideSaveIcon();
				}
				CoreLoop.InvokeNext(delegate
				{
					callback(arg1: false, result);
				});
				return;
			}
			try
			{
				GetBytesForSaveJsonAsync(result, delegate(byte[] fileBytes)
				{
					Platform.Current.WriteSaveSlot(saveSlot, fileBytes, delegate(bool didSave)
					{
						if (showSaveIcon)
						{
							HideSaveIcon();
						}
						callback(didSave, null);
					});
					try
					{
						if (showSaveIcon)
						{
							ShowSaveIcon();
						}
						Platform.Current.WriteSaveBackup(saveSlot, fileBytes, delegate
						{
							HideSaveIcon();
						});
					}
					catch (Exception)
					{
						if (showSaveIcon)
						{
							HideSaveIcon();
						}
					}
				});
			}
			catch (Exception ex2)
			{
				Exception ex3 = ex2;
				Exception e = ex3;
				if (showSaveIcon)
				{
					HideSaveIcon();
				}
				CoreLoop.InvokeNext(delegate
				{
					callback(arg1: false, e.ToString());
				});
			}
		});
	}

	private void DoSaveRestorePoint(int saveSlot, AutoSaveName autoSaveName, SaveGameData saveData, Action<bool> callback)
	{
		RestorePointData restorePointData = new RestorePointData(saveData, autoSaveName);
		restorePointData.SetVersion();
		restorePointData.SetDateString();
		string jsonData = SaveDataUtility.SerializeSaveData(restorePointData);
		byte[] bytesForSaveJson = GetBytesForSaveJson(jsonData);
		bool noTrim = autoSaveName == AutoSaveName.ACT_1 || autoSaveName == AutoSaveName.ACT_2 || autoSaveName == AutoSaveName.ACT_3;
		Platform.Current.CreateSaveRestorePoint(saveSlot, autoSaveName.ToString(), noTrim, bytesForSaveJson, callback);
	}

	private void PreparePlayerDataForSave(int saveSlot)
	{
		if (achievementHandler != null)
		{
			achievementHandler.FlushRecordsToDisk();
		}
		if (playerData != null)
		{
			playerData.playTime += sessionPlayTimer;
			ResetGameTimer();
			playerData.version = "1.0.28324";
			playerData.RevisionBreak = 28104;
			playerData.profileID = saveSlot;
			playerData.CountGameCompletion();
			playerData.OnBeforeSave();
		}
	}

	public void LoadGameFromUI(int saveSlot)
	{
		StartCoroutine(LoadGameFromUIRoutine(saveSlot));
	}

	public void LoadGameFromUI(int saveSlot, SaveGameData saveGameData)
	{
		StartCoroutine(LoadGameFromUIRoutine(saveSlot, saveGameData));
	}

	private IEnumerator LoadGameFromUIRoutine(int saveSlot)
	{
		ui.ContinueGame();
		bool finishedLoading = false;
		bool successfullyLoaded = false;
		LoadGame(saveSlot, delegate(bool didLoad)
		{
			finishedLoading = true;
			successfullyLoaded = didLoad;
		});
		while (!finishedLoading)
		{
			yield return null;
		}
		if (successfullyLoaded)
		{
			ContinueGame();
		}
		else
		{
			ui.UIGoToMainMenu();
		}
	}

	private IEnumerator LoadGameFromUIRoutine(int saveSlot, SaveGameData saveGameData)
	{
		ui.ContinueGame();
		bool successfullyLoaded = false;
		if (saveGameData == null)
		{
			bool finishedLoading = false;
			LoadGame(saveSlot, delegate(bool didLoad)
			{
				finishedLoading = true;
				successfullyLoaded = didLoad;
			});
			while (!finishedLoading)
			{
				yield return null;
			}
		}
		else
		{
			SetLoadedGameData(saveGameData, saveSlot);
			successfullyLoaded = true;
		}
		if (successfullyLoaded)
		{
			ContinueGame();
		}
		else
		{
			ui.UIGoToMainMenu();
		}
	}

	public void LoadGame(int saveSlot, Action<bool> callback)
	{
		if (!Platform.IsSaveSlotIndexValid(saveSlot))
		{
			Debug.LogErrorFormat($"Cannot load from invalid save slot index {saveSlot}");
			if (callback != null)
			{
				CoreLoop.InvokeNext(delegate
				{
					callback(obj: false);
				});
			}
			return;
		}
		if (DemoHelper.IsDemoMode && saveSlot > 0)
		{
			string jsonData = null;
			bool flag = DemoHelper.TryGetSaveData(saveSlot - 1, out jsonData);
			if (flag)
			{
				SetLoadedGameData(jsonData, saveSlot);
			}
			callback?.Invoke(flag);
			return;
		}
		Platform.Current.ReadSaveSlot(saveSlot, delegate(byte[] fileBytes)
		{
			if (fileBytes == null)
			{
				callback?.Invoke(obj: false);
			}
			else if (!CheatManager.UseAsyncSaveFileLoad)
			{
				bool obj;
				try
				{
					string jsonForSaveBytes = GetJsonForSaveBytes(fileBytes);
					SetLoadedGameData(jsonForSaveBytes, saveSlot);
					obj = true;
				}
				catch (Exception arg)
				{
					Debug.LogFormat($"Error loading save file for slot {saveSlot}: {arg}");
					obj = false;
				}
				callback(obj);
			}
			else
			{
				SaveDataUtility.AddTaskToAsyncQueue(delegate
				{
					bool success;
					try
					{
						string jsonForSaveBytes2 = GetJsonForSaveBytes(fileBytes);
						SetLoadedGameData(jsonForSaveBytes2, saveSlot);
						success = true;
					}
					catch (Exception arg2)
					{
						Debug.LogFormat($"Error loading save file for slot {saveSlot}: {arg2}");
						success = false;
					}
					if (callback != null)
					{
						CoreLoop.InvokeSafe(delegate
						{
							callback(success);
						});
					}
				});
			}
		});
	}

	private void SetLoadedGameData(string jsonData, int saveSlot)
	{
		SaveGameData saveGameData = SaveDataUtility.DeserializeSaveData<SaveGameData>(jsonData);
		SetLoadedGameData(saveGameData, saveSlot);
	}

	private void SetLoadedGameData(SaveGameData saveGameData, int saveSlot)
	{
		PlayerData playerData = saveGameData.playerData;
		SceneData sceneData = saveGameData.sceneData;
		playerData.ResetNonSerializableFields();
		PlayerData.instance = playerData;
		this.playerData = playerData;
		SceneData.instance = sceneData;
		this.sceneData = sceneData;
		profileID = saveSlot;
		playerData.silk = 0;
		playerData.silkParts = 0;
		playerData.SetupExistingPlayerData();
		inputHandler.RefreshPlayerData();
		QuestManager.UpgradeQuests();
		if ((bool)Platform.Current)
		{
			Platform.Current.OnSetGameData(profileID);
		}
	}

	public void ClearSaveFile(int saveSlot, Action<bool> callback)
	{
		if (!Platform.IsSaveSlotIndexValid(saveSlot))
		{
			Debug.LogErrorFormat($"Cannot clear invalid save slot index {saveSlot}");
			if (callback != null)
			{
				CoreLoop.InvokeNext(delegate
				{
					callback(obj: false);
				});
			}
			return;
		}
		Debug.LogFormat(string.Format("Save file {0} {1}", saveSlot, "clearing..."));
		if (DemoHelper.IsDemoMode)
		{
			Debug.LogFormat($"Save file {saveSlot} not cleared - cannot clear save files in demo mode!");
			callback?.Invoke(obj: false);
			return;
		}
		Platform.Current.ClearSaveSlot(saveSlot, delegate(bool didClear)
		{
			Debug.LogFormat(string.Format("Save file {0} {1}", saveSlot, didClear ? "cleared" : "failed to clear"));
			callback?.Invoke(didClear);
			Platform.Current.DeleteRestorePointsForSlot(saveSlot, delegate
			{
			});
		});
	}

	public void GetSaveStatsForSlot(int saveSlot, Action<SaveStats, string> callback)
	{
		if (!Platform.IsSaveSlotIndexValid(saveSlot))
		{
			if (callback != null)
			{
				CoreLoop.InvokeNext(delegate
				{
					callback(null, $"Cannot get save stats for invalid slot {saveSlot}");
				});
			}
			return;
		}
		if (DemoHelper.IsDemoMode && saveSlot > 0)
		{
			if (!Platform.Current.WillPreloadSaveFiles || CheatManager.UseAsyncSaveFileLoad)
			{
				SaveDataUtility.AddTaskToAsyncQueue(delegate
				{
					string jsonData;
					bool num = DemoHelper.TryGetSaveData(saveSlot - 1, out jsonData);
					SaveStats saveStats = null;
					if (num)
					{
						saveStats = GetLoadedSaveSlotData(jsonData);
					}
					if (callback != null)
					{
						CoreLoop.InvokeSafe(delegate
						{
							callback(saveStats, null);
						});
					}
				});
				return;
			}
			string jsonData2;
			bool num2 = DemoHelper.TryGetSaveData(saveSlot - 1, out jsonData2);
			SaveStats saveStats2 = null;
			if (num2)
			{
				saveStats2 = GetLoadedSaveSlotData(jsonData2);
			}
			if (callback != null)
			{
				CoreLoop.InvokeNext(delegate
				{
					callback(saveStats2, null);
				});
			}
			return;
		}
		Platform.Current.ReadSaveSlot(saveSlot, delegate(byte[] fileBytes)
		{
			if (fileBytes == null)
			{
				if (callback != null)
				{
					CoreLoop.InvokeNext(delegate
					{
						callback(null, null);
					});
				}
			}
			else
			{
				if (Platform.Current.WillPreloadSaveFiles && !CheatManager.UseAsyncSaveFileLoad)
				{
					try
					{
						string jsonForSaveBytes = GetJsonForSaveBytes(fileBytes);
						SaveStats saveStats3 = GetLoadedSaveSlotData(jsonForSaveBytes);
						if (callback != null)
						{
							CoreLoop.InvokeNext(delegate
							{
								callback(saveStats3, null);
							});
						}
						return;
					}
					catch (Exception ex)
					{
						Exception ex2 = ex;
						Exception e = ex2;
						Debug.LogError($"Error while loading save file for slot {saveSlot} Exception: {e}");
						if (callback != null)
						{
							CoreLoop.InvokeNext(delegate
							{
								callback(null, e.ToString());
							});
						}
						return;
					}
				}
				if (CheatManager.UseTasksForJsonConversion)
				{
					Task.Run(delegate
					{
						try
						{
							string jsonForSaveBytes2 = GetJsonForSaveBytes(fileBytes);
							SaveGameData saveGameData = SaveDataUtility.DeserializeSaveData<SaveGameData>(jsonForSaveBytes2);
							if (callback != null)
							{
								CoreLoop.InvokeSafe(delegate
								{
									SaveStats saveStatsFromData = GetSaveStatsFromData(saveGameData);
									callback(saveStatsFromData, null);
								});
							}
						}
						catch (Exception ex3)
						{
							Exception ex4 = ex3;
							Exception e2 = ex4;
							if (callback != null)
							{
								CoreLoop.InvokeSafe(delegate
								{
									callback(null, e2.ToString());
								});
							}
						}
					});
				}
				else
				{
					SaveDataUtility.AddTaskToAsyncQueue(delegate
					{
						try
						{
							string jsonForSaveBytes3 = GetJsonForSaveBytes(fileBytes);
							SaveStats saveStats4 = GetLoadedSaveSlotData(jsonForSaveBytes3);
							if (callback != null)
							{
								CoreLoop.InvokeSafe(delegate
								{
									callback(saveStats4, null);
								});
							}
						}
						catch (Exception ex5)
						{
							Exception ex6 = ex5;
							Exception e3 = ex6;
							if (callback != null)
							{
								CoreLoop.InvokeSafe(delegate
								{
									callback(null, e3.ToString());
								});
							}
						}
					});
				}
			}
		});
	}

	public string GetJsonForSaveBytes(byte[] fileBytes)
	{
		if (gameConfig.useSaveEncryption && !Platform.Current.IsFileSystemProtected)
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			MemoryStream serializationStream = new MemoryStream(fileBytes);
			return Encryption.Decrypt((string)binaryFormatter.Deserialize(serializationStream));
		}
		return Encoding.UTF8.GetString(fileBytes);
	}

	public static string GetJsonForSaveBytesStatic(byte[] fileBytes)
	{
		if ((bool)instance)
		{
			return instance.GetJsonForSaveBytes(fileBytes);
		}
		if (!Platform.Current.IsFileSystemProtected)
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			MemoryStream serializationStream = new MemoryStream(fileBytes);
			return Encryption.Decrypt((string)binaryFormatter.Deserialize(serializationStream));
		}
		return Encoding.UTF8.GetString(fileBytes);
	}

	public byte[] GetBytesForSaveJson(string jsonData)
	{
		byte[] result;
		if (gameConfig.useSaveEncryption && !Platform.Current.IsFileSystemProtected)
		{
			string graph = Encryption.Encrypt(jsonData);
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			MemoryStream memoryStream = new MemoryStream();
			binaryFormatter.Serialize(memoryStream, graph);
			result = memoryStream.ToArray();
			memoryStream.Close();
		}
		else
		{
			result = Encoding.UTF8.GetBytes(jsonData);
		}
		return result;
	}

	public void GetBytesForSaveJsonAsync(string jsonData, Action<byte[]> callback)
	{
		SaveDataUtility.AddTaskToAsyncQueue(delegate
		{
			byte[] bytes = GetBytesForSaveJson(jsonData);
			if (callback != null)
			{
				CoreLoop.InvokeSafe(delegate
				{
					callback(bytes);
				});
			}
		});
	}

	public static byte[] GetBytesForSaveJsonStatic(string jsonData)
	{
		if ((bool)instance)
		{
			return instance.GetBytesForSaveJson(jsonData);
		}
		Debug.LogError("Missing Game Manager. Using fallback get bytes method.");
		byte[] result;
		if (!Platform.Current.IsFileSystemProtected)
		{
			string graph = Encryption.Encrypt(jsonData);
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			MemoryStream memoryStream = new MemoryStream();
			binaryFormatter.Serialize(memoryStream, graph);
			result = memoryStream.ToArray();
			memoryStream.Close();
		}
		else
		{
			result = Encoding.UTF8.GetBytes(jsonData);
		}
		return result;
	}

	public static string GetJson<T>(T dataClassInstance)
	{
		return SaveDataUtility.SerializeSaveData(dataClassInstance);
	}

	public static byte[] DataToBytes<T>(T dataClassInstance)
	{
		return GetBytesForSaveJsonStatic(GetJson(dataClassInstance));
	}

	public static T BytesToData<T>(byte[] bytes) where T : new()
	{
		return SaveDataUtility.DeserializeSaveData<T>(GetJsonForSaveBytesStatic(bytes));
	}

	public byte[] GetBytesForSaveData(SaveGameData saveGameData)
	{
		string json = GetJson(saveGameData);
		return GetBytesForSaveJson(json);
	}

	private SaveStats GetLoadedSaveSlotData(string jsonData)
	{
		try
		{
			SaveGameData saveGameData = SaveDataUtility.DeserializeSaveData<SaveGameData>(jsonData);
			return new SaveStats(saveGameData.playerData, saveGameData);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			return SaveStats.Blank;
		}
	}

	public static SaveStats GetSaveStatsFromData(SaveGameData saveGameData)
	{
		return new SaveStats(saveGameData.playerData, saveGameData);
	}

	public IEnumerator PauseGameToggleByMenu()
	{
		yield return null;
		IEnumerator iterator = PauseGameToggle(playSound: false);
		while (iterator.MoveNext())
		{
			yield return iterator.Current;
		}
	}

	public IEnumerator PauseGameToggle(bool playSound)
	{
		if (TimeSlowed || GenericMessageCanvas.IsActive)
		{
			yield break;
		}
		bool flag = (bool)InteractManager.BlockingInteractable && !playerData.atBench;
		if (!playerData.disablePause && GameState == GameState.PLAYING && !flag)
		{
			isPaused = true;
			ui.SetState(UIState.PAUSED);
			SetPausedState(value: true);
			SetState(GameState.PAUSED);
			if (HeroController.instance != null)
			{
				HeroController.instance.Pause();
			}
			gameCams.MoveMenuToHUDCamera();
			inputHandler.PreventPause();
			inputHandler.StopUIInput();
			if (playSound)
			{
				ui.uiAudioPlayer.PlayPause();
			}
			yield return new WaitForSecondsRealtime(0.3f);
			inputHandler.AllowPause();
		}
		else if (GameState == GameState.PAUSED)
		{
			isPaused = false;
			inputHandler.PreventPause();
			ui.SetState(UIState.PLAYING);
			SetPausedState(value: false);
			SetState(GameState.PLAYING);
			if (HeroController.instance != null)
			{
				HeroController.instance.UnPause();
			}
			MenuButtonList.ClearAllLastSelected();
			if (playSound)
			{
				ui.uiAudioPlayer.PlayUnpause();
			}
			yield return new WaitForSecondsRealtime(0.3f);
			inputHandler.AllowPause();
		}
	}

	private void SetPausedState(bool value)
	{
		if (value)
		{
			gameCams.StopCameraShake();
			actorSnapshotPaused.TransitionToSafe(0f);
			ui.AudioGoToPauseMenu(0.2f);
			SetTimeScale(0f);
			GlobalSettings.Camera.MainCameraShakeManager.ApplyOffsets();
		}
		else
		{
			gameCams.ResumeCameraShake();
			actorSnapshotUnpaused.TransitionToSafe(0f);
			ui.AudioGoToGameplay(0.2f);
			SetTimeScale(1f);
		}
		this.GamePausedChange?.Invoke(value);
	}

	private IEnumerator SetTimeScale(float newTimeScale, float duration)
	{
		float lastTimeScale = TimeManager.TimeScale;
		for (float timer = 0f; timer < duration; timer += Time.unscaledDeltaTime)
		{
			float t = Mathf.Clamp01(timer / duration);
			SetTimeScale(Mathf.Lerp(lastTimeScale, newTimeScale, t));
			yield return null;
		}
		SetTimeScale(newTimeScale);
	}

	public void SetTimeScale(float newTimeScale)
	{
		if (timeSlowedCount > 1)
		{
			newTimeScale = Mathf.Min(newTimeScale, TimeManager.TimeScale);
		}
		TimeManager.TimeScale = ((newTimeScale > 0.01f) ? newTimeScale : 0f);
	}

	private IEnumerator SetTimeScale(TimeManager.TimeControlInstance controlInstance, float newTimeScale, float duration)
	{
		float lastTimeScale = controlInstance.TimeScale;
		for (float timer = 0f; timer < duration; timer += Time.unscaledDeltaTime)
		{
			float t = Mathf.Clamp01(timer / duration);
			controlInstance.TimeScale = Mathf.Lerp(lastTimeScale, newTimeScale, t);
			yield return null;
		}
		controlInstance.TimeScale = newTimeScale;
	}

	public void FreezeMoment(int type)
	{
		FreezeMoment((FreezeMomentTypes)type);
	}

	public void FreezeMoment(FreezeMomentTypes type, Action onFinish = null)
	{
		switch (type)
		{
		case FreezeMomentTypes.HeroDamage:
			StartCoroutine(FreezeMoment(0.01f, 0.28f, 0.1f, 0f, onFinish));
			break;
		case FreezeMomentTypes.EnemyDeath:
			StartCoroutine(FreezeMoment(0.04f, 0.024f, 0.04f, 0f, onFinish));
			break;
		case FreezeMomentTypes.BossDeathStrike:
			StartCoroutine(FreezeMoment(0f, 0.35f, 0.1f, 0f, onFinish));
			break;
		case FreezeMomentTypes.NailClashEffect:
			StartCoroutine(FreezeMoment(0.01f, 0.25f, 0.1f, 0f, onFinish));
			break;
		case FreezeMomentTypes.EnemyDeathShort:
			StartCoroutine(FreezeMoment(0.04f, 0.015f, 0.04f, 0f, onFinish));
			break;
		case FreezeMomentTypes.BossStun:
			StartCoroutine(FreezeMoment(0f, 0.25f, 0.1f, 0f, onFinish));
			break;
		case FreezeMomentTypes.QuickFreeze:
			StartCoroutine(FreezeMoment(0.0001f, 0.02f, 0.0001f, 0.0001f, onFinish));
			break;
		case FreezeMomentTypes.ZapFreeze:
			StartCoroutine(FreezeMoment(0f, 0.1f, 0f, 0f, onFinish));
			break;
		case FreezeMomentTypes.WitchBindHit:
			StartCoroutine(FreezeMoment(0.04f, 0.03f, 0.04f, 0f, onFinish));
			break;
		case FreezeMomentTypes.HeroDamageShort:
			StartCoroutine(FreezeMoment(0.001f, 0.15f, 0.05f, 0f, onFinish));
			break;
		case FreezeMomentTypes.BossDeathSlow:
			StartCoroutine(FreezeMoment(0.1f, 1.15f, 0.1f, 0.05f, onFinish));
			break;
		case FreezeMomentTypes.RaceWinSlow:
			StartCoroutine(FreezeMoment(0.5f, 3f, 0.3f, 0.1f, onFinish));
			break;
		case FreezeMomentTypes.EnemyBattleEndSlow:
			StartCoroutine(FreezeMoment(0.1f, 1f, 0.75f, 0.25f, onFinish));
			break;
		case FreezeMomentTypes.BigEnemyDeathSlow:
			StartCoroutine(FreezeMoment(0.04f, 0.06f, 0.04f, 0f, onFinish));
			break;
		case FreezeMomentTypes.BindBreak:
			StartCoroutine(FreezeMoment(0.01f, 0.4f, 0.1f, 0f, onFinish));
			break;
		}
	}

	public IEnumerator FreezeMoment(float rampDownTime, float waitTime, float rampUpTime, float targetSpeed, Action onFinish = null)
	{
		timeSlowedCount++;
		TimeManager.TimeControlInstance timeControl = TimeManager.CreateTimeControl(1f);
		try
		{
			yield return StartCoroutine(SetTimeScale(timeControl, targetSpeed, rampDownTime));
			for (float timer = 0f; timer < waitTime; timer += Time.unscaledDeltaTime)
			{
				yield return null;
			}
			yield return StartCoroutine(SetTimeScale(timeControl, 1f, rampUpTime));
		}
		finally
		{
			GameManager gameManager = this;
			timeControl.Release();
			gameManager.timeSlowedCount--;
			onFinish?.Invoke();
		}
	}

	public IEnumerator FreezeMomentGC(float rampDownTime, float waitTime, float rampUpTime, float targetSpeed)
	{
		timeSlowedCount++;
		yield return StartCoroutine(SetTimeScale(targetSpeed, rampDownTime));
		for (float timer = 0f; timer < waitTime; timer += Time.unscaledDeltaTime)
		{
			yield return null;
		}
		GCManager.Collect();
		yield return StartCoroutine(SetTimeScale(1f, rampUpTime));
		timeSlowedCount--;
	}

	public void EnsureSaveSlotSpace(Action<bool> callback)
	{
		Platform.Current.EnsureSaveSlotSpace(profileID, callback);
	}

	public void StartNewGame(bool permadeathMode = false, bool bossRushMode = false)
	{
		playerData = PlayerData.CreateNewSingleton(addEditorOverrides: false);
		playerData.permadeathMode = (permadeathMode ? PermadeathModes.On : PermadeathModes.Off);
		Platform.Current.PrepareForNewGame(profileID);
		if (bossRushMode)
		{
			playerData.AddGGPlayerDataOverrides();
			StartCoroutine(RunContinueGame());
		}
		else
		{
			StartCoroutine(RunStartNewGame());
		}
	}

	public IEnumerator RunStartNewGame()
	{
		ui.FadeScreenOut();
		noMusicSnapshot.TransitionToSafe(2f);
		noAtmosSnapshot.TransitionToSafe(2f);
		yield return new WaitForSeconds(2.6f);
		Platform.Current.SetSceneLoadState(isInProgress: true, isHighPriority: true);
		Platform.Current.DeleteVersionBackupsForSlot(profileID);
		ui.MakeMenuLean();
		AsyncOperationHandle<GameObject> handle = LoadGlobalPoolPrefab();
		yield return handle;
		UnityEngine.Object.Instantiate(handle.Result);
		ObjectPool.CreateStartupPools();
		BeginSceneTransition(new SceneLoadInfo
		{
			AlwaysUnloadUnusedAssets = true,
			IsFirstLevelForPlayer = true,
			PreventCameraFadeOut = true,
			WaitForSceneTransitionCameraFade = false,
			SceneName = "Opening_Sequence",
			Visualization = SceneLoadVisualizations.Custom
		});
	}

	public void ContinueGame()
	{
		StartCoroutine(RunContinueGame(IsMenuScene()));
	}

	public IEnumerator RunContinueGame(bool fromMenu = true)
	{
		if (fromMenu)
		{
			ui.FadeScreenOut();
			noMusicSnapshot.TransitionToSafe(2f);
			noAtmosSnapshot.TransitionToSafe(2f);
			yield return new WaitForSeconds(1f);
			ui.FadeOutBlackThreadLoop();
			yield return new WaitForSeconds(1.6f);
			audioManager.ApplyMusicCue(noMusicCue, 0f, 0f, applySnapshot: false);
			ui.MakeMenuLean();
		}
		else
		{
			SetPausedState(value: false);
			isPaused = false;
		}
		Platform.Current.SetSceneLoadState(isInProgress: true, isHighPriority: true);
		isLoading = true;
		SetState(GameState.LOADING);
		if (playerData.IsAct3IntroQueued)
		{
			loadVisualization = SceneLoadVisualizations.Custom;
			StartAct3();
		}
		else
		{
			loadVisualization = SceneLoadVisualizations.ContinueFromSave;
		}
		SaveDataUpgradeHandler.UpgradeSaveData(ref playerData);
		TimePassesLoadedIn();
		AsyncOperationHandle<GameObject> handle = LoadGlobalPoolPrefab();
		yield return handle;
		UnityEngine.Object.Instantiate(handle.Result);
		ObjectPool.CreateStartupPools();
		handle = LoadHeroPrefab();
		yield return handle;
		UnityEngine.Object.Instantiate(handle.Result);
		SetupSceneRefs(refreshTilemapInfo: false);
		yield return null;
		yield return null;
		Platform.Current.SetSceneLoadState(isInProgress: false);
		needFirstFadeIn = true;
		isLoading = false;
		if (hero_ctrl == null)
		{
			SetupHeroRefs();
		}
		if (hero_ctrl != null)
		{
			hero_ctrl.IgnoreInput();
		}
		if (playerData.IsAct3IntroQueued)
		{
			BeginSceneTransition(new SceneLoadInfo
			{
				AlwaysUnloadUnusedAssets = true,
				IsFirstLevelForPlayer = true,
				PreventCameraFadeOut = true,
				WaitForSceneTransitionCameraFade = false,
				SceneName = "Opening_Sequence_Act3",
				Visualization = SceneLoadVisualizations.Custom
			});
		}
		else
		{
			ReadyForRespawn(isFirstLevelForPlayer: true);
		}
	}

	public AsyncOperationHandle<GameObject> LoadGlobalPoolPrefab()
	{
		if (globalPoolPrefabHandle.IsValid())
		{
			return globalPoolPrefabHandle;
		}
		globalPoolPrefabHandle = Addressables.LoadAssetAsync<GameObject>("GlobalPool");
		return globalPoolPrefabHandle;
	}

	public void UnloadGlobalPoolPrefab()
	{
		if (globalPoolPrefabHandle.IsValid())
		{
			Addressables.Release(globalPoolPrefabHandle);
			globalPoolPrefabHandle = default(AsyncOperationHandle<GameObject>);
		}
	}

	public AsyncOperationHandle<GameObject> LoadHeroPrefab()
	{
		if (heroPrefabHandle.IsValid())
		{
			return heroPrefabHandle;
		}
		heroPrefabHandle = Addressables.LoadAssetAsync<GameObject>("Hero_Hornet");
		return heroPrefabHandle;
	}

	public void UnloadHeroPrefab()
	{
		if (heroPrefabHandle.IsValid())
		{
			Addressables.Release(heroPrefabHandle);
			heroPrefabHandle = default(AsyncOperationHandle<GameObject>);
		}
	}

	public IEnumerator ReturnToMainMenu(bool willSave, Action<bool> callback = null, bool isEndGame = false, bool forceMainMenu = false)
	{
		PersistentAudioManager.OnLeaveScene();
		inputHandler.PreventPause();
		VibrationManager.StopAllVibration();
		AwardQueuedAchievements();
		if (BossSequenceController.IsInSequence)
		{
			BossSequenceController.RestoreBindings();
		}
		TimePasses();
		if (willSave)
		{
			bool? saveComplete = null;
			SaveGame(delegate(bool didSave)
			{
				saveComplete = didSave;
			});
			while (!saveComplete.HasValue)
			{
				yield return null;
			}
			callback?.Invoke(saveComplete.Value);
			if (!forceMainMenu && !saveComplete.Value)
			{
				yield break;
			}
		}
		else
		{
			callback?.Invoke(obj: false);
		}
		string previousSceneName = SceneManager.GetActiveScene().name;
		AsyncOperationHandle<SceneInstance> opHandle = Addressables.LoadSceneAsync("Scenes/Quit_To_Menu", LoadSceneMode.Single, activateOnLoad: false);
		opHandle.Completed += delegate
		{
			ReportUnload(previousSceneName);
		};
		cameraCtrl.FreezeInPlace(freezeTarget: true);
		cameraCtrl.FadeOut(CameraFadeType.TO_MENU);
		silentSnapshot.TransitionToSafe(2.5f);
		for (float timer = 0f; timer < 2.5f; timer += Time.unscaledDeltaTime)
		{
			yield return null;
		}
		audioManager.StopAndClearMusic();
		audioManager.StopAndClearAtmos();
		EventRegister.SendEvent(EventRegisterEvents.FsmCancel);
		Platform.Current.SetSceneLoadState(isInProgress: true, isHighPriority: true);
		StandaloneLoadingSpinner standaloneLoadingSpinner = UnityEngine.Object.Instantiate(isEndGame ? standaloneLoadingSpinnerEndGamePrefab : standaloneLoadingSpinnerPrefab);
		standaloneLoadingSpinner.Setup(this);
		UnityEngine.Object.DontDestroyOnLoad(standaloneLoadingSpinner.gameObject);
		if (this.UnloadingLevel != null)
		{
			try
			{
				this.UnloadingLevel();
			}
			catch (Exception ex)
			{
				Debug.LogErrorFormat("Error while UnloadingLevel in QuitToMenu, attempting to continue regardless.");
				CheatManager.LastErrorText = ex.ToString();
				Debug.LogException(ex);
			}
		}
		if (this.NextSceneWillActivate != null)
		{
			try
			{
				this.NextSceneWillActivate();
			}
			catch (Exception ex2)
			{
				Debug.LogErrorFormat("Error while DestroyingPersonalPools in QuitToMenu, attempting to continue regardless.");
				CheatManager.LastErrorText = ex2.ToString();
				Debug.LogException(ex2);
			}
		}
		PlayMakerFSM.BroadcastEvent("QUIT TO MENU");
		waitForManualLevelStart = true;
		StaticVariableList.Clear();
		TrackingTrail.ClearStatic();
		sceneSeedTrackers.Clear();
		yield return opHandle;
		yield return opHandle.Result.ActivateAsync();
		inputHandler.AllowPause();
		didEmergencyQuit = false;
	}

	public void ReturnToMainMenuNoSave()
	{
		StartCoroutine(ReturnToMainMenu(willSave: false));
	}

	private void EmergencyReturnToMenu()
	{
		didEmergencyQuit = true;
		StartCoroutine(ui.EmergencyReturnToMainMenu());
	}

	public void DoEmergencyQuit()
	{
		if (!didEmergencyQuit)
		{
			EmergencyReturnToMenu();
		}
	}

	public IEnumerator QuitGame()
	{
		FSMUtility.SendEventToGameObject(GameObject.Find("Quit Blanker"), "START FADE");
		yield return new WaitForSeconds(0.5f);
		Application.Quit();
	}

	public void LoadedBoss()
	{
		this.OnLoadedBoss?.Invoke();
	}

	public void DoDestroyPersonalPools()
	{
		this.NextSceneWillActivate?.Invoke();
	}

	public float GetImplicitCinematicVolume()
	{
		return Mathf.Clamp01(gameSettings.masterVolume / 10f) * Mathf.Clamp01(gameSettings.soundVolume / 10f);
	}

	public void SetIsInventoryOpen(bool value)
	{
		if (_inventoryInputBlocker == null)
		{
			_inventoryInputBlocker = new object();
		}
		PlayerData.instance.isInventoryOpen = value;
		SetPausedState(value);
		if (value)
		{
			hero_ctrl.AddInputBlocker(_inventoryInputBlocker);
		}
		else
		{
			hero_ctrl.RemoveInputBlocker(_inventoryInputBlocker);
		}
	}

	public bool CanPickupsExist()
	{
		if (!BossSceneController.IsBossScene)
		{
			return !IsMemoryScene();
		}
		return false;
	}

	public bool IsMemoryScene()
	{
		bool flag = forceCurrentSceneMemory || IsMemoryScene(GetCurrentMapZoneEnum());
		if (flag)
		{
			bool flag2 = sm != null;
			if (flag2)
			{
				GetSceneManager();
				flag2 = sm != null;
			}
			if (flag2 && sm.ForceNotMemory)
			{
				return false;
			}
		}
		return flag;
	}

	public static bool IsMemoryScene(MapZone mapZone)
	{
		if (mapZone == MapZone.CLOVER || mapZone == MapZone.MEMORY)
		{
			return true;
		}
		return false;
	}

	public void ForceCurrentSceneIsMemory(bool value)
	{
		forceCurrentSceneMemory = true;
	}
}

using System;
using System.Collections.Generic;
using GlobalEnums;
using InControl;
using UnityEngine;
using UnityEngine.Profiling;

public abstract class Platform : MonoBehaviour
{
	public enum ResolutionModes
	{
		Native = 0,
		Scale = 1,
		NativeHUDScaledMain = 2
	}

	[Flags]
	public enum ScreenModeState
	{
		Standard = 0,
		HandHeld = 2,
		HandHeldSmall = 4,
		IncludeFutureFlags = int.MinValue
	}

	[Flags]
	public enum HandHeldTypes
	{
		None = 0,
		Switch = 1,
		SteamDeck = 4,
		IncludeFutureFlags = int.MinValue
	}

	public delegate void ScreenModeChanged(ScreenModeState screenMode);

	public delegate void SaveStoreMountEvent(bool mounted);

	protected enum SaveSlotFileNameUsage
	{
		Primary = 0,
		Backup = 1,
		BackupMarkedForDeletion = 2
	}

	public interface ISharedData
	{
		bool HasKey(string key);

		void DeleteKey(string key);

		void DeleteAll();

		void ImportData(ISharedData otherData);

		bool GetBool(string key, bool def);

		void SetBool(string key, bool val);

		int GetInt(string key, int def);

		void SetInt(string key, int val);

		float GetFloat(string key, float def);

		void SetFloat(string key, float val);

		string GetString(string key, string def);

		void SetString(string key, string val);

		void Save();
	}

	public abstract class ImportDataInfo
	{
		public int slot;

		public bool isSaveSlot;

		public abstract void Save(int slot, Action<bool> callback = null);
	}

	public struct ImportDataResult
	{
		public int importedCount;

		public int importTotal;

		public bool success;
	}

	public delegate void AchievementsFetchedDelegate();

	public enum GraphicsTiers
	{
		VeryLow = 0,
		Low = 1,
		Medium = 2,
		High = 3
	}

	public delegate void GraphicsTierChangedDelegate(GraphicsTiers graphicsTier);

	public enum AcceptRejectInputStyles
	{
		NonJapaneseStyle = 0,
		JapaneseStyle = 1
	}

	public delegate void AcceptRejectInputStyleChangedDelegate();

	public enum MenuActions
	{
		None = 0,
		Submit = 1,
		Cancel = 2,
		Extra = 3,
		Super = 4
	}

	public enum EngagementRequirements
	{
		Invisible = 0,
		MustDisplay = 1
	}

	public enum EngagementStates
	{
		NotEngaged = 0,
		EngagePending = 1,
		Engaged = 2
	}

	public interface IDisengageHandler
	{
		void OnDisengage(Action next);
	}

	public delegate void OnEngagedDisplayInfoChanged();

	protected ScreenModeState screenMode;

	public const int SaveSlotCount = 5;

	protected const string FirstSaveFileName = "user.dat";

	protected const string NonFirstSaveFileNameFormat = "user{0}.dat";

	protected const string BackupSuffix = ".bak";

	protected const string BackupMarkedForDeletionSuffix = ".del";

	private ResolutionModes resolutionMode;

	private ResolutionModes? resolutionModeOverride;

	private GraphicsTiers graphicsTier;

	protected bool canRestore;

	protected int vSyncCount;

	protected int originalFrameRate;

	protected Resolution originalResolution;

	protected FullScreenMode originalFullScreenMode;

	private ThreadPriority? originalLoadingPriority;

	private IDisengageHandler disengageHandler;

	private static Platform current;

	public abstract string DisplayName { get; }

	public abstract bool ShowLanguageSelect { get; }

	public virtual bool IsRunningOnHandHeld => ScreenMode >= ScreenModeState.HandHeld;

	public virtual ScreenModeState ScreenMode
	{
		get
		{
			return screenMode;
		}
		set
		{
			SetScreenMode(value);
		}
	}

	public virtual HandHeldTypes HandHeldType => HandHeldTypes.None;

	public virtual float EnterSceneWait => 0f;

	public virtual bool IsFileSystemProtected => false;

	public virtual bool WillPreloadSaveFiles => true;

	public virtual bool ShowSaveFileWriteIcon => true;

	public virtual bool IsFileWriteLimited => false;

	public virtual bool IsSaveStoreMounted => true;

	public virtual bool IsSharedDataMounted => true;

	public abstract ISharedData LocalSharedData { get; }

	public abstract ISharedData RoamingSharedData { get; }

	public virtual string UserDataDirectory => Application.persistentDataPath;

	public virtual SaveRestoreHandler SaveRestoreHandler { get; }

	public virtual bool ShowSaveDataImport => false;

	public virtual string SaveImportLabel => "Save Import";

	public virtual bool IsFiringAchievementsFromSavesAllowed => true;

	public abstract bool AreAchievementsFetched { get; }

	public virtual bool HasNativeAchievementsDialog => false;

	public virtual bool WillManageResolution => false;

	public virtual bool WillDisplayGraphicsSettings => true;

	public virtual bool LimitedGraphicsSettings => false;

	public ResolutionModes ResolutionMode
	{
		get
		{
			return resolutionModeOverride ?? resolutionMode;
		}
		set
		{
			resolutionMode = value;
			ChangeGraphicsTier(GraphicsTier, isForced: true);
		}
	}

	protected virtual GraphicsTiers InitialGraphicsTier => GraphicsTiers.High;

	public GraphicsTiers GraphicsTier => graphicsTier;

	public virtual bool IsSpriteScalingApplied => false;

	public virtual float SpriteScalingFactor => 1f;

	public virtual int DefaultHudSetting => 0;

	public virtual int ExtendedVideoBlankFrames => 0;

	public virtual int MaxVideoFrameRate => 60;

	public virtual bool WillDisplayControllerSettings => true;

	public virtual bool WillDisplayKeyboardSettings => true;

	public virtual bool WillDisplayQuitButton => true;

	public virtual bool IsControllerImplicit => false;

	public virtual bool IsMouseSupported => true;

	public virtual bool WillEverPauseOnControllerDisconnected => false;

	public virtual bool IsPausingOnControllerDisconnected => false;

	public virtual bool SupportsVibrationFromAudio => false;

	public abstract AcceptRejectInputStyles AcceptRejectInputStyle { get; }

	public bool WasLastInputKeyboard
	{
		get
		{
			InputHandler instance = ManagerSingleton<InputHandler>.Instance;
			if (instance.lastActiveController != BindingSourceType.KeyBindingSource)
			{
				return instance.lastActiveController == BindingSourceType.MouseBindingSource;
			}
			return true;
		}
	}

	public virtual bool FetchScenesBeforeFade => !CheatManager.DisableAsyncSceneLoad;

	public virtual float MaximumLoadDurationForNonCriticalGarbageCollection => 0f;

	public virtual int MaximumSceneTransitionsWithoutNonCriticalGarbageCollection => 0;

	protected virtual bool ChangesBackgroundLoadingPriority => true;

	public static bool UseFieldInfoCache { get; set; } = true;

	public virtual EngagementRequirements EngagementRequirement => EngagementRequirements.Invisible;

	public virtual EngagementStates EngagementState => EngagementStates.Engaged;

	public virtual bool IsSavingAllowedByEngagement => true;

	public virtual bool CanReEngage => false;

	public virtual string EngagedDisplayName => null;

	public virtual Texture2D EngagedDisplayImage => null;

	public IDisengageHandler DisengageHandler => disengageHandler;

	public virtual bool IsPlayerPrefsLoaded => true;

	public virtual bool RequiresPreferencesSyncOnEngage => false;

	public static Platform Current => current;

	protected static bool IsPlatformSimulationEnabled => false;

	public static event Action PlatformBecameCurrent;

	public event ScreenModeChanged OnScreenModeChanged;

	public static event SaveStoreMountEvent OnSaveStoreStateChanged;

	public static event AchievementsFetchedDelegate AchievementsFetched;

	public static event GraphicsTierChangedDelegate GraphicsTierChanged;

	public static event AcceptRejectInputStyleChangedDelegate AcceptRejectInputStyleChanged;

	public event OnEngagedDisplayInfoChanged EngagedDisplayInfoChanged;

	public virtual SystemLanguage GetSystemLanguage()
	{
		return Application.systemLanguage;
	}

	public virtual int GetEstimateAllocatableMemoryMB()
	{
		long totalReservedMemoryLong = Profiler.GetTotalReservedMemoryLong();
		long totalUnusedReservedMemoryLong = Profiler.GetTotalUnusedReservedMemoryLong();
		int num = (int)((totalReservedMemoryLong - totalUnusedReservedMemoryLong) / 1024 / 1024);
		return Mathf.Max(b: SystemInfo.systemMemorySize / 2, a: (int)(totalReservedMemoryLong / 1024 / 1024)) - num;
	}

	protected void SetScreenMode(ScreenModeState newState)
	{
		if (screenMode != newState)
		{
			screenMode = newState;
			this.OnScreenModeChanged?.Invoke(newState);
		}
	}

	public bool IsTargetHandHeld(HandHeldTypes targetType)
	{
		if (targetType == HandHeldTypes.None)
		{
			return true;
		}
		return (HandHeldType & targetType) != 0;
	}

	public void NotifySaveMountStateChanged(bool mounted)
	{
		if (Platform.OnSaveStoreStateChanged != null)
		{
			CoreLoop.InvokeSafe(delegate
			{
				Platform.OnSaveStoreStateChanged(mounted);
			});
		}
	}

	public virtual void LoadSharedDataAndNotify(bool mounted)
	{
		NotifySaveMountStateChanged(mounted);
	}

	public virtual void MountSaveStore()
	{
	}

	public static bool IsSaveSlotIndexValid(int slotIndex)
	{
		if (slotIndex >= 0)
		{
			return slotIndex < 5;
		}
		return false;
	}

	protected string GetSaveSlotFileName(int slotIndex, SaveSlotFileNameUsage usage)
	{
		string text = ((slotIndex != 0) ? $"user{slotIndex}.dat" : "user.dat");
		switch (usage)
		{
		case SaveSlotFileNameUsage.Backup:
			text += ".bak";
			break;
		case SaveSlotFileNameUsage.BackupMarkedForDeletion:
			text += ".del";
			break;
		}
		return text;
	}

	public virtual void PrepareForNewGame(int slotIndex)
	{
	}

	public virtual void OnSetGameData(int slotIndex)
	{
	}

	public abstract void IsSaveSlotInUse(int slotIndex, Action<bool> callback);

	public abstract void ReadSaveSlot(int slotIndex, Action<byte[]> callback);

	public abstract void EnsureSaveSlotSpace(int slotIndex, Action<bool> callback);

	public abstract void WriteSaveSlot(int slotIndex, byte[] binary, Action<bool> callback);

	public abstract void ClearSaveSlot(int slotIndex, Action<bool> callback);

	public virtual void SaveScreenCapture(Texture2D texture2D, Action<bool> callback)
	{
		Debug.LogError("SaveScreenCapture not implemented");
		callback?.Invoke(obj: false);
	}

	public virtual void LoadScreenCaptures(Action<Texture2D[]> captures)
	{
		Debug.LogError("LoadScreenCaptures not implemented");
		captures(null);
	}

	public void CreateSaveRestorePoint(int slot, string identifier, bool noTrim, byte[] bytes, Action<bool> callback = null)
	{
		if (SaveRestoreHandler == null)
		{
			Debug.LogError("Unable to create save restore point. Missing Save Restore Handler.", this);
			callback?.Invoke(obj: false);
			return;
		}
		Action<bool> callback2 = ((callback == null) ? null : ((Action<bool>)delegate(bool success)
		{
			CoreLoop.InvokeSafe(delegate
			{
				callback(success);
			});
		}));
		SaveRestoreHandler.WriteSaveRestorePoint(slot, identifier, noTrim, bytes, callback2);
	}

	public void WriteSaveBackup(int slot, byte[] bytes, Action<bool> callback = null)
	{
		if (SaveRestoreHandler == null)
		{
			Debug.LogError("Unable to write save backup point. Missing Save Restore Handler.", this);
			callback?.Invoke(obj: false);
			return;
		}
		Action<bool> callback2 = ((callback == null) ? null : ((Action<bool>)delegate(bool success)
		{
			CoreLoop.InvokeSafe(delegate
			{
				callback(success);
			});
		}));
		SaveRestoreHandler.WriteVersionBackup(slot, bytes, callback2);
	}

	public FetchDataRequest FetchRestorePoints(int slot)
	{
		if (SaveRestoreHandler == null)
		{
			Debug.LogError("Unable to fetch save restore point. Missing Save Restore Handler.", this);
			return FetchDataRequest.Error;
		}
		return SaveRestoreHandler.FetchRestorePoints(slot);
	}

	public FetchDataRequest FetchVersionRestorePoints(int slot)
	{
		if (SaveRestoreHandler == null)
		{
			Debug.LogError("Unable to fetch version restore point. Missing Save Restore Handler.", this);
			return FetchDataRequest.Error;
		}
		return SaveRestoreHandler.FetchVersionBackupPoints(slot);
	}

	public void DeleteRestorePointsForSlot(int slot, Action<bool> callback = null)
	{
		if (SaveRestoreHandler == null)
		{
			Debug.LogError("Unable to delete save restore point. Missing Save Restore Handler.", this);
			callback?.Invoke(obj: false);
			return;
		}
		Action<bool> callback2 = ((callback == null) ? null : ((Action<bool>)delegate(bool success)
		{
			CoreLoop.InvokeSafe(delegate
			{
				callback(success);
			});
		}));
		SaveRestoreHandler.DeleteRestorePoints(slot, callback2);
	}

	public void DeleteVersionBackupsForSlot(int slot, Action<bool> callback = null)
	{
		if (SaveRestoreHandler == null)
		{
			Debug.LogError("Unable to delete backup files from previous versions. Missing Save Restore Handler.", this);
			callback?.Invoke(obj: false);
			return;
		}
		Action<bool> callback2 = ((callback == null) ? null : ((Action<bool>)delegate(bool success)
		{
			CoreLoop.InvokeSafe(delegate
			{
				callback(success);
			});
		}));
		SaveRestoreHandler.DeleteVersionBackups(slot, callback2);
	}

	public virtual void FetchImportData(Action<List<ImportDataInfo>> callback)
	{
		callback?.Invoke(null);
	}

	public virtual void DisplayImportDataResultMessage(ImportDataResult importDataResult, Action callback = null)
	{
		callback?.Invoke();
	}

	public virtual void CloseSystemDialogs(Action callback = null)
	{
		callback?.Invoke();
	}

	public virtual void AdjustGameSettings(GameSettings gameSettings)
	{
	}

	public virtual bool TryGetAchievementState(string achievementId, out AchievementState state)
	{
		bool? flag = IsAchievementUnlocked(achievementId);
		if (!flag.HasValue)
		{
			state = default(AchievementState);
			return false;
		}
		state = new AchievementState
		{
			isValid = true,
			isUnlocked = flag.Value
		};
		return true;
	}

	public abstract bool? IsAchievementUnlocked(string achievementId);

	public abstract void PushAchievementUnlock(string achievementId);

	public virtual void UpdateAchievementProgress(string achievementId, int value, int max)
	{
	}

	public abstract void ResetAchievements();

	protected void OnAchievementsFetched()
	{
		if (Platform.AchievementsFetched != null)
		{
			Platform.AchievementsFetched();
		}
	}

	public virtual void ShowNativeAchievementsDialog()
	{
	}

	public virtual void SetSocialPresence(string socialStatusKey, bool isActive)
	{
	}

	public virtual void AddSocialStat(string name, int amount)
	{
	}

	public virtual void FlushSocialEvents()
	{
	}

	public virtual void UpdateLocation(string location)
	{
	}

	public virtual void UpdatePlayTime(float playTime)
	{
	}

	public void DropResolutionModeInScene(ResolutionModes newMode)
	{
		switch (newMode)
		{
		case ResolutionModes.Scale:
		{
			ResolutionModes resolutionModes = ResolutionMode;
			if (resolutionModes == ResolutionModes.Native || resolutionModes == ResolutionModes.NativeHUDScaledMain)
			{
				resolutionModeOverride = newMode;
			}
			break;
		}
		case ResolutionModes.NativeHUDScaledMain:
			if (ResolutionMode == ResolutionModes.Native)
			{
				resolutionModeOverride = newMode;
			}
			break;
		}
		ChangeGraphicsTier(GraphicsTier, isForced: true);
	}

	public virtual void AdjustGraphicsSettings(GameSettings gameSettings)
	{
	}

	protected void ChangeGraphicsTier(GraphicsTiers graphicsTier, bool isForced)
	{
		if (this.graphicsTier != graphicsTier || isForced)
		{
			this.graphicsTier = graphicsTier;
			RefreshGraphicsTier();
		}
	}

	public void RefreshGraphicsTier()
	{
		OnGraphicsTierChanged(graphicsTier);
	}

	protected virtual void OnGraphicsTierChanged(GraphicsTiers graphicsTier)
	{
		Shader.globalMaximumLOD = GetMaximumShaderLOD(graphicsTier);
		if (Platform.GraphicsTierChanged != null)
		{
			Platform.GraphicsTierChanged(graphicsTier);
		}
	}

	public static int GetMaximumShaderLOD(GraphicsTiers graphicsTier)
	{
		return graphicsTier switch
		{
			GraphicsTiers.VeryLow => 700, 
			GraphicsTiers.Low => 800, 
			GraphicsTiers.Medium => 900, 
			GraphicsTiers.High => 1000, 
			_ => int.MaxValue, 
		};
	}

	public virtual GamepadType OverrideGamepadDisplay(GamepadType currentGamepadType)
	{
		return currentGamepadType;
	}

	public virtual void SetTargetFrameRate(int frameRate)
	{
		if (frameRate == 0)
		{
			Debug.LogError("Cannot set target frame rate to 0.");
			return;
		}
		Resolution currentResolution = Screen.currentResolution;
		RecordFrameRate(currentResolution);
		int num = Mathf.RoundToInt((float)currentResolution.refreshRateRatio.value);
		if (num % frameRate == 0)
		{
			QualitySettings.vSyncCount = num / frameRate;
		}
		else
		{
			QualitySettings.vSyncCount = 0;
		}
		Application.targetFrameRate = frameRate;
	}

	protected virtual bool RecordFrameRate(Resolution currentResolution)
	{
		if (canRestore)
		{
			return false;
		}
		originalResolution = currentResolution;
		vSyncCount = QualitySettings.vSyncCount;
		originalFrameRate = Application.targetFrameRate;
		originalFullScreenMode = Screen.fullScreenMode;
		canRestore = true;
		return true;
	}

	public virtual bool RestoreFrameRate()
	{
		if (!canRestore)
		{
			return false;
		}
		QualitySettings.vSyncCount = vSyncCount;
		Application.targetFrameRate = originalFrameRate;
		canRestore = false;
		return true;
	}

	public MenuActions GetMenuAction(HeroActions ia, bool ignoreAttack = false, bool isContinuous = false)
	{
		return GetMenuAction(GetPressedState(ia.MenuSubmit, isContinuous), GetPressedState(ia.MenuCancel, isContinuous), GetPressedState(ia.Jump, isContinuous), !ignoreAttack && GetPressedState(ia.Attack, isContinuous), GetPressedState(ia.Cast, isContinuous), GetPressedState(ia.MenuExtra, isContinuous), ia.MenuSuper.WasPressed, ia.Dash.WasPressed, ia.DreamNail.WasPressed);
	}

	private bool GetPressedState(PlayerAction action, bool isContinuous)
	{
		if (!isContinuous)
		{
			return action.WasPressed;
		}
		return action.IsPressed;
	}

	public MenuActions GetMenuAction(bool menuSubmitInput, bool menuCancelInput, bool jumpInput, bool attackInput, bool castInput, bool menuExtraInput = false, bool menuSuperInput = false, bool dashInput = false, bool dreamNailInput = false)
	{
		if (WasLastInputKeyboard)
		{
			if (menuSubmitInput || jumpInput)
			{
				return MenuActions.Submit;
			}
			if (menuCancelInput || attackInput || castInput)
			{
				return MenuActions.Cancel;
			}
			if (menuExtraInput || dashInput)
			{
				return MenuActions.Extra;
			}
			if (menuSuperInput || dreamNailInput)
			{
				return MenuActions.Super;
			}
		}
		else
		{
			if (menuSubmitInput)
			{
				return MenuActions.Submit;
			}
			if (menuCancelInput)
			{
				return MenuActions.Cancel;
			}
			if (menuExtraInput)
			{
				return MenuActions.Extra;
			}
			if (menuSuperInput)
			{
				return MenuActions.Super;
			}
		}
		return MenuActions.None;
	}

	public virtual void SetSceneLoadState(bool isInProgress, bool isHighPriority = false)
	{
		if (isInProgress && isHighPriority)
		{
			CheatManager.BoostModeActive = true;
			if (ChangesBackgroundLoadingPriority)
			{
				SetBackgroundLoadingPriority(ThreadPriority.High);
			}
			GameCameras instance = GameCameras.instance;
			if ((bool)instance && (bool)instance.mainCamera)
			{
				instance.SetMainCameraActive(value: false);
			}
		}
		else
		{
			CheatManager.BoostModeActive = false;
			if (ChangesBackgroundLoadingPriority)
			{
				RestoreBackgroundLoadingPriority();
			}
			GameCameras instance2 = GameCameras.instance;
			if ((bool)instance2 && (bool)instance2.mainCamera)
			{
				instance2.SetMainCameraActive(value: true);
			}
		}
	}

	public void SetBackgroundLoadingPriority(ThreadPriority threadPriority)
	{
		if (!originalLoadingPriority.HasValue)
		{
			originalLoadingPriority = Application.backgroundLoadingPriority;
		}
		Application.backgroundLoadingPriority = threadPriority;
	}

	public void RestoreBackgroundLoadingPriority()
	{
		if (originalLoadingPriority.HasValue)
		{
			Application.backgroundLoadingPriority = originalLoadingPriority.Value;
		}
	}

	public virtual void SetGameManagerReady()
	{
	}

	public virtual void OnScreenFaded()
	{
		if (resolutionModeOverride.HasValue)
		{
			resolutionModeOverride = null;
			ChangeGraphicsTier(GraphicsTier, isForced: true);
		}
	}

	public virtual void ClearEngagement()
	{
	}

	public virtual void BeginEngagement()
	{
	}

	public virtual void UpdateWaitingForEngagement()
	{
	}

	public virtual void SetDisengageHandler(IDisengageHandler disengageHandler)
	{
		this.disengageHandler = disengageHandler;
	}

	public void NotifyEngagedDisplayInfoChanged()
	{
		if (this.EngagedDisplayInfoChanged != null)
		{
			this.EngagedDisplayInfoChanged();
		}
	}

	protected virtual void Awake()
	{
		current = this;
		ChangeGraphicsTier(InitialGraphicsTier, isForced: true);
		if (!IsMouseSupported)
		{
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		}
	}

	protected virtual void OnDestroy()
	{
	}

	protected virtual void Update()
	{
	}

	protected virtual void BecomeCurrent()
	{
		current = this;
		if (Platform.PlatformBecameCurrent != null)
		{
			Platform.PlatformBecameCurrent();
		}
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void Init()
	{
		CreatePlatform().BecomeCurrent();
	}

	private static Platform CreatePlatform()
	{
		return CreatePlatform<DesktopPlatform>();
	}

	private static Platform CreatePlatform<PlatformTy>() where PlatformTy : Platform
	{
		GameObject obj = new GameObject(typeof(PlatformTy).Name);
		PlatformTy result = obj.AddComponent<PlatformTy>();
		UnityEngine.Object.DontDestroyOnLoad(obj);
		return result;
	}
}

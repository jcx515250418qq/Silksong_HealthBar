using System;
using System.Collections;
using TeamCherry.GameCore;
using UnityEngine;

public sealed class XBoxConsolePlatform : Platform, VibrationManager.IVibrationMixerProvider
{
	private GameCoreSharedData localSharedData;

	private GameCoreSharedData sharedData;

	private AchievementIDMap achievementIDMap;

	private bool isPlayerPrefsLoaded;

	private PlatformVibrationHelper platformVibrationHelper = new PlatformVibrationHelper();

	private Coroutine loadingSharedDataRoutine;

	private volatile bool hasMountedSharedData;

	private bool hasEverCalledResolution;

	public override string DisplayName => Application.platform.ToString();

	public override bool ShowLanguageSelect => false;

	public override bool IsFileSystemProtected => false;

	public override bool WillPreloadSaveFiles => false;

	public override bool ShowSaveFileWriteIcon => true;

	public override bool IsFileWriteLimited => false;

	public override bool IsSaveStoreMounted => true;

	public override bool IsSharedDataMounted => true;

	public override ISharedData LocalSharedData => localSharedData;

	public override ISharedData RoamingSharedData => sharedData;

	public override SaveRestoreHandler SaveRestoreHandler => null;

	public override bool IsFiringAchievementsFromSavesAllowed => false;

	public override bool AreAchievementsFetched => false;

	public override bool HasNativeAchievementsDialog => true;

	public override bool WillManageResolution => false;

	public override bool WillDisplayGraphicsSettings => true;

	public override bool LimitedGraphicsSettings => true;

	public override bool IsSpriteScalingApplied => false;

	public override int DefaultHudSetting => 0;

	public override int ExtendedVideoBlankFrames
	{
		get
		{
			if (Application.platform == RuntimePlatform.XboxOne || Application.platform == RuntimePlatform.GameCoreXboxOne)
			{
				return 1;
			}
			return base.ExtendedVideoBlankFrames;
		}
	}

	public override int MaxVideoFrameRate
	{
		get
		{
			if (Application.platform == RuntimePlatform.XboxOne || Application.platform == RuntimePlatform.GameCoreXboxOne)
			{
				return 30;
			}
			return base.MaxVideoFrameRate;
		}
	}

	public override float EnterSceneWait
	{
		get
		{
			if (Application.platform == RuntimePlatform.XboxOne)
			{
				return 0.2f;
			}
			return base.EnterSceneWait;
		}
	}

	public override bool WillDisplayControllerSettings => true;

	public override bool WillDisplayKeyboardSettings => false;

	public override bool WillDisplayQuitButton => false;

	public override bool IsControllerImplicit => true;

	public override bool IsMouseSupported => false;

	public override AcceptRejectInputStyles AcceptRejectInputStyle => AcceptRejectInputStyles.NonJapaneseStyle;

	public override EngagementRequirements EngagementRequirement => EngagementRequirements.MustDisplay;

	public override EngagementStates EngagementState => EngagementStates.Engaged;

	public override bool IsSavingAllowedByEngagement => true;

	public override bool CanReEngage => false;

	public override string EngagedDisplayName => string.Empty;

	public override Texture2D EngagedDisplayImage => null;

	public override bool IsPlayerPrefsLoaded => isPlayerPrefsLoaded;

	public override bool RequiresPreferencesSyncOnEngage => true;

	protected override void Awake()
	{
		base.Awake();
		achievementIDMap = Resources.Load<AchievementIDMap>("XB1AchievementMap");
		localSharedData = new GameCoreSharedData("localSharedData", "localSharedData.dat");
		sharedData = new GameCoreSharedData("sharedData", "sharedData.dat");
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		platformVibrationHelper.Destroy();
	}

	protected override void Update()
	{
		base.Update();
		platformVibrationHelper.UpdateVibration();
	}

	private void OnPlayerPrefsInitialised()
	{
		isPlayerPrefsLoaded = true;
	}

	public override void LoadSharedDataAndNotify(bool mounted)
	{
		CoreLoop.InvokeSafe(delegate
		{
			if (loadingSharedDataRoutine != null)
			{
				StopCoroutine(loadingSharedDataRoutine);
			}
			if (mounted)
			{
				loadingSharedDataRoutine = StartCoroutine(LoadSharedAndNotifyRoutine(mounted: true));
			}
			else
			{
				hasMountedSharedData = false;
				NotifySaveMountStateChanged(mounted: false);
			}
		});
	}

	private IEnumerator LoadSharedAndNotifyRoutine(bool mounted)
	{
		try
		{
			localSharedData.LoadData();
			sharedData.LoadData();
			while (!localSharedData.HasLoaded || !sharedData.HasLoaded)
			{
				yield return null;
			}
		}
		finally
		{
			XBoxConsolePlatform xBoxConsolePlatform = this;
			xBoxConsolePlatform.hasMountedSharedData = true;
			xBoxConsolePlatform.NotifySaveMountStateChanged(mounted);
			xBoxConsolePlatform.loadingSharedDataRoutine = null;
		}
	}

	public override SystemLanguage GetSystemLanguage()
	{
		return base.GetSystemLanguage();
	}

	public override void MountSaveStore()
	{
		base.MountSaveStore();
	}

	public override void PrepareForNewGame(int slotIndex)
	{
		base.PrepareForNewGame(slotIndex);
	}

	public override void IsSaveSlotInUse(int slotIndex, Action<bool> callback)
	{
		callback?.Invoke(obj: false);
	}

	public override void ReadSaveSlot(int slotIndex, Action<byte[]> callback)
	{
		callback?.Invoke(null);
	}

	public override void EnsureSaveSlotSpace(int slotIndex, Action<bool> callback)
	{
		callback?.Invoke(obj: true);
	}

	public override void WriteSaveSlot(int slotIndex, byte[] binary, Action<bool> callback)
	{
		callback?.Invoke(obj: false);
	}

	public override void ClearSaveSlot(int slotIndex, Action<bool> callback)
	{
		callback?.Invoke(obj: false);
	}

	public override void OnSetGameData(int slotIndex)
	{
	}

	public override void SaveScreenCapture(Texture2D texture2D, Action<bool> callback)
	{
		base.SaveScreenCapture(texture2D, callback);
	}

	public override void LoadScreenCaptures(Action<Texture2D[]> captures)
	{
		base.LoadScreenCaptures(captures);
	}

	public override void AdjustGameSettings(GameSettings gameSettings)
	{
		base.AdjustGameSettings(gameSettings);
	}

	public override bool TryGetAchievementState(string achievementId, out AchievementState state)
	{
		state = default(AchievementState);
		achievementIDMap.TryGetAchievementInformation(achievementId, out var _);
		state.isValid = false;
		return false;
	}

	public override bool? IsAchievementUnlocked(string achievementId)
	{
		if (TryGetAchievementState(achievementId, out var state) && state.isValid)
		{
			return state.isUnlocked;
		}
		return null;
	}

	public override void PushAchievementUnlock(string achievementId)
	{
		achievementIDMap.TryGetAchievementInformation(achievementId, out var _);
	}

	public override void UpdateAchievementProgress(string achievementId, int value, int max)
	{
		if (achievementIDMap.TryGetAchievementInformation(achievementId, out var _))
		{
			Mathf.Min(Mathf.RoundToInt((float)value / (float)max * 100f), 100);
		}
	}

	public override void ResetAchievements()
	{
	}

	public override void ShowNativeAchievementsDialog()
	{
		base.ShowNativeAchievementsDialog();
	}

	public override void SetSocialPresence(string socialStatusKey, bool isActive)
	{
		base.SetSocialPresence(socialStatusKey, isActive);
	}

	public override void AddSocialStat(string name, int amount)
	{
		base.AddSocialStat(name, amount);
	}

	public override void FlushSocialEvents()
	{
		base.FlushSocialEvents();
	}

	public override void UpdateLocation(string location)
	{
		base.UpdateLocation(location);
	}

	public override void UpdatePlayTime(float playTime)
	{
		base.UpdatePlayTime(playTime);
	}

	public override void SetTargetFrameRate(int frameRate)
	{
		if (Application.platform == RuntimePlatform.GameCoreXboxOne || Application.platform == RuntimePlatform.GameCoreXboxSeries)
		{
			if (!hasEverCalledResolution)
			{
				hasEverCalledResolution = true;
				_ = Screen.resolutions;
			}
			Resolution currentResolution = Screen.currentResolution;
			if (!canRestore)
			{
				originalResolution = currentResolution;
				vSyncCount = QualitySettings.vSyncCount;
				originalFrameRate = Application.targetFrameRate;
				originalFullScreenMode = Screen.fullScreenMode;
				canRestore = true;
			}
			FullScreenMode fullScreenMode = Screen.fullScreenMode;
			RefreshRate refreshRate = default(RefreshRate);
			refreshRate.denominator = 1u;
			refreshRate.numerator = (uint)frameRate;
			RefreshRate preferredRefreshRate = refreshRate;
			Screen.SetResolution(currentResolution.width, currentResolution.height, fullScreenMode, preferredRefreshRate);
			QualitySettings.vSyncCount = 1;
			Application.targetFrameRate = frameRate;
		}
		else
		{
			base.SetTargetFrameRate(frameRate);
		}
	}

	public override bool RestoreFrameRate()
	{
		if (!canRestore)
		{
			return false;
		}
		Screen.SetResolution(originalResolution.width, originalResolution.height, originalFullScreenMode, originalResolution.refreshRateRatio);
		base.RestoreFrameRate();
		canRestore = false;
		return true;
	}

	public override void ClearEngagement()
	{
	}

	public override void BeginEngagement()
	{
	}

	public override void UpdateWaitingForEngagement()
	{
	}

	public override void SetDisengageHandler(IDisengageHandler disengageHandler)
	{
		base.SetDisengageHandler(disengageHandler);
	}

	protected override void BecomeCurrent()
	{
		base.BecomeCurrent();
	}

	public VibrationMixer GetVibrationMixer()
	{
		return platformVibrationHelper.GetMixer();
	}
}

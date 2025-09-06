using System;
using System.Collections.Generic;
using System.IO;
using GlobalEnums;
using InControl;
using UnityEngine;

public class DesktopPlatform : Platform, VibrationManager.IVibrationMixerProvider
{
	private delegate DesktopOnlineSubsystem CreateOnlineSubsystemDelegate();

	private string saveDirPath;

	private ISharedData localSharedData;

	private ISharedData roamingSharedData;

	private DesktopOnlineSubsystem onlineSubsystem;

	private PlatformVibrationHelper vibrationHelper;

	private DesktopSaveRestoreHandler desktopSaveRestoreHandler;

	public ISharedData LocalRoamingSharedData => roamingSharedData;

	public override bool IsRunningOnHandHeld
	{
		get
		{
			if (onlineSubsystem is SteamOnlineSubsystem steamOnlineSubsystem)
			{
				return steamOnlineSubsystem.IsRunningOnSteamDeck;
			}
			return base.IsRunningOnHandHeld;
		}
	}

	public override ScreenModeState ScreenMode
	{
		get
		{
			if (onlineSubsystem is SteamOnlineSubsystem steamOnlineSubsystem)
			{
				if (steamOnlineSubsystem.IsRunningOnSteamDeck)
				{
					SetScreenMode(ScreenModeState.HandHeld);
				}
				return base.ScreenMode;
			}
			return base.ScreenMode;
		}
		set
		{
			if (!(onlineSubsystem is SteamOnlineSubsystem))
			{
				SetScreenMode(value);
			}
		}
	}

	public override HandHeldTypes HandHeldType
	{
		get
		{
			if (onlineSubsystem is SteamOnlineSubsystem { IsRunningOnSteamDeck: not false })
			{
				return HandHeldTypes.SteamDeck;
			}
			return base.HandHeldType;
		}
	}

	public override string DisplayName => "Desktop";

	public override SaveRestoreHandler SaveRestoreHandler => onlineSubsystem?.SaveRestoreHandler ?? desktopSaveRestoreHandler;

	public override ISharedData LocalSharedData => localSharedData;

	public override ISharedData RoamingSharedData
	{
		get
		{
			DesktopOnlineSubsystem desktopOnlineSubsystem = onlineSubsystem;
			if (desktopOnlineSubsystem == null || !desktopOnlineSubsystem.HandlesRoamingSharedData)
			{
				return roamingSharedData;
			}
			return onlineSubsystem.RoamingSharedData;
		}
	}

	public override bool AreAchievementsFetched => onlineSubsystem?.AreAchievementsFetched ?? true;

	public override bool HasNativeAchievementsDialog => onlineSubsystem?.HasNativeAchievementsDialog ?? base.HasNativeAchievementsDialog;

	public override AcceptRejectInputStyles AcceptRejectInputStyle => AcceptRejectInputStyles.NonJapaneseStyle;

	public override bool ShowLanguageSelect => true;

	public override bool IsControllerImplicit
	{
		get
		{
			if ((bool)ManagerSingleton<InputHandler>.Instance && ManagerSingleton<InputHandler>.Instance.lastActiveController == BindingSourceType.DeviceBindingSource)
			{
				return true;
			}
			return false;
		}
	}

	public override bool WillPreloadSaveFiles => onlineSubsystem?.WillPreloadSaveFiles ?? base.WillPreloadSaveFiles;

	public override EngagementRequirements EngagementRequirement => onlineSubsystem?.EngagementRequirement ?? base.EngagementRequirement;

	public override EngagementStates EngagementState => onlineSubsystem?.EngagementState ?? base.EngagementState;

	public override string EngagedDisplayName => onlineSubsystem?.EngagedDisplayName ?? base.EngagedDisplayName;

	public override Texture2D EngagedDisplayImage
	{
		get
		{
			if (onlineSubsystem == null)
			{
				return base.EngagedDisplayImage;
			}
			return onlineSubsystem.EngagedDisplayImage;
		}
	}

	public override int DefaultHudSetting => onlineSubsystem?.DefaultHudSetting ?? 1;

	public override bool LimitedGraphicsSettings => onlineSubsystem?.LimitedGraphicsSettings ?? base.LimitedGraphicsSettings;

	protected override void Awake()
	{
		base.Awake();
		saveDirPath = Application.persistentDataPath;
		CreateOnlineSubsystem();
		string path = "default";
		if (onlineSubsystem != null)
		{
			string userId = onlineSubsystem.UserId;
			if (!string.IsNullOrEmpty(userId))
			{
				path = userId;
			}
		}
		saveDirPath = Path.Combine(saveDirPath, path);
		localSharedData = new PlayerPrefsSharedData(isEncrypted: false);
		roamingSharedData = new JsonSharedData(saveDirPath, "shared.dat", useEncryption: true);
		desktopSaveRestoreHandler = new DesktopSaveRestoreHandler(saveDirPath);
		Platform.UseFieldInfoCache = false;
		if (onlineSubsystem == null)
		{
			OnOnlineSubsystemAchievementsFetched();
		}
		vibrationHelper = new PlatformVibrationHelper();
	}

	private void CreateOnlineSubsystem()
	{
		List<CreateOnlineSubsystemDelegate> list = new List<CreateOnlineSubsystemDelegate>();
		if (SteamOnlineSubsystem.IsPackaged(this))
		{
			list.Add(() => new SteamOnlineSubsystem(this));
		}
		if (GOGGalaxyOnlineSubsystem.IsPackaged(this))
		{
			list.Add(() => new GOGGalaxyOnlineSubsystem(this));
		}
		if (GameCoreOnlineSubsystem.IsPackaged(this))
		{
			list.Add(() => new GameCoreOnlineSubsystem(this));
		}
		if (StreamingOnlineSubsystem.IsPackaged(this))
		{
			list.Add(() => new StreamingOnlineSubsystem());
		}
		if (list.Count == 0)
		{
			Debug.LogFormat(this, "No online subsystems packaged.");
			return;
		}
		if (list.Count > 1)
		{
			Debug.LogErrorFormat(this, "Multiple online subsystems packaged.");
			Application.Quit();
			return;
		}
		onlineSubsystem = list[0]();
		Debug.LogFormat(this, "Selected online subsystem " + onlineSubsystem.GetType().Name);
		if (onlineSubsystem is GOGGalaxyOnlineSubsystem { DidInitialize: false })
		{
			onlineSubsystem = null;
			Debug.LogError("GOG was not initialised, will not be used as online subsystem.");
		}
	}

	protected override void OnDestroy()
	{
		if (onlineSubsystem != null)
		{
			onlineSubsystem.Dispose();
			onlineSubsystem = null;
		}
		vibrationHelper.Destroy();
		base.OnDestroy();
	}

	protected override void Update()
	{
		base.Update();
		onlineSubsystem?.Update();
		vibrationHelper.UpdateVibration();
	}

	public override void BeginEngagement()
	{
		onlineSubsystem?.BeginEngagement();
	}

	public override void ClearEngagement()
	{
		onlineSubsystem?.ClearEngagment();
	}

	public override void LoadSharedDataAndNotify(bool mounted)
	{
		DesktopOnlineSubsystem desktopOnlineSubsystem = onlineSubsystem;
		if (desktopOnlineSubsystem != null && desktopOnlineSubsystem.HandleLoadSharedDataAndNotify)
		{
			onlineSubsystem.LoadSharedDataAndNotify(mounted);
		}
		else
		{
			base.LoadSharedDataAndNotify(mounted);
		}
	}

	private string GetSaveSlotPath(int slotIndex, SaveSlotFileNameUsage usage)
	{
		return Path.Combine(saveDirPath, GetSaveSlotFileName(slotIndex, usage));
	}

	public override void IsSaveSlotInUse(int slotIndex, Action<bool> callback)
	{
		DesktopOnlineSubsystem desktopOnlineSubsystem = onlineSubsystem;
		if (desktopOnlineSubsystem != null && desktopOnlineSubsystem.HandlesGameSaves)
		{
			onlineSubsystem.IsSaveSlotInUse(slotIndex, callback);
		}
		else
		{
			LocalIsSaveSlotInUse(slotIndex, callback);
		}
	}

	public void LocalIsSaveSlotInUse(int slotIndex, Action<bool> callback)
	{
		string saveSlotPath = GetSaveSlotPath(slotIndex, SaveSlotFileNameUsage.Primary);
		bool inUse;
		try
		{
			inUse = File.Exists(saveSlotPath);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			inUse = false;
		}
		CoreLoop.InvokeNext(delegate
		{
			callback?.Invoke(inUse);
		});
	}

	public override void ReadSaveSlot(int slotIndex, Action<byte[]> callback)
	{
		DesktopOnlineSubsystem desktopOnlineSubsystem = onlineSubsystem;
		if (desktopOnlineSubsystem != null && desktopOnlineSubsystem.HandlesGameSaves)
		{
			onlineSubsystem.ReadSaveSlot(slotIndex, callback);
		}
		else
		{
			LocalReadSaveSlot(slotIndex, callback);
		}
	}

	public void LocalReadSaveSlot(int slotIndex, Action<byte[]> callback)
	{
		string saveSlotPath = GetSaveSlotPath(slotIndex, SaveSlotFileNameUsage.Primary);
		byte[] bytes;
		try
		{
			if (File.Exists(saveSlotPath))
			{
				bytes = File.ReadAllBytes(saveSlotPath);
			}
			else
			{
				bytes = null;
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			bytes = null;
		}
		CoreLoop.InvokeSafe(delegate
		{
			callback?.Invoke(bytes);
		});
	}

	public override void EnsureSaveSlotSpace(int slotIndex, Action<bool> callback)
	{
		CoreLoop.InvokeNext(delegate
		{
			callback?.Invoke(obj: true);
		});
	}

	public override void WriteSaveSlot(int slotIndex, byte[] bytes, Action<bool> callback)
	{
		DesktopOnlineSubsystem desktopOnlineSubsystem = onlineSubsystem;
		if (desktopOnlineSubsystem != null && desktopOnlineSubsystem.HandlesGameSaves)
		{
			onlineSubsystem.WriteSaveSlot(slotIndex, bytes, callback);
			return;
		}
		string saveSlotPath = GetSaveSlotPath(slotIndex, SaveSlotFileNameUsage.Primary);
		string saveSlotPath2 = GetSaveSlotPath(slotIndex, SaveSlotFileNameUsage.Backup);
		string text = saveSlotPath + ".new";
		if (File.Exists(text))
		{
			Debug.LogWarning($"Temp file <b>{text}</b> was found and is likely corrupted. The file has been deleted.");
		}
		try
		{
			File.WriteAllBytes(text, bytes);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
		bool successful;
		try
		{
			if (File.Exists(saveSlotPath))
			{
				File.Replace(text, saveSlotPath, saveSlotPath2 + GetBackupNumber(saveSlotPath2));
			}
			else
			{
				File.Move(text, saveSlotPath);
			}
			successful = true;
		}
		catch (Exception exception2)
		{
			Debug.LogException(exception2);
			successful = false;
		}
		CoreLoop.InvokeNext(delegate
		{
			callback?.Invoke(successful);
		});
	}

	private int GetBackupNumber(string backupPath)
	{
		int num = 0;
		int num2 = 3;
		string[] files = Directory.GetFiles(Path.GetDirectoryName(backupPath));
		List<string> list = new List<string>();
		string[] array = files;
		foreach (string text in array)
		{
			if (text.StartsWith(backupPath))
			{
				list.Add(text);
			}
		}
		if (list.Count > 0)
		{
			int index = 0;
			int num3 = int.MaxValue;
			int num4 = 0;
			for (int num5 = list.Count - 1; num5 >= 0; num5--)
			{
				string text2 = list[num5].Replace(backupPath, "");
				if (text2 != "")
				{
					try
					{
						num = int.Parse(text2);
						if (num < num3)
						{
							num3 = num;
							index = num5;
						}
						if (num > num4)
						{
							num4 = num;
						}
					}
					catch
					{
						Debug.LogWarning($"Backup file: {list[num5]} does not have a numerical extension, and will be ignored.");
					}
				}
			}
			num = num4;
			if (list.Count >= num2)
			{
				File.Delete(list[index]);
			}
		}
		return num + 1;
	}

	public override void ClearSaveSlot(int slotIndex, Action<bool> callback)
	{
		DesktopOnlineSubsystem desktopOnlineSubsystem = onlineSubsystem;
		if (desktopOnlineSubsystem != null && desktopOnlineSubsystem.HandlesGameSaves)
		{
			onlineSubsystem.ClearSaveSlot(slotIndex, callback);
			return;
		}
		string saveSlotPath = GetSaveSlotPath(slotIndex, SaveSlotFileNameUsage.Primary);
		bool successful;
		try
		{
			File.Delete(saveSlotPath);
			successful = true;
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			successful = false;
		}
		CoreLoop.InvokeNext(delegate
		{
			callback?.Invoke(successful);
		});
	}

	public override bool? IsAchievementUnlocked(string achievementId)
	{
		return onlineSubsystem?.IsAchievementUnlocked(achievementId) ?? RoamingSharedData.GetBool(achievementId, def: false);
	}

	public override void PushAchievementUnlock(string achievementId)
	{
		onlineSubsystem?.PushAchievementUnlock(achievementId);
		RoamingSharedData.SetBool(achievementId, val: true);
	}

	public override void UpdateAchievementProgress(string achievementId, int value, int max)
	{
		onlineSubsystem?.UpdateAchievementProgress(achievementId, value, max);
	}

	public override void ResetAchievements()
	{
		onlineSubsystem?.ResetAchievements();
	}

	public bool IncludesPlugin(string pluginName)
	{
		string path = "Plugins";
		string path2 = Path.Combine(Path.Combine(Application.dataPath, path), pluginName);
		if (!File.Exists(path2))
		{
			return Directory.Exists(path2);
		}
		return true;
	}

	public void OnOnlineSubsystemAchievementsFetched()
	{
		OnAchievementsFetched();
	}

	public void OnOnlineSubsystemAchievementsFailed()
	{
		onlineSubsystem = null;
		OnAchievementsFetched();
	}

	public VibrationMixer GetVibrationMixer()
	{
		return vibrationHelper.GetMixer();
	}

	public override GamepadType OverrideGamepadDisplay(GamepadType currentGamepadType)
	{
		return onlineSubsystem?.OverrideGamepadDisplay(currentGamepadType) ?? base.OverrideGamepadDisplay(currentGamepadType);
	}
}

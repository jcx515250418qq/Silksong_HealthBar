using System;
using System.IO;
using System.Text;
using GlobalEnums;
using Steamworks;
using UnityEngine;

public class SteamOnlineSubsystem : DesktopOnlineSubsystem
{
	private readonly DesktopPlatform platform;

	private bool didInitialize;

	private bool statsReceived;

	private readonly SteamAPIWarningMessageHook_t warningCallback;

	private Callback<GameOverlayActivated_t> gameOverlayCallback;

	private Callback<UserStatsReceived_t> statsReceivedCallback;

	private Callback<SteamShutdown_t> steamShutdownCallback;

	private Callback<UserAchievementStored_t> achievementStoredCallback;

	public bool DidInitialize => didInitialize;

	public override int DefaultHudSetting
	{
		get
		{
			if (!didInitialize || !SteamUtils.IsSteamRunningOnSteamDeck())
			{
				return 1;
			}
			return 0;
		}
	}

	public bool IsRunningOnSteamDeck
	{
		get
		{
			if (didInitialize)
			{
				return SteamUtils.IsSteamRunningOnSteamDeck();
			}
			return false;
		}
	}

	public override string UserId
	{
		get
		{
			if (!didInitialize)
			{
				return null;
			}
			return SteamUser.GetSteamID().GetAccountID().ToString();
		}
	}

	public override bool AreAchievementsFetched => statsReceived;

	public static bool IsPackaged(DesktopPlatform desktopPlatform)
	{
		return desktopPlatform.IncludesPlugin(Path.Combine("x86_64", "steam_api64.dll"));
	}

	public SteamOnlineSubsystem(DesktopPlatform platform)
	{
		this.platform = platform;
		if (!Packsize.Test())
		{
			Debug.LogErrorFormat("Steamworks packsize incorrect.");
		}
		if (!DllCheck.Test())
		{
			Debug.LogErrorFormat("Steamworks binaries out of date or missing.");
		}
		if (SteamAPI.RestartAppIfNecessary(new AppId_t(1030300u)))
		{
			Debug.LogError("Application was not launched through Steam! Shutting down...");
			Application.Quit();
		}
		Debug.LogFormat("Steam initializing");
		didInitialize = SteamAPI.Init();
		if (didInitialize)
		{
			warningCallback = OnSteamLogMessage;
			SteamClient.SetWarningMessageHook(warningCallback);
			gameOverlayCallback = Callback<GameOverlayActivated_t>.Create(OnGameOverlayActivated);
			statsReceivedCallback = Callback<UserStatsReceived_t>.Create(OnStatsReceived);
			steamShutdownCallback = Callback<SteamShutdown_t>.Create(OnSteamShutdown);
			achievementStoredCallback = Callback<UserAchievementStored_t>.Create(OnAchievementStored);
			string personaName = SteamFriends.GetPersonaName();
			Debug.LogFormat("Steam logged in as {0}", personaName);
			if (!SteamUserStats.RequestCurrentStats())
			{
				Debug.LogErrorFormat("Steam unable to request current stats.");
			}
		}
		else
		{
			Debug.LogErrorFormat("Steam failed to initialize");
		}
	}

	public override void Dispose()
	{
		if (didInitialize)
		{
			Debug.LogFormat("Shutting down Steam API.");
			SteamAPI.Shutdown();
		}
		base.Dispose();
	}

	public override void Update()
	{
		base.Update();
		if (didInitialize)
		{
			SteamAPI.RunCallbacks();
		}
	}

	private void OnSteamLogMessage(int severity, StringBuilder content)
	{
		string format = "Steam: " + content;
		if (severity == 1)
		{
			Debug.LogWarningFormat(format);
		}
		else
		{
			Debug.LogFormat(format);
		}
	}

	private void OnGameOverlayActivated(GameOverlayActivated_t ev)
	{
		Debug.LogFormat("Steam overlay became {0}.", (ev.m_bActive == 0) ? "closed" : "opened");
	}

	private void OnStatsReceived(UserStatsReceived_t ev)
	{
		if (ev.m_eResult == EResult.k_EResultOK)
		{
			statsReceived = true;
			Debug.LogFormat("Steam stats received.");
			platform.OnOnlineSubsystemAchievementsFetched();
		}
		else
		{
			Debug.LogErrorFormat("Steam failed to receive stats: {0}", ev.m_eResult);
		}
	}

	public override GamepadType OverrideGamepadDisplay(GamepadType currentGamepadType)
	{
		if (didInitialize && SteamUtils.IsSteamRunningOnSteamDeck())
		{
			return GamepadType.STEAM_DECK;
		}
		return base.OverrideGamepadDisplay(currentGamepadType);
	}

	private void OnSteamShutdown(SteamShutdown_t ev)
	{
		Debug.LogFormat("Steam shut down.");
		didInitialize = false;
	}

	private void OnAchievementStored(UserAchievementStored_t ev)
	{
		Debug.LogFormat("Steam achievement {0} ({1}/{2}) upload complete", ev.m_rgchAchievementName, ev.m_nCurProgress, ev.m_nMaxProgress);
	}

	public override void PushAchievementUnlock(string achievementId)
	{
		if (!didInitialize)
		{
			Debug.LogErrorFormat("Unable to unlock achievement {0}, because Steam is not initialized", achievementId);
			return;
		}
		try
		{
			SteamUserStats.SetAchievement(achievementId);
			SteamUserStats.StoreStats();
			Debug.LogFormat("Pushing achievement {0}", achievementId);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	public override bool? IsAchievementUnlocked(string achievementId)
	{
		if (!didInitialize)
		{
			Debug.LogErrorFormat("Unable to retrieve achievement state for {0}, because Steam is not initialized", achievementId);
			return null;
		}
		try
		{
			if (SteamUserStats.GetAchievement(achievementId, out var pbAchieved))
			{
				return pbAchieved;
			}
			Debug.LogErrorFormat("Failed to retrieve achievement state for {0}", achievementId);
			return null;
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			return null;
		}
	}

	public override void UpdateAchievementProgress(string achievementId, int value, int max)
	{
		if (!didInitialize)
		{
			Debug.LogErrorFormat("Unable to update achievement progress for {0}, because Steam is not initialized", achievementId);
			return;
		}
		try
		{
			SteamUserStats.SetStat(achievementId + "_STAT", value);
			SteamUserStats.StoreStats();
			Debug.LogFormat("Pushing stat {0}_STAT value: {1}/{2}", achievementId, value, max);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	public override void ResetAchievements()
	{
		if (!didInitialize)
		{
			Debug.LogErrorFormat("Unable to reset all stats, because Steam is not initialized");
			return;
		}
		try
		{
			SteamUserStats.ResetAllStats(bAchievementsToo: true);
			Debug.LogFormat("Reset all stats");
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}
}

using System;
using System.IO;
using Galaxy.Api;
using UnityEngine;

public class GOGGalaxyOnlineSubsystem : DesktopOnlineSubsystem
{
	private class Authorization : GlobalAuthListener
	{
		private readonly GOGGalaxyOnlineSubsystem subsystem;

		private IUser user;

		private bool isAuthorized;

		public bool IsAuthorized => isAuthorized;

		public Authorization(GOGGalaxyOnlineSubsystem subsystem)
		{
			this.subsystem = subsystem;
			isAuthorized = false;
		}

		public override void OnAuthSuccess()
		{
			isAuthorized = true;
			Debug.LogFormat("GOG authorized");
			subsystem.OnAuthorized();
		}

		public override void OnAuthFailure(FailureReason failureReason)
		{
			isAuthorized = false;
			Debug.LogErrorFormat("GOG authorization failed: {0}", failureReason);
			subsystem.OnAuthFailed();
		}

		public override void OnAuthLost()
		{
			isAuthorized = false;
			Debug.LogErrorFormat("GOG authorization lost");
			subsystem.OnAuthFailed();
		}

		public void SignIn()
		{
			try
			{
				Debug.LogFormat("GOG signing in...");
				user = GalaxyInstance.User();
				user.SignInGalaxy();
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}
	}

	private class Statistics : GlobalUserStatsAndAchievementsRetrieveListener
	{
		private readonly GOGGalaxyOnlineSubsystem subsystem;

		private bool didReceive;

		private IStats stats;

		public bool DidReceive => didReceive;

		public Statistics(GOGGalaxyOnlineSubsystem subsystem)
		{
			this.subsystem = subsystem;
			didReceive = false;
		}

		public override void OnUserStatsAndAchievementsRetrieveSuccess(GalaxyID userID)
		{
			Debug.LogFormat("Retrieved stats");
			didReceive = true;
			subsystem.OnStatisticsReceived();
		}

		public override void OnUserStatsAndAchievementsRetrieveFailure(GalaxyID userID, FailureReason failureReason)
		{
			Debug.LogErrorFormat("Failed to retrieve stats: {0}", failureReason);
		}

		public void Request()
		{
			Debug.LogFormat("GOG requesting user stats and achievements...");
			stats = GalaxyInstance.Stats();
			stats.RequestUserStatsAndAchievements();
		}
	}

	private const string ClientId = "58684736355579041";

	private const string ClientSecret = "6df4eee80bbe552d39094e3ee31e457ff109405df303f1cebf263f964c1a8a1e";

	private DesktopPlatform platform;

	private bool didInitialize;

	private Authorization authorization;

	private Statistics statistics;

	public bool DidInitialize => didInitialize;

	public override bool AreAchievementsFetched
	{
		get
		{
			if (statistics != null)
			{
				return statistics.DidReceive;
			}
			return false;
		}
	}

	public static bool IsPackaged(DesktopPlatform desktopPlatform)
	{
		return desktopPlatform.IncludesPlugin(Path.Combine("x86_64", "GalaxyCSharpGlue.dll"));
	}

	public GOGGalaxyOnlineSubsystem(DesktopPlatform platform)
	{
		this.platform = platform;
		try
		{
			GalaxyInstance.Init(new InitParams("58684736355579041", "6df4eee80bbe552d39094e3ee31e457ff109405df303f1cebf263f964c1a8a1e"));
			didInitialize = true;
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
		if (didInitialize)
		{
			IListenerRegistrar listenerRegistrar = GalaxyInstance.ListenerRegistrar();
			authorization = new Authorization(this);
			listenerRegistrar.Register(GalaxyTypeAwareListenerAuth.GetListenerType(), authorization);
			statistics = new Statistics(this);
			listenerRegistrar.Register(GalaxyTypeAwareListenerUserStatsAndAchievementsRetrieve.GetListenerType(), statistics);
			authorization.SignIn();
		}
		else
		{
			Debug.LogErrorFormat("GOG failed to initialize");
		}
	}

	public override void Dispose()
	{
		if (statistics != null)
		{
			statistics.Dispose();
			statistics = null;
		}
		if (authorization != null)
		{
			authorization.Dispose();
			authorization = null;
		}
		if (didInitialize)
		{
			GalaxyInstance.Shutdown(unloadModule: true);
			didInitialize = false;
		}
		base.Dispose();
	}

	public override void Update()
	{
		base.Update();
		if (didInitialize)
		{
			GalaxyInstance.ProcessData();
		}
	}

	private void OnAuthorized()
	{
		statistics.Request();
	}

	private void OnAuthFailed()
	{
		platform.OnOnlineSubsystemAchievementsFailed();
	}

	private void OnStatisticsReceived()
	{
		platform.OnOnlineSubsystemAchievementsFetched();
	}

	public override bool? IsAchievementUnlocked(string achievementId)
	{
		if (!authorization.IsAuthorized || !statistics.DidReceive)
		{
			Debug.LogErrorFormat("Unable to get achievement {0}, because GOG is not authenticated.", achievementId);
			return null;
		}
		bool unlocked = false;
		uint unlockTime = 0u;
		try
		{
			GalaxyInstance.Stats().GetAchievement(achievementId, ref unlocked, ref unlockTime);
			return unlocked;
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			return null;
		}
	}

	public override void PushAchievementUnlock(string achievementId)
	{
		if (authorization.IsAuthorized)
		{
			try
			{
				GalaxyInstance.Stats().SetAchievement(achievementId);
				GalaxyInstance.Stats().StoreStatsAndAchievements();
				return;
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
				return;
			}
		}
		Debug.LogErrorFormat("Unable to push achievement {0}, because GOG is not authenticated.", achievementId);
	}

	public override void UpdateAchievementProgress(string achievementId, int value, int max)
	{
	}

	public override void ResetAchievements()
	{
		if (authorization.IsAuthorized)
		{
			try
			{
				GalaxyInstance.Stats().ResetStatsAndAchievements();
				return;
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
				return;
			}
		}
		Debug.LogErrorFormat("Unable to reset achievements, because GOG is not authenticated.");
	}
}

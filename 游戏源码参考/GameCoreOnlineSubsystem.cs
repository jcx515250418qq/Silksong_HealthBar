using System;
using System.Collections.Generic;
using System.IO;
using TeamCherry.GameCore;
using UnityEngine;

public class GameCoreOnlineSubsystem : DesktopOnlineSubsystem
{
	private const string SCID = "00000000-0000-0000-0000-0000636f5860";

	private AchievementIDMap achievementIdMap;

	private HashSet<int> awardedAchievements;

	private DesktopPlatform platform;

	private GameCoreSaveRestoreHandler gamecoreSaveRestoreHandler;

	private GameCoreSharedData sharedData;

	public override Platform.EngagementRequirements EngagementRequirement => Platform.EngagementRequirements.MustDisplay;

	public override Platform.EngagementStates EngagementState
	{
		get
		{
			if (GameCoreRuntimeManager.UserSignedIn)
			{
				return Platform.EngagementStates.Engaged;
			}
			if (GameCoreRuntimeManager.UserSignInPending)
			{
				return Platform.EngagementStates.EngagePending;
			}
			return Platform.EngagementStates.NotEngaged;
		}
	}

	public override string EngagedDisplayName => GameCoreRuntimeManager.GamerTag;

	public override Texture2D EngagedDisplayImage => GameCoreRuntimeManager.UserDisplayImage;

	public override bool AreAchievementsFetched => false;

	public override bool HasNativeAchievementsDialog => true;

	public override bool HandlesGameSaves => true;

	public override bool HandlesRoamingSharedData => true;

	public override Platform.ISharedData RoamingSharedData => sharedData;

	public override bool WillPreloadSaveFiles => false;

	public override SaveRestoreHandler SaveRestoreHandler => gamecoreSaveRestoreHandler;

	public static bool IsPackaged(DesktopPlatform desktopPlatform)
	{
		return desktopPlatform.IncludesPlugin(Path.Combine("x86_64", "XGamingRuntimeThunks.dll"));
	}

	public GameCoreOnlineSubsystem(DesktopPlatform platform)
	{
		this.platform = platform;
		achievementIdMap = Resources.Load<AchievementIDMap>("XB1AchievementMap");
		awardedAchievements = new HashSet<int>();
		GameCoreRuntimeManager.InitializeRuntime();
		gamecoreSaveRestoreHandler = new GameCoreSaveRestoreHandler();
		sharedData = new GameCoreSharedData("sharedData", "sharedData.dat");
		if (GameCoreRuntimeManager.SaveSystemInitialised)
		{
			MigrateLocalSaves();
		}
		GameCoreRuntimeManager.OnSaveSystemInitialised += MigrateLocalSaves;
	}

	~GameCoreOnlineSubsystem()
	{
		GameCoreRuntimeManager.OnSaveSystemInitialised -= MigrateLocalSaves;
	}

	public override void BeginEngagement()
	{
		GameCoreRuntimeManager.RequestUserSignIn();
	}

	public override void ClearEngagment()
	{
	}

	public override void PushAchievementUnlock(string achievementId)
	{
		if (achievementIdMap.TryGetAchievementInformation(achievementId, out var info))
		{
			try
			{
				GameCoreRuntimeManager.UnlockAchievement(info.ServiceId);
				GameCoreRuntimeManager.FetchAchievements();
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
		}
	}

	public override bool? IsAchievementUnlocked(string achievementId)
	{
		return null;
	}

	public override void UpdateAchievementProgress(string achievementId, int progressValue, int maxValue)
	{
		if (achievementIdMap.TryGetAchievementInformation(achievementId, out var info))
		{
			uint num = (uint)Mathf.Min(Mathf.RoundToInt((float)progressValue / (float)maxValue * 100f), 100);
			GameCoreRuntimeManager.UnlockAchievement(info.ServiceId, num);
			if (num > maxValue)
			{
				GameCoreRuntimeManager.FetchAchievements();
			}
		}
	}

	public override void ResetAchievements()
	{
	}

	private string GetSaveContainerName(int slotIndex)
	{
		return GameCoreRuntimeManager.GetSaveSlotContainerName(slotIndex);
	}

	private string GetSaveFileName(int slotIndex)
	{
		return GameCoreRuntimeManager.GetMainSaveName(slotIndex);
	}

	public override void IsSaveSlotInUse(int slotIndex, Action<bool> callback)
	{
		if (callback == null)
		{
			return;
		}
		GameCoreRuntimeManager.FileExists(GetSaveContainerName(slotIndex), GetSaveFileName(slotIndex), delegate(bool exists)
		{
			CoreLoop.InvokeSafe(delegate
			{
				callback(exists);
			});
		});
	}

	public override void ReadSaveSlot(int slotIndex, Action<byte[]> callback)
	{
		if (callback == null)
		{
			return;
		}
		GameCoreRuntimeManager.LoadSaveData(GetSaveContainerName(slotIndex), GetSaveFileName(slotIndex), delegate(byte[] success)
		{
			CoreLoop.InvokeSafe(delegate
			{
				callback(success);
			});
		});
	}

	public override void WriteSaveSlot(int slotIndex, byte[] bytes, Action<bool> callback)
	{
		GameCoreRuntimeManager.Save(GetSaveContainerName(slotIndex), GetSaveFileName(slotIndex), bytes, delegate(bool success)
		{
			if (callback != null)
			{
				CoreLoop.InvokeSafe(delegate
				{
					callback(success);
				});
			}
		});
	}

	public override void ClearSaveSlot(int slotIndex, Action<bool> callback)
	{
		GameCoreRuntimeManager.DeleteContainer(GetSaveContainerName(slotIndex), delegate(bool success)
		{
			if (callback != null)
			{
				CoreLoop.InvokeSafe(delegate
				{
					callback(success);
				});
			}
		});
	}

	private void MigrateLocalSaves()
	{
		for (int i = 1; i <= 4; i++)
		{
			int slotIndex = i;
			platform.LocalReadSaveSlot(slotIndex, delegate(byte[] localBytes)
			{
				if (localBytes != null)
				{
					platform.ReadSaveSlot(slotIndex, delegate(byte[] bytes)
					{
						if (bytes == null)
						{
							platform.WriteSaveSlot(slotIndex, localBytes, delegate
							{
								Debug.Log($"Migrated local slot {slotIndex} to cloud.");
							});
						}
					});
				}
			});
		}
		sharedData.LoadData(delegate
		{
			sharedData.ImportData(platform.LocalRoamingSharedData);
		});
	}
}

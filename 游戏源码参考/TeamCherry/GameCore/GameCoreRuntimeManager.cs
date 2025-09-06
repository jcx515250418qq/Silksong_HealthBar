using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using UnityEngine;
using XGamingRuntime;
using XGamingRuntime.Interop;

namespace TeamCherry.GameCore
{
	public static class GameCoreRuntimeManager
	{
		public enum UserState
		{
			NotSignedIn = 0,
			SignedIn = 1,
			SigningOut = 2,
			SignedOut = 3
		}

		public sealed class User : IDisposable
		{
			public XGamingRuntime.XUserHandle userHandle;

			public XUserLocalId localId;

			private XGamingRuntime.XblContextHandle xblContextHandle;

			public XStoreContext xblStoreContext;

			public GameCoreSaveHandler saveHandler;

			public object saveLock = new object();

			public ConcurrentQueue<Action> saveQueue = new ConcurrentQueue<Action>();

			private RateLimiter fetchRateLimiter = new RateLimiter(1, TimeSpan.FromSeconds(30.0));

			private volatile bool saveInitialised;

			public bool HasFetchedAchievement;

			private bool isFetchingAchievementData;

			public Dictionary<int, XblAchievement> xblAchievements = new Dictionary<int, XblAchievement>();

			private bool isActiveUser;

			private bool hasGamerTag;

			private XUserSignOutDeferralHandle signOutDeferralHandle;

			private float signOutTimer;

			private bool isDisposed;

			private object unlockLock = new object();

			private Dictionary<string, uint> activeUnlocks = new Dictionary<string, uint>();

			public UserState UserState { get; private set; }

			public string GamerTag { get; private set; }

			public Texture2D UserDisplayImage { get; private set; }

			public XGamingRuntime.XblContextHandle XblContextHandle
			{
				get
				{
					if (xblContextHandle == null)
					{
						Succeeded(SDK.XBL.XblContextCreateHandle(userHandle, out xblContextHandle), "Create Xbox Live context : " + GetGamerID());
					}
					return xblContextHandle;
				}
				set
				{
					xblContextHandle = value;
				}
			}

			public RateLimiter FetchRateLimiter => fetchRateLimiter;

			public bool SaveInitialised
			{
				get
				{
					return saveInitialised;
				}
				private set
				{
					saveInitialised = value;
				}
			}

			public bool IsFetchingAchievementData => isFetchingAchievementData;

			public bool IsDisposed => isDisposed;

			public User(XGamingRuntime.XUserHandle userHandle, XUserLocalId localId)
			{
				this.userHandle = userHandle;
				this.localId = localId;
			}

			~User()
			{
				ReleaseUnmanagedResources();
			}

			private void ReleaseUnmanagedResources()
			{
				if (isDisposed)
				{
					return;
				}
				UserState = UserState.NotSignedIn;
				isDisposed = true;
				if (userHandle != null)
				{
					SDK.XUserCloseHandle(userHandle);
					userHandle = null;
				}
				if (xblContextHandle != null)
				{
					SDK.XBL.XblContextCloseHandle(xblContextHandle);
					xblContextHandle = null;
				}
				if (xblStoreContext != null)
				{
					SDK.XStoreCloseContextHandle(xblStoreContext);
					xblStoreContext = null;
				}
				if (UserDisplayImage != null)
				{
					Texture2D image = UserDisplayImage;
					CoreLoop.InvokeSafe(delegate
					{
						UnityEngine.Object.Destroy(image);
					});
					UserDisplayImage = null;
				}
				FinishUserSignOut();
				saveHandler?.Dispose();
				saveHandler = null;
			}

			public void Dispose()
			{
				ReleaseUnmanagedResources();
				GC.SuppressFinalize(this);
			}

			public void Init()
			{
				if (isDisposed)
				{
					return;
				}
				hasGamerTag = Succeeded(SDK.XUserGetGamertag(userHandle, XUserGamertagComponent.UniqueModern, out var gamertag), "Get gamertag : " + GetGamerID());
				if (hasGamerTag)
				{
					GamerTag = gamertag;
					if (isActiveUser)
					{
						UpdateEngagedUser(this);
					}
				}
				SDK.XUserGetGamerPictureAsync(userHandle, XUserGamerPictureSize.Small, delegate(int hresult, byte[] buffer)
				{
					if (Succeeded(hresult, "Get user display pic."))
					{
						CoreLoop.InvokeSafe(delegate
						{
							if (UserDisplayImage != null)
							{
								UnityEngine.Object.Destroy(UserDisplayImage);
							}
							UserDisplayImage = new Texture2D(64, 64, TextureFormat.BGRA32, mipChain: false, linear: false);
							UserDisplayImage.LoadImage(buffer);
							if (isActiveUser)
							{
								UpdateEngagedUser(this);
							}
						});
					}
				});
				InitialiseSaveSystem(reinitialise: false);
				if (xblContextHandle == null)
				{
					Succeeded(SDK.XBL.XblContextCreateHandle(userHandle, out xblContextHandle), "Create Xbox Live context : " + GetGamerID());
				}
				if (xblStoreContext == null)
				{
					Succeeded(SDK.XStoreCreateContext(userHandle, out xblStoreContext), "Create Xbox store context : " + GetGamerID());
				}
			}

			public string GetGamerID()
			{
				return $"\"{GamerTag}\" - {localId.value}";
			}

			public void SetUserState(UserState userState)
			{
				if (!isDisposed)
				{
					UserState = userState;
				}
			}

			public void SetUserActive(bool isActive)
			{
				isActiveUser = isActive;
			}

			public void InitialiseSaveSystem(bool reinitialise)
			{
				if (isDisposed)
				{
					return;
				}
				lock (saveLock)
				{
					if (reinitialise && saveHandler != null)
					{
						saveHandler.Dispose();
						saveHandler = null;
					}
					if (saveHandler != null)
					{
						return;
					}
					saveHandler = new GameCoreSaveHandler();
					SaveInitialised = false;
					saveHandler.InitializeAsync(userHandle, SCID, delegate(int hresult)
					{
						lock (saveLock)
						{
							if (Succeeded(hresult, "Initialise game save provider : " + GetGamerID()))
							{
								SaveInitialised = true;
							}
							else
							{
								saveHandler = null;
							}
						}
						NotifyUserSaveInitialised(this, hresult);
					});
				}
			}

			public void DeferUserSignOut()
			{
				if (signOutDeferralHandle == null && Succeeded(SDK.XUserGetSignOutDeferral(out signOutDeferralHandle), "Delay user " + GetGamerID() + " sign out"))
				{
					signOutTimer = 2f;
					AddDeferredSignOut(this);
				}
			}

			public bool UpdateSignOutDeferral(float deltaTime)
			{
				signOutTimer -= deltaTime;
				if (signOutTimer > 0f)
				{
					return true;
				}
				FinishUserSignOut();
				return false;
			}

			public void FinishUserSignOut()
			{
				if (signOutDeferralHandle != null)
				{
					SDK.XUserCloseSignOutDeferralHandle(signOutDeferralHandle);
					signOutDeferralHandle = null;
					UserState = UserState.SignedOut;
					DisposeSave();
				}
			}

			public void DisposeSave()
			{
				lock (saveLock)
				{
					bool flag = SaveInitialised;
					if (SaveInitialised || saveHandler != null)
					{
						SaveInitialised = false;
						saveHandler?.Dispose();
						flag = true;
					}
					saveHandler = null;
					if (!flag)
					{
						return;
					}
				}
				lock (userLock)
				{
					if (activeUser == this)
					{
						lock (GameCoreRuntimeManager.saveLock)
						{
							SaveSystemInitialised = false;
							return;
						}
					}
				}
			}

			public void FetchAchievements()
			{
				if (isDisposed || isFetchingAchievementData || XblContextHandle == null)
				{
					return;
				}
				isFetchingAchievementData = true;
				if (!Succeeded(SDK.XUserGetId(userHandle, out var userId), "Get Xbox user ID"))
				{
					isFetchingAchievementData = false;
					return;
				}
				SDK.XBL.XblAchievementsGetAchievementsForTitleIdAsync(XblContextHandle, userId, TitleID, XblAchievementType.All, unlockedOnly: false, XblAchievementOrderBy.DefaultOrder, 0u, 100u, delegate(int hresult, XblAchievementsResultHandle result)
				{
					if (!Succeeded(hresult, "Fetching Achievements for " + GetGamerID() + "."))
					{
						isFetchingAchievementData = false;
						SDK.XBL.XblAchievementsResultCloseHandle(result);
					}
					else
					{
						SDK.XBL.XblAchievementsResultGetAchievements(result, out var achievements);
						XblAchievement[] array = achievements;
						foreach (XblAchievement xblAchievement in array)
						{
							if (int.TryParse(xblAchievement.Id, out var result2))
							{
								xblAchievements[result2] = xblAchievement;
							}
						}
						SDK.XBL.XblAchievementsResultCloseHandle(result);
					}
				});
			}

			public void UpdateAchievementState(string achievementID)
			{
				if (isDisposed || isFetchingAchievementData || XblContextHandle == null)
				{
					return;
				}
				if (!Succeeded(SDK.XUserGetId(userHandle, out var userId), "Get Xbox user ID"))
				{
					isFetchingAchievementData = false;
					return;
				}
				SDK.XBL.XblAchievementsGetAchievementAsync(XblContextHandle, userId, SCID, achievementID, delegate(int hresult, XblAchievementsResultHandle result)
				{
					if (!Succeeded(hresult, "Updating Achievement '" + achievementID + "' for " + GetGamerID() + "."))
					{
						SDK.XBL.XblAchievementsResultCloseHandle(result);
					}
					else
					{
						SDK.XBL.XblAchievementsResultGetAchievements(result, out var achievements);
						if (achievements != null)
						{
							XblAchievement[] array = achievements;
							foreach (XblAchievement xblAchievement in array)
							{
								if (int.TryParse(xblAchievement.Id, out var result2))
								{
									xblAchievements[result2] = xblAchievement;
								}
							}
						}
						SDK.XBL.XblAchievementsResultCloseHandle(result);
					}
				});
			}

			public void UnlockAchievement(string achievementId, uint progress)
			{
				if (!Succeeded(SDK.XUserGetId(userHandle, out var userId), "Get Xbox user ID"))
				{
					return;
				}
				lock (unlockLock)
				{
					if (activeUnlocks.TryGetValue(achievementId, out var value) && value >= progress)
					{
						return;
					}
					activeUnlocks[achievementId] = progress;
				}
				hasFetchedSinceUnlock = false;
				SDK.XBL.XblAchievementsUpdateAchievementAsync(XblContextHandle, userId, achievementId, progress, delegate(int hresult)
				{
					lock (unlockLock)
					{
						if (activeUnlocks.TryGetValue(achievementId, out var value2) && value2 <= progress)
						{
							activeUnlocks.Remove(achievementId);
						}
					}
					if (Succeeded(hresult, "Unlock achievement " + achievementId) && progress >= 100 && !hasFetchedSinceUnlock)
					{
						GameCoreRuntimeManager.UpdateAchievementState(achievementId);
					}
				});
			}
		}

		public sealed class RateLimiter
		{
			private DateTime nextAllowedTime = DateTime.MinValue;

			private readonly TimeSpan callInterval;

			private readonly TimeSpan burstLimit;

			public RateLimiter(int maxCalls, TimeSpan perTimeSpan)
			{
				if (maxCalls <= 0)
				{
					UnityEngine.Debug.LogError("Max calls must be at least 1");
					maxCalls = 1;
				}
				callInterval = TimeSpan.FromTicks(perTimeSpan.Ticks / maxCalls);
				burstLimit = callInterval * (maxCalls - 1);
			}

			public bool CanMakeCall()
			{
				DateTime utcNow = DateTime.UtcNow;
				if (utcNow + burstLimit < nextAllowedTime)
				{
					return false;
				}
				nextAllowedTime = ((utcNow > nextAllowedTime) ? (utcNow + callInterval) : (nextAllowedTime + callInterval));
				return true;
			}
		}

		public delegate void UserSignInEvent(bool success);

		private const int _100PercentAchievementProgress = 100;

		private const string DEFAULT_SCID = "00000000-0000-0000-0000-0000636f5860";

		private const float SIGN_OUT_DELAY = 2f;

		private static uint s_TitleId;

		private static volatile bool threadEnding;

		private static volatile bool threadActive;

		private static Thread dispatchThread;

		private static Dictionary<int, string> _hresultToFriendlyErrorLookup;

		private static bool saveSystemInitialised;

		private static object saveLock = new object();

		public static UserSignInEvent userSignInCallback;

		private static readonly List<User> signOutDeferrals = new List<User>();

		private static readonly Dictionary<ulong, User> connectedUsers = new Dictionary<ulong, User>();

		private static User activeUser;

		private static readonly object userLock = new object();

		private static float timeToSignOut;

		private static volatile bool isAddingUser;

		private static bool hasSignedInOnce;

		private static bool isDelayingSignOut;

		private static bool signInRequested;

		private static bool registeredUserEvents;

		private static volatile bool hasFetchedSinceUnlock;

		private static XRegistrationToken userEventToken;

		private static DeltaTimeCalculator deltaTimeCalculator = new DeltaTimeCalculator();

		private static RateLimiter achivementRateLimiter = new RateLimiter(1, TimeSpan.FromSeconds(5.0));

		private static volatile bool achievementUpdateQueued;

		private static bool registerSuspendEvent;

		private static volatile bool reconfirmUser;

		public static bool Initialized { get; private set; }

		public static uint TitleID => s_TitleId;

		public static string SCID { get; private set; }

		public static bool SaveSystemInitialised
		{
			get
			{
				return saveSystemInitialised;
			}
			private set
			{
				if (saveSystemInitialised != value)
				{
					saveSystemInitialised = value;
					Platform.Current.LoadSharedDataAndNotify(value);
				}
			}
		}

		public static string GamerTag { get; private set; }

		public static Texture2D UserDisplayImage { get; private set; }

		public static bool UserSignedIn
		{
			get
			{
				if (!isAddingUser)
				{
					User user = activeUser;
					if (user != null)
					{
						return user.UserState == UserState.SignedIn;
					}
					return false;
				}
				return false;
			}
		}

		public static bool UserSignInPending => isAddingUser;

		public static event Action OnSaveSystemInitialised;

		public static void InitializeRuntime(bool forceInitialization = false)
		{
			if (!Initialized || forceInitialization)
			{
				InitializeHresultToFriendlyErrorLookup();
				SCID = GetSCID();
				if (Succeeded(SDK.XGameRuntimeInitialize(), "Initializing Game Core runtime"))
				{
					StartAsyncDispatchThread();
					Initialized = true;
					Succeeded(SDK.XGameGetXboxTitleId(out s_TitleId), "Retrieving Title ID");
					Succeeded(SDK.XBL.XblInitialize(SCID), "Initialize Xbox Live");
					RegisterEvents();
					SignIn();
				}
			}
		}

		public static void CleanUpRuntime()
		{
			if (!Initialized)
			{
				return;
			}
			CloseSaveHandler();
			foreach (User value in connectedUsers.Values)
			{
				value.Dispose();
			}
			connectedUsers.Clear();
			if (dispatchThread != null)
			{
				threadEnding = true;
				dispatchThread.Join();
				dispatchThread = null;
			}
			SDK.XGameRuntimeUninitialize();
			UnregisterEvents();
			Initialized = false;
		}

		public static void CleanUpUsers()
		{
			List<ulong> list = new List<ulong>();
			foreach (KeyValuePair<ulong, User> connectedUser in connectedUsers)
			{
				if (connectedUser.Value != activeUser)
				{
					list.Add(connectedUser.Key);
					connectedUser.Value.Dispose();
				}
			}
			foreach (ulong item in list)
			{
				connectedUsers.Remove(item);
			}
		}

		public static void ClearAllUsers()
		{
			foreach (KeyValuePair<ulong, User> connectedUser in connectedUsers)
			{
				connectedUser.Value?.Dispose();
			}
			connectedUsers.Clear();
		}

		private static void RegisterEvents()
		{
			if (!registerSuspendEvent)
			{
				registerSuspendEvent = true;
				RegisterUserChangeEvents();
			}
		}

		private static void UnregisterEvents()
		{
			if (registerSuspendEvent)
			{
				registerSuspendEvent = false;
				UnregisterUserEvents();
			}
		}

		private static string GetSCID()
		{
			return "00000000-0000-0000-0000-0000636f5860";
		}

		private static void InitializeHresultToFriendlyErrorLookup()
		{
			if (_hresultToFriendlyErrorLookup == null)
			{
				_hresultToFriendlyErrorLookup = new Dictionary<int, string>();
				_hresultToFriendlyErrorLookup.Add(-2143330041, "IAP_UNEXPECTED: Does the player you are signed in as have a license for the game? You can get one by downloading your game from the store and purchasing it first. If you can't find your game in the store, have you published it in Partner Center?");
				_hresultToFriendlyErrorLookup.Add(-1994108656, "E_GAMEUSER_NO_PACKAGE_IDENTITY: Are you trying to call GDK APIs from the Unity editor? To call GDK APIs, you must use the GDK > Build and Run menu. You can debug your code by attaching the Unity debugger once yourgame is launched.");
				_hresultToFriendlyErrorLookup.Add(-1994129152, "E_GAMERUNTIME_NOT_INITIALIZED: Are you trying to call GDK APIs from the Unity editor? To call GDK APIs, you must use the GDK > Build and Run menu. You can debug your code by attaching the Unity debugger once yourgame is launched.");
				_hresultToFriendlyErrorLookup.Add(-2015559675, "AM_E_XAST_UNEXPECTED: Have you added the Windows 10 PC platform on the Xbox Settings page in Partner Center? Learn more: aka.ms/sandboxtroubleshootingguide");
			}
		}

		private static void StartAsyncDispatchThread()
		{
			if (dispatchThread == null)
			{
				dispatchThread = new Thread(DispatchGXDKTaskQueue)
				{
					Name = "GXDK Task Queue Dispatch",
					IsBackground = true
				};
				threadEnding = false;
				threadActive = true;
				dispatchThread.Start();
			}
		}

		private static void DispatchGXDKTaskQueue()
		{
			while (threadActive)
			{
				if (reconfirmUser)
				{
					reconfirmUser = false;
					ConfirmActiveUser();
				}
				if (activeUser == null && signInRequested)
				{
					SignIn();
				}
				if (achievementUpdateQueued)
				{
					FetchAchievements();
				}
				UpdateDeferredSignOuts();
				SDK.XTaskQueueDispatch();
				if (threadEnding)
				{
					break;
				}
				Thread.Sleep(32);
			}
			threadActive = false;
		}

		private static void OnSuspend()
		{
		}

		private static void OnResume(double secondssuspended)
		{
			CoreLoop.InvokeSafe(delegate
			{
				InitializeRuntime(forceInitialization: true);
				reconfirmUser = true;
				TrySet120hz();
			});
		}

		private static void ConfirmActiveUser()
		{
			reconfirmUser = false;
			User user = activeUser;
			SDK.XUserGetState(user.userHandle, out var state);
			switch (state)
			{
			case XUserState.SignedIn:
				lock (saveLock)
				{
					SaveSystemInitialised = false;
				}
				user.InitialiseSaveSystem(reinitialise: true);
				return;
			case XUserState.SigningOut:
				return;
			}
			lock (userLock)
			{
				user.DisposeSave();
				RemoveUser(user);
				ClearAllUsers();
			}
		}

		public static void TrySet120hz()
		{
			if (Application.platform != RuntimePlatform.GameCoreXboxSeries)
			{
				return;
			}
			Resolution[] resolutions = Screen.resolutions;
			Resolution current = Screen.currentResolution;
			List<Resolution> list = resolutions.Where((Resolution r) => r.width == current.width && r.height == current.height).ToList();
			Resolution? resolution = null;
			foreach (Resolution item in list)
			{
				if (Mathf.Approximately((float)item.refreshRateRatio.value, 120f))
				{
					resolution = item;
					break;
				}
			}
			if (!resolution.HasValue)
			{
				using IEnumerator<Resolution> enumerator2 = (from r in list
					where r.refreshRateRatio.value < 120.0
					orderby r.refreshRateRatio.value descending
					select r).GetEnumerator();
				if (enumerator2.MoveNext())
				{
					Resolution current3 = enumerator2.Current;
					resolution = current3;
				}
			}
			if (resolution.HasValue)
			{
				Resolution value = resolution.Value;
				if (!Mathf.Approximately((float)value.refreshRateRatio.value, (float)current.refreshRateRatio.value))
				{
					UnityEngine.Debug.Log($"Setting resolution to {value.width}x{value.height} @ {value.refreshRateRatio.value}Hz");
					Screen.SetResolution(value.width, value.height, FullScreenMode.FullScreenWindow, value.refreshRateRatio);
				}
				else
				{
					UnityEngine.Debug.Log($"Already at desired resolution: {value.width}x{value.height} @ {value.refreshRateRatio.value}Hz");
				}
			}
			else
			{
				UnityEngine.Debug.LogWarning($"No matching refresh rate found for current resolution: {current.width}x{current.height} @ {current.refreshRateRatio.value}Hz.");
			}
		}

		[System.Diagnostics.Conditional("UNITY_GAMECORE")]
		private static void LogNetworkStatus(string context)
		{
		}

		private static void NotifyUserSaveInitialised(User user, int hresult)
		{
			if (!Succeeded(hresult, "Initialize game save provider") || user == null || activeUser != user)
			{
				return;
			}
			lock (saveLock)
			{
				SaveSystemInitialised = false;
				SaveSystemInitialised = true;
				GameCoreRuntimeManager.OnSaveSystemInitialised?.Invoke();
				Action result;
				while (user.saveQueue.TryDequeue(out result))
				{
					try
					{
						result?.Invoke();
					}
					catch (Exception)
					{
					}
				}
			}
		}

		private static void CloseSaveHandler()
		{
			lock (saveLock)
			{
				SaveSystemInitialised = false;
			}
		}

		public static string GetSaveSlotContainerName(int slot)
		{
			return $"save{slot}";
		}

		public static string GetMainSaveName(int slot)
		{
			return $"user{slot}.dat";
		}

		public static string GetRestoreContainerName(int slot)
		{
			return $"restore{slot}";
		}

		public static void Save(string containerName, string blobName, byte[] data, Action<bool> callback)
		{
			string operationFriendlyName = "Save : \"" + containerName + "\" : \"" + blobName + "\"";
			User user = activeUser;
			if (user != null)
			{
				lock (saveLock)
				{
					if (!SaveSystemInitialised)
					{
						user.saveQueue.Enqueue(DoOperation);
						return;
					}
				}
				DoOperation();
			}
			else
			{
				UnityEngine.Debug.LogError(operationFriendlyName + " failed. No user detected.");
				callback?.Invoke(obj: false);
			}
			void DoOperation()
			{
				bool flag = false;
				lock (user.saveLock)
				{
					if (user.saveHandler != null)
					{
						user.saveHandler.Save(containerName, blobName, data, GameSaveSaveCompleted);
					}
					else
					{
						flag = true;
					}
				}
				if (flag)
				{
					callback?.Invoke(obj: false);
				}
			}
			void GameSaveSaveCompleted(int hresult)
			{
				bool obj = Succeeded(hresult, operationFriendlyName);
				callback?.Invoke(obj);
			}
		}

		public static void FileExists(string containerName, string blobName, Action<bool> callback)
		{
			string operationFriendlyName = "Check File Exists : \"" + containerName + "\" : \"" + blobName + "\"";
			if (callback == null)
			{
				return;
			}
			User user = activeUser;
			if (user != null)
			{
				lock (saveLock)
				{
					if (!SaveSystemInitialised)
					{
						user.saveQueue.Enqueue(DoOperation);
						return;
					}
				}
				DoOperation();
			}
			else
			{
				UnityEngine.Debug.LogError(operationFriendlyName + " failed. No user detected.");
				callback?.Invoke(obj: false);
			}
			void DoOperation()
			{
				bool flag = false;
				lock (user.saveLock)
				{
					if (user.saveHandler != null)
					{
						user.saveHandler.QueryContainerBlobs(containerName, QueryCompleted);
					}
					else
					{
						flag = true;
					}
				}
				if (flag)
				{
					callback?.Invoke(obj: false);
				}
			}
			void QueryCompleted(int hresult, Dictionary<string, uint> blobInfos)
			{
				Succeeded(hresult, operationFriendlyName);
				bool obj = blobInfos.ContainsKey(blobName);
				callback?.Invoke(obj);
			}
		}

		public static void EnumerateFiles(string containerName, Action<string[]> callback)
		{
			string operationFriendlyName = "Enumerate \"" + containerName + "\"";
			if (callback == null)
			{
				return;
			}
			User user = activeUser;
			if (user != null)
			{
				lock (saveLock)
				{
					if (!SaveSystemInitialised)
					{
						user.saveQueue.Enqueue(DoOperation);
						return;
					}
				}
				DoOperation();
			}
			else
			{
				UnityEngine.Debug.LogError(operationFriendlyName + " failed. No user detected.");
				callback?.Invoke(null);
			}
			void DoOperation()
			{
				bool flag = false;
				lock (user.saveLock)
				{
					if (user.saveHandler != null)
					{
						user.saveHandler.QueryContainerBlobs(containerName, EnumerateComplete);
					}
					else
					{
						flag = true;
					}
				}
				if (flag)
				{
					callback?.Invoke(null);
				}
			}
			void EnumerateComplete(int hresult, Dictionary<string, uint> results)
			{
				if (Succeeded(hresult, operationFriendlyName))
				{
					if (results.Count > 0)
					{
						callback(results.Keys.ToArray());
					}
					else
					{
						callback(null);
					}
				}
				else
				{
					callback(null);
				}
			}
		}

		public static void LoadSaveData(string containerName, string blobName, Action<byte[]> callback)
		{
			string operationFriendlyName = "Loaded Blob : \"" + containerName + "\" : \"" + blobName + "\"";
			if (callback == null)
			{
				return;
			}
			User user = activeUser;
			if (user != null)
			{
				lock (saveLock)
				{
					if (!SaveSystemInitialised)
					{
						user.saveQueue.Enqueue(DoOperation);
						return;
					}
				}
				DoOperation();
			}
			else
			{
				UnityEngine.Debug.LogError(operationFriendlyName + " failed. No user detected.");
				callback?.Invoke(null);
			}
			void DoOperation()
			{
				bool flag = false;
				lock (user.saveLock)
				{
					if (user.saveHandler != null)
					{
						user.saveHandler.Load(containerName, blobName, GameSaveLoadCompleted);
					}
					else
					{
						flag = true;
					}
				}
				if (flag)
				{
					callback?.Invoke(null);
				}
			}
			void GameSaveLoadCompleted(int hresult, byte[] savedData)
			{
				Succeeded(hresult, operationFriendlyName);
				callback(savedData);
			}
		}

		public static void DeleteContainer(string containerName, Action<bool> callback)
		{
			string operationFriendlyName = "Deleted Container : \"" + containerName + "\"";
			User user = activeUser;
			if (user != null)
			{
				lock (saveLock)
				{
					if (!SaveSystemInitialised)
					{
						user.saveQueue.Enqueue(DoOperation);
						return;
					}
				}
				DoOperation();
			}
			else
			{
				UnityEngine.Debug.LogError(operationFriendlyName + " failed. No user detected.");
				callback?.Invoke(obj: false);
			}
			void DoOperation()
			{
				bool flag = false;
				lock (user.saveLock)
				{
					if (user.saveHandler != null)
					{
						user.saveHandler.Delete(containerName, GameSaveLoadCompleted);
					}
					else
					{
						flag = true;
					}
				}
				if (flag)
				{
					callback?.Invoke(obj: false);
				}
			}
			void GameSaveLoadCompleted(int hresult)
			{
				bool obj = Succeeded(hresult, operationFriendlyName);
				callback?.Invoke(obj);
			}
		}

		public static void DeleteBlob(string containerName, string blobName, Action<bool> callback)
		{
			string operationFriendlyName = "Deleted Blob : \"" + containerName + "\" : \"" + blobName + "\"";
			User user = activeUser;
			if (user != null)
			{
				lock (saveLock)
				{
					if (!SaveSystemInitialised)
					{
						user.saveQueue.Enqueue(DoOperation);
						return;
					}
				}
				DoOperation();
			}
			else
			{
				UnityEngine.Debug.LogError(operationFriendlyName + " failed. No user detected.");
				callback?.Invoke(obj: false);
			}
			void DoOperation()
			{
				bool flag = false;
				lock (user.saveLock)
				{
					if (user.saveHandler != null)
					{
						user.saveHandler.Delete(containerName, blobName, GameSaveLoadCompleted);
					}
					else
					{
						flag = true;
					}
				}
				if (flag)
				{
					callback?.Invoke(obj: false);
				}
			}
			void GameSaveLoadCompleted(int hresult)
			{
				bool obj = Succeeded(hresult, operationFriendlyName);
				callback?.Invoke(obj);
			}
		}

		private static void RegisterUserChangeEvents()
		{
			if (!registeredUserEvents)
			{
				registeredUserEvents = true;
				SDK.XUserRegisterForChangeEvent(UserChangeEventCallback, out userEventToken);
			}
		}

		private static void UnregisterUserEvents()
		{
			if (registeredUserEvents)
			{
				registeredUserEvents = false;
				SDK.XUserUnregisterForChangeEvent(userEventToken);
				userEventToken = null;
			}
		}

		private static void UserChangeEventCallback(XUserLocalId userLocalId, XUserChangeEvent eventType)
		{
			connectedUsers.TryGetValue(userLocalId.value, out var value);
			switch (eventType)
			{
			case XUserChangeEvent.SignedInAgain:
				value?.SetUserState(UserState.SignedIn);
				value?.InitialiseSaveSystem(reinitialise: true);
				break;
			case XUserChangeEvent.SigningOut:
				value?.SetUserState(UserState.SigningOut);
				if (activeUser != null && activeUser.localId.value == userLocalId.value)
				{
					activeUser.DeferUserSignOut();
					if (GamerTag != null || UserDisplayImage != null)
					{
						GamerTag = null;
						UserDisplayImage = null;
						NotifyPlatformEngagedDisplayInfoChanged();
					}
				}
				break;
			case XUserChangeEvent.SignedOut:
				value?.SetUserState(UserState.NotSignedIn);
				RemoveActiveUser(userLocalId);
				break;
			case XUserChangeEvent.Gamertag:
			case XUserChangeEvent.GamerPicture:
			case XUserChangeEvent.Privileges:
				break;
			}
		}

		public static void UpdateDeferredSignOuts()
		{
			if (!isDelayingSignOut)
			{
				return;
			}
			float deltaTimeSeconds = deltaTimeCalculator.GetDeltaTimeSeconds();
			for (int i = 0; i < signOutDeferrals.Count; i++)
			{
				if (!signOutDeferrals[i].UpdateSignOutDeferral(deltaTimeSeconds))
				{
					signOutDeferrals.RemoveAt(i);
				}
			}
			if (signOutDeferrals.Count == 0)
			{
				deltaTimeCalculator.Reset();
				isDelayingSignOut = false;
			}
		}

		private static void AddDeferredSignOut(User user)
		{
			if (user != null)
			{
				if (signOutDeferrals.Count == 0)
				{
					deltaTimeCalculator.Reset();
					deltaTimeCalculator.Start();
				}
				signOutDeferrals.Add(user);
				isDelayingSignOut = true;
			}
		}

		public static void SwitchUser()
		{
			RemoveUser(activeUser);
			lock (saveLock)
			{
				SaveSystemInitialised = false;
			}
		}

		public static void RequestUserSignIn(UserSignInEvent callback = null)
		{
			lock (userLock)
			{
				if (!isAddingUser)
				{
					userSignInCallback = (UserSignInEvent)Delegate.Combine(userSignInCallback, callback);
					SignIn();
				}
				else
				{
					callback?.Invoke(success: false);
				}
			}
		}

		private static void SignIn()
		{
			lock (userLock)
			{
				if (!isAddingUser)
				{
					isAddingUser = true;
					signInRequested = false;
					if (activeUser != null)
					{
						UpdateEngagedUser(null);
					}
					RegisterUserChangeEvents();
					SDK.XUserAddAsync(hasSignedInOnce ? XUserAddOptions.AddDefaultUserSilently : XUserAddOptions.AddDefaultUserAllowingUI, AddUserComplete);
				}
			}
		}

		public static void SignOutUser()
		{
		}

		private static void RemoveActiveUser(XUserLocalId userLocalId)
		{
			User value;
			if (activeUser != null && activeUser.localId.value == userLocalId.value)
			{
				UpdateEngagedUser(null);
				RemoveUser(activeUser);
			}
			else if (connectedUsers.TryGetValue(userLocalId.value, out value))
			{
				RemoveUser(value);
			}
		}

		private static void RemoveUser(User user)
		{
			if (user == null)
			{
				return;
			}
			user.SetUserState(UserState.NotSignedIn);
			user.FinishUserSignOut();
			lock (userLock)
			{
				if (activeUser == user)
				{
					activeUser.SetUserActive(isActive: false);
					UpdateEngagedUser(null);
					activeUser = null;
					lock (saveLock)
					{
						SaveSystemInitialised = false;
					}
				}
			}
			if (connectedUsers.TryGetValue(user.localId.value, out var value))
			{
				if (value != user)
				{
					connectedUsers.Remove(user.localId.value);
					value?.Dispose();
					user.Dispose();
				}
			}
			else
			{
				user.Dispose();
			}
		}

		private static void AddUserComplete(int hresult, XGamingRuntime.XUserHandle userHandle)
		{
			UserSignInEvent userSignInEvent = null;
			bool flag = Succeeded(hresult, "Sign in.");
			User value = null;
			if (flag)
			{
				if (Succeeded(SDK.XUserGetLocalId(userHandle, out var userLocalId), "Get Local User ID"))
				{
					if (!connectedUsers.TryGetValue(userLocalId.value, out value) || value.IsDisposed)
					{
						value = new User(userHandle, userLocalId);
						value.Init();
						connectedUsers[userLocalId.value] = value;
					}
				}
				else
				{
					flag = false;
				}
			}
			if (value == null)
			{
				if (userHandle != null)
				{
					SDK.XUserCloseHandle(userHandle);
				}
				flag = false;
			}
			else
			{
				if (userHandle != value.userHandle)
				{
					SDK.XUserCompare(userHandle, value.userHandle, out var comparisonResult);
					if (comparisonResult == 0)
					{
						SDK.XUserCloseHandle(userHandle);
						userHandle = value.userHandle;
					}
					else
					{
						flag = false;
					}
				}
				value.SetUserState(UserState.SignedIn);
			}
			if (flag)
			{
				if (!hasSignedInOnce)
				{
					hasSignedInOnce = true;
				}
				SetActiveUser(value);
			}
			else if (hasSignedInOnce && activeUser != null)
			{
				SetActiveUser(activeUser);
			}
			lock (userLock)
			{
				userSignInEvent = userSignInCallback;
				isAddingUser = false;
				userSignInCallback = null;
			}
			userSignInEvent?.Invoke(flag);
		}

		private static void SetActiveUser(User user)
		{
			if (user == null)
			{
				return;
			}
			lock (userLock)
			{
				bool flag = false;
				if (activeUser != null && activeUser != user)
				{
					flag = true;
					RemoveUser(activeUser);
				}
				activeUser = user;
				user.SetUserActive(isActive: true);
				if (user.SaveInitialised)
				{
					lock (saveLock)
					{
						if (flag)
						{
							SaveSystemInitialised = false;
						}
						SaveSystemInitialised = true;
					}
				}
				else
				{
					lock (saveLock)
					{
						SaveSystemInitialised = false;
					}
					user.InitialiseSaveSystem(reinitialise: false);
				}
			}
			FetchAchievements();
			UpdateEngagedUser(user);
		}

		private static void UpdateEngagedUser(User user)
		{
			if (user != null)
			{
				GamerTag = user.GamerTag;
				UserDisplayImage = user.UserDisplayImage;
			}
			else
			{
				GamerTag = null;
				UserDisplayImage = null;
			}
			NotifyPlatformEngagedDisplayInfoChanged();
		}

		private static void NotifyPlatformEngagedDisplayInfoChanged()
		{
			CoreLoop.InvokeSafe(delegate
			{
				if ((bool)Platform.Current)
				{
					Platform.Current.NotifyEngagedDisplayInfoChanged();
				}
			});
		}

		public static void FetchAchievements()
		{
			hasFetchedSinceUnlock = true;
			if (activeUser == null)
			{
				achievementUpdateQueued = false;
			}
			else if (achivementRateLimiter.CanMakeCall() && activeUser.FetchRateLimiter.CanMakeCall())
			{
				activeUser.FetchAchievements();
				achievementUpdateQueued = false;
			}
			else
			{
				achievementUpdateQueued = true;
			}
		}

		public static void UpdateAchievementState(string achievementId)
		{
			if (activeUser == null)
			{
				achievementUpdateQueued = false;
			}
			else if (!achievementUpdateQueued && !activeUser.IsFetchingAchievementData)
			{
				activeUser.UpdateAchievementState(achievementId);
			}
		}

		public static bool TryGetAchievement(int achievementId, out XblAchievement xblAchievement)
		{
			if (activeUser == null)
			{
				xblAchievement = null;
				return false;
			}
			return activeUser.xblAchievements.TryGetValue(achievementId, out xblAchievement);
		}

		public static void UnlockAchievement(int achievementId)
		{
			UnlockAchievement(achievementId.ToString());
		}

		public static void UnlockAchievement(string achievementId)
		{
			UnlockAchievement(achievementId, 100u);
		}

		public static void UnlockAchievement(int achievementId, uint progress)
		{
			UnlockAchievement(achievementId.ToString(), progress);
		}

		public static void UnlockAchievement(string achievementId, uint progress)
		{
			if (!string.IsNullOrEmpty(achievementId))
			{
				activeUser?.UnlockAchievement(achievementId, progress);
			}
		}

		private static void HandleQueryForUpdatesComplete(int hresult, XStorePackageUpdate[] packageUpdates)
		{
			if (activeUser == null)
			{
				return;
			}
			XStoreContext xblStoreContext = activeUser.xblStoreContext;
			List<string> list = new List<string>();
			if (hresult >= 0 && packageUpdates != null && packageUpdates.Length != 0)
			{
				foreach (XStorePackageUpdate xStorePackageUpdate in packageUpdates)
				{
					list.Add(xStorePackageUpdate.PackageIdentifier);
				}
				SDK.XStoreDownloadAndInstallPackageUpdatesAsync(xblStoreContext, list.ToArray(), DownloadFinishedCallback);
			}
		}

		private static void DownloadFinishedCallback(int hresult)
		{
			Succeeded(hresult, "DownloadAndInstallPackageUpdates callback");
		}

		public static bool Succeeded(int hresult, string operationFriendlyName)
		{
			bool result = false;
			if (XGamingRuntime.Interop.HR.SUCCEEDED(hresult))
			{
				result = true;
			}
			return result;
		}
	}
}

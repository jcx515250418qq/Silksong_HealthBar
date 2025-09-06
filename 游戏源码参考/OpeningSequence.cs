using System.Collections;
using GlobalEnums;
using TeamCherry.NestedFadeGroup;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class OpeningSequence : MonoBehaviour, GameManager.ISkippable
{
	[SerializeField]
	private ChainSequence chainSequence;

	[SerializeField]
	private ThreadPriority streamingLoadPriority;

	[SerializeField]
	private ThreadPriority completedLoadPriority;

	[SerializeField]
	private float skipChargeDuration;

	[SerializeField]
	private bool loadSave;

	[SerializeField]
	private NestedFadeGroupBase blanker;

	private bool isAsync;

	private bool isLevelReady;

	private float skipChargeTimer;

	protected void OnEnable()
	{
		chainSequence.TransitionedToNextSequence += OnChangingSequences;
	}

	protected void OnDisable()
	{
		chainSequence.TransitionedToNextSequence -= OnChangingSequences;
	}

	private void OnDestroy()
	{
		GameManager silentInstance = GameManager.SilentInstance;
		if ((bool)silentInstance)
		{
			silentInstance.DeregisterSkippable(this);
		}
	}

	protected IEnumerator Start()
	{
		blanker.AlphaSelf = 0f;
		isAsync = Platform.Current.FetchScenesBeforeFade;
		if (!isAsync)
		{
			return StartSync();
		}
		return StartAsync();
	}

	private IEnumerator StartAsync()
	{
		GameCameras gc = GameCameras.instance;
		GameManager gm = GameManager.instance;
		PlayerData instance = PlayerData.instance;
		gc.cameraFadeFSM.SendEventSafe("FADE SCENE IN");
		gc.OnCinematicBegin();
		PlayMakerFSM.BroadcastEvent("START FADE OUT");
		gm.ui.SetState(UIState.CUTSCENE);
		gm.inputHandler.SetSkipMode(SkipPromptMode.NOT_SKIPPABLE_DUE_TO_LOADING);
		gm.RegisterSkippable(this);
		chainSequence.Begin();
		Platform.Current.SetSceneLoadState(isInProgress: true);
		Platform.Current.SetBackgroundLoadingPriority(streamingLoadPriority);
		string worldSceneName = (loadSave ? instance.respawnScene : "Tut_01");
		bool knightLoadDone = false;
		bool worldLoadDone = false;
		AsyncOperationHandle<SceneInstance> asyncWorldLoad = default(AsyncOperationHandle<SceneInstance>);
		AsyncOperationHandle<GameObject> heroPrefabHandle = gm.LoadHeroPrefab();
		heroPrefabHandle.Completed += delegate
		{
			knightLoadDone = true;
			asyncWorldLoad = Addressables.LoadSceneAsync("Scenes/" + worldSceneName, LoadSceneMode.Single, activateOnLoad: false);
			asyncWorldLoad.Completed += delegate
			{
				worldLoadDone = true;
			};
		};
		isLevelReady = false;
		bool forceUpdateSkip = true;
		while (chainSequence.IsPlaying)
		{
			if (!isLevelReady)
			{
				isLevelReady = knightLoadDone && worldLoadDone;
				_ = isLevelReady;
			}
			SkipPromptMode skipPromptMode = ((!chainSequence.IsCurrentSkipped && chainSequence.CanSkipCurrent) ? ((!isLevelReady || skipChargeTimer < skipChargeDuration) ? SkipPromptMode.NOT_SKIPPABLE_DUE_TO_LOADING : SkipPromptMode.SKIP_PROMPT) : SkipPromptMode.NOT_SKIPPABLE);
			if (gm.inputHandler.SkipMode != skipPromptMode || forceUpdateSkip)
			{
				forceUpdateSkip = false;
				gm.inputHandler.SetSkipMode(skipPromptMode);
			}
			yield return null;
		}
		Platform.Current.SetBackgroundLoadingPriority(completedLoadPriority);
		gm.inputHandler.SetSkipMode(SkipPromptMode.NOT_SKIPPABLE);
		Platform.Current.SetSceneLoadState(isInProgress: true, isHighPriority: true);
		ObjectPool.CreateStartupPools();
		Object.Instantiate(heroPrefabHandle.Result);
		yield return null;
		yield return null;
		if (loadSave)
		{
			gm.RespawningHero = true;
		}
		else
		{
			gm.OnWillActivateFirstLevel();
		}
		gm.nextSceneName = worldSceneName;
		AsyncOperation asyncOperation = asyncWorldLoad.Result.ActivateAsync();
		gm.LastSceneLoad = new SceneLoad(asyncWorldLoad, new GameManager.SceneLoadInfo
		{
			IsFirstLevelForPlayer = true,
			SceneName = worldSceneName
		});
		Platform.Current.RestoreBackgroundLoadingPriority();
		gm.SetupSceneRefs(refreshTilemapInfo: true);
		gm.BeginScene();
		gm.OnNextLevelReady();
		gc.OnCinematicEnd();
		Platform.Current.SetSceneLoadState(isInProgress: false);
		yield return asyncOperation;
	}

	protected IEnumerator StartSync()
	{
		GameCameras gc = GameCameras.instance;
		GameManager gm = GameManager.instance;
		PlayerData pd = PlayerData.instance;
		gc.cameraFadeFSM.SendEventSafe("FADE SCENE IN");
		gc.OnCinematicBegin();
		PlayMakerFSM.BroadcastEvent("START FADE OUT");
		Debug.LogFormat(this, "Starting opening sequence.");
		gm.ui.SetState(UIState.CUTSCENE);
		gm.RegisterSkippable(this);
		chainSequence.Begin();
		bool forceUpdateSkipMode = true;
		while (chainSequence.IsPlaying)
		{
			SkipPromptMode skipPromptMode = ((chainSequence.IsCurrentSkipped || !chainSequence.CanSkipCurrent) ? SkipPromptMode.NOT_SKIPPABLE : ((skipChargeTimer < skipChargeDuration) ? SkipPromptMode.NOT_SKIPPABLE_DUE_TO_LOADING : SkipPromptMode.SKIP_PROMPT));
			if (gm.inputHandler.SkipMode != skipPromptMode || forceUpdateSkipMode)
			{
				forceUpdateSkipMode = false;
				gm.inputHandler.SetSkipMode(skipPromptMode);
			}
			yield return null;
		}
		gm.inputHandler.SetSkipMode(SkipPromptMode.NOT_SKIPPABLE);
		Platform.Current.SetSceneLoadState(isInProgress: true, isHighPriority: true);
		AsyncOperationHandle<GameObject> heroPrefabHandle = gm.LoadHeroPrefab();
		yield return heroPrefabHandle;
		Object.Instantiate(heroPrefabHandle.Result);
		ObjectPool.CreateStartupPools();
		yield return null;
		yield return null;
		if (loadSave)
		{
			gm.RespawningHero = true;
		}
		else
		{
			gm.OnWillActivateFirstLevel();
		}
		string worldSceneName = (gm.nextSceneName = (loadSave ? pd.respawnScene : "Tut_01"));
		AsyncOperationHandle<SceneInstance> worldLoad = Addressables.LoadSceneAsync("Scenes/" + worldSceneName, LoadSceneMode.Single, activateOnLoad: false);
		Platform.Current.SetSceneLoadState(isInProgress: false);
		yield return worldLoad;
		AsyncOperation asyncOperation = worldLoad.Result.ActivateAsync();
		gm.LastSceneLoad = new SceneLoad(worldLoad, new GameManager.SceneLoadInfo
		{
			IsFirstLevelForPlayer = true,
			SceneName = worldSceneName
		});
		gm.SetupSceneRefs(refreshTilemapInfo: true);
		gm.BeginScene();
		gm.OnNextLevelReady();
		gc.OnCinematicEnd();
		yield return asyncOperation;
	}

	protected void Update()
	{
		skipChargeTimer += Time.unscaledDeltaTime;
	}

	public IEnumerator Skip()
	{
		if (chainSequence.CanSkipCurrent)
		{
			for (float elapsed = 0f; elapsed < 0.3f; elapsed += Time.deltaTime)
			{
				blanker.AlphaSelf = elapsed / 0.3f;
				yield return null;
			}
			blanker.AlphaSelf = 1f;
			yield return null;
			chainSequence.SkipSingle();
			while (chainSequence.IsCurrentSkipped)
			{
				skipChargeTimer = 0f;
				yield return null;
			}
		}
	}

	private void OnChangingSequences()
	{
		Debug.LogFormat("Opening sequence changing sequences.");
		skipChargeTimer = 0f;
		blanker.AlphaSelf = 0f;
	}
}

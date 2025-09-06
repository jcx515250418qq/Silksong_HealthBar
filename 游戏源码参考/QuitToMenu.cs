using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

public class QuitToMenu : MonoBehaviour
{
	private static Queue<AsyncOperationHandle> _loadedAssets;

	public const string EXHIBITION_PROFILE_VAR = "ExhibitionModeProfileId";

	protected IEnumerator Start()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		yield return null;
		Platform.Current.SetSceneLoadState(isInProgress: true, isHighPriority: true);
		UIManager instance = UIManager.instance;
		if (instance != null)
		{
			UnityEngine.Object.Destroy(instance.gameObject);
		}
		HeroController instance2 = HeroController.instance;
		if (instance2 != null)
		{
			UnityEngine.Object.Destroy(instance2.gameObject);
		}
		GameCameras instance3 = GameCameras.instance;
		if (instance3 != null)
		{
			UnityEngine.Object.Destroy(instance3.gameObject);
		}
		GameManager instance4 = GameManager.instance;
		if (instance4 != null)
		{
			if (DemoHelper.IsExhibitionMode)
			{
				StaticVariableList.SetValue("ExhibitionModeProfileId", instance4.profileID);
			}
			try
			{
				PersonalObjectPool.ForceReleasePoolManagers();
			}
			catch (Exception exception)
			{
				Debug.LogErrorFormat("Error while cleaning personal object pools as part of quit, attempting to continue regardless.");
				Debug.LogException(exception);
			}
			try
			{
				ObjectPool.RecycleAll();
			}
			catch (Exception exception2)
			{
				Debug.LogErrorFormat("Error while recycling all as part of quit, attempting to continue regardless.");
				Debug.LogException(exception2);
			}
			instance4.playerData = PlayerData.CreateNewSingleton(addEditorOverrides: false);
			instance4.sceneData.Reset();
			instance4.UnloadGlobalPoolPrefab();
			instance4.UnloadHeroPrefab();
			UnityEngine.Object.Destroy(instance4.gameObject);
			ObjectPool instance5 = ObjectPool.instance;
			if ((bool)instance5)
			{
				UnityEngine.Object.Destroy(instance5.gameObject);
			}
		}
		TimeManager.Reset();
		BossSequenceController.Reset();
		QuestTargetCounter.ClearStatic();
		PerformanceHud.ReInit();
		CheatManager.ReInit();
		yield return null;
		yield return null;
		if (_loadedAssets != null)
		{
			while (_loadedAssets.Count > 0)
			{
				Addressables.Release(_loadedAssets.Dequeue());
			}
		}
		yield return null;
		ToolItemLimiter.ClearStatic();
		CollectableItemManager.ClearStatic();
		TweenExtensions.ClenaupInactiveCoroutines();
		ValidatePlayMaker();
		GCManager.ForceCollect();
		yield return Resources.UnloadUnusedAssets();
		StartLoadCoreManagers();
		yield return null;
		bool finishedPrewarm = false;
		AsyncLoadOrderingManager.DoActionAfterAllLoadsComplete(delegate
		{
			finishedPrewarm = true;
		});
		while (!finishedPrewarm)
		{
			yield return null;
		}
		AsyncOperationHandle<SceneInstance> asyncOperationHandle = Addressables.LoadSceneAsync("Scenes/Menu_Title");
		yield return asyncOperationHandle;
		GCManager.ForceCollect();
		yield return Resources.UnloadUnusedAssets();
		UnityEngine.Object.Destroy(base.gameObject);
		GCManager.ForceCollect();
		Platform.Current.SetSceneLoadState(isInProgress: false);
	}

	private static void ValidatePlayMaker()
	{
		List<string> list = PlayMakerValidator.ValidatePlayMakerState();
		if (list == null || list.Count <= 0)
		{
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("Invalid PlayMaker state! Issues were fixed automatically. Look at the errors below to find out what went wrong.");
		stringBuilder.AppendLine();
		foreach (string item in list)
		{
			stringBuilder.AppendLine(item);
			stringBuilder.AppendLine();
		}
		Debug.LogError(stringBuilder.ToString());
		stringBuilder.Clear();
	}

	public static void StartLoadCoreManagers()
	{
		if (_loadedAssets == null)
		{
			_loadedAssets = new Queue<AsyncOperationHandle>();
		}
		StartLoadCoreManager("_GameManager");
		StartLoadCoreManager("_UIManager");
		StartLoadCoreManager("_GameCameras");
	}

	private static void StartLoadCoreManager(string address)
	{
		AsyncOperationHandle<GameObject> asyncOperationHandle = Addressables.LoadAssetAsync<GameObject>(address);
		AsyncLoadOrderingManager.OnStartedLoad(asyncOperationHandle, out var loadHandle);
		asyncOperationHandle.Completed += delegate(AsyncOperationHandle<GameObject> handle)
		{
			_loadedAssets.Enqueue(handle);
			AsyncLoadOrderingManager.OnCompletedLoad(handle, loadHandle);
		};
	}
}

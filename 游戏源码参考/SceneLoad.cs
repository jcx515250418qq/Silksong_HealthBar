using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class SceneLoad
{
	public enum Phases
	{
		FetchBlocked = 0,
		ClearMemPreFetch = 1,
		Fetch = 2,
		ActivationBlocked = 3,
		Activation = 4,
		ClearMemPostActivation = 5,
		GarbageCollect = 6,
		StartCall = 7,
		LoadBoss = 8
	}

	private class PhaseInfo
	{
		public float? BeginTime;

		public float? EndTime;
	}

	public delegate void FetchCompleteDelegate();

	public delegate void WillActivateDelegate();

	public delegate void ActivationCompleteDelegate();

	public delegate void CompleteDelegate();

	public delegate void StartCalledDelegate();

	public delegate void BossLoadCompleteDelegate();

	public delegate void FinishDelegate();

	public readonly GameManager.SceneLoadInfo SceneLoadInfo;

	private static readonly List<AsyncOperationHandle<SceneInstance>> _tempOps = new List<AsyncOperationHandle<SceneInstance>>();

	private AsyncOperationHandle<SceneInstance> operationHandle;

	private readonly MonoBehaviour runner;

	public const int PhaseCount = 9;

	private readonly PhaseInfo[] phaseInfos;

	private static readonly List<AsyncOperation> _assetUnloadOps = new List<AsyncOperation>(10);

	public AsyncOperationHandle<SceneInstance> OperationHandle => operationHandle;

	public string TargetSceneName => SceneLoadInfo.SceneName;

	public bool IsFetchAllowed { get; set; }

	public bool IsActivationAllowed { get; set; }

	public bool IsUnloadAssetsRequired { get; set; }

	public bool IsGarbageCollectRequired { get; set; }

	public bool IsFinished { get; private set; }

	public bool WaitForFade { get; set; }

	public float? BeginTime => phaseInfos[0].BeginTime;

	public event FetchCompleteDelegate FetchComplete;

	public event WillActivateDelegate WillActivate;

	public event ActivationCompleteDelegate ActivationComplete;

	public event CompleteDelegate Complete;

	public event StartCalledDelegate StartCalled;

	public event BossLoadCompleteDelegate BossLoaded;

	public event FinishDelegate Finish;

	public SceneLoad(MonoBehaviour runner, GameManager.SceneLoadInfo sceneLoadInfo)
	{
		this.runner = runner;
		SceneLoadInfo = sceneLoadInfo;
		phaseInfos = new PhaseInfo[9];
		for (int i = 0; i < 9; i++)
		{
			phaseInfos[i] = new PhaseInfo
			{
				BeginTime = null
			};
		}
	}

	public SceneLoad(AsyncOperationHandle<SceneInstance> fromOperationHandle, GameManager.SceneLoadInfo sceneLoadInfo)
	{
		operationHandle = fromOperationHandle;
		SceneLoadInfo = sceneLoadInfo;
	}

	private static string GetMemoryReadOut()
	{
		double num = (double)GCManager.GetMemoryUsage() / 1024.0 / 1024.0;
		double num2 = (double)GCManager.GetMemoryTotal() / 1024.0 / 1024.0;
		double num3 = SystemInfo.systemMemorySize;
		return $"CPU Mem.: {num:n} / {num2:n} / {num3:n}";
	}

	public static bool ShouldLog()
	{
		return CheatManager.EnableLogMessages;
	}

	private void RecordBeginTime(Phases phase)
	{
		phaseInfos[(int)phase].BeginTime = Time.realtimeSinceStartup;
	}

	private void RecordEndTime(Phases phase)
	{
		phaseInfos[(int)phase].EndTime = Time.realtimeSinceStartup;
	}

	public float? GetDuration(Phases phase)
	{
		PhaseInfo phaseInfo = phaseInfos[(int)phase];
		if (phaseInfo.BeginTime.HasValue && phaseInfo.EndTime.HasValue)
		{
			return phaseInfo.EndTime.Value - phaseInfo.BeginTime.Value;
		}
		return null;
	}

	public void Begin()
	{
		runner.StartCoroutine(BeginRoutine());
	}

	private IEnumerator BeginRoutine()
	{
		SceneAdditiveLoadConditional.LoadInSequence = true;
		string address = "Scenes/" + SceneLoadInfo.SceneName;
		AsyncOperationHandle<SceneInstance>? preLoadOperation = ScenePreloader.TakeSceneLoadOperation(address, LoadSceneMode.Additive);
		bool wasPreloaded = preLoadOperation.HasValue;
		RecordBeginTime(Phases.FetchBlocked);
		while (!IsFetchAllowed)
		{
			yield return null;
		}
		RecordEndTime(Phases.FetchBlocked);
		bool hasClearedMemory = false;
		if (IsClearMemoryRequired())
		{
			GameManager.IsCollectingGarbage = true;
			RecordBeginTime(Phases.ClearMemPreFetch);
			yield return LocalTryClearMemory(revertGlobalPool: true, waitForUnload: false);
			hasClearedMemory = true;
			RecordEndTime(Phases.ClearMemPreFetch);
		}
		RecordBeginTime(Phases.Fetch);
		int priority = SceneLoadInfo.AsyncPriority;
		if (CheatManager.OverrideSceneLoadPriority)
		{
			priority = CheatManager.SceneLoadPriority;
		}
		if (wasPreloaded)
		{
			operationHandle = preLoadOperation.Value;
		}
		else if (SceneLoadInfo.SceneResourceLocation != null)
		{
			operationHandle = Addressables.LoadSceneAsync(SceneLoadInfo.SceneResourceLocation, LoadSceneMode.Additive, activateOnLoad: false, priority);
		}
		else
		{
			operationHandle = Addressables.LoadSceneAsync(address, LoadSceneMode.Additive, activateOnLoad: false, priority);
		}
		yield return operationHandle;
		RecordEndTime(Phases.Fetch);
		if (this.FetchComplete != null)
		{
			try
			{
				this.FetchComplete();
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in responders to SceneLoad.FetchComplete. Attempting to continue load regardless.");
				CheatManager.LastErrorText = ex.ToString();
				Debug.LogException(ex);
			}
		}
		RecordBeginTime(Phases.ActivationBlocked);
		if (!wasPreloaded && ScenePreloader.HasPendingOperations)
		{
			yield return runner.StartCoroutine(ScenePreloader.ForceEndPendingOperations());
		}
		while (!IsActivationAllowed)
		{
			yield return null;
		}
		SceneAdditiveLoadConditional.Unload(SceneManager.GetActiveScene(), _tempOps);
		RecordEndTime(Phases.ActivationBlocked);
		RecordBeginTime(Phases.Activation);
		if (this.WillActivate != null)
		{
			try
			{
				this.WillActivate();
			}
			catch (Exception ex2)
			{
				Debug.LogError("Exception in responders to SceneLoad.WillActivate. Attempting to continue load regardless.");
				CheatManager.LastErrorText = ex2.ToString();
				Debug.LogException(ex2);
			}
		}
		if (operationHandle.OperationException != null)
		{
			Debug.LogError("Exception in scene load OperationHandle:");
			CheatManager.LastErrorText = operationHandle.OperationException.ToString();
			Debug.LogException(operationHandle.OperationException);
		}
		yield return operationHandle.Result.ActivateAsync();
		RecordEndTime(Phases.Activation);
		if (this.ActivationComplete != null)
		{
			try
			{
				this.ActivationComplete();
			}
			catch (Exception ex3)
			{
				Debug.LogError("Exception in responders to SceneLoad.ActivationComplete. Attempting to continue load regardless.");
				CheatManager.LastErrorText = ex3.ToString();
				Debug.LogException(ex3);
			}
		}
		foreach (AsyncOperationHandle<SceneInstance> tempOp in _tempOps)
		{
			yield return tempOp;
		}
		_tempOps.Clear();
		while (_assetUnloadOps.Count > 0)
		{
			int index = _assetUnloadOps.Count - 1;
			AsyncOperation assetUnloadOp = _assetUnloadOps[index];
			_assetUnloadOps.RemoveAt(index);
			if (assetUnloadOp != null && !assetUnloadOp.isDone)
			{
				float t = 5f;
				while (!assetUnloadOp.isDone && t > 0f)
				{
					t -= Time.deltaTime;
					yield return null;
				}
				if (!assetUnloadOp.isDone)
				{
					Debug.LogError("Timed out while waiting for asset unload.");
				}
			}
		}
		if (IsUnloadAssetsRequired || IsClearMemoryRequired())
		{
			GameManager.IsCollectingGarbage = true;
			RecordBeginTime(Phases.ClearMemPostActivation);
			yield return TryClearMemory(!hasClearedMemory);
			RecordEndTime(Phases.ClearMemPostActivation);
		}
		else if (IsGarbageCollectRequired)
		{
			GameManager.IsCollectingGarbage = true;
			RecordBeginTime(Phases.GarbageCollect);
			GCManager.Collect();
			RecordEndTime(Phases.GarbageCollect);
		}
		GameManager.IsCollectingGarbage = false;
		if (this.Complete != null)
		{
			try
			{
				this.Complete();
			}
			catch (Exception ex4)
			{
				Debug.LogError("Exception in responders to SceneLoad.Complete. Attempting to continue load regardless.");
				CheatManager.LastErrorText = ex4.ToString();
				Debug.LogException(ex4);
			}
		}
		RecordBeginTime(Phases.StartCall);
		yield return null;
		RecordEndTime(Phases.StartCall);
		if (this.StartCalled != null)
		{
			try
			{
				this.StartCalled();
			}
			catch (Exception ex5)
			{
				Debug.LogError("Exception in responders to SceneLoad.StartCalled. Attempting to continue load regardless.");
				CheatManager.LastErrorText = ex5.ToString();
				Debug.LogException(ex5);
			}
		}
		if (SceneAdditiveLoadConditional.ShouldLoadBoss)
		{
			RecordBeginTime(Phases.LoadBoss);
			yield return runner.StartCoroutine(SceneAdditiveLoadConditional.LoadAll());
			RecordEndTime(Phases.LoadBoss);
			try
			{
				if (this.BossLoaded != null)
				{
					this.BossLoaded();
				}
				if ((bool)GameManager.instance)
				{
					GameManager.instance.LoadedBoss();
				}
			}
			catch (Exception ex6)
			{
				Debug.LogError("Exception in responders to SceneLoad.BossLoaded. Attempting to continue load regardless.");
				CheatManager.LastErrorText = ex6.ToString();
				Debug.LogException(ex6);
			}
		}
		try
		{
			ScenePreloader.Cleanup();
		}
		catch (Exception ex7)
		{
			Debug.LogError("Exception in responders to ScenePreloader.Cleanup. Attempting to continue load regardless.");
			CheatManager.LastErrorText = ex7.ToString();
			Debug.LogException(ex7);
		}
		IsFinished = true;
		if (this.Finish != null)
		{
			try
			{
				this.Finish();
			}
			catch (Exception ex8)
			{
				Debug.LogError("Exception in responders to SceneLoad.Finish. Attempting to continue load regardless.");
				CheatManager.LastErrorText = ex8.ToString();
				Debug.LogException(ex8);
			}
		}
	}

	public static bool IsClearMemoryRequired()
	{
		if (CheatManager.IsForcingUnloads)
		{
			return true;
		}
		if (Platform.Current.GetEstimateAllocatableMemoryMB() > 512)
		{
			return false;
		}
		return true;
	}

	private IEnumerator LocalTryClearMemory(bool revertGlobalPool, bool waitForUnload = true)
	{
		if (WaitForFade)
		{
			float t = 0.51f;
			while (WaitForFade && t > 0f)
			{
				t -= Time.unscaledDeltaTime;
				yield return null;
			}
		}
		yield return TryClearMemory(revertGlobalPool, waitForUnload);
	}

	public static IEnumerator TryClearMemory(bool revertGlobalPool, bool waitForUnload = true)
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		if (revertGlobalPool)
		{
			if (ShouldLog())
			{
				Debug.Log($"Reverting Object Pool State : T {realtimeSinceStartup:0.00}s");
			}
			try
			{
				ObjectPool instance = ObjectPool.instance;
				if ((bool)instance)
				{
					instance.RevertToStartState();
				}
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
			if (ShouldLog())
			{
				Debug.Log($"Finished Reverting Object Pool State : T {Time.realtimeSinceStartup:0.00}s : Time Taken {Time.realtimeSinceStartup - realtimeSinceStartup:0.00}s");
			}
			yield return null;
		}
		realtimeSinceStartup = Time.realtimeSinceStartup;
		if (ShouldLog())
		{
			Debug.Log($"Beginning Asset Unload : T {realtimeSinceStartup:0.00}s");
		}
		AsyncOperation unloadOperation = Resources.UnloadUnusedAssets();
		if (waitForUnload)
		{
			if (ShouldLog())
			{
				Debug.Log("Waiting for asset unload.");
			}
			float timeOut = 5f;
			while (!unloadOperation.isDone && timeOut > 0f)
			{
				timeOut -= Time.unscaledDeltaTime;
				yield return null;
			}
			if (!unloadOperation.isDone && ShouldLog())
			{
				Debug.LogError("Asset unload operation timed out");
			}
			yield return unloadOperation;
		}
		if (!unloadOperation.isDone)
		{
			_assetUnloadOps.Add(unloadOperation);
		}
		if (ShouldLog())
		{
			Debug.Log($"Finished Asset Unload : T {Time.realtimeSinceStartup:0.00}s : Time Taken {Time.realtimeSinceStartup - realtimeSinceStartup:0.00}s");
		}
		realtimeSinceStartup = Time.realtimeSinceStartup;
		if (ShouldLog())
		{
			Debug.Log($"Beginning GC : T {realtimeSinceStartup:0.00}s");
		}
		GCManager.ForceCollect();
		if (ShouldLog())
		{
			Debug.Log($"Finished GC : T {Time.realtimeSinceStartup:0.00}s : Time Taken {Time.realtimeSinceStartup - realtimeSinceStartup:0.00}s");
		}
	}
}

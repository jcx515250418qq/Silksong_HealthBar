using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class ScenePreloader : MonoBehaviour
{
	public class SceneLoadOp
	{
		public AsyncOperationHandle<SceneInstance> Operation { get; private set; }

		public string Address { get; private set; }

		public LoadSceneMode Mode { get; private set; }

		public SceneLoadOp(string address, AsyncOperationHandle<SceneInstance> operation, LoadSceneMode mode)
		{
			Address = address;
			Operation = operation;
			Mode = mode;
		}
	}

	[SerializeField]
	private string sceneNameToLoad = "";

	[SerializeField]
	private PlayerDataTest test;

	[SerializeField]
	private TriggerEnterEvent activateTrigger;

	[SerializeField]
	private TransitionPoint[] entryGateWhiteList;

	[SerializeField]
	private TransitionPoint[] entryGateBlackList;

	private bool startedLoad;

	private static readonly List<SceneLoadOp> _pendingOperations = new List<SceneLoadOp>();

	private static readonly List<SceneLoadOp> _forceEndedOperations = new List<SceneLoadOp>();

	public static bool HasPendingOperations
	{
		get
		{
			if (_pendingOperations != null)
			{
				return _pendingOperations.Count > 0;
			}
			return false;
		}
	}

	private void Start()
	{
		if ((bool)activateTrigger)
		{
			activateTrigger.OnTriggerEntered += delegate
			{
				StartPreload();
			};
		}
		else
		{
			StartPreload();
		}
	}

	private void StartPreload(LoadSceneMode mode = LoadSceneMode.Additive)
	{
		if (startedLoad || !Platform.Current.FetchScenesBeforeFade || !test.IsFulfilled)
		{
			return;
		}
		string entryGateName = GameManager.instance.entryGateName;
		if (!string.IsNullOrEmpty(entryGateName))
		{
			TransitionPoint[] array = entryGateWhiteList;
			for (int i = 0; i < array.Length; i++)
			{
				if (!array[i].gameObject.name.Equals(entryGateName))
				{
					return;
				}
			}
			array = entryGateBlackList;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].gameObject.name.Equals(entryGateName))
				{
					return;
				}
			}
		}
		startedLoad = true;
		StartCoroutine(LoadRoutine(mode));
	}

	private IEnumerator LoadRoutine(LoadSceneMode mode)
	{
		yield return null;
		string text = "Scenes/" + sceneNameToLoad;
		AsyncOperationHandle<SceneInstance> operation = Addressables.LoadSceneAsync(text, mode, activateOnLoad: false);
		_pendingOperations.Add(new SceneLoadOp(text, operation, mode));
	}

	public static AsyncOperationHandle<SceneInstance>? TakeSceneLoadOperation(string sceneName, LoadSceneMode expectedMode)
	{
		if (_pendingOperations == null)
		{
			return null;
		}
		for (int num = _pendingOperations.Count - 1; num >= 0; num--)
		{
			SceneLoadOp sceneLoadOp = _pendingOperations[num];
			if (sceneLoadOp.Address.Equals(sceneName))
			{
				if (sceneLoadOp.Mode != expectedMode)
				{
					Debug.LogErrorFormat("Preloaded scene was not loaded with the expected load method! Expected: {0}, Was: {1}", expectedMode.ToString(), sceneLoadOp.Mode.ToString());
				}
				_pendingOperations.RemoveAt(num);
				return sceneLoadOp.Operation;
			}
		}
		return null;
	}

	public static IEnumerator ForceEndPendingOperations()
	{
		if (_pendingOperations == null)
		{
			yield break;
		}
		foreach (SceneLoadOp op in _pendingOperations)
		{
			if (!op.Operation.IsDone)
			{
				yield return op.Operation;
			}
			_forceEndedOperations.Add(op);
			yield return op.Operation.Result.ActivateAsync();
		}
		_pendingOperations.Clear();
	}

	public static void Cleanup()
	{
		if (_forceEndedOperations == null)
		{
			return;
		}
		foreach (SceneLoadOp forceEndedOperation in _forceEndedOperations)
		{
			SceneManager.UnloadSceneAsync(forceEndedOperation.Address);
		}
		_forceEndedOperations.Clear();
	}

	public static IEnumerable<SceneLoadOp> GetOperations()
	{
		return _pendingOperations;
	}

	public static void SpawnPreloader(string sceneName, LoadSceneMode mode)
	{
		if (string.IsNullOrEmpty(sceneName))
		{
			Debug.LogError("Cannot preload scene with empty name!");
		}
		else if (!SceneAdditiveLoadConditional.IsAnyLoaded)
		{
			ScenePreloader component = new GameObject("Scene Preloader", typeof(ScenePreloader)).GetComponent<ScenePreloader>();
			component.sceneNameToLoad = sceneName;
			component.test = new PlayerDataTest();
			component.entryGateWhiteList = new TransitionPoint[0];
			component.entryGateBlackList = new TransitionPoint[0];
			component.StartPreload(mode);
		}
	}
}

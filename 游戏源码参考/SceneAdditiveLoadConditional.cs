using System;
using System.Collections;
using System.Collections.Generic;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class SceneAdditiveLoadConditional : MonoBehaviour, IApplyExtraLoadSettings
{
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("IsSceneNameValid")]
	private string sceneNameToLoad;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("IsSceneNameValid")]
	private string altSceneNameToLoad;

	[SerializeField]
	private PlayerDataTest tests;

	[SerializeField]
	private QuestTest[] questTests;

	[Space]
	[SerializeField]
	private string[] doorWhiteList;

	[SerializeField]
	private string[] doorBlackList;

	[SerializeField]
	private SceneAdditiveLoadConditional[] otherLoaderBlacklist;

	[Space]
	[SerializeField]
	[PlayerDataField(typeof(bool), false)]
	private string setPdBoolOnLoad;

	[Space]
	[SerializeField]
	private bool repositionScene;

	private bool appliedSettings;

	private bool loadAlt;

	private bool sceneLoaded;

	private AsyncOperationHandle<SceneInstance>? loadOp;

	private static readonly List<SceneAdditiveLoadConditional> _additiveSceneLoads = new List<SceneAdditiveLoadConditional>();

	public static bool LoadInSequence;

	private string SceneNameToLoad
	{
		get
		{
			if (!loadAlt)
			{
				return sceneNameToLoad;
			}
			return altSceneNameToLoad;
		}
	}

	public static bool ShouldLoadBoss
	{
		get
		{
			if (_additiveSceneLoads == null)
			{
				return false;
			}
			return _additiveSceneLoads.Count > 0;
		}
	}

	public static bool IsAnyLoaded
	{
		get
		{
			foreach (SceneAdditiveLoadConditional additiveSceneLoad in _additiveSceneLoads)
			{
				if (additiveSceneLoad.sceneLoaded)
				{
					return true;
				}
			}
			return false;
		}
	}

	public event Action ApplyExtraLoadSettings;

	private bool? IsSceneNameValid(string sceneName)
	{
		return null;
	}

	private void OnEnable()
	{
		if (LoadInSequence && !sceneLoaded && TryTestLoad())
		{
			_additiveSceneLoads.Add(this);
		}
	}

	private void Start()
	{
		if (LoadInSequence || sceneLoaded || !TryTestLoad())
		{
			return;
		}
		_additiveSceneLoads.Add(this);
		ApplySettings();
		for (int i = 0; i < SceneManager.sceneCount; i++)
		{
			if (SceneManager.GetSceneAt(i).name == SceneNameToLoad)
			{
				sceneLoaded = true;
				return;
			}
		}
		StartCoroutine(LoadRoutine(callEvent: true, this));
	}

	private void ApplySettings()
	{
		if (!appliedSettings)
		{
			appliedSettings = true;
			this.ApplyExtraLoadSettings?.Invoke();
		}
	}

	private bool TryTestLoad()
	{
		bool flag = !tests.IsFulfilled;
		QuestTest[] array = questTests;
		foreach (QuestTest questTest in array)
		{
			if (!questTest.IsFulfilled)
			{
				flag = true;
				break;
			}
		}
		string entryGateName = GameManager.instance.entryGateName;
		if (!flag && doorWhiteList.Length != 0)
		{
			bool flag2 = false;
			string[] array2 = doorWhiteList;
			for (int i = 0; i < array2.Length; i++)
			{
				if (array2[i].Equals(entryGateName))
				{
					flag2 = true;
					break;
				}
			}
			if (!flag2)
			{
				flag = true;
			}
		}
		if (!flag && doorBlackList.Length != 0)
		{
			string[] array2 = doorBlackList;
			for (int i = 0; i < array2.Length; i++)
			{
				if (array2[i].Equals(entryGateName))
				{
					flag = true;
					break;
				}
			}
		}
		if (!flag && otherLoaderBlacklist.Length != 0)
		{
			SceneAdditiveLoadConditional[] array3 = otherLoaderBlacklist;
			for (int i = 0; i < array3.Length; i++)
			{
				if (array3[i].TryTestLoad())
				{
					flag = true;
					break;
				}
			}
		}
		if (flag)
		{
			loadAlt = true;
		}
		return !string.IsNullOrEmpty(SceneNameToLoad);
	}

	private void OnDisable()
	{
		Unload();
	}

	private AsyncOperationHandle<SceneInstance>? Unload()
	{
		if (!sceneLoaded)
		{
			return null;
		}
		sceneLoaded = false;
		_additiveSceneLoads.Remove(this);
		if (loadOp.HasValue)
		{
			return Addressables.UnloadSceneAsync(loadOp.Value);
		}
		return null;
	}

	public static void Unload(Scene owningScene, List<AsyncOperationHandle<SceneInstance>> storeOperations)
	{
		for (int num = _additiveSceneLoads.Count - 1; num >= 0; num--)
		{
			SceneAdditiveLoadConditional sceneAdditiveLoadConditional = _additiveSceneLoads[num];
			if (!(sceneAdditiveLoadConditional.gameObject.scene != owningScene))
			{
				AsyncOperationHandle<SceneInstance>? asyncOperationHandle = sceneAdditiveLoadConditional.Unload();
				if (asyncOperationHandle.HasValue)
				{
					storeOperations.Add(asyncOperationHandle.Value);
				}
			}
		}
	}

	public static IEnumerator LoadAll()
	{
		if (_additiveSceneLoads != null)
		{
			foreach (SceneAdditiveLoadConditional additiveSceneLoad in _additiveSceneLoads)
			{
				if ((bool)additiveSceneLoad)
				{
					yield return additiveSceneLoad.StartCoroutine(additiveSceneLoad.LoadRoutine(callEvent: false, additiveSceneLoad));
				}
			}
		}
		LoadInSequence = false;
	}

	private IEnumerator LoadRoutine(bool callEvent, SceneAdditiveLoadConditional sceneLoader)
	{
		ApplySettings();
		_ = LoadInSequence;
		yield return null;
		string text = "Scenes/" + SceneNameToLoad;
		loadOp = ScenePreloader.TakeSceneLoadOperation(text, LoadSceneMode.Additive) ?? Addressables.LoadSceneAsync(text, LoadSceneMode.Additive);
		yield return loadOp;
		if (loadOp.Value.OperationException != null)
		{
			Debug.LogError("Additive scene load for " + SceneNameToLoad + " failed with exception:");
			Debug.LogException(loadOp.Value.OperationException, this);
		}
		else
		{
			sceneLoaded = true;
			if (repositionScene)
			{
				GameObject[] rootGameObjects = SceneManager.GetSceneByName(SceneNameToLoad).GetRootGameObjects();
				Vector3 position = base.transform.position;
				GameObject[] array = rootGameObjects;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].transform.position += position;
				}
			}
		}
		if (callEvent && (bool)GameManager.instance)
		{
			GameManager.instance.LoadedBoss();
		}
		sceneLoader.OnWasLoaded();
	}

	private void OnWasLoaded()
	{
		if (!string.IsNullOrEmpty(setPdBoolOnLoad))
		{
			PlayerData.instance.SetVariable(setPdBoolOnLoad, value: true);
		}
	}
}

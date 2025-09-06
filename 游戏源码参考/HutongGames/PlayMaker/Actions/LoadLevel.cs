using UnityEngine;
using UnityEngine.SceneManagement;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Level)]
	[Tooltip("Loads a Level by Name. NOTE: Before you can load a level, you have to add it to the list of levels defined in File->Build Settings...")]
	public class LoadLevel : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The name of the level to load. NOTE: Must be in the list of levels defined in File->Build Settings... ")]
		public FsmString levelName;

		[Tooltip("Load the level additively, keeping the current scene.")]
		public bool additive;

		[Tooltip("Load the level asynchronously in the background.")]
		public bool async;

		[Tooltip("Event to send when the level has loaded. NOTE: This only makes sense if the FSM is still in the scene!")]
		public FsmEvent loadedEvent;

		[Tooltip("Keep this GameObject in the new level. NOTE: The GameObject and components is disabled then enabled on load; uncheck Reset On Disable to keep the active state.")]
		public FsmBool dontDestroyOnLoad;

		[Tooltip("Event to send if the level cannot be loaded.")]
		public FsmEvent failedEvent;

		private AsyncOperation asyncOperation;

		public override void Reset()
		{
			levelName = "";
			additive = false;
			async = false;
			loadedEvent = null;
			dontDestroyOnLoad = false;
		}

		public override void OnEnter()
		{
			if (!Application.CanStreamedLevelBeLoaded(levelName.Value))
			{
				base.Fsm.Event(failedEvent);
				Finish();
				return;
			}
			if (dontDestroyOnLoad.Value)
			{
				Object.DontDestroyOnLoad(base.Owner.transform.root.gameObject);
			}
			if (additive)
			{
				if (async)
				{
					asyncOperation = SceneManager.LoadSceneAsync(levelName.Value, LoadSceneMode.Additive);
					Debug.Log("LoadLevelAdditiveAsyc: " + levelName.Value);
					return;
				}
				SceneManager.LoadScene(levelName.Value, LoadSceneMode.Additive);
				Debug.Log("LoadLevelAdditive: " + levelName.Value);
			}
			else
			{
				if (async)
				{
					asyncOperation = SceneManager.LoadSceneAsync(levelName.Value, LoadSceneMode.Single);
					Debug.Log("LoadLevelAsync: " + levelName.Value);
					return;
				}
				SceneManager.LoadScene(levelName.Value, LoadSceneMode.Single);
				Debug.Log("LoadLevel: " + levelName.Value);
			}
			Log("LOAD COMPLETE");
			base.Fsm.Event(loadedEvent);
			Finish();
		}

		public override void OnUpdate()
		{
			if (asyncOperation.isDone)
			{
				base.Fsm.Event(loadedEvent);
				Finish();
			}
		}
	}
}

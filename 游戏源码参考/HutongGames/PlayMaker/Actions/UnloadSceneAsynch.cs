using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Scene)]
	[Tooltip("Unload a scene asynchronously by its name or index in Build Settings. Destroys all GameObjects associated with the given scene and removes the scene from the SceneManager.")]
	public class UnloadSceneAsynch : FsmStateAction
	{
		public enum SceneReferenceOptions
		{
			ActiveScene = 0,
			SceneAtBuildIndex = 1,
			SceneAtIndex = 2,
			SceneByName = 3,
			SceneByPath = 4,
			SceneByGameObject = 5
		}

		[Tooltip("The reference options of the Scene")]
		public SceneReferenceOptions sceneReference;

		[Tooltip("The name of the scene to load. The given sceneName can either be the last part of the path, without .unity extension or the full path still without the .unity extension")]
		public FsmString sceneByName;

		[Tooltip("The build index of the scene to unload.")]
		public FsmInt sceneAtBuildIndex;

		[Tooltip("The index of the scene to unload.")]
		public FsmInt sceneAtIndex;

		[Tooltip("The scene Path.")]
		public FsmString sceneByPath;

		[Tooltip("The GameObject unload scene of")]
		public FsmOwnerDefault sceneByGameObject;

		[Tooltip("lets you tweak in which order async operation calls will be performed. Leave to none for default")]
		public FsmInt operationPriority;

		[ActionSection("Result")]
		[Tooltip("The loading's progress.")]
		[UIHint(UIHint.Variable)]
		public FsmFloat progress;

		[Tooltip("True when loading is done")]
		[UIHint(UIHint.Variable)]
		public FsmBool isDone;

		[Tooltip("Event sent when scene loading is done")]
		public FsmEvent doneEvent;

		[Tooltip("Event sent if the scene to load was not found")]
		public FsmEvent sceneNotFoundEvent;

		private AsyncOperation _asyncOperation;

		public override void Reset()
		{
			sceneReference = SceneReferenceOptions.SceneAtBuildIndex;
			sceneByName = null;
			sceneAtBuildIndex = null;
			sceneAtIndex = null;
			sceneByPath = null;
			sceneByGameObject = null;
			operationPriority = new FsmInt
			{
				UseVariable = true
			};
			isDone = null;
			progress = null;
			doneEvent = null;
			sceneNotFoundEvent = null;
		}

		public override void OnEnter()
		{
			isDone.Value = false;
			progress.Value = 0f;
			if (!DoUnLoadAsynch())
			{
				base.Fsm.Event(sceneNotFoundEvent);
				Finish();
			}
		}

		private bool DoUnLoadAsynch()
		{
			try
			{
				switch (sceneReference)
				{
				case SceneReferenceOptions.ActiveScene:
					_asyncOperation = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
					break;
				case SceneReferenceOptions.SceneAtBuildIndex:
					_asyncOperation = SceneManager.UnloadSceneAsync(sceneAtBuildIndex.Value);
					break;
				case SceneReferenceOptions.SceneAtIndex:
					_asyncOperation = SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(sceneAtIndex.Value));
					break;
				case SceneReferenceOptions.SceneByName:
					_asyncOperation = SceneManager.UnloadSceneAsync(sceneByName.Value);
					break;
				case SceneReferenceOptions.SceneByPath:
					_asyncOperation = SceneManager.UnloadSceneAsync(SceneManager.GetSceneByPath(sceneByPath.Value));
					break;
				case SceneReferenceOptions.SceneByGameObject:
				{
					GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(sceneByGameObject);
					if (ownerDefaultTarget == null)
					{
						throw new Exception("Null GameObject");
					}
					_asyncOperation = SceneManager.UnloadSceneAsync(ownerDefaultTarget.scene);
					break;
				}
				}
			}
			catch (Exception ex)
			{
				LogError(ex.Message);
				return false;
			}
			if (!operationPriority.IsNone)
			{
				_asyncOperation.priority = operationPriority.Value;
			}
			return true;
		}

		public override void OnUpdate()
		{
			if (_asyncOperation != null)
			{
				if (_asyncOperation.isDone)
				{
					isDone.Value = true;
					progress.Value = _asyncOperation.progress;
					_asyncOperation = null;
					base.Fsm.Event(doneEvent);
					Finish();
				}
				else
				{
					progress.Value = _asyncOperation.progress;
				}
			}
		}

		public override void OnExit()
		{
			_asyncOperation = null;
		}
	}
}

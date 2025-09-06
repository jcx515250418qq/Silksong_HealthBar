using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Scene)]
	[Tooltip("Loads the scene by its name or index in Build Settings.")]
	public class LoadSceneAsynch : FsmStateAction
	{
		[Tooltip("The reference options of the Scene")]
		public GetSceneActionBase.SceneSimpleReferenceOptions sceneReference;

		[Tooltip("The name of the scene to load. The given sceneName can either be the last part of the path, without .unity extension or the full path still without the .unity extension")]
		public FsmString sceneByName;

		[Tooltip("The index of the scene to load.")]
		public FsmInt sceneAtIndex;

		[Tooltip("Allows you to specify whether or not to load the scene additively. See LoadSceneMode Unity doc for more information about the options.")]
		[ObjectType(typeof(LoadSceneMode))]
		public FsmEnum loadSceneMode;

		[Tooltip("Allow the scene to be activated as soon as it's ready")]
		public FsmBool allowSceneActivation;

		[Tooltip("lets you tweak in which order async operation calls will be performed. Leave to none for default")]
		public FsmInt operationPriority;

		[ActionSection("Result")]
		[Tooltip("Use this hash to activate the Scene if you have set 'AllowSceneActivation' to false, you'll need to use it in the action 'AllowSceneActivation' to effectively load the scene.")]
		[UIHint(UIHint.Variable)]
		public FsmInt aSyncOperationHashCode;

		[Tooltip("The loading's progress.")]
		[UIHint(UIHint.Variable)]
		public FsmFloat progress;

		[Tooltip("True when loading is done")]
		[UIHint(UIHint.Variable)]
		public FsmBool isDone;

		[Tooltip("True when loading is done but still waiting for scene activation")]
		[UIHint(UIHint.Variable)]
		public FsmBool pendingActivation;

		[Tooltip("Event sent when scene loading is done")]
		public FsmEvent doneEvent;

		[Tooltip("Event sent when scene loading is done but scene not yet activated. Use aSyncOperationHashCode value in 'AllowSceneActivation' to proceed")]
		public FsmEvent pendingActivationEvent;

		[Tooltip("Event sent if the scene to load was not found")]
		public FsmEvent sceneNotFoundEvent;

		private AsyncOperation _asyncOperation;

		private int _asynchOperationUid = -1;

		private bool pendingActivationCallBackDone;

		public static Dictionary<int, AsyncOperation> aSyncOperationLUT;

		private static int aSynchUidCounter;

		public override void Reset()
		{
			sceneReference = GetSceneActionBase.SceneSimpleReferenceOptions.SceneAtIndex;
			sceneByName = null;
			sceneAtIndex = null;
			loadSceneMode = null;
			aSyncOperationHashCode = null;
			allowSceneActivation = null;
			operationPriority = new FsmInt
			{
				UseVariable = true
			};
			pendingActivation = null;
			pendingActivationEvent = null;
			isDone = null;
			progress = null;
			doneEvent = null;
			sceneNotFoundEvent = null;
		}

		public override void OnEnter()
		{
			pendingActivationCallBackDone = false;
			pendingActivation.Value = false;
			isDone.Value = false;
			progress.Value = 0f;
			if (!DoLoadAsynch())
			{
				base.Fsm.Event(sceneNotFoundEvent);
				Finish();
			}
		}

		private bool DoLoadAsynch()
		{
			if (sceneReference == GetSceneActionBase.SceneSimpleReferenceOptions.SceneAtIndex)
			{
				if (SceneManager.GetActiveScene().buildIndex == sceneAtIndex.Value)
				{
					return false;
				}
				_asyncOperation = SceneManager.LoadSceneAsync(sceneAtIndex.Value, (LoadSceneMode)(object)loadSceneMode.Value);
			}
			else
			{
				if (SceneManager.GetActiveScene().name == sceneByName.Value)
				{
					return false;
				}
				_asyncOperation = SceneManager.LoadSceneAsync(sceneByName.Value, (LoadSceneMode)(object)loadSceneMode.Value);
			}
			if (_asyncOperation == null)
			{
				return false;
			}
			if (!operationPriority.IsNone)
			{
				_asyncOperation.priority = operationPriority.Value;
			}
			_asyncOperation.allowSceneActivation = allowSceneActivation.Value;
			if (!aSyncOperationHashCode.IsNone)
			{
				if (aSyncOperationLUT == null)
				{
					aSyncOperationLUT = new Dictionary<int, AsyncOperation>();
				}
				_asynchOperationUid = ++aSynchUidCounter;
				aSyncOperationHashCode.Value = _asynchOperationUid;
				aSyncOperationLUT.Add(_asynchOperationUid, _asyncOperation);
			}
			return true;
		}

		public override void OnUpdate()
		{
			if (_asyncOperation == null)
			{
				return;
			}
			if (_asyncOperation.isDone)
			{
				isDone.Value = true;
				progress.Value = _asyncOperation.progress;
				if (aSyncOperationLUT != null && _asynchOperationUid != -1)
				{
					aSyncOperationLUT.Remove(_asynchOperationUid);
				}
				_asyncOperation = null;
				base.Fsm.Event(doneEvent);
				Finish();
				return;
			}
			progress.Value = _asyncOperation.progress;
			if (!_asyncOperation.allowSceneActivation && allowSceneActivation.Value)
			{
				_asyncOperation.allowSceneActivation = true;
			}
			if (_asyncOperation.progress == 0.9f && !_asyncOperation.allowSceneActivation && !pendingActivationCallBackDone)
			{
				pendingActivationCallBackDone = true;
				if (!pendingActivation.IsNone)
				{
					pendingActivation.Value = true;
				}
				base.Fsm.Event(pendingActivationEvent);
			}
		}

		public override void OnExit()
		{
			_asyncOperation = null;
		}
	}
}

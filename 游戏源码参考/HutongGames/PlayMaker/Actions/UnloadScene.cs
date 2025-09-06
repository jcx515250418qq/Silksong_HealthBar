using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HutongGames.PlayMaker.Actions
{
	[Obsolete("Use UnloadSceneAsynch Instead.")]
	[ActionCategory(ActionCategory.Scene)]
	[Tooltip("Unload Scene. Note that assets are currently not unloaded, in order to free up asset memory call Resources.UnloadUnusedAssets.")]
	public class UnloadScene : FsmStateAction
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

		[ActionSection("Result")]
		[Tooltip("True if scene was unloaded")]
		[UIHint(UIHint.Variable)]
		public FsmBool unloaded;

		[Tooltip("Event sent if scene was unloaded ")]
		public FsmEvent unloadedEvent;

		[Tooltip("Event sent scene was not unloaded")]
		[UIHint(UIHint.Variable)]
		public FsmEvent failureEvent;

		public override void Reset()
		{
			sceneReference = SceneReferenceOptions.SceneAtBuildIndex;
			sceneByName = null;
			sceneAtBuildIndex = null;
			sceneAtIndex = null;
			sceneByPath = null;
			sceneByGameObject = null;
			unloaded = null;
			unloadedEvent = null;
			failureEvent = null;
		}

		public override void OnEnter()
		{
			bool flag = false;
			try
			{
				switch (sceneReference)
				{
				case SceneReferenceOptions.ActiveScene:
					flag = SceneManager.UnloadScene(SceneManager.GetActiveScene());
					break;
				case SceneReferenceOptions.SceneAtBuildIndex:
					flag = SceneManager.UnloadScene(sceneAtBuildIndex.Value);
					break;
				case SceneReferenceOptions.SceneAtIndex:
					flag = SceneManager.UnloadScene(SceneManager.GetSceneAt(sceneAtIndex.Value));
					break;
				case SceneReferenceOptions.SceneByName:
					flag = SceneManager.UnloadScene(sceneByName.Value);
					break;
				case SceneReferenceOptions.SceneByPath:
					flag = SceneManager.UnloadScene(SceneManager.GetSceneByPath(sceneByPath.Value));
					break;
				case SceneReferenceOptions.SceneByGameObject:
				{
					GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(sceneByGameObject);
					if (ownerDefaultTarget == null)
					{
						throw new Exception("Null GameObject");
					}
					flag = SceneManager.UnloadScene(ownerDefaultTarget.scene);
					break;
				}
				}
			}
			catch (Exception ex)
			{
				LogError(ex.Message);
			}
			if (!unloaded.IsNone)
			{
				unloaded.Value = flag;
			}
			if (flag)
			{
				base.Fsm.Event(unloadedEvent);
			}
			else
			{
				base.Fsm.Event(failureEvent);
			}
			Finish();
		}

		public override string ErrorCheck()
		{
			return sceneReference switch
			{
				_ => string.Empty, 
			};
		}
	}
}

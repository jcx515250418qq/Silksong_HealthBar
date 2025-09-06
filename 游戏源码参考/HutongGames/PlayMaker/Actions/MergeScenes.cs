using System;
using UnityEngine.SceneManagement;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Scene)]
	[Tooltip("This will merge the source scene into the destinationScene. This function merges the contents of the source scene into the destination scene, and deletes the source scene. All GameObjects at the root of the source scene are moved to the root of the destination scene. NOTE: This function is destructive: The source scene will be destroyed once the merge has been completed.")]
	public class MergeScenes : FsmStateAction
	{
		[ActionSection("Source")]
		[Tooltip("The reference options of the source Scene")]
		public GetSceneActionBase.SceneAllReferenceOptions sourceReference;

		[Tooltip("The source scene Index.")]
		public FsmInt sourceAtIndex;

		[Tooltip("The source scene Name.")]
		public FsmString sourceByName;

		[Tooltip("The source scene Path.")]
		public FsmString sourceByPath;

		[Tooltip("The source scene from GameObject")]
		public FsmOwnerDefault sourceByGameObject;

		[ActionSection("Destination")]
		[Tooltip("The reference options of the destination Scene")]
		public GetSceneActionBase.SceneAllReferenceOptions destinationReference;

		[Tooltip("The destination scene Index.")]
		public FsmInt destinationAtIndex;

		[Tooltip("The destination scene Name.")]
		public FsmString destinationByName;

		[Tooltip("The destination scene Path.")]
		public FsmString destinationByPath;

		[Tooltip("The destination scene from GameObject")]
		public FsmOwnerDefault destinationByGameObject;

		[ActionSection("Result")]
		[Tooltip("True if the merge succeeded")]
		[UIHint(UIHint.Variable)]
		public FsmBool success;

		[Tooltip("Event sent if merge succeeded")]
		public FsmEvent successEvent;

		[Tooltip("Event sent if merge failed")]
		public FsmEvent failureEvent;

		private Scene _sourceScene;

		private bool _sourceFound;

		private Scene _destinationScene;

		private bool _destinationFound;

		public override void Reset()
		{
			sourceReference = GetSceneActionBase.SceneAllReferenceOptions.SceneAtIndex;
			sourceByPath = null;
			sourceByName = null;
			sourceAtIndex = null;
			sourceByGameObject = null;
			destinationReference = GetSceneActionBase.SceneAllReferenceOptions.ActiveScene;
			destinationByPath = null;
			destinationByName = null;
			destinationAtIndex = null;
			destinationByGameObject = null;
			success = null;
			successEvent = null;
			failureEvent = null;
		}

		public override void OnEnter()
		{
			GetSourceScene();
			GetDestinationScene();
			if (_destinationFound && _sourceFound)
			{
				if (_destinationScene.Equals(_sourceScene))
				{
					LogError("Source and Destination scenes can not be the same");
				}
				else
				{
					SceneManager.MergeScenes(_sourceScene, _destinationScene);
				}
				success.Value = true;
				base.Fsm.Event(successEvent);
			}
			else
			{
				success.Value = false;
				base.Fsm.Event(failureEvent);
			}
			Finish();
		}

		private void GetSourceScene()
		{
			try
			{
				switch (sourceReference)
				{
				case GetSceneActionBase.SceneAllReferenceOptions.ActiveScene:
					_sourceScene = SceneManager.GetActiveScene();
					break;
				case GetSceneActionBase.SceneAllReferenceOptions.SceneAtIndex:
					_sourceScene = SceneManager.GetSceneAt(sourceAtIndex.Value);
					break;
				case GetSceneActionBase.SceneAllReferenceOptions.SceneByName:
					_sourceScene = SceneManager.GetSceneByName(sourceByName.Value);
					break;
				case GetSceneActionBase.SceneAllReferenceOptions.SceneByPath:
					_sourceScene = SceneManager.GetSceneByPath(sourceByPath.Value);
					break;
				}
			}
			catch (Exception ex)
			{
				LogError(ex.Message);
			}
			if (_sourceScene == default(Scene))
			{
				_sourceFound = false;
			}
			else
			{
				_sourceFound = true;
			}
		}

		private void GetDestinationScene()
		{
			try
			{
				switch (sourceReference)
				{
				case GetSceneActionBase.SceneAllReferenceOptions.ActiveScene:
					_destinationScene = SceneManager.GetActiveScene();
					break;
				case GetSceneActionBase.SceneAllReferenceOptions.SceneAtIndex:
					_destinationScene = SceneManager.GetSceneAt(destinationAtIndex.Value);
					break;
				case GetSceneActionBase.SceneAllReferenceOptions.SceneByName:
					_destinationScene = SceneManager.GetSceneByName(destinationByName.Value);
					break;
				case GetSceneActionBase.SceneAllReferenceOptions.SceneByPath:
					_destinationScene = SceneManager.GetSceneByPath(destinationByPath.Value);
					break;
				}
			}
			catch (Exception ex)
			{
				LogError(ex.Message);
			}
			if (_destinationScene == default(Scene))
			{
				_destinationFound = false;
			}
			else
			{
				_destinationFound = true;
			}
		}

		public override string ErrorCheck()
		{
			if (sourceReference == GetSceneActionBase.SceneAllReferenceOptions.ActiveScene && destinationReference == GetSceneActionBase.SceneAllReferenceOptions.ActiveScene)
			{
				return "Source and Destination scenes can not be the same";
			}
			return string.Empty;
		}
	}
}

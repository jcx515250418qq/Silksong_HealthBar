using UnityEngine.SceneManagement;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Scene)]
	[Tooltip("Get the number of scenes in Build Settings.")]
	public class GetSceneCountInBuildSettings : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The number of scenes in Build Settings.")]
		public FsmInt sceneCountInBuildSettings;

		public override void Reset()
		{
			sceneCountInBuildSettings = null;
		}

		public override void OnEnter()
		{
			DoGetSceneCountInBuildSettings();
			Finish();
		}

		private void DoGetSceneCountInBuildSettings()
		{
			sceneCountInBuildSettings.Value = SceneManager.sceneCountInBuildSettings;
		}
	}
}

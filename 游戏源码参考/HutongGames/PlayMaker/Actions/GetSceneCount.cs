using UnityEngine.SceneManagement;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Scene)]
	[Tooltip("Get the total number of currently loaded scenes.")]
	public class GetSceneCount : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The number of currently loaded scenes.")]
		public FsmInt sceneCount;

		[Tooltip("Repeat every Frame")]
		public bool everyFrame;

		public override void Reset()
		{
			sceneCount = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoGetSceneCount();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoGetSceneCount();
		}

		private void DoGetSceneCount()
		{
			sceneCount.Value = SceneManager.sceneCount;
		}
	}
}

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Game Manager")]
	[Tooltip("Perform a generic scene transition.")]
	public class BeginSceneTransition : FsmStateAction
	{
		public FsmString sceneName;

		public FsmString entryGateName;

		public FsmFloat entryDelay;

		[ObjectType(typeof(GameManager.SceneLoadVisualizations))]
		public FsmEnum visualization;

		public bool preventCameraFadeOut;

		public override void Reset()
		{
			sceneName = "";
			entryGateName = "left1";
			entryDelay = 0f;
			visualization = new FsmEnum
			{
				Value = GameManager.SceneLoadVisualizations.Default
			};
			preventCameraFadeOut = false;
		}

		public override void OnEnter()
		{
			GameManager unsafeInstance = GameManager.UnsafeInstance;
			if (unsafeInstance == null)
			{
				LogError("Cannot BeginSceneTransition() before the game manager is loaded.");
			}
			else
			{
				unsafeInstance.BeginSceneTransition(new GameManager.SceneLoadInfo
				{
					SceneName = sceneName.Value,
					EntryGateName = entryGateName.Value,
					EntryDelay = entryDelay.Value,
					Visualization = (GameManager.SceneLoadVisualizations)(object)visualization.Value,
					PreventCameraFadeOut = true,
					WaitForSceneTransitionCameraFade = !preventCameraFadeOut
				});
			}
			Finish();
		}
	}
}

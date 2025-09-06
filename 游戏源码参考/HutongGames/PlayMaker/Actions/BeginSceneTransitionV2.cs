namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Game Manager")]
	[Tooltip("Perform a generic scene transition.")]
	public class BeginSceneTransitionV2 : FsmStateAction
	{
		public FsmString SceneName;

		public FsmString EntryGateName;

		public FsmFloat EntryDelay;

		[ObjectType(typeof(GameManager.SceneLoadVisualizations))]
		public FsmEnum Visualization;

		public FsmBool PreventCameraFadeOut;

		public FsmBool TryClearMemory;

		public override void Reset()
		{
			SceneName = "";
			EntryGateName = "left1";
			EntryDelay = 0f;
			Visualization = new FsmEnum
			{
				Value = GameManager.SceneLoadVisualizations.Default
			};
			PreventCameraFadeOut = false;
			TryClearMemory = false;
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
					SceneName = SceneName.Value,
					EntryGateName = EntryGateName.Value,
					EntryDelay = EntryDelay.Value,
					Visualization = (GameManager.SceneLoadVisualizations)(object)Visualization.Value,
					PreventCameraFadeOut = true,
					WaitForSceneTransitionCameraFade = !PreventCameraFadeOut.Value,
					AlwaysUnloadUnusedAssets = TryClearMemory.Value
				});
			}
			Finish();
		}
	}
}

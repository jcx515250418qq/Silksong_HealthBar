namespace HutongGames.PlayMaker.Actions
{
	public class GetSceneDimensions : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmFloat StoreWidth;

		[UIHint(UIHint.Variable)]
		public FsmFloat StoreHeight;

		public override void Reset()
		{
			StoreWidth = null;
			StoreHeight = null;
		}

		public override void OnEnter()
		{
			CameraController cameraController = GameCameras.instance.cameraController;
			StoreWidth.Value = cameraController.sceneWidth;
			StoreHeight.Value = cameraController.sceneHeight;
			Finish();
		}
	}
}

namespace HutongGames.PlayMaker.Actions
{
	public class StartFreeCameraMode : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmGameObject StoreObject;

		public override void Reset()
		{
			StoreObject = null;
		}

		public override void OnEnter()
		{
			CameraTarget cameraTarget = GameCameras.instance.cameraTarget;
			cameraTarget.StartFreeMode(useXOffset: true);
			StoreObject.Value = cameraTarget.gameObject;
			Finish();
		}
	}
}

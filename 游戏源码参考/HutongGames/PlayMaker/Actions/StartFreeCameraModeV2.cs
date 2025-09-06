namespace HutongGames.PlayMaker.Actions
{
	public class StartFreeCameraModeV2 : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmGameObject StoreObject;

		public FsmBool UseXOffset;

		public override void Reset()
		{
			StoreObject = null;
			UseXOffset = null;
		}

		public override void OnEnter()
		{
			CameraTarget cameraTarget = GameCameras.instance.cameraTarget;
			cameraTarget.StartFreeMode(UseXOffset.Value);
			StoreObject.Value = cameraTarget.gameObject;
			Finish();
		}
	}
}

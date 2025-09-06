namespace HutongGames.PlayMaker.Actions
{
	public class EndFreeCameraMode : FsmStateAction
	{
		public override void OnEnter()
		{
			GameCameras instance = GameCameras.instance;
			instance.cameraTarget.EndFreeMode();
			CameraController cameraController = instance.cameraController;
			cameraController.SetMode(CameraController.CameraMode.FOLLOWING);
			CameraLockArea currentLockArea = cameraController.CurrentLockArea;
			if ((bool)currentLockArea)
			{
				cameraController.LockToArea(currentLockArea);
			}
			Finish();
		}
	}
}

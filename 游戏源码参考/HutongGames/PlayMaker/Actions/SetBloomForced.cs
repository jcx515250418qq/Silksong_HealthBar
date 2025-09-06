namespace HutongGames.PlayMaker.Actions
{
	public class SetBloomForced : FsmStateAction
	{
		public FsmBool IsBloomForced;

		public override void Reset()
		{
			IsBloomForced = null;
		}

		public override void OnEnter()
		{
			GameCameras.instance.cameraController.IsBloomForced = IsBloomForced.Value;
			Finish();
		}
	}
}

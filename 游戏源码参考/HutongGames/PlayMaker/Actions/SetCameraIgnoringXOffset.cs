namespace HutongGames.PlayMaker.Actions
{
	public class SetCameraIgnoringXOffset : FsmStateAction
	{
		public FsmBool IgnoreXOffset;

		public override void Reset()
		{
			IgnoreXOffset = null;
		}

		public override void OnEnter()
		{
			GameCameras.instance.cameraTarget.SetIgnoringXOffset(IgnoreXOffset.Value);
			Finish();
		}
	}
}

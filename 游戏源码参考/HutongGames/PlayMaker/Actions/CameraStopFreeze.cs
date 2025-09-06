namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class CameraStopFreeze : FsmStateAction
	{
		public FsmBool freezeTargetAlso;

		public override void Reset()
		{
			freezeTargetAlso = true;
		}

		public override void OnEnter()
		{
			if ((bool)GameManager.instance.cameraCtrl)
			{
				GameManager.instance.cameraCtrl.StopFreeze(freezeTargetAlso.Value);
			}
			Finish();
		}
	}
}

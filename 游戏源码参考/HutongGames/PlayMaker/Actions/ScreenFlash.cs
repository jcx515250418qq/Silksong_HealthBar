namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class ScreenFlash : FsmStateAction
	{
		public FsmColor flashColour;

		public override void OnEnter()
		{
			if ((bool)GameManager.instance && (bool)GameManager.instance.cameraCtrl)
			{
				GameManager.instance.cameraCtrl.ScreenFlash(flashColour.Value);
			}
			Finish();
		}
	}
}

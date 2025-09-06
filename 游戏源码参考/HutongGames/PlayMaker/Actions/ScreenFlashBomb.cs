namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class ScreenFlashBomb : FsmStateAction
	{
		public override void OnEnter()
		{
			if ((bool)GameManager.instance && (bool)GameManager.instance.cameraCtrl)
			{
				GameManager.instance.cameraCtrl.ScreenFlashBomb();
			}
			Finish();
		}
	}
}

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class CameraRepositionToHero : FsmStateAction
	{
		public override void OnEnter()
		{
			if ((bool)GameManager.instance && (bool)GameManager.instance.cameraCtrl)
			{
				GameManager.instance.cameraCtrl.PositionToHero(forceDirect: false);
			}
			Finish();
		}
	}
}

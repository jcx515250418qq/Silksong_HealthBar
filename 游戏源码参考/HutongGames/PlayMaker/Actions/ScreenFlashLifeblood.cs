using GlobalSettings;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class ScreenFlashLifeblood : FsmStateAction
	{
		public override void OnEnter()
		{
			GameManager instance = GameManager.instance;
			if ((bool)instance && (bool)instance.cameraCtrl)
			{
				if (Gameplay.PoisonPouchTool.IsEquipped)
				{
					instance.cameraCtrl.ScreenFlashPoison();
				}
				else
				{
					instance.cameraCtrl.ScreenFlashLifeblood();
				}
			}
			Finish();
		}
	}
}

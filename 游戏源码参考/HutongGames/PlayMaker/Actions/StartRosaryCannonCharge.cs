namespace HutongGames.PlayMaker.Actions
{
	public sealed class StartRosaryCannonCharge : FsmStateAction
	{
		public override void Reset()
		{
		}

		public override void OnEnter()
		{
			HeroController instance = HeroController.instance;
			if (instance != null)
			{
				HeroVibrationController vibrationCtrl = instance.GetVibrationCtrl();
				if (vibrationCtrl != null)
				{
					vibrationCtrl.StartRosaryCannonCharge();
				}
			}
			Finish();
		}
	}
}

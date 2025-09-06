namespace HutongGames.PlayMaker.Actions
{
	public class HeroPlayLookDownAnim : FsmStateAction
	{
		public override void Reset()
		{
		}

		public override void OnEnter()
		{
			HeroController instance = HeroController.instance;
			if (instance != null)
			{
				HeroAnimationController animCtrl = instance.AnimCtrl;
				if ((bool)animCtrl)
				{
					animCtrl.PlayLookDown();
				}
			}
			Finish();
		}
	}
}

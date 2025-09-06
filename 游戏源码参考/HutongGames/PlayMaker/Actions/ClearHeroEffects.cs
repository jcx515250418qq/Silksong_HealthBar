namespace HutongGames.PlayMaker.Actions
{
	public class ClearHeroEffects : FsmStateAction
	{
		private HeroController hc;

		public override void OnEnter()
		{
			hc = HeroController.instance;
			hc.ClearEffects();
			Finish();
		}
	}
}

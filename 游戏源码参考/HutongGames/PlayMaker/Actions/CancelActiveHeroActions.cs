namespace HutongGames.PlayMaker.Actions
{
	public sealed class CancelActiveHeroActions : FsmStateAction
	{
		public override void OnEnter()
		{
			HeroUtility.CancelCancellables();
			Finish();
		}
	}
}

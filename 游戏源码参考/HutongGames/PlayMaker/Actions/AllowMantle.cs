namespace HutongGames.PlayMaker.Actions
{
	public class AllowMantle : FsmStateAction
	{
		public override void OnEnter()
		{
			base.Owner.GetComponent<HeroController>().AllowMantle(allow: true);
			Finish();
		}

		public override void OnExit()
		{
			base.Owner.GetComponent<HeroController>().AllowMantle(allow: false);
		}
	}
}

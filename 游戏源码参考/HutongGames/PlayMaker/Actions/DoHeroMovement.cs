namespace HutongGames.PlayMaker.Actions
{
	public class DoHeroMovement : FsmStateAction
	{
		private HeroController hc;

		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		public override void OnEnter()
		{
			hc = HeroController.instance;
		}

		public override void OnUpdate()
		{
			hc.UpdateMoveInput();
		}

		public override void OnFixedUpdate()
		{
			hc.DoMovement(useInput: true);
		}
	}
}

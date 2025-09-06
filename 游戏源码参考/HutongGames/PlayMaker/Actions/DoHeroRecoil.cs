namespace HutongGames.PlayMaker.Actions
{
	public class DoHeroRecoil : FsmStateAction
	{
		public FsmInt RecoilSteps;

		public FsmFloat RecoilSpeed;

		public FsmBool RecoilRight;

		public override void Reset()
		{
			RecoilSteps = null;
			RecoilSpeed = null;
			RecoilRight = new FsmBool
			{
				UseVariable = true
			};
		}

		public override void OnEnter()
		{
			HeroController instance = HeroController.instance;
			if (RecoilRight.IsNone)
			{
				instance.Recoil(RecoilSteps.Value, RecoilSpeed.Value);
			}
			else
			{
				instance.Recoil(RecoilRight.Value, RecoilSteps.Value, RecoilSpeed.Value);
			}
			Finish();
		}
	}
}

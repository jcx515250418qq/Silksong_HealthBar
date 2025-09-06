namespace HutongGames.PlayMaker.Actions
{
	public sealed class SetFrostVignette : FsmStateAction
	{
		public FsmFloat FrostVignetteTargetValue;

		public override void Reset()
		{
			FrostVignetteTargetValue = null;
		}

		public override void OnEnter()
		{
			HeroController instance = HeroController.instance;
			if (instance != null)
			{
				instance.SetFrostAmount(FrostVignetteTargetValue.Value);
			}
			StatusVignette.SetFrostVignetteAmount(FrostVignetteTargetValue.Value);
			Finish();
		}
	}
}

namespace HutongGames.PlayMaker.Actions
{
	public class HeroBoxControlV2 : HeroBoxControl
	{
		[ObjectType(typeof(HeroBoxState))]
		public FsmEnum setOnExit;

		protected override bool IsEveryFrame => !setOnExit.IsNone;

		public override void Reset()
		{
			base.Reset();
			setOnExit = new FsmEnum
			{
				UseVariable = true
			};
		}

		public override void OnExit()
		{
			if (!setOnExit.IsNone)
			{
				HeroBoxState boxState = (HeroBoxState)(object)setOnExit.Value;
				SetBoxState(boxState);
			}
		}
	}
}

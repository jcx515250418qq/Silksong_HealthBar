namespace HutongGames.PlayMaker.Actions
{
	public class TakeDamage : FsmStateAction
	{
		public FsmGameObject Target;

		public FsmInt AttackType;

		public FsmBool CircleDirection;

		public FsmInt DamageDealt;

		public FsmFloat Direction;

		public FsmBool IgnoreInvulnerable;

		public FsmFloat MagnitudeMultiplier;

		public FsmFloat MoveAngle;

		public FsmBool MoveDirection;

		public FsmFloat Multiplier;

		public FsmInt SpecialType;

		public override void Reset()
		{
			base.Reset();
			Target = new FsmGameObject
			{
				UseVariable = true
			};
			AttackType = new FsmInt
			{
				UseVariable = true
			};
			CircleDirection = new FsmBool
			{
				UseVariable = true
			};
			DamageDealt = new FsmInt
			{
				UseVariable = true
			};
			Direction = new FsmFloat
			{
				UseVariable = true
			};
			IgnoreInvulnerable = new FsmBool
			{
				UseVariable = true
			};
			MagnitudeMultiplier = new FsmFloat
			{
				UseVariable = true
			};
			MoveAngle = new FsmFloat
			{
				UseVariable = true
			};
			MoveDirection = new FsmBool
			{
				UseVariable = true
			};
			Multiplier = new FsmFloat
			{
				UseVariable = true
			};
			SpecialType = new FsmInt
			{
				UseVariable = true
			};
		}

		public override void OnEnter()
		{
			base.OnEnter();
			HitTaker.Hit(Target.Value, new HitInstance
			{
				Source = base.Owner,
				AttackType = (AttackTypes)AttackType.Value,
				CircleDirection = CircleDirection.Value,
				DamageDealt = DamageDealt.Value,
				Direction = Direction.Value,
				IgnoreInvulnerable = IgnoreInvulnerable.Value,
				MagnitudeMultiplier = MagnitudeMultiplier.Value,
				MoveAngle = MoveAngle.Value,
				MoveDirection = MoveDirection.Value,
				Multiplier = (Multiplier.IsNone ? 1f : Multiplier.Value),
				SpecialType = (SpecialTypes)SpecialType.Value
			});
			Finish();
		}
	}
}

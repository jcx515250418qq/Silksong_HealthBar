namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Vector2)]
	public class ConvertVector2ToAngle : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmVector2 vector;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmFloat storeAngle;

		[Tooltip("Repeat every frame")]
		public bool everyFrame;

		public override void Reset()
		{
			vector = null;
			storeAngle = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoCalculate();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoCalculate();
		}

		private void DoCalculate()
		{
			storeAngle.Value = vector.Value.DirectionToAngle();
		}
	}
}

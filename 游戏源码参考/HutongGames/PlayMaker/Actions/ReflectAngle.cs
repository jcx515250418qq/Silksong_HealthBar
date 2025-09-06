namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Reflect the selected angle horizontally or vertically.")]
	public class ReflectAngle : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The angle to reflect. Must be expressed in degrees, ")]
		public FsmFloat angle;

		public bool reflectHorizontally;

		public bool reflectVertically;

		public bool disallowNegative;

		[Tooltip("Float to store the reflected angle in.")]
		[UIHint(UIHint.Variable)]
		public FsmFloat storeResult;

		public override void Reset()
		{
			angle = null;
			reflectHorizontally = false;
			reflectVertically = false;
			disallowNegative = false;
			storeResult = null;
		}

		public override void OnEnter()
		{
			DoReflectAngle();
			Finish();
		}

		private void DoReflectAngle()
		{
			float num = angle.Value;
			if (reflectHorizontally)
			{
				num = 180f - num;
			}
			if (reflectVertically)
			{
				num = 0f - num;
			}
			while (num > 360f)
			{
				num -= 360f;
			}
			for (; num < -360f; num += 360f)
			{
			}
			if (disallowNegative)
			{
				for (; num < 0f; num += 360f)
				{
				}
			}
			storeResult.Value = num;
		}
	}
}

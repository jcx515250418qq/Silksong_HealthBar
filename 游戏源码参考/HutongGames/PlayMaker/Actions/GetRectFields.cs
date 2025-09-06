namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Rect)]
	[Tooltip("Get the individual fields of a Rect Variable and store them in Float Variables.")]
	public class GetRectFields : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Rect Variable.")]
		public FsmRect rectVariable;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the X value in a Float Variable.")]
		public FsmFloat storeX;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the X value in a Float Variable.")]
		public FsmFloat storeY;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the Width in a Float Variable.")]
		public FsmFloat storeWidth;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the Height in a Float Variable.")]
		public FsmFloat storeHeight;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the Min position in a Vector2 Variable.")]
		public FsmVector2 storeMin;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the Max position in a Vector2 Variable.")]
		public FsmVector2 storeMax;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the Size in a Vector2 Variable.")]
		public FsmVector2 storeSize;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the Center in a Vector2 Variable.")]
		public FsmVector2 storeCenter;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			rectVariable = null;
			storeX = null;
			storeY = null;
			storeWidth = null;
			storeHeight = null;
			storeMin = null;
			storeMax = null;
			storeSize = null;
			storeCenter = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoGetRectFields();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoGetRectFields();
		}

		private void DoGetRectFields()
		{
			if (!rectVariable.IsNone)
			{
				storeX.Value = rectVariable.Value.x;
				storeY.Value = rectVariable.Value.y;
				storeWidth.Value = rectVariable.Value.width;
				storeHeight.Value = rectVariable.Value.height;
				storeMin.Value = rectVariable.Value.min;
				storeMax.Value = rectVariable.Value.max;
				storeSize.Value = rectVariable.Value.size;
				storeCenter.Value = rectVariable.Value.center;
			}
		}
	}
}

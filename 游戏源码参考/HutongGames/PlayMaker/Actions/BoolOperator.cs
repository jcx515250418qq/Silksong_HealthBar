namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Performs boolean operations on 2 Bool Variables.")]
	public class BoolOperator : FsmStateAction
	{
		public enum Operation
		{
			AND = 0,
			NAND = 1,
			OR = 2,
			XOR = 3
		}

		[RequiredField]
		[Tooltip("The first Bool variable.")]
		public FsmBool bool1;

		[RequiredField]
		[Tooltip("The second Bool variable.")]
		public FsmBool bool2;

		[Tooltip("Boolean Operation.")]
		public Operation operation;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a Bool Variable.")]
		public FsmBool storeResult;

		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;

		public override void Reset()
		{
			bool1 = false;
			bool2 = false;
			operation = Operation.AND;
			storeResult = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoBoolOperator();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoBoolOperator();
		}

		private void DoBoolOperator()
		{
			bool value = bool1.Value;
			bool value2 = bool2.Value;
			switch (operation)
			{
			case Operation.AND:
				storeResult.Value = value && value2;
				break;
			case Operation.NAND:
				storeResult.Value = !(value && value2);
				break;
			case Operation.OR:
				storeResult.Value = value || value2;
				break;
			case Operation.XOR:
				storeResult.Value = value ^ value2;
				break;
			}
		}
	}
}

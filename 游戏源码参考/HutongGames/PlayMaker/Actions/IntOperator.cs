using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Performs math operation on 2 Integers: Add, Subtract, Multiply, Divide, Min, Max.")]
	public class IntOperator : FsmStateAction
	{
		public enum Operation
		{
			Add = 0,
			Subtract = 1,
			Multiply = 2,
			Divide = 3,
			Min = 4,
			Max = 5
		}

		[RequiredField]
		[Tooltip("The first integer.")]
		public FsmInt integer1;

		[RequiredField]
		[Tooltip("The second integer.")]
		public FsmInt integer2;

		[Tooltip("The operation to perform on the 2 integers.")]
		public Operation operation;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in an Integer Variable.")]
		public FsmInt storeResult;

		[Tooltip("Perform this action every frame. Useful if you're using variables that are changing.")]
		public bool everyFrame;

		public override void Reset()
		{
			integer1 = null;
			integer2 = null;
			operation = Operation.Add;
			storeResult = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoIntOperator();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoIntOperator();
		}

		private void DoIntOperator()
		{
			int value = integer1.Value;
			int value2 = integer2.Value;
			switch (operation)
			{
			case Operation.Add:
				storeResult.Value = value + value2;
				break;
			case Operation.Subtract:
				storeResult.Value = value - value2;
				break;
			case Operation.Multiply:
				storeResult.Value = value * value2;
				break;
			case Operation.Divide:
				storeResult.Value = value / value2;
				break;
			case Operation.Min:
				storeResult.Value = Mathf.Min(value, value2);
				break;
			case Operation.Max:
				storeResult.Value = Mathf.Max(value, value2);
				break;
			}
		}
	}
}

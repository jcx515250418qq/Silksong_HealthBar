using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Vector2)]
	[Tooltip("Performs most possible operations on 2 Vector2: Dot product, Distance, Angle, Add, Subtract, Multiply, Divide, Min, Max")]
	public class Vector2Operator : FsmStateAction
	{
		public enum Vector2Operation
		{
			DotProduct = 0,
			Distance = 1,
			Angle = 2,
			Add = 3,
			Subtract = 4,
			Multiply = 5,
			Divide = 6,
			Min = 7,
			Max = 8
		}

		[RequiredField]
		[Tooltip("The first vector")]
		public FsmVector2 vector1;

		[RequiredField]
		[Tooltip("The second vector")]
		public FsmVector2 vector2;

		[Tooltip("The operation")]
		public Vector2Operation operation = Vector2Operation.Add;

		[UIHint(UIHint.Variable)]
		[Tooltip("The Vector2 result when it applies.")]
		public FsmVector2 storeVector2Result;

		[UIHint(UIHint.Variable)]
		[Tooltip("The float result when it applies")]
		public FsmFloat storeFloatResult;

		[Tooltip("Repeat every frame")]
		public bool everyFrame;

		public override void Reset()
		{
			vector1 = null;
			vector2 = null;
			operation = Vector2Operation.Add;
			storeVector2Result = null;
			storeFloatResult = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoVector2Operator();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoVector2Operator();
		}

		private void DoVector2Operator()
		{
			Vector2 value = vector1.Value;
			Vector2 value2 = vector2.Value;
			switch (operation)
			{
			case Vector2Operation.DotProduct:
				storeFloatResult.Value = Vector2.Dot(value, value2);
				break;
			case Vector2Operation.Distance:
				storeFloatResult.Value = Vector2.Distance(value, value2);
				break;
			case Vector2Operation.Angle:
				storeFloatResult.Value = Vector2.Angle(value, value2);
				break;
			case Vector2Operation.Add:
				storeVector2Result.Value = value + value2;
				break;
			case Vector2Operation.Subtract:
				storeVector2Result.Value = value - value2;
				break;
			case Vector2Operation.Multiply:
			{
				Vector2 zero2 = Vector2.zero;
				zero2.x = value.x * value2.x;
				zero2.y = value.y * value2.y;
				storeVector2Result.Value = zero2;
				break;
			}
			case Vector2Operation.Divide:
			{
				Vector2 zero = Vector2.zero;
				zero.x = value.x / value2.x;
				zero.y = value.y / value2.y;
				storeVector2Result.Value = zero;
				break;
			}
			case Vector2Operation.Min:
				storeVector2Result.Value = Vector2.Min(value, value2);
				break;
			case Vector2Operation.Max:
				storeVector2Result.Value = Vector2.Max(value, value2);
				break;
			}
		}
	}
}

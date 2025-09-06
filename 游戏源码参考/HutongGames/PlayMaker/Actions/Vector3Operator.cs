using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Vector3)]
	[Tooltip("Performs most possible operations on 2 Vector3: Dot product, Cross product, Distance, Angle, Project, Reflect, Add, Subtract, Multiply, Divide, Min, Max")]
	public class Vector3Operator : FsmStateAction
	{
		public enum Vector3Operation
		{
			DotProduct = 0,
			CrossProduct = 1,
			Distance = 2,
			Angle = 3,
			Project = 4,
			Reflect = 5,
			Add = 6,
			Subtract = 7,
			Multiply = 8,
			Divide = 9,
			Min = 10,
			Max = 11
		}

		[RequiredField]
		[Tooltip("The first vector in the operation.")]
		public FsmVector3 vector1;

		[RequiredField]
		[Tooltip("The second vector in the operation.")]
		public FsmVector3 vector2;

		[Tooltip("The operation to perform.")]
		public Vector3Operation operation = Vector3Operation.Add;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a Vector3 Variable.")]
		public FsmVector3 storeVector3Result;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store a float result in a Float Variable (E.g., Dot, Distance, Angle)")]
		public FsmFloat storeFloatResult;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			vector1 = null;
			vector2 = null;
			operation = Vector3Operation.Add;
			storeVector3Result = null;
			storeFloatResult = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoVector3Operator();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoVector3Operator();
		}

		private void DoVector3Operator()
		{
			Vector3 value = vector1.Value;
			Vector3 value2 = vector2.Value;
			switch (operation)
			{
			case Vector3Operation.DotProduct:
				storeFloatResult.Value = Vector3.Dot(value, value2);
				break;
			case Vector3Operation.CrossProduct:
				storeVector3Result.Value = Vector3.Cross(value, value2);
				break;
			case Vector3Operation.Distance:
				storeFloatResult.Value = Vector3.Distance(value, value2);
				break;
			case Vector3Operation.Angle:
				storeFloatResult.Value = Vector3.Angle(value, value2);
				break;
			case Vector3Operation.Project:
				storeVector3Result.Value = Vector3.Project(value, value2);
				break;
			case Vector3Operation.Reflect:
				storeVector3Result.Value = Vector3.Reflect(value, value2);
				break;
			case Vector3Operation.Add:
				storeVector3Result.Value = value + value2;
				break;
			case Vector3Operation.Subtract:
				storeVector3Result.Value = value - value2;
				break;
			case Vector3Operation.Multiply:
			{
				Vector3 zero2 = Vector3.zero;
				zero2.x = value.x * value2.x;
				zero2.y = value.y * value2.y;
				zero2.z = value.z * value2.z;
				storeVector3Result.Value = zero2;
				break;
			}
			case Vector3Operation.Divide:
			{
				Vector3 zero = Vector3.zero;
				zero.x = value.x / value2.x;
				zero.y = value.y / value2.y;
				zero.z = value.z / value2.z;
				storeVector3Result.Value = zero;
				break;
			}
			case Vector3Operation.Min:
				storeVector3Result.Value = Vector3.Min(value, value2);
				break;
			case Vector3Operation.Max:
				storeVector3Result.Value = Vector3.Max(value, value2);
				break;
			}
		}
	}
}

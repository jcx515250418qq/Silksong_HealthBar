using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Vector2)]
	[Tooltip("Sets a Vector2 Variable to a random value.")]
	public class Vector2RandomValue : FsmStateAction
	{
		public enum Option
		{
			Circle = 0,
			Rectangle = 1,
			InArc = 2,
			AtAngles = 3
		}

		private static bool showPreview;

		[PreviewField("DrawPreview")]
		[Tooltip("Controls the distribution of the random Vector2 values.")]
		public Option shape;

		[Tooltip("The minimum length for the random Vector2 value.")]
		public FsmFloat minLength;

		[Tooltip("The maximum length for the random Vector2 value.")]
		public FsmFloat maxLength;

		[Tooltip("Context sensitive parameter. Depends on the Shape.")]
		public FsmFloat floatParam1;

		[Tooltip("Context sensitive parameter. Depends on the Shape.")]
		public FsmFloat floatParam2;

		[Tooltip("Scale the vector in Y (e.g., to squash a circle into an oval)")]
		public FsmFloat yScale;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a Vector2 variable.")]
		public FsmVector2 storeResult;

		private Vector2 v2;

		public override void Reset()
		{
			shape = Option.Circle;
			minLength = 0f;
			maxLength = 1f;
			floatParam1 = null;
			floatParam2 = null;
			yScale = 1f;
			storeResult = null;
		}

		public override void OnEnter()
		{
			DoRandomVector2();
			storeResult.Value = v2;
			Finish();
		}

		private void DoRandomVector2()
		{
			switch (shape)
			{
			case Option.Circle:
				v2 = UnityEngine.Random.insideUnitCircle.normalized * UnityEngine.Random.Range(minLength.Value, maxLength.Value);
				break;
			case Option.Rectangle:
			{
				float value = minLength.Value;
				float value2 = maxLength.Value;
				v2.x = UnityEngine.Random.Range(value, value2);
				if (UnityEngine.Random.Range(0, 100) < 50)
				{
					v2.x = 0f - v2.x;
				}
				v2.y = UnityEngine.Random.Range(value, value2);
				if (UnityEngine.Random.Range(0, 100) < 50)
				{
					v2.y = 0f - v2.y;
				}
				break;
			}
			case Option.InArc:
			{
				float f2 = MathF.PI / 180f * UnityEngine.Random.Range(floatParam1.Value, floatParam2.Value);
				float num3 = UnityEngine.Random.Range(minLength.Value, maxLength.Value);
				v2.x = Mathf.Cos(f2) * num3;
				v2.y = Mathf.Sin(f2) * num3;
				break;
			}
			case Option.AtAngles:
			{
				int num = (int)floatParam1.Value;
				int maxExclusive = 360 / num;
				int num2 = UnityEngine.Random.Range(0, maxExclusive);
				float f = MathF.PI / 180f * (float)num2 * (float)num;
				float num3 = UnityEngine.Random.Range(minLength.Value, maxLength.Value);
				v2.x = Mathf.Cos(f) * num3;
				v2.y = Mathf.Sin(f) * num3;
				break;
			}
			default:
				throw new ArgumentOutOfRangeException();
			}
			v2.y *= yScale.Value;
		}
	}
}

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Math)]
	public class ClampAngleByScale : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		public Space space;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Float variable to clamp.")]
		public FsmFloat angleVariable;

		[RequiredField]
		public FsmFloat positiveMinValue;

		[RequiredField]
		public FsmFloat positiveMaxValue;

		[RequiredField]
		public FsmFloat negativeMinValue;

		[RequiredField]
		public FsmFloat negativeMaxValue;

		[Tooltip("Repeat every frame. Useful if the float variable is changing.")]
		public bool everyFrame;

		private float minValue;

		private float maxValue;

		public override void Reset()
		{
			angleVariable = null;
			positiveMinValue = null;
			positiveMaxValue = null;
			negativeMinValue = null;
			negativeMaxValue = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!(ownerDefaultTarget == null))
			{
				Vector3 obj = ((space == Space.World) ? ownerDefaultTarget.transform.lossyScale : ownerDefaultTarget.transform.localScale);
				if (obj.x > 0f)
				{
					minValue = positiveMinValue.Value;
					maxValue = positiveMaxValue.Value;
				}
				else
				{
					minValue = negativeMinValue.Value;
					maxValue = negativeMaxValue.Value;
				}
				angleVariable.Value = DoClamp(angleVariable.Value, minValue, maxValue);
				if (angleVariable.Value >= 360f)
				{
					angleVariable.Value -= 360f;
				}
				if (angleVariable.Value < 0f)
				{
					angleVariable.Value += 360f;
				}
				if (!everyFrame)
				{
					Finish();
				}
			}
		}

		public override void OnUpdate()
		{
			angleVariable.Value = DoClamp(angleVariable.Value, minValue, maxValue);
			if (angleVariable.Value >= 360f)
			{
				angleVariable.Value -= 360f;
			}
			if (angleVariable.Value < 0f)
			{
				angleVariable.Value += 360f;
			}
		}

		private float DoClamp(float angle, float min, float max)
		{
			if (min < 0f && max > 0f && (angle > max || angle < min))
			{
				angle -= 360f;
				if (angle > max || angle < min)
				{
					if (Mathf.Abs(Mathf.DeltaAngle(angle, min)) < Mathf.Abs(Mathf.DeltaAngle(angle, max)))
					{
						return min;
					}
					return max;
				}
			}
			else if (min > 0f && (angle > max || angle < min))
			{
				angle += 360f;
				if (angle > max || angle < min)
				{
					if (Mathf.Abs(Mathf.DeltaAngle(angle, min)) < Mathf.Abs(Mathf.DeltaAngle(angle, max)))
					{
						return min;
					}
					return max;
				}
			}
			if (angle < min)
			{
				return min;
			}
			if (angle > max)
			{
				return max;
			}
			return angle;
		}
	}
}

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Math)]
	public class ClampAngle : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Float variable to clamp.")]
		public FsmFloat angleVariable;

		[RequiredField]
		[Tooltip("The minimum value.")]
		public FsmFloat minValue;

		[RequiredField]
		[Tooltip("The maximum value.")]
		public FsmFloat maxValue;

		[Tooltip("Repeat every frame. Useful if the float variable is changing.")]
		public bool everyFrame;

		public override void Reset()
		{
			angleVariable = null;
			minValue = null;
			maxValue = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			angleVariable.Value = DoClamp(angleVariable.Value, minValue.Value, maxValue.Value);
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

		public override void OnUpdate()
		{
			angleVariable.Value = DoClamp(angleVariable.Value, minValue.Value, maxValue.Value);
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

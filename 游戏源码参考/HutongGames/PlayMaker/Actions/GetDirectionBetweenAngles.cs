using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Math)]
	public class GetDirectionBetweenAngles : FsmStateAction
	{
		[RequiredField]
		public FsmFloat startAngle;

		[RequiredField]
		public FsmFloat targetAngle;

		[RequiredField]
		public FsmBool storeDirection;

		public FsmFloat storeDistance;

		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;

		public override void Reset()
		{
			everyFrame = false;
		}

		public override void OnEnter()
		{
			GetDirection();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			GetDirection();
		}

		private void GetDirection()
		{
			float value = startAngle.Value;
			float value2 = targetAngle.Value;
			value %= 360f;
			float num = value2 % 360f;
			float num2 = 360f - value;
			if ((num + num2) % 360f < 180f)
			{
				storeDirection.Value = true;
			}
			else
			{
				storeDirection.Value = false;
			}
			if (!storeDistance.IsNone)
			{
				float num3 = Mathf.Abs(startAngle.Value - targetAngle.Value) % 360f;
				if (num3 > 180f)
				{
					num3 = 360f - num3;
				}
				storeDistance.Value = num3;
			}
		}
	}
}

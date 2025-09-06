using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Input)]
	[Tooltip("Sends events based on the direction of Input Axis (Left/Right/Up/Down...).")]
	public class AxisEvent : FsmStateAction
	{
		[Tooltip("Horizontal axis as defined in the Input Manager")]
		public FsmString horizontalAxis;

		[Tooltip("Vertical axis as defined in the Input Manager")]
		public FsmString verticalAxis;

		[Tooltip("Event to send if input is to the left.")]
		public FsmEvent leftEvent;

		[Tooltip("Event to send if input is to the right.")]
		public FsmEvent rightEvent;

		[Tooltip("Event to send if input is to the up.")]
		public FsmEvent upEvent;

		[Tooltip("Event to send if input is to the down.")]
		public FsmEvent downEvent;

		[Tooltip("Event to send if input is in any direction.")]
		public FsmEvent anyDirection;

		[Tooltip("Event to send if no axis input (centered).")]
		public FsmEvent noDirection;

		public override void Reset()
		{
			horizontalAxis = "Horizontal";
			verticalAxis = "Vertical";
			leftEvent = null;
			rightEvent = null;
			upEvent = null;
			downEvent = null;
			anyDirection = null;
			noDirection = null;
		}

		public override void OnUpdate()
		{
			float num = ((horizontalAxis.Value != "") ? Input.GetAxis(horizontalAxis.Value) : 0f);
			float num2 = ((verticalAxis.Value != "") ? Input.GetAxis(verticalAxis.Value) : 0f);
			if ((num * num + num2 * num2).Equals(0f))
			{
				if (noDirection != null)
				{
					base.Fsm.Event(noDirection);
				}
				return;
			}
			float num3 = Mathf.Atan2(num2, num) * 57.29578f + 45f;
			if (num3 < 0f)
			{
				num3 += 360f;
			}
			int num4 = (int)(num3 / 90f);
			if (num4 == 0 && rightEvent != null)
			{
				base.Fsm.Event(rightEvent);
			}
			else if (num4 == 1 && upEvent != null)
			{
				base.Fsm.Event(upEvent);
			}
			else if (num4 == 2 && leftEvent != null)
			{
				base.Fsm.Event(leftEvent);
			}
			else if (num4 == 3 && downEvent != null)
			{
				base.Fsm.Event(downEvent);
			}
			else if (anyDirection != null)
			{
				base.Fsm.Event(anyDirection);
			}
		}
	}
}

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Sends Events based on the comparison of 2 Rect variables.")]
	public class RectCompare : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The first Rect variable.")]
		public FsmRect rect1;

		[RequiredField]
		[Tooltip("The second Rect variable.")]
		public FsmRect rect2;

		[RequiredField]
		[Tooltip("Tolerance for the Equal test (almost equal).")]
		public FsmFloat tolerance;

		[Tooltip("Event sent if Rect 1 equals Rect 2 (within Tolerance)")]
		public FsmEvent equal;

		[Tooltip("Event sent if Rect 1 does not equal Rect 2 (within Tolerance)")]
		public FsmEvent notEqual;

		[Tooltip("Repeat every frame. Useful if the variables are changing and you're waiting for a particular result.")]
		public bool everyFrame;

		public override void Reset()
		{
			rect1 = null;
			rect2 = null;
			tolerance = 0f;
			equal = null;
			notEqual = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoCompare();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoCompare();
		}

		private void DoCompare()
		{
			if (Mathf.Abs(rect1.Value.x - rect2.Value.x) > tolerance.Value || Mathf.Abs(rect1.Value.y - rect2.Value.y) > tolerance.Value || Mathf.Abs(rect1.Value.width - rect2.Value.width) > tolerance.Value || Mathf.Abs(rect1.Value.height - rect2.Value.height) > tolerance.Value)
			{
				base.Fsm.Event(notEqual);
			}
			else
			{
				base.Fsm.Event(equal);
			}
		}

		public override string ErrorCheck()
		{
			if (FsmEvent.IsNullOrEmpty(equal) && FsmEvent.IsNullOrEmpty(notEqual))
			{
				return "Action sends no events!";
			}
			return "";
		}
	}
}

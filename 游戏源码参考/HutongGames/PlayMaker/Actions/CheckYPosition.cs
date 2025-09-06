using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform)]
	public class CheckYPosition : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		public FsmFloat compareTo;

		public FsmFloat compareToOffset;

		[RequiredField]
		[Tooltip("Tolerance for the Equal test (almost equal).\nNOTE: Floats that look the same are often not exactly the same, so you often need to use a small tolerance.")]
		public FsmFloat tolerance;

		[Tooltip("Event sent if Float 1 equals Float 2 (within Tolerance)")]
		public FsmEvent equal;

		[Tooltip("Event sent if Float 1 is less than Float 2")]
		public FsmEvent lessThan;

		[Tooltip("Event sent if Float 1 is greater than Float 2")]
		public FsmEvent greaterThan;

		[Tooltip("Repeat every frame. Useful if the variables are changing and you're waiting for a particular result.")]
		public bool everyFrame;

		public Space space;

		public FsmBool activeBool;

		public override void Reset()
		{
			gameObject = null;
			compareTo = 0f;
			compareToOffset = 0f;
			tolerance = 0f;
			equal = null;
			lessThan = null;
			greaterThan = null;
			everyFrame = false;
			activeBool = new FsmBool
			{
				UseVariable = true
			};
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
			if (!activeBool.IsNone && !activeBool.Value)
			{
				return;
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!(ownerDefaultTarget == null))
			{
				float num = 0f;
				if (space == Space.World)
				{
					num = ownerDefaultTarget.transform.position.y;
				}
				if (space == Space.Self)
				{
					num = ownerDefaultTarget.transform.localPosition.y;
				}
				float num2 = compareTo.Value + compareToOffset.Value;
				if (Mathf.Abs(num - num2) <= tolerance.Value)
				{
					base.Fsm.Event(equal);
				}
				else if (num < num2)
				{
					base.Fsm.Event(lessThan);
				}
				else if (num > num2)
				{
					base.Fsm.Event(greaterThan);
				}
			}
		}

		public override string ErrorCheck()
		{
			if (FsmEvent.IsNullOrEmpty(equal) && FsmEvent.IsNullOrEmpty(lessThan) && FsmEvent.IsNullOrEmpty(greaterThan))
			{
				return "Action sends no events!";
			}
			return "";
		}
	}
}

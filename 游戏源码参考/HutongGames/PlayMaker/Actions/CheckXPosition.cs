using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform)]
	public class CheckXPosition : FsmStateAction
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

		[UIHint(UIHint.Variable)]
		public FsmBool equalBool;

		[Tooltip("Event sent if Float 1 is less than Float 2")]
		public FsmEvent lessThan;

		[UIHint(UIHint.Variable)]
		public FsmBool lessThanBool;

		[Tooltip("Event sent if Float 1 is greater than Float 2")]
		public FsmEvent greaterThan;

		[UIHint(UIHint.Variable)]
		public FsmBool greaterThanBool;

		[Tooltip("Repeat every frame. Useful if the variables are changing and you're waiting for a particular result.")]
		public bool everyFrame;

		public Space space;

		public FsmBool activeBool;

		public override void Reset()
		{
			gameObject = null;
			compareTo = 0f;
			tolerance = 0f;
			equal = null;
			lessThan = null;
			greaterThan = null;
			equalBool = null;
			lessThanBool = null;
			greaterThanBool = null;
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
					num = ownerDefaultTarget.transform.position.x;
				}
				if (space == Space.Self)
				{
					num = ownerDefaultTarget.transform.localPosition.x;
				}
				float num2 = compareTo.Value + compareToOffset.Value;
				if (Mathf.Abs(num - num2) <= tolerance.Value)
				{
					base.Fsm.Event(equal);
					equalBool.Value = true;
					greaterThanBool.Value = false;
					lessThanBool.Value = false;
				}
				else if (num < num2)
				{
					base.Fsm.Event(lessThan);
					equalBool.Value = false;
					greaterThanBool.Value = false;
					lessThanBool.Value = true;
				}
				else if (num > num2)
				{
					base.Fsm.Event(greaterThan);
					equalBool.Value = false;
					greaterThanBool.Value = true;
					lessThanBool.Value = false;
				}
			}
		}
	}
}

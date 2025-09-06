using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine)]
	public class CheckFacingTarget : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault facingObject;

		[UIHint(UIHint.Variable)]
		public FsmGameObject target;

		public bool spriteFacesRight;

		public bool everyFrame;

		public FsmEvent facingEvent;

		public FsmEvent notFacingEvent;

		[UIHint(UIHint.Variable)]
		public FsmBool facingBool;

		[UIHint(UIHint.Variable)]
		public FsmBool notFacingBool;

		private GameObject self;

		private Transform targetTransform;

		public override void Reset()
		{
			facingObject = null;
			target = null;
			facingEvent = null;
			notFacingEvent = null;
			facingBool = null;
			notFacingBool = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			self = base.Fsm.GetOwnerDefaultTarget(facingObject);
			targetTransform = target.Value.transform;
			CheckFacing();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			CheckFacing();
		}

		private void CheckFacing()
		{
			if (targetTransform == null)
			{
				Finish();
				return;
			}
			if (self == null)
			{
				Finish();
				return;
			}
			bool flag = ((self.transform.position.x < targetTransform.position.x) ? true : false);
			bool flag2 = (spriteFacesRight ? (!(self.transform.lossyScale.x < 0f)) : (self.transform.lossyScale.x < 0f));
			if ((flag2 && flag) || (!flag2 && !flag))
			{
				if (facingEvent != null)
				{
					FSMUtility.SendEventToGameObject(self, facingEvent);
				}
				facingBool.Value = true;
			}
			else
			{
				if (notFacingEvent != null)
				{
					FSMUtility.SendEventToGameObject(self, notFacingEvent);
				}
				facingBool.Value = false;
			}
		}
	}
}

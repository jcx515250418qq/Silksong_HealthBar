namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Enemy AI")]
	[Tooltip("Check whether target is left/right/up/down relative to object")]
	public class CheckTargetDirection : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		public FsmGameObject target;

		public FsmEvent aboveEvent;

		public FsmEvent belowEvent;

		public FsmEvent rightEvent;

		public FsmEvent leftEvent;

		[UIHint(UIHint.Variable)]
		public FsmBool aboveBool;

		[UIHint(UIHint.Variable)]
		public FsmBool belowBool;

		[UIHint(UIHint.Variable)]
		public FsmBool rightBool;

		[UIHint(UIHint.Variable)]
		public FsmBool leftBool;

		public FsmFloat selfOffsetX;

		public FsmFloat selfOffsetY;

		public bool reverseIfNegativeScale;

		public bool everyFrame;

		private FsmGameObject self;

		public override void Reset()
		{
			gameObject = null;
			target = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			self = base.Fsm.GetOwnerDefaultTarget(gameObject);
			bool flag = false;
			if (target.Value == null || target == null)
			{
				Finish();
				flag = true;
			}
			DoCheckDirection();
			if (!flag && !everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoCheckDirection();
			if (!everyFrame)
			{
				Finish();
			}
		}

		private void DoCheckDirection()
		{
			if (target.Value == null || target == null)
			{
				return;
			}
			float num = self.Value.transform.position.x + selfOffsetX.Value;
			float num2 = self.Value.transform.position.y + selfOffsetY.Value;
			float x = target.Value.transform.position.x;
			float y = target.Value.transform.position.y;
			if (reverseIfNegativeScale && self.Value.transform.localScale.x < 0f)
			{
				if (num < x)
				{
					base.Fsm.Event(leftEvent);
					leftBool.Value = true;
				}
				else
				{
					leftBool.Value = false;
				}
				if (num >= x)
				{
					base.Fsm.Event(rightEvent);
					rightBool.Value = true;
				}
				else
				{
					rightBool.Value = false;
				}
			}
			else
			{
				if (num <= x)
				{
					base.Fsm.Event(rightEvent);
					rightBool.Value = true;
				}
				else
				{
					rightBool.Value = false;
				}
				if (num > x)
				{
					base.Fsm.Event(leftEvent);
					leftBool.Value = true;
				}
				else
				{
					leftBool.Value = false;
				}
			}
			if (num2 <= y)
			{
				base.Fsm.Event(aboveEvent);
				aboveBool.Value = true;
			}
			else
			{
				aboveBool.Value = false;
			}
			if (num2 > y)
			{
				base.Fsm.Event(belowEvent);
				belowBool.Value = true;
			}
			else
			{
				belowBool.Value = false;
			}
		}
	}
}

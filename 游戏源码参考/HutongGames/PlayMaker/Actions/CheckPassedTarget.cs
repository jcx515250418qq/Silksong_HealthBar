using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Enemy AI")]
	public class CheckPassedTarget : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault Self;

		[RequiredField]
		public FsmGameObject Target;

		public FsmBool IsActive;

		public FsmBool DefaultFacingRight;

		public FsmFloat DistancePast;

		[UIHint(UIHint.Variable)]
		public FsmBool StoreResult;

		public FsmEvent PassedEvent;

		public FsmEvent NotPassedEvent;

		private Transform self;

		private Transform target;

		public bool EveryFrame;

		public override void Reset()
		{
			Self = null;
			Target = null;
			IsActive = true;
			DefaultFacingRight = null;
			DistancePast = null;
			StoreResult = null;
			PassedEvent = null;
			NotPassedEvent = null;
			EveryFrame = false;
		}

		public override void OnEnter()
		{
			self = null;
			target = null;
			if (!UpdateStartingValues())
			{
				Finish();
				return;
			}
			DoAction();
			if (!EveryFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			if (UpdateStartingValues())
			{
				DoAction();
			}
		}

		private bool UpdateStartingValues()
		{
			GameObject safe = Self.GetSafe(this);
			if (!safe || !Target.Value)
			{
				return false;
			}
			self = safe.transform;
			target = Target.Value.transform;
			GetParams(out var _, out var _, out var _);
			return true;
		}

		private void DoAction()
		{
			if (IsActive.Value)
			{
				GetParams(out var facingPositive, out var selfPosition, out var targetPosition);
				float num = targetPosition + DistancePast.Value * (float)(facingPositive ? 1 : (-1));
				bool flag = (facingPositive && selfPosition >= num) || (!facingPositive && selfPosition <= num);
				StoreResult.Value = flag;
				base.Fsm.Event(flag ? PassedEvent : NotPassedEvent);
			}
		}

		private void GetParams(out bool facingPositive, out float selfPosition, out float targetPosition)
		{
			float z = self.eulerAngles.z;
			if (z >= 45f && z <= 135f)
			{
				facingPositive = self.localScale.x > 0f;
				selfPosition = self.position.y;
				targetPosition = target.position.y;
			}
			else if (z >= 135f && z <= 225f)
			{
				facingPositive = self.localScale.x < 0f;
				selfPosition = self.position.x;
				targetPosition = target.position.x;
			}
			else if (z >= 225f && z <= 315f)
			{
				facingPositive = self.localScale.x < 0f;
				selfPosition = self.position.y;
				targetPosition = target.position.y;
			}
			else
			{
				facingPositive = self.localScale.x > 0f;
				selfPosition = self.position.x;
				targetPosition = target.position.x;
			}
			if (!DefaultFacingRight.Value)
			{
				facingPositive = !facingPositive;
			}
		}
	}
}

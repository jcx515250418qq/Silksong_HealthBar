using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class MovedPastTargetEvent : FsmStateAction
	{
		public FsmOwnerDefault Mover;

		public FsmGameObject Target;

		public FsmVector2 DistancePast;

		public FsmFloat MinTime;

		public FsmEvent PassedXEvent;

		public FsmEvent PassedYEvent;

		private Transform mover;

		private Transform target;

		private Vector2 previousPosition;

		public override void Reset()
		{
			Mover = null;
			Target = null;
			DistancePast = null;
			MinTime = null;
			PassedXEvent = null;
			PassedYEvent = null;
		}

		public override void OnEnter()
		{
			if (PassedXEvent == null && PassedYEvent == null)
			{
				Finish();
				return;
			}
			GameObject safe = Mover.GetSafe(this);
			if ((bool)safe)
			{
				mover = safe.transform;
			}
			if ((bool)Target.Value)
			{
				target = Target.Value.transform;
			}
			if (!mover || !target)
			{
				Finish();
				return;
			}
			previousPosition = mover.position;
			DoAction();
		}

		public override void OnUpdate()
		{
			DoAction();
		}

		private void DoAction()
		{
			Vector2 vector = mover.position;
			Vector2 vector2 = vector - previousPosition;
			previousPosition = vector;
			if (!(vector2.magnitude < Mathf.Epsilon) && !(base.State.StateTime < MinTime.Value))
			{
				Vector2 value = DistancePast.Value;
				Vector2 vector3 = target.position;
				vector3.x += ((vector2.x > 0f) ? value.x : (0f - value.x));
				vector3.y += ((vector2.y > 0f) ? value.y : (0f - value.y));
				if (value.x != 0f)
				{
					SendEventIfPassed(vector2.x, vector.x, vector3.x, PassedXEvent);
				}
				if (value.y != 0f)
				{
					SendEventIfPassed(vector2.y, vector.y, vector3.y, PassedYEvent);
				}
			}
		}

		private void SendEventIfPassed(float offset, float pos, float targetPos, FsmEvent fsmEvent)
		{
			if ((offset > 0f && pos >= targetPos) || (offset < 0f && pos <= targetPos))
			{
				base.Fsm.Event(fsmEvent);
			}
		}
	}
}

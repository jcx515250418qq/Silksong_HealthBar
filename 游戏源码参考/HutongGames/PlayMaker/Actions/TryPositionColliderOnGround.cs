using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public sealed class TryPositionColliderOnGround : FsmStateAction
	{
		[CheckForComponent(typeof(Collider2D))]
		public FsmOwnerDefault Target;

		public FsmVector2 GroundPos;

		public FsmFloat MaxCorrection;

		public FsmBool ClampOnFailure;

		public FsmEvent SuccessEvent;

		public FsmEvent FailedEvent;

		public override void Reset()
		{
			Target = null;
			GroundPos = new FsmVector2
			{
				UseVariable = true
			};
			MaxCorrection = null;
			ClampOnFailure = null;
			SuccessEvent = null;
			FailedEvent = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe)
			{
				Collider2D component = safe.GetComponent<Collider2D>();
				if ((bool)component)
				{
					UnityEngine.Bounds bounds = component.bounds;
					Vector2 vector = bounds.center;
					Vector2 vector2 = new Vector2(vector.x, bounds.min.y);
					if (Helper.IsRayHittingNoTriggers((!GroundPos.IsNone) ? GroundPos.Value : vector, Vector2.down, 10f, 256, out var closestHit))
					{
						float num = safe.transform.position.y - vector2.y;
						Vector2 point = closestHit.point;
						point.y += num;
						float num2 = safe.transform.position.y - point.y;
						if (MaxCorrection.Value > 0f && Mathf.Abs(num2) > MaxCorrection.Value)
						{
							base.Fsm.Event(FailedEvent);
							if (!ClampOnFailure.Value)
							{
								Finish();
								return;
							}
							point.y = safe.transform.position.y + Mathf.Clamp(num2, 0f - MaxCorrection.Value, MaxCorrection.Value);
						}
						else
						{
							base.Fsm.Event(SuccessEvent);
						}
						safe.transform.SetPosition2D(point);
					}
				}
			}
			Finish();
		}
	}
}

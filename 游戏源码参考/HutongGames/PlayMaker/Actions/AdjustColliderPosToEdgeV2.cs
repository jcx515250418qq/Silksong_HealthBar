using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class AdjustColliderPosToEdgeV2 : FsmStateAction
	{
		[CheckForComponent(typeof(Rigidbody2D), typeof(Collider2D))]
		public FsmOwnerDefault Target;

		public FsmFloat Width;

		public FsmFloat slideTime = 0.15f;

		private Rigidbody2D body;

		private bool hasCollided;

		private float slideTimeLeft;

		private float edgeAdjustVelocity;

		private CustomPlayMakerCollisionStay2D eventProxy;

		private CustomPlayMakerPhysicsEvent<Collision2D>.EventResponder responder;

		public override void Reset()
		{
			Target = null;
			Width = 1f;
		}

		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		private void RemoveEventProxy()
		{
			if ((bool)eventProxy)
			{
				if (responder != null)
				{
					eventProxy.Remove(responder);
					responder = null;
				}
				else
				{
					eventProxy.Remove(this);
				}
				eventProxy = null;
			}
			else if (responder != null)
			{
				responder.pendingRemoval = true;
				responder = null;
			}
		}

		public override void OnEnter()
		{
			eventProxy = CustomPlayMakerCollisionStay2D.GetEventSender(base.Fsm.Owner.gameObject);
			responder = eventProxy.Add(this, DoCollisionStay2D);
			hasCollided = false;
			slideTimeLeft = 0f;
		}

		public override void OnExit()
		{
			RemoveEventProxy();
		}

		public override void OnFixedUpdate()
		{
			if (!(slideTimeLeft <= 0f))
			{
				slideTimeLeft -= Time.deltaTime;
				MoveBody();
			}
		}

		public override void DoCollisionStay2D(Collision2D collisionInfo)
		{
			if (!hasCollided)
			{
				hasCollided = true;
				UpdateEdgeAdjustX();
			}
		}

		private void UpdateEdgeAdjustX()
		{
			float num = 0f;
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe)
			{
				body = safe.GetComponent<Rigidbody2D>();
				Collider2D component = safe.GetComponent<Collider2D>();
				float value = Width.Value;
				bool facingRight = body.transform.lossyScale.x > 0f;
				if (value != 0f)
				{
					float num2 = (component.bounds.size.x - value) * 0.5f;
					num = EdgeAdjustHelper.GetEdgeAdjustX(component, facingRight, num2, num2);
				}
				else
				{
					num = EdgeAdjustHelper.GetEdgeAdjustX(component, facingRight);
				}
			}
			if (num != 0f)
			{
				edgeAdjustVelocity = num / slideTime.Value;
				slideTimeLeft = slideTime.Value;
				MoveBody();
			}
			else
			{
				edgeAdjustVelocity = 0f;
				slideTimeLeft = 0f;
			}
		}

		private void MoveBody()
		{
			body.MovePosition(body.position + new Vector2(edgeAdjustVelocity * Time.deltaTime, 0f));
		}
	}
}

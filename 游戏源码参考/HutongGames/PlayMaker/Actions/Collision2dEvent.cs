using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Detect collisions between Game Objects that have RigidBody2D/Collider2D components.")]
	public class Collision2dEvent : FsmStateAction
	{
		[Tooltip("The GameObject to detect collisions on.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The type of collision to detect.")]
		public Collision2DType collision;

		[UIHint(UIHint.TagMenu)]
		[Tooltip("Filter by Tag.")]
		public FsmString collideTag;

		[Tooltip("Event to send if a collision is detected.")]
		public FsmEvent sendEvent;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the GameObject that collided with the Owner of this FSM.")]
		public FsmGameObject storeCollider;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the force of the collision. NOTE: Use Get Collision 2D Info to get more info about the collision.")]
		public FsmFloat storeForce;

		private PlayMakerProxyBase cachedProxy;

		private CustomPlayMakerCollisionStay2D eventProxy;

		private CustomPlayMakerPhysicsEvent<Collision2D>.EventResponder responder;

		public override void Reset()
		{
			collision = Collision2DType.OnCollisionEnter2D;
			collideTag = "";
			sendEvent = null;
			storeCollider = null;
			storeForce = null;
		}

		public override void OnPreprocess()
		{
			if (gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
			{
				switch (collision)
				{
				case Collision2DType.OnCollisionEnter2D:
					base.Fsm.HandleCollisionEnter2D = true;
					break;
				case Collision2DType.OnCollisionExit2D:
					base.Fsm.HandleCollisionExit2D = true;
					break;
				case Collision2DType.OnParticleCollision:
					base.Fsm.HandleParticleCollision = true;
					break;
				case Collision2DType.OnCollisionStay2D:
					break;
				}
			}
			else
			{
				GetProxyComponent();
			}
		}

		private void RemoveEventProxy()
		{
			if (eventProxy != null)
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
			if (gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
			{
				RemoveEventProxy();
				eventProxy = CustomPlayMakerCollisionStay2D.GetEventSender(base.Fsm.Owner.gameObject);
				responder = eventProxy.Add(this, DoCollisionStay2D);
				return;
			}
			if (cachedProxy == null)
			{
				GetProxyComponent();
			}
			AddCallback();
			gameObject.GameObject.OnChange += UpdateCallback;
		}

		public override void OnExit()
		{
			RemoveEventProxy();
			if (gameObject.OwnerOption != 0)
			{
				RemoveCallback();
				gameObject.GameObject.OnChange -= UpdateCallback;
				if (responder != null)
				{
					responder.pendingRemoval = true;
					responder = null;
				}
			}
		}

		private void UpdateCallback()
		{
			RemoveCallback();
			GetProxyComponent();
			AddCallback();
		}

		private void GetProxyComponent()
		{
			cachedProxy = null;
			GameObject value = gameObject.GameObject.Value;
			if (!(value == null))
			{
				switch (collision)
				{
				case Collision2DType.OnCollisionEnter2D:
					cachedProxy = PlayMakerFSM.GetEventHandlerComponent<PlayMakerCollisionEnter2D>(value);
					break;
				case Collision2DType.OnCollisionStay2D:
					cachedProxy = PlayMakerFSM.GetEventHandlerComponent<PlayMakerCollisionStay2D>(value);
					break;
				case Collision2DType.OnCollisionExit2D:
					cachedProxy = PlayMakerFSM.GetEventHandlerComponent<PlayMakerCollisionExit2D>(value);
					break;
				case Collision2DType.OnParticleCollision:
					cachedProxy = PlayMakerFSM.GetEventHandlerComponent<PlayMakerParticleCollision>(value);
					break;
				}
			}
		}

		private void AddCallback()
		{
			if (collision == Collision2DType.OnCollisionStay2D)
			{
				RemoveEventProxy();
				eventProxy = CustomPlayMakerCollisionStay2D.GetEventSender(base.Fsm.Owner.gameObject);
				responder = eventProxy.Add(this, DoCollisionStay2D);
			}
			else if (!(cachedProxy == null))
			{
				switch (collision)
				{
				case Collision2DType.OnCollisionEnter2D:
					cachedProxy.AddCollision2DEventCallback(CollisionEnter2D);
					break;
				case Collision2DType.OnCollisionStay2D:
					cachedProxy.AddCollision2DEventCallback(CollisionStay2D);
					break;
				case Collision2DType.OnCollisionExit2D:
					cachedProxy.AddCollision2DEventCallback(CollisionExit2D);
					break;
				case Collision2DType.OnParticleCollision:
					cachedProxy.AddParticleCollisionEventCallback(ParticleCollision);
					break;
				}
			}
		}

		private void RemoveCallback()
		{
			if (collision == Collision2DType.OnCollisionStay2D)
			{
				RemoveEventProxy();
			}
			else if (!(cachedProxy == null))
			{
				switch (collision)
				{
				case Collision2DType.OnCollisionEnter2D:
					cachedProxy.RemoveCollision2DEventCallback(CollisionEnter2D);
					break;
				case Collision2DType.OnCollisionStay2D:
					cachedProxy.RemoveCollision2DEventCallback(CollisionStay2D);
					break;
				case Collision2DType.OnCollisionExit2D:
					cachedProxy.RemoveCollision2DEventCallback(CollisionExit2D);
					break;
				case Collision2DType.OnParticleCollision:
					cachedProxy.RemoveParticleCollisionEventCallback(ParticleCollision);
					break;
				}
			}
		}

		private void StoreCollisionInfo(Collision2D collisionInfo)
		{
			storeCollider.Value = collisionInfo.gameObject;
			storeForce.Value = collisionInfo.relativeVelocity.magnitude;
		}

		public override void DoCollisionEnter2D(Collision2D collisionInfo)
		{
			if (gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
			{
				CollisionEnter2D(collisionInfo);
			}
		}

		public override void DoCollisionStay2D(Collision2D collisionInfo)
		{
			if (gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
			{
				CollisionStay2D(collisionInfo);
			}
		}

		public override void DoCollisionExit2D(Collision2D collisionInfo)
		{
			if (gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
			{
				CollisionExit2D(collisionInfo);
			}
		}

		public override void DoParticleCollision(GameObject other)
		{
			if (gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
			{
				ParticleCollision(other);
			}
		}

		private void CollisionEnter2D(Collision2D collisionInfo)
		{
			if (collision == Collision2DType.OnCollisionEnter2D && FsmStateAction.TagMatches(collideTag, collisionInfo))
			{
				StoreCollisionInfo(collisionInfo);
				base.Fsm.Event(sendEvent);
			}
		}

		private void CollisionStay2D(Collision2D collisionInfo)
		{
			if (collision == Collision2DType.OnCollisionStay2D && FsmStateAction.TagMatches(collideTag, collisionInfo))
			{
				StoreCollisionInfo(collisionInfo);
				base.Fsm.Event(sendEvent);
			}
		}

		private void CollisionExit2D(Collision2D collisionInfo)
		{
			if (collision == Collision2DType.OnCollisionExit2D && FsmStateAction.TagMatches(collideTag, collisionInfo))
			{
				StoreCollisionInfo(collisionInfo);
				base.Fsm.Event(sendEvent);
			}
		}

		private void ParticleCollision(GameObject other)
		{
			if (collision == Collision2DType.OnParticleCollision && FsmStateAction.TagMatches(collideTag, other))
			{
				if (storeCollider != null)
				{
					storeCollider.Value = other;
				}
				storeForce.Value = 0f;
				base.Fsm.Event(sendEvent);
			}
		}

		public override string ErrorCheck()
		{
			return ActionHelpers.CheckPhysics2dSetup(base.Fsm.GetOwnerDefaultTarget(gameObject));
		}
	}
}

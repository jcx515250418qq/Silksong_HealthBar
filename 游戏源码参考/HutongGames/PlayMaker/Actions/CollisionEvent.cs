using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics)]
	[Tooltip("Sends the specified event when the owner collides with a tagged object. Optionally store the collider and collision force in variables for later use. NOTE: Collisions are processed after other actions, so this action should be ordered last in the action list.")]
	public class CollisionEvent : FsmStateAction
	{
		[Tooltip("The GameObject to detect collisions on. Unlike regular MonoBehaviour scripts, PlayMaker lets you easily detect collisions on other objects. This lets you organize your behaviours the way you want.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The type of collision to detect.")]
		public CollisionType collision;

		[UIHint(UIHint.TagMenu)]
		[Tooltip("Tags to collide with.")]
		public FsmString collideTag;

		[Tooltip("Event to send if a collision is detected.")]
		public FsmEvent sendEvent;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the Game Object collided with in a {{Game Object Variable}}.")]
		public FsmGameObject storeCollider;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the force of the collision. NOTE: Use Get Collision Info to get more info about the collision.")]
		public FsmFloat storeForce;

		private PlayMakerProxyBase cachedProxy;

		public override void Reset()
		{
			gameObject = null;
			collision = CollisionType.OnCollisionEnter;
			collideTag = "";
			sendEvent = null;
			storeCollider = null;
			storeForce = null;
		}

		public override void OnPreprocess()
		{
			if (gameObject == null)
			{
				gameObject = new FsmOwnerDefault();
			}
			if (gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
			{
				switch (collision)
				{
				case CollisionType.OnCollisionEnter:
					base.Fsm.HandleCollisionEnter = true;
					break;
				case CollisionType.OnCollisionStay:
					base.Fsm.HandleCollisionStay = true;
					break;
				case CollisionType.OnCollisionExit:
					base.Fsm.HandleCollisionExit = true;
					break;
				case CollisionType.OnControllerColliderHit:
					base.Fsm.HandleControllerColliderHit = true;
					break;
				case CollisionType.OnParticleCollision:
					base.Fsm.HandleParticleCollision = true;
					break;
				}
			}
			else
			{
				GetProxyComponent();
			}
		}

		public override void OnEnter()
		{
			if (gameObject.OwnerOption != 0)
			{
				if (cachedProxy == null)
				{
					GetProxyComponent();
				}
				AddCallback();
				gameObject.GameObject.OnChange += UpdateCallback;
			}
		}

		public override void OnExit()
		{
			if (gameObject.OwnerOption != 0)
			{
				RemoveCallback();
				gameObject.GameObject.OnChange -= UpdateCallback;
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
				case CollisionType.OnCollisionEnter:
					cachedProxy = PlayMakerFSM.GetEventHandlerComponent<PlayMakerCollisionEnter>(value);
					break;
				case CollisionType.OnCollisionStay:
					cachedProxy = PlayMakerFSM.GetEventHandlerComponent<PlayMakerCollisionStay>(value);
					break;
				case CollisionType.OnCollisionExit:
					cachedProxy = PlayMakerFSM.GetEventHandlerComponent<PlayMakerCollisionExit>(value);
					break;
				case CollisionType.OnParticleCollision:
					cachedProxy = PlayMakerFSM.GetEventHandlerComponent<PlayMakerParticleCollision>(value);
					break;
				case CollisionType.OnControllerColliderHit:
					cachedProxy = PlayMakerFSM.GetEventHandlerComponent<PlayMakerControllerColliderHit>(value);
					break;
				}
			}
		}

		private void AddCallback()
		{
			if (!(cachedProxy == null))
			{
				switch (collision)
				{
				case CollisionType.OnCollisionEnter:
					cachedProxy.AddCollisionEventCallback(CollisionEnter);
					break;
				case CollisionType.OnCollisionStay:
					cachedProxy.AddCollisionEventCallback(CollisionStay);
					break;
				case CollisionType.OnCollisionExit:
					cachedProxy.AddCollisionEventCallback(CollisionExit);
					break;
				case CollisionType.OnParticleCollision:
					cachedProxy.AddParticleCollisionEventCallback(ParticleCollision);
					break;
				case CollisionType.OnControllerColliderHit:
					cachedProxy.AddControllerCollisionEventCallback(ControllerColliderHit);
					break;
				}
			}
		}

		private void RemoveCallback()
		{
			if (!(cachedProxy == null))
			{
				switch (collision)
				{
				case CollisionType.OnCollisionEnter:
					cachedProxy.RemoveCollisionEventCallback(CollisionEnter);
					break;
				case CollisionType.OnCollisionStay:
					cachedProxy.RemoveCollisionEventCallback(CollisionStay);
					break;
				case CollisionType.OnCollisionExit:
					cachedProxy.RemoveCollisionEventCallback(CollisionExit);
					break;
				case CollisionType.OnParticleCollision:
					cachedProxy.RemoveParticleCollisionEventCallback(ParticleCollision);
					break;
				case CollisionType.OnControllerColliderHit:
					cachedProxy.RemoveControllerCollisionEventCallback(ControllerColliderHit);
					break;
				}
			}
		}

		private void StoreCollisionInfo(Collision collisionInfo)
		{
			storeCollider.Value = collisionInfo.gameObject;
			storeForce.Value = collisionInfo.relativeVelocity.magnitude;
		}

		public override void DoCollisionEnter(Collision collisionInfo)
		{
			if (gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
			{
				CollisionEnter(collisionInfo);
			}
		}

		public override void DoCollisionStay(Collision collisionInfo)
		{
			if (gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
			{
				CollisionStay(collisionInfo);
			}
		}

		public override void DoCollisionExit(Collision collisionInfo)
		{
			if (gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
			{
				CollisionExit(collisionInfo);
			}
		}

		public override void DoControllerColliderHit(ControllerColliderHit collisionInfo)
		{
			if (gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
			{
				ControllerColliderHit(collisionInfo);
			}
		}

		public override void DoParticleCollision(GameObject other)
		{
			if (gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
			{
				ParticleCollision(other);
			}
		}

		private void CollisionEnter(Collision collisionInfo)
		{
			if (collision == CollisionType.OnCollisionEnter && FsmStateAction.TagMatches(collideTag, collisionInfo))
			{
				StoreCollisionInfo(collisionInfo);
				base.Fsm.Event(sendEvent);
			}
		}

		private void CollisionStay(Collision collisionInfo)
		{
			if (collision == CollisionType.OnCollisionStay && FsmStateAction.TagMatches(collideTag, collisionInfo))
			{
				StoreCollisionInfo(collisionInfo);
				base.Fsm.Event(sendEvent);
			}
		}

		private void CollisionExit(Collision collisionInfo)
		{
			if (collision == CollisionType.OnCollisionExit && FsmStateAction.TagMatches(collideTag, collisionInfo))
			{
				StoreCollisionInfo(collisionInfo);
				base.Fsm.Event(sendEvent);
			}
		}

		private void ControllerColliderHit(ControllerColliderHit collisionInfo)
		{
			if (collision == CollisionType.OnControllerColliderHit && FsmStateAction.TagMatches(collideTag, collisionInfo))
			{
				if (storeCollider != null)
				{
					storeCollider.Value = collisionInfo.gameObject;
				}
				storeForce.Value = 0f;
				base.Fsm.Event(sendEvent);
			}
		}

		private void ParticleCollision(GameObject other)
		{
			if (collision == CollisionType.OnParticleCollision && FsmStateAction.TagMatches(collideTag, other))
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
			return ActionHelpers.CheckPhysicsSetup(base.Fsm.GetOwnerDefaultTarget(gameObject));
		}
	}
}

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Detect 2D trigger collisions between Game Objects that have RigidBody2D/Collider2D components.")]
	public class Trigger2dEvent : FsmStateAction
	{
		[Tooltip("The GameObject to detect collisions on.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The type of trigger event to detect.")]
		public Trigger2DType trigger;

		[UIHint(UIHint.TagMenu)]
		[Tooltip("Filter by Tag.")]
		public FsmString collideTag;

		[Tooltip("Event to send if the trigger event is detected.")]
		public FsmEvent sendEvent;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the GameObject that collided with the Owner of this FSM.")]
		public FsmGameObject storeCollider;

		private PlayMakerProxyBase cachedProxy;

		public override void Reset()
		{
			gameObject = null;
			trigger = Trigger2DType.OnTriggerEnter2D;
			collideTag = "";
			sendEvent = null;
			storeCollider = null;
		}

		public override void OnPreprocess()
		{
			if (gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
			{
				switch (trigger)
				{
				case Trigger2DType.OnTriggerEnter2D:
					base.Fsm.HandleTriggerEnter2D = true;
					break;
				case Trigger2DType.OnTriggerStay2D:
					base.Fsm.HandleTriggerStay2D = true;
					break;
				case Trigger2DType.OnTriggerExit2D:
					base.Fsm.HandleTriggerExit2D = true;
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
				switch (trigger)
				{
				case Trigger2DType.OnTriggerEnter2D:
					cachedProxy = PlayMakerFSM.GetEventHandlerComponent<PlayMakerTriggerEnter2D>(value);
					break;
				case Trigger2DType.OnTriggerStay2D:
					cachedProxy = PlayMakerFSM.GetEventHandlerComponent<PlayMakerTriggerStay2D>(value);
					break;
				case Trigger2DType.OnTriggerExit2D:
					cachedProxy = PlayMakerFSM.GetEventHandlerComponent<PlayMakerTriggerExit2D>(value);
					break;
				}
			}
		}

		private void AddCallback()
		{
			if (!(cachedProxy == null))
			{
				switch (trigger)
				{
				case Trigger2DType.OnTriggerEnter2D:
					cachedProxy.AddTrigger2DEventCallback(TriggerEnter2D);
					break;
				case Trigger2DType.OnTriggerStay2D:
					cachedProxy.AddTrigger2DEventCallback(TriggerStay2D);
					break;
				case Trigger2DType.OnTriggerExit2D:
					cachedProxy.AddTrigger2DEventCallback(TriggerExit2D);
					break;
				}
			}
		}

		private void RemoveCallback()
		{
			if (!(cachedProxy == null))
			{
				switch (trigger)
				{
				case Trigger2DType.OnTriggerEnter2D:
					cachedProxy.RemoveTrigger2DEventCallback(TriggerEnter2D);
					break;
				case Trigger2DType.OnTriggerStay2D:
					cachedProxy.RemoveTrigger2DEventCallback(TriggerStay2D);
					break;
				case Trigger2DType.OnTriggerExit2D:
					cachedProxy.RemoveTrigger2DEventCallback(TriggerExit2D);
					break;
				}
			}
		}

		private void StoreCollisionInfo(Collider2D collisionInfo)
		{
			storeCollider.Value = collisionInfo.gameObject;
		}

		public override void DoTriggerEnter2D(Collider2D other)
		{
			if (gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
			{
				TriggerEnter2D(other);
			}
		}

		public override void DoTriggerStay2D(Collider2D other)
		{
			if (gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
			{
				TriggerStay2D(other);
			}
		}

		public override void DoTriggerExit2D(Collider2D other)
		{
			if (gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
			{
				TriggerExit2D(other);
			}
		}

		private void TriggerEnter2D(Collider2D other)
		{
			if (trigger == Trigger2DType.OnTriggerEnter2D && FsmStateAction.TagMatches(collideTag, other))
			{
				StoreCollisionInfo(other);
				base.Fsm.Event(sendEvent);
			}
		}

		private void TriggerStay2D(Collider2D other)
		{
			if (trigger == Trigger2DType.OnTriggerStay2D && FsmStateAction.TagMatches(collideTag, other))
			{
				StoreCollisionInfo(other);
				base.Fsm.Event(sendEvent);
			}
		}

		private void TriggerExit2D(Collider2D other)
		{
			if (trigger == Trigger2DType.OnTriggerExit2D && FsmStateAction.TagMatches(collideTag, other))
			{
				StoreCollisionInfo(other);
				base.Fsm.Event(sendEvent);
			}
		}

		public override string ErrorCheck()
		{
			return ActionHelpers.CheckPhysics2dSetup(base.Fsm.GetOwnerDefaultTarget(gameObject));
		}
	}
}

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Detect 2D trigger collisions between Game Objects that have RigidBody2D/Collider2D components.")]
	public class Trigger2dEventCheckName : FsmStateAction
	{
		[Tooltip("The GameObject to detect collisions on.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The type of trigger event to detect.")]
		public Trigger2DType trigger;

		[UIHint(UIHint.TagMenu)]
		[Tooltip("Filter by Tag.")]
		public FsmString collideTag;

		[UIHint(UIHint.Layer)]
		[Tooltip("Filter by Layer.")]
		public FsmInt collideLayer;

		[Tooltip("Filter by GameObject name.")]
		public FsmString requiredGameObjectName;

		[Tooltip("Event to send if the trigger event is detected.")]
		public FsmEvent sendEvent;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the GameObject that collided with the Owner of this FSM.")]
		public FsmGameObject storeCollider;

		private PlayMakerProxyBase cachedProxy;

		private CustomPlayMakerTriggerStay2D triggerStayEventProxy;

		private bool addedCallback;

		public override void Reset()
		{
			gameObject = null;
			trigger = Trigger2DType.OnTriggerEnter2D;
			collideTag = "";
			collideLayer = new FsmInt();
			sendEvent = null;
			storeCollider = null;
			requiredGameObjectName = null;
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
				case Trigger2DType.OnTriggerExit2D:
					base.Fsm.HandleTriggerExit2D = true;
					break;
				case Trigger2DType.OnTriggerStay2D:
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
			if (AddCustomProxy())
			{
				AddCallbackTriggerStay();
			}
			else if (gameObject.OwnerOption != 0)
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
			RemoveCallbackTriggerStay();
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
			if (AddCustomProxy())
			{
				return;
			}
			cachedProxy = null;
			GameObject value = gameObject.GameObject.Value;
			if (!(value == null))
			{
				switch (trigger)
				{
				case Trigger2DType.OnTriggerEnter2D:
					cachedProxy = PlayMakerFSM.GetEventHandlerComponent<PlayMakerTriggerEnter2D>(value);
					break;
				case Trigger2DType.OnTriggerExit2D:
					cachedProxy = PlayMakerFSM.GetEventHandlerComponent<PlayMakerTriggerExit2D>(value);
					break;
				case Trigger2DType.OnTriggerStay2D:
					break;
				}
			}
		}

		private bool AddCustomProxy()
		{
			if (trigger == Trigger2DType.OnTriggerStay2D)
			{
				if (triggerStayEventProxy == null)
				{
					GameObject safe = gameObject.GetSafe(this);
					if ((bool)safe)
					{
						triggerStayEventProxy = CustomPlayMakerTriggerStay2D.GetEventSender(safe);
					}
				}
				return true;
			}
			return false;
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

		private void AddCallbackTriggerStay()
		{
			if (trigger == Trigger2DType.OnTriggerStay2D && !(triggerStayEventProxy == null))
			{
				addedCallback = true;
				triggerStayEventProxy.Add(this, TriggerStay2D);
			}
		}

		private void RemoveCallbackTriggerStay()
		{
			if (addedCallback && !(triggerStayEventProxy == null))
			{
				addedCallback = false;
				triggerStayEventProxy.Remove(this);
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
			if (trigger == Trigger2DType.OnTriggerEnter2D && FsmStateAction.TagMatches(collideTag, other) && other.gameObject.name == requiredGameObjectName.Value && (other.gameObject.layer == collideLayer.Value || collideLayer.IsNone))
			{
				StoreCollisionInfo(other);
				base.Fsm.Event(sendEvent);
			}
		}

		private void TriggerStay2D(Collider2D other)
		{
			if (trigger == Trigger2DType.OnTriggerStay2D && FsmStateAction.TagMatches(collideTag, other) && other.gameObject.name == requiredGameObjectName.Value && (other.gameObject.layer == collideLayer.Value || collideLayer.IsNone))
			{
				StoreCollisionInfo(other);
				base.Fsm.Event(sendEvent);
			}
		}

		private void TriggerExit2D(Collider2D other)
		{
			if (trigger == Trigger2DType.OnTriggerExit2D && FsmStateAction.TagMatches(collideTag, other) && other.gameObject.name == requiredGameObjectName.Value && (other.gameObject.layer == collideLayer.Value || collideLayer.IsNone))
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

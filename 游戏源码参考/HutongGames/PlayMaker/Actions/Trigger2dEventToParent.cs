using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Physics 2d")]
	[Tooltip("Detect 2D trigger collisions between the Owner of this FSM and other Game Objects that have RigidBody2D components.\nNOTE: The system events, TRIGGER ENTER 2D, TRIGGER STAY 2D, and TRIGGER EXIT 2D are sent automatically on collisions triggers with any object. Use this action to filter collision triggers by Tag.")]
	public class Trigger2dEventToParent : FsmStateAction
	{
		[Tooltip("The type of trigger to detect.")]
		public PlayMakerUnity2d.Trigger2DType trigger;

		[UIHint(UIHint.Tag)]
		[Tooltip("Filter by Tag.")]
		public FsmString collideTag;

		[UIHint(UIHint.Layer)]
		[Tooltip("Filter by Layer.")]
		public FsmInt collideLayer;

		[RequiredField]
		[Tooltip("Event to send if a collision is detected.")]
		public FsmEvent sendEvent;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the GameObject that collided with the Owner of this FSM.")]
		public FsmGameObject storeCollider;

		private PlayMakerUnity2DProxy _proxy;

		public override void Reset()
		{
			trigger = PlayMakerUnity2d.Trigger2DType.OnTriggerEnter2D;
			collideTag = new FsmString
			{
				UseVariable = true
			};
			collideLayer = new FsmInt
			{
				UseVariable = true
			};
			sendEvent = null;
			storeCollider = null;
		}

		public override void OnEnter()
		{
			_proxy = base.Owner.GetComponent<PlayMakerUnity2DProxy>();
			if (_proxy == null)
			{
				_proxy = base.Owner.AddComponent<PlayMakerUnity2DProxy>();
			}
			switch (trigger)
			{
			case PlayMakerUnity2d.Trigger2DType.OnTriggerEnter2D:
				_proxy.AddOnTriggerEnter2dDelegate(DoTriggerEnter2D);
				break;
			case PlayMakerUnity2d.Trigger2DType.OnTriggerStay2D:
				_proxy.AddOnTriggerStay2dDelegate(DoTriggerStay2D);
				break;
			case PlayMakerUnity2d.Trigger2DType.OnTriggerExit2D:
				_proxy.AddOnTriggerExit2dDelegate(DoTriggerExit2D);
				break;
			}
		}

		public override void OnExit()
		{
			if (!(_proxy == null))
			{
				switch (trigger)
				{
				case PlayMakerUnity2d.Trigger2DType.OnTriggerEnter2D:
					_proxy.RemoveOnTriggerEnter2dDelegate(DoTriggerEnter2D);
					break;
				case PlayMakerUnity2d.Trigger2DType.OnTriggerStay2D:
					_proxy.RemoveOnTriggerStay2dDelegate(DoTriggerStay2D);
					break;
				case PlayMakerUnity2d.Trigger2DType.OnTriggerExit2D:
					_proxy.RemoveOnTriggerExit2dDelegate(DoTriggerExit2D);
					break;
				}
			}
		}

		private void StoreCollisionInfo(Collider2D collisionInfo)
		{
			storeCollider.Value = collisionInfo.gameObject;
		}

		public override void DoTriggerEnter2D(Collider2D collisionInfo)
		{
			if (trigger == PlayMakerUnity2d.Trigger2DType.OnTriggerEnter2D && (collisionInfo.gameObject.tag == collideTag.Value || collideTag.IsNone || string.IsNullOrEmpty(collideTag.Value)) && (collisionInfo.gameObject.layer == collideLayer.Value || collideLayer.IsNone))
			{
				StoreCollisionInfo(collisionInfo);
				base.Fsm.Event(sendEvent);
			}
		}

		public override void DoTriggerStay2D(Collider2D collisionInfo)
		{
			if (trigger == PlayMakerUnity2d.Trigger2DType.OnTriggerStay2D && (collisionInfo.gameObject.tag == collideTag.Value || collideTag.IsNone || string.IsNullOrEmpty(collideTag.Value)) && (collisionInfo.gameObject.layer == collideLayer.Value || collideLayer.IsNone))
			{
				StoreCollisionInfo(collisionInfo);
				base.Fsm.Event(sendEvent);
			}
		}

		public override void DoTriggerExit2D(Collider2D collisionInfo)
		{
			if (trigger == PlayMakerUnity2d.Trigger2DType.OnTriggerExit2D && (collisionInfo.gameObject.tag == collideTag.Value || collideTag.IsNone || string.IsNullOrEmpty(collideTag.Value)) && (collisionInfo.gameObject.layer == collideLayer.Value || collideLayer.IsNone))
			{
				StoreCollisionInfo(collisionInfo);
				base.Fsm.Event(sendEvent);
			}
		}

		public override string ErrorCheck()
		{
			string text = string.Empty;
			if (base.Owner != null && base.Owner.GetComponent<Collider2D>() == null && base.Owner.GetComponent<Rigidbody2D>() == null)
			{
				text += "Owner requires a RigidBody2D or Collider2D!\n";
			}
			return text;
		}
	}
}

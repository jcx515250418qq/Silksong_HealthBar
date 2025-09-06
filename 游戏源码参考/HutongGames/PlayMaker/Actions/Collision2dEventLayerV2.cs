using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Physics 2d")]
	[Tooltip("Detect 2D collisions between the Owner of this FSM and other Game Objects that have RigidBody2D components.\nNOTE: The system events, COLLISION ENTER 2D, COLLISION STAY 2D, and COLLISION EXIT 2D are sent automatically on collisions with any object. Use this action to filter collisions by Tag.")]
	public class Collision2dEventLayerV2 : FsmStateAction
	{
		[Tooltip("The type of collision to detect.")]
		public PlayMakerUnity2d.Collision2DType collision;

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

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the force of the collision. NOTE: Use Get Collision Info to get more info about the collision.")]
		public FsmFloat storeForce;

		[UIHint(UIHint.Variable)]
		public FsmVector3 contactPoint;

		[UIHint(UIHint.Variable)]
		public FsmVector3 contactNormal;

		public bool ignoreFloor;

		public bool ignoreRoof;

		public bool onlyAbove;

		public bool onlyLeft;

		public bool onlyRight;

		private PlayMakerUnity2DProxy _proxy;

		public override void Reset()
		{
			collision = PlayMakerUnity2d.Collision2DType.OnCollisionEnter2D;
			collideTag = new FsmString
			{
				UseVariable = true
			};
			sendEvent = null;
			storeCollider = null;
			storeForce = null;
			ignoreFloor = false;
			ignoreRoof = false;
			onlyAbove = false;
			onlyLeft = false;
			onlyRight = false;
		}

		public override void OnEnter()
		{
			_proxy = base.Owner.GetComponent<PlayMakerUnity2DProxy>();
			if (_proxy == null)
			{
				_proxy = base.Owner.AddComponent<PlayMakerUnity2DProxy>();
			}
			switch (collision)
			{
			case PlayMakerUnity2d.Collision2DType.OnCollisionEnter2D:
				_proxy.AddOnCollisionEnter2dDelegate(DoCollisionEnter2D);
				break;
			case PlayMakerUnity2d.Collision2DType.OnCollisionStay2D:
				_proxy.AddOnCollisionStay2dDelegate(DoCollisionStay2D);
				break;
			case PlayMakerUnity2d.Collision2DType.OnCollisionExit2D:
				_proxy.AddOnCollisionExit2dDelegate(DoCollisionExit2D);
				break;
			}
		}

		public override void OnExit()
		{
			if (!(_proxy == null))
			{
				switch (collision)
				{
				case PlayMakerUnity2d.Collision2DType.OnCollisionEnter2D:
					_proxy.RemoveOnCollisionEnter2dDelegate(DoCollisionEnter2D);
					break;
				case PlayMakerUnity2d.Collision2DType.OnCollisionStay2D:
					_proxy.RemoveOnCollisionStay2dDelegate(DoCollisionStay2D);
					break;
				case PlayMakerUnity2d.Collision2DType.OnCollisionExit2D:
					_proxy.RemoveOnCollisionExit2dDelegate(DoCollisionExit2D);
					break;
				}
			}
		}

		private void StoreCollisionInfo(Collision2D collisionInfo)
		{
			storeCollider.Value = collisionInfo.gameObject;
			storeForce.Value = collisionInfo.relativeVelocity.magnitude;
			if (collisionInfo.contacts != null && collisionInfo.contacts.Length != 0)
			{
				contactPoint.Value = collisionInfo.contacts[0].point;
				contactNormal.Value = collisionInfo.contacts[0].normal;
			}
		}

		public override void DoCollisionEnter2D(Collision2D collisionInfo)
		{
			if (collision == PlayMakerUnity2d.Collision2DType.OnCollisionEnter2D && (collisionInfo.collider.gameObject.tag == collideTag.Value || collideTag.IsNone || string.IsNullOrEmpty(collideTag.Value)) && (collisionInfo.gameObject.layer == collideLayer.Value || collideLayer.IsNone))
			{
				StoreCollisionInfo(collisionInfo);
				float x = collisionInfo.contacts[0].normal.x;
				float y = collisionInfo.contacts[0].normal.y;
				if (CheckConditions(x, y))
				{
					base.Fsm.Event(sendEvent);
				}
			}
		}

		public override void DoCollisionStay2D(Collision2D collisionInfo)
		{
			if (collision == PlayMakerUnity2d.Collision2DType.OnCollisionStay2D && (collisionInfo.collider.gameObject.tag == collideTag.Value || collideTag.IsNone || string.IsNullOrEmpty(collideTag.Value)) && (collisionInfo.gameObject.layer == collideLayer.Value || collideLayer.IsNone))
			{
				StoreCollisionInfo(collisionInfo);
				float x = collisionInfo.contacts[0].normal.x;
				float y = collisionInfo.contacts[0].normal.y;
				if (CheckConditions(x, y))
				{
					base.Fsm.Event(sendEvent);
				}
			}
		}

		public override void DoCollisionExit2D(Collision2D collisionInfo)
		{
			if (collision == PlayMakerUnity2d.Collision2DType.OnCollisionExit2D && (collisionInfo.collider.gameObject.tag == collideTag.Value || collideTag.IsNone || string.IsNullOrEmpty(collideTag.Value)) && (collisionInfo.gameObject.layer == collideLayer.Value || collideLayer.IsNone))
			{
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

		private bool CheckConditions(float normalX, float normalY)
		{
			if ((!ignoreFloor || !Mathf.Approximately(normalY, 1f)) && (!ignoreRoof || !Mathf.Approximately(normalY, -1f)) && (!onlyAbove || Mathf.Approximately(normalY, -1f)) && (!onlyLeft || Mathf.Approximately(normalX, 1f)) && (!onlyRight || Mathf.Approximately(normalX, -1f)))
			{
				return true;
			}
			return false;
		}
	}
}

using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics)]
	[Tooltip("Detect additional collisions between objects with additional raycasting.")]
	public class CheckCollisionSide : FsmStateAction
	{
		public enum CollisionSide
		{
			top = 0,
			left = 1,
			right = 2,
			bottom = 3,
			other = 4
		}

		private const int TERRAIN_LAYER_MASK = 33554688;

		public FsmOwnerDefault collidingObject;

		[UIHint(UIHint.Variable)]
		public FsmBool topHit;

		[UIHint(UIHint.Variable)]
		public FsmBool rightHit;

		[UIHint(UIHint.Variable)]
		public FsmBool bottomHit;

		[UIHint(UIHint.Variable)]
		public FsmBool leftHit;

		public FsmEvent topHitEvent;

		public FsmEvent rightHitEvent;

		public FsmEvent bottomHitEvent;

		public FsmEvent leftHitEvent;

		public bool otherLayer;

		public int otherLayerNumber;

		public FsmBool ignoreTriggers;

		private Collider2D col2d;

		private Rigidbody2D body;

		private int terrainLayerMask;

		public const float RAYCAST_LENGTH = 0.08f;

		public const float SMALL_VALUE = 0.001f;

		private List<Vector2> topRays;

		private List<Vector2> rightRays;

		private List<Vector2> bottomRays;

		private List<Vector2> leftRays;

		private bool checkUp;

		private bool checkDown;

		private bool checkLeft;

		private bool checkRight;

		private CustomPlayMakerCollisionStay2D eventProxy;

		private CustomPlayMakerPhysicsEvent<Collision2D>.EventResponder responder;

		private bool hasBody;

		private static ContactPoint2D[] contactPoint2Ds = new ContactPoint2D[1];

		private int lastUpdateCycle;

		private int enterCount;

		public override void Reset()
		{
			checkUp = false;
			checkDown = false;
			checkLeft = false;
			checkRight = false;
		}

		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
			base.Fsm.HandleCollisionExit2D = true;
		}

		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
			base.Fsm.HandleCollisionExit2D = true;
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
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(collidingObject);
			if (ownerDefaultTarget == null)
			{
				hasBody = false;
				col2d = null;
				body = null;
				Finish();
				return;
			}
			terrainLayerMask = ((ignoreTriggers.Value && ownerDefaultTarget.layer == 9) ? 33562880 : 33554688);
			col2d = ownerDefaultTarget.GetComponent<Collider2D>();
			body = ownerDefaultTarget.GetComponent<Rigidbody2D>();
			hasBody = body != null;
			topRays = new List<Vector2>(3);
			rightRays = new List<Vector2>(3);
			bottomRays = new List<Vector2>(3);
			leftRays = new List<Vector2>(3);
			if (!topHit.IsNone || topHitEvent != null)
			{
				checkUp = true;
			}
			else
			{
				checkUp = false;
			}
			if (!rightHit.IsNone || rightHitEvent != null)
			{
				checkRight = true;
			}
			else
			{
				checkRight = false;
			}
			if (!bottomHit.IsNone || bottomHitEvent != null)
			{
				checkDown = true;
			}
			else
			{
				checkDown = false;
			}
			if (!leftHit.IsNone || leftHitEvent != null)
			{
				checkLeft = true;
			}
			else
			{
				checkLeft = false;
			}
			if (topHit.Value || bottomHit.Value || rightHit.Value || leftHit.Value)
			{
				if (!otherLayer)
				{
					CheckTouching(terrainLayerMask);
				}
				else
				{
					CheckTouching(1 << otherLayerNumber);
				}
			}
		}

		public override void OnExit()
		{
			RemoveEventProxy();
		}

		public override void OnFixedUpdate()
		{
			if (topHit.Value || bottomHit.Value || rightHit.Value || leftHit.Value)
			{
				if (!otherLayer)
				{
					CheckTouching(terrainLayerMask);
				}
				else
				{
					CheckTouching(1 << otherLayerNumber);
				}
			}
		}

		public override void DoCollisionStay2D(Collision2D collision)
		{
			if (base.Fsm.Finished)
			{
				return;
			}
			if (!otherLayer)
			{
				if (((1 << collision.gameObject.layer) & terrainLayerMask) != 0)
				{
					CheckTouching(terrainLayerMask);
				}
			}
			else
			{
				CheckTouching(1 << otherLayerNumber);
			}
		}

		public override void DoCollisionExit2D(Collision2D collision)
		{
			topHit.Value = false;
			rightHit.Value = false;
			bottomHit.Value = false;
			leftHit.Value = false;
		}

		private void CheckTouching(int layerMask)
		{
			if (lastUpdateCycle == CustomPlayerLoop.FixedUpdateCycle)
			{
				return;
			}
			lastUpdateCycle = CustomPlayerLoop.FixedUpdateCycle;
			UnityEngine.Bounds bounds = col2d.bounds;
			Vector3 max = bounds.max;
			Vector3 min = bounds.min;
			Vector3 center = bounds.center;
			if (checkUp && (!hasBody || body.linearVelocity.y >= -0.001f))
			{
				topRays.Clear();
				topRays.Add(new Vector2(min.x, max.y));
				topRays.Add(new Vector2(center.x, max.y));
				topRays.Add(max);
				topHit.Value = false;
				for (int i = 0; i < 3; i++)
				{
					Collider2D collider = Helper.Raycast2D(topRays[i], Vector2.up, 0.08f, layerMask).collider;
					if (!(collider == null) && (!ignoreTriggers.Value || !collider.isTrigger))
					{
						topHit.Value = true;
						base.Fsm.Event(topHitEvent);
						break;
					}
				}
			}
			if (checkRight && (!hasBody || body.linearVelocity.x >= -0.001f))
			{
				rightRays.Clear();
				rightRays.Add(max);
				rightRays.Add(new Vector2(max.x, center.y));
				rightRays.Add(new Vector2(max.x, min.y));
				rightHit.Value = false;
				for (int j = 0; j < 3; j++)
				{
					Collider2D collider2 = Helper.Raycast2D(rightRays[j], Vector2.right, 0.08f, layerMask).collider;
					if (collider2 != null && (!ignoreTriggers.Value || !collider2.isTrigger))
					{
						rightHit.Value = true;
						base.Fsm.Event(rightHitEvent);
						break;
					}
				}
			}
			if (checkDown && (!hasBody || body.linearVelocity.y <= 0.001f))
			{
				bottomRays.Clear();
				bottomRays.Add(new Vector2(max.x, min.y));
				bottomRays.Add(new Vector2(center.x, min.y));
				bottomRays.Add(min);
				bottomHit.Value = false;
				for (int k = 0; k < 3; k++)
				{
					Collider2D collider3 = Helper.Raycast2D(bottomRays[k], -Vector2.up, 0.08f, layerMask).collider;
					if (collider3 != null && (!ignoreTriggers.Value || !collider3.isTrigger))
					{
						bottomHit.Value = true;
						base.Fsm.Event(bottomHitEvent);
						break;
					}
				}
			}
			if (!checkLeft || (hasBody && !(body.linearVelocity.x <= 0.001f)))
			{
				return;
			}
			leftRays.Clear();
			leftRays.Add(min);
			leftRays.Add(new Vector2(min.x, center.y));
			leftRays.Add(new Vector2(min.x, max.y));
			leftHit.Value = false;
			for (int l = 0; l < 3; l++)
			{
				Collider2D collider4 = Helper.Raycast2D(leftRays[l], -Vector2.right, 0.08f, layerMask).collider;
				if (collider4 != null && (!ignoreTriggers.Value || !collider4.isTrigger))
				{
					leftHit.Value = true;
					base.Fsm.Event(leftHitEvent);
					break;
				}
			}
		}
	}
}

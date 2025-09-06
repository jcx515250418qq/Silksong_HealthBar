using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics)]
	[Tooltip("Detect additional collisions between the Owner of this FSM and other object with additional raycasting.")]
	public class CheckCollisionSideEnter : FsmStateAction
	{
		public enum CollisionSide
		{
			top = 0,
			left = 1,
			right = 2,
			bottom = 3,
			other = 4
		}

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

		private PlayMakerUnity2DProxy _proxy;

		private Collider2D col2d;

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

		private int lastUpdateCycle;

		private int enterCount;

		private Rigidbody2D body;

		private bool hasBody;

		public override void Reset()
		{
			topHit = null;
			rightHit = null;
			bottomHit = null;
			leftHit = null;
			topHitEvent = null;
			rightHitEvent = null;
			bottomHitEvent = null;
			leftHitEvent = null;
		}

		public override void OnPreprocess()
		{
			base.Fsm.HandleCollisionEnter2D = true;
		}

		public override void OnEnter()
		{
			col2d = base.Fsm.GameObject.GetComponent<Collider2D>();
			body = base.Fsm.GameObject.GetComponent<Rigidbody2D>();
			hasBody = body != null;
			_proxy = base.Owner.GetComponent<PlayMakerUnity2DProxy>();
			topRays = new List<Vector2>(3);
			rightRays = new List<Vector2>(3);
			bottomRays = new List<Vector2>(3);
			leftRays = new List<Vector2>(3);
			if (_proxy == null)
			{
				_proxy = base.Owner.AddComponent<PlayMakerUnity2DProxy>();
			}
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
					CheckTouching(8);
				}
				else
				{
					CheckTouching(1 << otherLayerNumber);
				}
			}
		}

		public override void DoCollisionEnter2D(Collision2D collision)
		{
			if (!otherLayer)
			{
				if (collision.gameObject.layer == 8)
				{
					CheckTouching(8);
				}
			}
			else
			{
				CheckTouching(otherLayerNumber);
			}
		}

		private void RecordTrigger()
		{
			lastUpdateCycle = CustomPlayerLoop.FixedUpdateCycle;
		}

		private void CheckTouching(int layerMask)
		{
			if (lastUpdateCycle == CustomPlayerLoop.FixedUpdateCycle)
			{
				return;
			}
			layerMask = 1 << layerMask;
			UnityEngine.Bounds bounds = col2d.bounds;
			Vector3 max = bounds.max;
			Vector3 min = bounds.min;
			Vector3 center = bounds.center;
			topHit.Value = false;
			rightHit.Value = false;
			bottomHit.Value = false;
			leftHit.Value = false;
			if (checkUp && (!hasBody || body.linearVelocity.y >= -0.001f))
			{
				topRays.Clear();
				topRays.Add(new Vector2(min.x, max.y));
				topRays.Add(new Vector2(center.x, max.y));
				topRays.Add(max);
				for (int i = 0; i < 3; i++)
				{
					Collider2D collider = Helper.Raycast2D(topRays[i], Vector2.up, 0.08f, layerMask).collider;
					if (!(collider == null) && (!ignoreTriggers.Value || !collider.isTrigger))
					{
						RecordTrigger();
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
				for (int j = 0; j < 3; j++)
				{
					Collider2D collider2 = Helper.Raycast2D(rightRays[j], Vector2.right, 0.08f, layerMask).collider;
					if (collider2 != null && (!ignoreTriggers.Value || !collider2.isTrigger))
					{
						RecordTrigger();
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
				for (int k = 0; k < 3; k++)
				{
					Collider2D collider3 = Helper.Raycast2D(bottomRays[k], -Vector2.up, 0.08f, layerMask).collider;
					if (collider3 != null && (!ignoreTriggers.Value || !collider3.isTrigger))
					{
						RecordTrigger();
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
			for (int l = 0; l < 3; l++)
			{
				Collider2D collider4 = Helper.Raycast2D(leftRays[l], -Vector2.right, 0.08f, layerMask).collider;
				if (collider4 != null && (!ignoreTriggers.Value || !collider4.isTrigger))
				{
					RecordTrigger();
					leftHit.Value = true;
					base.Fsm.Event(leftHitEvent);
					break;
				}
			}
		}
	}
}

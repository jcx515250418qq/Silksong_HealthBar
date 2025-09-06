using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Physics 2d")]
	[Tooltip("Same as V2, but now uses FixedUpdate for performance.")]
	public class RayCast2dV3 : FsmStateAction
	{
		[ActionSection("Setup")]
		public FsmOwnerDefault FromGameObject;

		public FsmVector2 FromPosition;

		public FsmVector2 Direction;

		public Space Space;

		public FsmFloat Distance;

		public FsmInt MinDepth;

		public FsmInt MaxDepth;

		[ActionSection("Result")]
		[UIHint(UIHint.Variable)]
		public FsmEvent HitEvent;

		[UIHint(UIHint.Variable)]
		public FsmEvent NoHitEvent;

		[UIHint(UIHint.Variable)]
		public FsmBool StoreDidHit;

		[UIHint(UIHint.Variable)]
		public FsmGameObject StoreHitObject;

		[UIHint(UIHint.Variable)]
		public FsmVector2 StoreHitPoint;

		[UIHint(UIHint.Variable)]
		public FsmVector2 StoreHitNormal;

		[UIHint(UIHint.Variable)]
		public FsmFloat StoreHitFraction;

		[UIHint(UIHint.Variable)]
		public FsmFloat StoreHitDistance;

		[ActionSection("Filter")]
		[Tooltip("Set how often to cast a ray. 0 = once, don't repeat; 1 = everyFrame; 2 = every other frame... \nSince raycasts can get expensive use the highest repeat interval you can get away with.")]
		public FsmInt RepeatInterval;

		[UIHint(UIHint.Layer)]
		public FsmInt[] LayerMask;

		public FsmBool InvertMask;

		public FsmBool IgnoreTriggers;

		[ActionSection("Debug")]
		[Tooltip("The color to use for the debug line.")]
		public FsmColor DebugColor;

		[Tooltip("Draw a debug line. Note: Check Gizmos in the Game View to see it in game.")]
		public FsmBool Debug;

		private Transform trans;

		private int repeat;

		public override void Reset()
		{
			FromGameObject = null;
			FromPosition = new FsmVector2
			{
				UseVariable = true
			};
			Direction = new FsmVector2
			{
				UseVariable = true
			};
			Space = Space.Self;
			MinDepth = new FsmInt
			{
				UseVariable = true
			};
			MaxDepth = new FsmInt
			{
				UseVariable = true
			};
			Distance = 100f;
			HitEvent = null;
			NoHitEvent = null;
			StoreDidHit = null;
			StoreHitObject = null;
			StoreHitPoint = null;
			StoreHitNormal = null;
			StoreHitFraction = null;
			StoreHitDistance = null;
			RepeatInterval = 1;
			LayerMask = Array.Empty<FsmInt>();
			InvertMask = false;
			IgnoreTriggers = false;
			DebugColor = Color.yellow;
			Debug = false;
		}

		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(FromGameObject);
			if (ownerDefaultTarget != null)
			{
				trans = ownerDefaultTarget.transform;
			}
			DoRaycast();
			if (RepeatInterval.Value == 0)
			{
				Finish();
			}
		}

		public override void OnFixedUpdate()
		{
			repeat--;
			if (repeat == 0)
			{
				DoRaycast();
			}
		}

		private void DoRaycast()
		{
			repeat = RepeatInterval.Value;
			if (Distance.Value == 0f)
			{
				return;
			}
			Vector2 value = FromPosition.Value;
			if (trans != null)
			{
				Vector3 position = trans.position;
				value.x += position.x;
				value.y += position.y;
			}
			float num = float.PositiveInfinity;
			if (Distance.Value > 0f)
			{
				num = Distance.Value;
			}
			Vector2 normalized = Direction.Value.normalized;
			if (trans != null && Space == Space.Self)
			{
				Vector3 vector = trans.TransformDirection(new Vector3(Direction.Value.x, Direction.Value.y, 0f));
				normalized.x = vector.x;
				normalized.y = vector.y;
			}
			RaycastHit2D closestHit;
			if (MinDepth.IsNone && MaxDepth.IsNone)
			{
				if (IgnoreTriggers.Value)
				{
					Helper.IsRayHittingNoTriggers(value, normalized, num, ActionHelpers.LayerArrayToLayerMask(LayerMask, InvertMask.Value), out closestHit);
				}
				else
				{
					closestHit = Helper.Raycast2D(value, normalized, num, ActionHelpers.LayerArrayToLayerMask(LayerMask, InvertMask.Value));
				}
			}
			else
			{
				float minDepth = (MinDepth.IsNone ? float.NegativeInfinity : ((float)MinDepth.Value));
				float maxDepth = (MaxDepth.IsNone ? float.PositiveInfinity : ((float)MaxDepth.Value));
				closestHit = Helper.Raycast2D(value, normalized, num, ActionHelpers.LayerArrayToLayerMask(LayerMask, InvertMask.Value), minDepth, maxDepth);
			}
			if (closestHit.collider != null && IgnoreTriggers.Value && closestHit.collider.isTrigger)
			{
				closestHit = default(RaycastHit2D);
			}
			bool flag = closestHit.collider != null;
			PlayMakerUnity2d.RecordLastRaycastHitInfo(base.Fsm, closestHit);
			StoreDidHit.Value = flag;
			if (flag)
			{
				StoreHitObject.Value = closestHit.collider.gameObject;
				StoreHitPoint.Value = closestHit.point;
				StoreHitNormal.Value = closestHit.normal;
				StoreHitFraction.Value = closestHit.fraction;
				StoreHitDistance.Value = closestHit.distance;
				base.Fsm.Event(HitEvent);
			}
			else
			{
				base.Fsm.Event(NoHitEvent);
			}
			if (Debug.Value)
			{
				float num2 = Mathf.Min(num, 1000f);
				Vector3 vector2 = new Vector3(value.x, value.y, 0f);
				Vector3 vector3 = new Vector3(normalized.x, normalized.y, 0f);
				Vector3 end = vector2 + vector3 * num2;
				UnityEngine.Debug.DrawLine(vector2, end, DebugColor.Value);
			}
		}
	}
}

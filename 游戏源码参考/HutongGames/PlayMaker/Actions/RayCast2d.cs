using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Casts a Ray against all Colliders in the scene. A raycast is conceptually like a laser beam that is fired from a point in space along a particular direction. Any object making contact with the beam can be detected and reported. Use GetRaycastHit2dInfo to get more detailed info.")]
	public class RayCast2d : FsmStateAction
	{
		[ActionSection("Setup")]
		[Tooltip("Start ray at game object position. \nOr use From Position parameter.")]
		public FsmOwnerDefault fromGameObject;

		[Tooltip("Start ray at a vector2 world position, or offset from the GameObject's position.")]
		public FsmVector2 fromPosition;

		[Tooltip("A vector2 direction vector")]
		public FsmVector2 direction;

		[Tooltip("Cast the ray in world or local space. Note if no Game Object is specified, the direction is in world space.")]
		public Space space;

		[Tooltip("The length of the ray. Set to -1 for infinity.")]
		public FsmFloat distance;

		[Tooltip("Only include objects with a Z coordinate (depth) greater than this value. Leave as None for no filtering.")]
		public FsmInt minDepth;

		[Tooltip("Only include objects with a Z coordinate (depth) less than this value. Leave as none for no filtering.")]
		public FsmInt maxDepth;

		[ActionSection("Result")]
		[Tooltip("Event to send if the ray hits an object.")]
		[UIHint(UIHint.Variable)]
		public FsmEvent hitEvent;

		[Tooltip("Set a bool variable to true if hit something, otherwise false.")]
		[UIHint(UIHint.Variable)]
		public FsmBool storeDidHit;

		[Tooltip("Store the game object hit in a variable.")]
		[UIHint(UIHint.Variable)]
		public FsmGameObject storeHitObject;

		[UIHint(UIHint.Variable)]
		[Tooltip("Get the 2d position of the ray hit point and store it in a variable.")]
		public FsmVector2 storeHitPoint;

		[UIHint(UIHint.Variable)]
		[Tooltip("Get the 2d normal at the hit point and store it in a variable. \nNote, this is a direction vector not a rotation.")]
		public FsmVector2 storeHitNormal;

		[UIHint(UIHint.Variable)]
		[Tooltip("Get the distance along the ray to the hit point and store it in a variable.")]
		public FsmFloat storeHitDistance;

		[UIHint(UIHint.Variable)]
		[Tooltip("Get the fraction along the ray to the hit point and store it in a variable. If the ray's direction vector is normalized then this value is simply the distance between the origin and the hit point. If the direction is not normalized then this distance is expressed as a 'fraction' (which could be greater than 1) of the vector's magnitude.")]
		public FsmFloat storeHitFraction;

		[ActionSection("Filter")]
		[Tooltip("Set how often to cast a ray. 0 = once, don't repeat; 1 = everyFrame; 2 = every other frame... \nBecause raycasts can get expensive use the highest repeat interval you can get away with.")]
		public FsmInt repeatInterval;

		[UIHint(UIHint.Layer)]
		[Tooltip("Pick only from these layers.")]
		public FsmInt[] layerMask;

		[Tooltip("Invert the mask, so you pick from all layers except those defined above.")]
		public FsmBool invertMask;

		[ActionSection("Debug")]
		[Tooltip("The color to use for the debug line.")]
		public FsmColor debugColor;

		[Tooltip("Draw a debug line. Note: Check Gizmos in the Game View to see it in game.")]
		public FsmBool debug;

		private Transform _transform;

		private int repeat;

		public override void Reset()
		{
			fromGameObject = null;
			fromPosition = new FsmVector2
			{
				UseVariable = true
			};
			direction = null;
			space = Space.Self;
			minDepth = new FsmInt
			{
				UseVariable = true
			};
			maxDepth = new FsmInt
			{
				UseVariable = true
			};
			distance = 100f;
			hitEvent = null;
			storeDidHit = null;
			storeHitObject = null;
			storeHitPoint = null;
			storeHitNormal = null;
			storeHitDistance = null;
			storeHitFraction = null;
			repeatInterval = 1;
			layerMask = new FsmInt[0];
			invertMask = false;
			debugColor = Color.yellow;
			debug = false;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(fromGameObject);
			if (ownerDefaultTarget != null)
			{
				_transform = ownerDefaultTarget.transform;
			}
			DoRaycast();
			if (repeatInterval.Value == 0)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			repeat--;
			if (repeat == 0)
			{
				DoRaycast();
			}
		}

		private void DoRaycast()
		{
			repeat = repeatInterval.Value;
			if (Math.Abs(distance.Value) < Mathf.Epsilon)
			{
				return;
			}
			Vector2 value = fromPosition.Value;
			if (_transform != null)
			{
				value.x += _transform.position.x;
				value.y += _transform.position.y;
			}
			float a = float.PositiveInfinity;
			if (distance.Value > 0f)
			{
				a = distance.Value;
			}
			Vector2 normalized = direction.Value.normalized;
			if (_transform != null && space == Space.Self)
			{
				Vector3 vector = _transform.TransformDirection(new Vector3(direction.Value.x, direction.Value.y, 0f));
				normalized.x = vector.x;
				normalized.y = vector.y;
			}
			RaycastHit2D info;
			if (minDepth.IsNone && maxDepth.IsNone)
			{
				info = Helper.Raycast2D(value, normalized, a, ActionHelpers.LayerArrayToLayerMask(layerMask, invertMask.Value));
			}
			else
			{
				float num = (minDepth.IsNone ? float.NegativeInfinity : ((float)minDepth.Value));
				float num2 = (maxDepth.IsNone ? float.PositiveInfinity : ((float)maxDepth.Value));
				info = Helper.Raycast2D(value, normalized, a, ActionHelpers.LayerArrayToLayerMask(layerMask, invertMask.Value), num, num2);
			}
			Fsm.RecordLastRaycastHit2DInfo(base.Fsm, info);
			bool flag = info.collider != null;
			storeDidHit.Value = flag;
			if (flag)
			{
				storeHitObject.Value = info.collider.gameObject;
				storeHitPoint.Value = info.point;
				storeHitNormal.Value = info.normal;
				storeHitDistance.Value = info.distance;
				storeHitFraction.Value = info.fraction;
				base.Fsm.Event(hitEvent);
			}
			if (debug.Value)
			{
				Vector3 vector2 = new Vector3(value.x, value.y, 0f);
				if (flag)
				{
					Debug.DrawLine(vector2, storeHitPoint.Value, debugColor.Value);
					return;
				}
				float num3 = Mathf.Min(a, 1000f);
				Vector3 vector3 = new Vector3(normalized.x, normalized.y, 0f);
				Vector3 end = vector2 + vector3 * num3;
				Debug.DrawLine(vector2, end, debugColor.Value);
			}
		}
	}
}

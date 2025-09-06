using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Iterate through a list of all colliders detected by a RayCastThe colliders iterated are sorted in order of increasing Z coordinate. No iteration will take place if there are no colliders within the area.")]
	public class GetNextRayCast2d : FsmStateAction
	{
		[ActionSection("Setup")]
		[Tooltip("Start ray at game object position. \nOr use From Position parameter.")]
		public FsmOwnerDefault fromGameObject;

		[Tooltip("Start ray at a vector2 world position. \nOr use Game Object parameter.")]
		public FsmVector2 fromPosition;

		[Tooltip("A vector2 direction vector")]
		public FsmVector2 direction;

		[Tooltip("Cast the ray in world or local space. Note if no Game Object is specified, the direction is in world space.")]
		public Space space;

		[Tooltip("The length of the ray. Set to -1 for infinity.")]
		public FsmFloat distance;

		[Tooltip("Only include objects with a Z coordinate (depth) greater than this value. leave to none for no effect")]
		public FsmInt minDepth;

		[Tooltip("Only include objects with a Z coordinate (depth) less than this value. leave to none")]
		public FsmInt maxDepth;

		[Tooltip("If you want to reset the iteration, raise this flag to true when you enter the state, it will indicate you want to start from the beginning again")]
		[UIHint(UIHint.Variable)]
		public FsmBool resetFlag;

		[ActionSection("Filter")]
		[UIHint(UIHint.Layer)]
		[Tooltip("Pick only from these layers.")]
		public FsmInt[] layerMask;

		[Tooltip("Invert the mask, so you pick from all layers except those defined above.")]
		public FsmBool invertMask;

		[ActionSection("Result")]
		[Tooltip("Store the number of colliders found for this overlap.")]
		[UIHint(UIHint.Variable)]
		public FsmInt collidersCount;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the next collider in a GameObject variable.")]
		public FsmGameObject storeNextCollider;

		[UIHint(UIHint.Variable)]
		[Tooltip("Get the 2d position of the next ray hit point and store it in a variable.")]
		public FsmVector2 storeNextHitPoint;

		[UIHint(UIHint.Variable)]
		[Tooltip("Get the 2d normal at the next hit point and store it in a variable.\nNote, this is a direction vector not a rotation.")]
		public FsmVector2 storeNextHitNormal;

		[UIHint(UIHint.Variable)]
		[Tooltip("Get the distance along the ray to the next hit point and store it in a variable.")]
		public FsmFloat storeNextHitDistance;

		[UIHint(UIHint.Variable)]
		[Tooltip("Get the fraction along the ray to the hit point and store it in a variable. If the ray's direction vector is normalized then this value is simply the distance between the origin and the hit point. If the direction is not normalized then this distance is expressed as a 'fraction' (which could be greater than 1) of the vector's magnitude.")]
		public FsmFloat storeNextHitFraction;

		[Tooltip("Event to send to get the next collider.")]
		public FsmEvent loopEvent;

		[Tooltip("Event to send when there are no more colliders to iterate.")]
		public FsmEvent finishedEvent;

		private RaycastHit2D[] hits;

		private int colliderCount;

		private int nextColliderIndex;

		public override void Reset()
		{
			fromGameObject = null;
			fromPosition = new FsmVector2
			{
				UseVariable = true
			};
			direction = new FsmVector2
			{
				UseVariable = true
			};
			space = Space.Self;
			minDepth = new FsmInt
			{
				UseVariable = true
			};
			maxDepth = new FsmInt
			{
				UseVariable = true
			};
			layerMask = new FsmInt[0];
			invertMask = false;
			resetFlag = null;
			collidersCount = null;
			storeNextCollider = null;
			storeNextHitPoint = null;
			storeNextHitNormal = null;
			storeNextHitDistance = null;
			storeNextHitFraction = null;
			loopEvent = null;
			finishedEvent = null;
		}

		public override void OnEnter()
		{
			if (hits == null || resetFlag.Value)
			{
				nextColliderIndex = 0;
				hits = GetRayCastAll();
				colliderCount = hits.Length;
				collidersCount.Value = colliderCount;
				resetFlag.Value = false;
			}
			DoGetNextCollider();
			Finish();
		}

		private void DoGetNextCollider()
		{
			if (nextColliderIndex >= colliderCount)
			{
				hits = null;
				nextColliderIndex = 0;
				base.Fsm.Event(finishedEvent);
				return;
			}
			Fsm.RecordLastRaycastHit2DInfo(base.Fsm, hits[nextColliderIndex]);
			storeNextCollider.Value = hits[nextColliderIndex].collider.gameObject;
			storeNextHitPoint.Value = hits[nextColliderIndex].point;
			storeNextHitNormal.Value = hits[nextColliderIndex].normal;
			storeNextHitDistance.Value = hits[nextColliderIndex].distance;
			storeNextHitFraction.Value = hits[nextColliderIndex].fraction;
			if (nextColliderIndex >= colliderCount)
			{
				hits = new RaycastHit2D[0];
				nextColliderIndex = 0;
				base.Fsm.Event(finishedEvent);
				return;
			}
			nextColliderIndex++;
			if (loopEvent != null)
			{
				base.Fsm.Event(loopEvent);
			}
		}

		private RaycastHit2D[] GetRayCastAll()
		{
			if (Math.Abs(distance.Value) < Mathf.Epsilon)
			{
				return new RaycastHit2D[0];
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(fromGameObject);
			Vector2 value = fromPosition.Value;
			if (ownerDefaultTarget != null)
			{
				value.x += ownerDefaultTarget.transform.position.x;
				value.y += ownerDefaultTarget.transform.position.y;
			}
			float num = float.PositiveInfinity;
			if (distance.Value > 0f)
			{
				num = distance.Value;
			}
			Vector2 normalized = direction.Value.normalized;
			if (ownerDefaultTarget != null && space == Space.Self)
			{
				Vector3 vector = ownerDefaultTarget.transform.TransformDirection(new Vector3(direction.Value.x, direction.Value.y, 0f));
				normalized.x = vector.x;
				normalized.y = vector.y;
			}
			if (minDepth.IsNone && maxDepth.IsNone)
			{
				return Physics2D.RaycastAll(value, normalized, num, ActionHelpers.LayerArrayToLayerMask(layerMask, invertMask.Value));
			}
			float num2 = (minDepth.IsNone ? float.NegativeInfinity : ((float)minDepth.Value));
			float num3 = (maxDepth.IsNone ? float.PositiveInfinity : ((float)maxDepth.Value));
			return Physics2D.RaycastAll(value, normalized, num, ActionHelpers.LayerArrayToLayerMask(layerMask, invertMask.Value), num2, num3);
		}
	}
}

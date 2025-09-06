using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Physics 2d")]
	public class RayCast2dNoTriggers : FsmStateAction
	{
		public FsmOwnerDefault FromGameObject;

		public FsmVector2 FromPosition;

		public FsmVector2 Direction;

		public FsmFloat Distance;

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
		public FsmFloat StoreHitDistance;

		[UIHint(UIHint.Variable)]
		public FsmFloat StoreDistance;

		public FsmInt RepeatInterval;

		[UIHint(UIHint.Layer)]
		public FsmInt[] LayerMask;

		public FsmBool InvertMask;

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
			Distance = 100f;
			HitEvent = null;
			NoHitEvent = null;
			StoreDidHit = null;
			StoreHitObject = null;
			StoreHitPoint = null;
			StoreHitNormal = null;
			StoreHitDistance = null;
			StoreDistance = null;
			RepeatInterval = 1;
			LayerMask = new FsmInt[0];
			InvertMask = false;
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
			repeat = RepeatInterval.Value;
			if (!(Math.Abs(Distance.Value) <= Mathf.Epsilon))
			{
				Vector2 value = FromPosition.Value;
				if (trans != null)
				{
					Vector3 position = trans.position;
					value.x += position.x;
					value.y += position.y;
				}
				float length = float.PositiveInfinity;
				if (Distance.Value > 0f)
				{
					length = Distance.Value;
				}
				Vector2 normalized = Direction.Value.normalized;
				RaycastHit2D closestHit;
				bool flag = Helper.IsRayHittingNoTriggers(value, normalized, length, ActionHelpers.LayerArrayToLayerMask(LayerMask, InvertMask.Value), out closestHit);
				PlayMakerUnity2d.RecordLastRaycastHitInfo(base.Fsm, closestHit);
				StoreDidHit.Value = flag;
				if (flag)
				{
					StoreHitObject.Value = closestHit.collider.gameObject;
					StoreHitPoint.Value = closestHit.point;
					StoreHitNormal.Value = closestHit.normal;
					StoreHitDistance.Value = closestHit.fraction;
					StoreDistance.Value = closestHit.distance;
					base.Fsm.Event(HitEvent);
				}
				else
				{
					base.Fsm.Event(NoHitEvent);
				}
			}
		}
	}
}

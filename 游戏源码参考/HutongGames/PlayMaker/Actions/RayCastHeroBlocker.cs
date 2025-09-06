using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class RayCastHeroBlocker : FsmStateAction
	{
		private const int MAX_RAY_HITS = 10;

		[ActionSection("Setup")]
		public FsmOwnerDefault FromGameObject;

		public FsmVector2 FromPosition;

		public FsmVector2 Direction;

		public Space Space;

		public FsmFloat Distance;

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
		public FsmFloat StoreHitDistance;

		[UIHint(UIHint.Variable)]
		public FsmFloat StoreDistance;

		[ActionSection("Filter")]
		public FsmInt RepeatInterval;

		[ActionSection("Debug")]
		public FsmColor DebugColor;

		public FsmBool Debug;

		private Transform trans;

		private int repeat;

		private readonly RaycastHit2D[] storeHits = new RaycastHit2D[10];

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
			Distance = 100f;
			HitEvent = null;
			NoHitEvent = null;
			StoreDidHit = null;
			StoreHitObject = null;
			StoreHitPoint = null;
			StoreHitNormal = null;
			StoreHitDistance = null;
			RepeatInterval = 1;
			DebugColor = Color.yellow;
			Debug = false;
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
			int num2 = Physics2D.RaycastNonAlloc(value, normalized, storeHits, num, 8448);
			if (num2 > 10)
			{
				UnityEngine.Debug.LogWarning("Raycast hit count exceeded allocated buffer", base.Owner);
				num2 = 10;
			}
			RaycastHit2D raycastHit2D = default(RaycastHit2D);
			bool flag = false;
			for (int i = 0; i < num2; i++)
			{
				RaycastHit2D raycastHit2D2 = storeHits[i];
				GameObject gameObject = raycastHit2D2.collider.gameObject;
				if ((gameObject.layer != 13 || (bool)raycastHit2D2.collider.GetComponent<SlideSurface>()) && !gameObject.CompareTag("Piercable Terrain"))
				{
					raycastHit2D = raycastHit2D2;
					flag = true;
					break;
				}
			}
			StoreDidHit.Value = flag;
			if (flag)
			{
				StoreHitObject.Value = raycastHit2D.collider.gameObject;
				StoreHitPoint.Value = raycastHit2D.point;
				StoreHitNormal.Value = raycastHit2D.normal;
				StoreHitDistance.Value = raycastHit2D.fraction;
				StoreDistance.Value = raycastHit2D.distance;
				base.Fsm.Event(HitEvent);
			}
			else
			{
				base.Fsm.Event(NoHitEvent);
			}
			if (Debug.Value)
			{
				float num3 = Mathf.Min(num, 1000f);
				Vector3 vector2 = new Vector3(value.x, value.y, 0f);
				Vector3 vector3 = new Vector3(normalized.x, normalized.y, 0f);
				Vector3 end = vector2 + vector3 * num3;
				UnityEngine.Debug.DrawLine(vector2, end, DebugColor.Value);
			}
		}
	}
}

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class PositionColliderOnGroundV2 : FsmStateAction
	{
		[CheckForComponent(typeof(Collider2D))]
		public FsmOwnerDefault Target;

		public FsmVector2 GroundPos;

		public override void Reset()
		{
			Target = null;
			GroundPos = new FsmVector2
			{
				UseVariable = true
			};
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe)
			{
				Collider2D component = safe.GetComponent<Collider2D>();
				if ((bool)component)
				{
					UnityEngine.Bounds bounds = component.bounds;
					Vector2 vector = bounds.center;
					Vector2 vector2 = new Vector2(vector.x, bounds.min.y);
					if (Helper.IsRayHittingNoTriggers((!GroundPos.IsNone) ? GroundPos.Value : vector, Vector2.down, 10f, 256, out var closestHit))
					{
						float num = safe.transform.position.y - vector2.y;
						Vector2 point = closestHit.point;
						point.y += num;
						safe.transform.SetPosition2D(point);
					}
				}
			}
			Finish();
		}
	}
}

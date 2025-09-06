using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class PositionColliderOnGround : FsmStateAction
	{
		[CheckForComponent(typeof(Collider2D))]
		public FsmOwnerDefault Target;

		public override void Reset()
		{
			Target = null;
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
					Vector3 center = bounds.center;
					Vector2 vector = new Vector2(center.x, bounds.min.y);
					if (Helper.IsRayHittingNoTriggers(center, Vector2.down, 10f, 256, out var closestHit))
					{
						float num = safe.transform.position.y - vector.y;
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

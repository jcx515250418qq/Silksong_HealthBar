using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class DetectGroundEdge : FsmStateAction
	{
		private const float RAY_HEIGHT = 0.5f;

		[CheckForComponent(typeof(Collider2D), typeof(Rigidbody2D))]
		public FsmOwnerDefault Body;

		public FsmFloat EdgeDistance;

		public FsmFloat GroundDistance;

		public bool EveryFixedUpdate;

		public FsmEvent FoundWall;

		public FsmEvent FoundEdge;

		private Collider2D collider;

		private Rigidbody2D body;

		public override void Reset()
		{
			Body = null;
			EdgeDistance = 0.5f;
			GroundDistance = 1f;
			EveryFixedUpdate = true;
		}

		public override void OnEnter()
		{
			GameObject safe = Body.GetSafe(this);
			if ((bool)safe)
			{
				collider = safe.GetComponent<Collider2D>();
				body = safe.GetComponent<Rigidbody2D>();
			}
			if (!collider || !body)
			{
				Finish();
				return;
			}
			DoAction();
			if (!EveryFixedUpdate)
			{
				Finish();
			}
		}

		public override void OnFixedUpdate()
		{
			DoAction();
		}

		private void DoAction()
		{
			bool num = body.linearVelocity.x > 0f;
			UnityEngine.Bounds bounds = collider.bounds;
			Vector2 vector = bounds.min;
			Vector2 vector2 = bounds.center;
			Vector2 vector3 = bounds.extents;
			int layerMask = 256;
			Vector2 vector4 = new Vector2(vector2.x, vector.y + 0.5f);
			float num2 = vector3.x + EdgeDistance.Value;
			Vector2 vector5 = (num ? Vector2.right : Vector2.left);
			Vector2 vector6 = vector4 + vector5 * num2;
			Debug.DrawLine(vector4, vector6);
			if (Helper.Raycast2D(vector4, vector5, num2, layerMask).collider != null)
			{
				base.Fsm.Event(FoundWall);
			}
			float num3 = 0.5f + GroundDistance.Value;
			Debug.DrawLine(vector6, vector6 + Vector2.down * num3);
			if (Helper.Raycast2D(vector6, Vector2.down, num3, layerMask).collider == null)
			{
				base.Fsm.Event(FoundEdge);
			}
		}
	}
}

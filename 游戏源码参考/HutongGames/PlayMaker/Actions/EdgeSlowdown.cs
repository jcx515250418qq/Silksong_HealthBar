using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics2D)]
	public class EdgeSlowdown : ComponentAction<Rigidbody2D>
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[Tooltip("The GameObject with the Rigidbody2D attached")]
		public FsmOwnerDefault gameObject;

		public FsmFloat rayRadius;

		public FsmFloat rayDepth;

		public FsmFloat deceleration;

		private static readonly int TERRAIN_MASK = 256;

		public override void Reset()
		{
			gameObject = null;
			rayRadius = 2f;
			rayDepth = 2f;
			deceleration = 0.9f;
		}

		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		public override void OnFixedUpdate()
		{
			DoEdgeSlowdown();
		}

		private void DoEdgeSlowdown()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			_ = ownerDefaultTarget.transform;
			RaycastHit2D raycastHit2D = Helper.Raycast2D(new Vector2(ownerDefaultTarget.transform.position.x - rayRadius.Value, ownerDefaultTarget.transform.position.y), Vector2.down, rayDepth.Value, TERRAIN_MASK);
			RaycastHit2D raycastHit2D2 = Helper.Raycast2D(new Vector2(ownerDefaultTarget.transform.position.x + rayRadius.Value, ownerDefaultTarget.transform.position.y), Vector2.down, rayDepth.Value, TERRAIN_MASK);
			bool flag = raycastHit2D.collider;
			bool flag2 = raycastHit2D2.collider;
			if (!flag && flag2)
			{
				Rigidbody2D component = ownerDefaultTarget.GetComponent<Rigidbody2D>();
				Vector2 linearVelocity = component.linearVelocity;
				if (linearVelocity.x < 0f)
				{
					linearVelocity.x *= deceleration.Value;
					component.linearVelocity = linearVelocity;
				}
			}
			else if (flag && !flag2)
			{
				Rigidbody2D component2 = ownerDefaultTarget.GetComponent<Rigidbody2D>();
				Vector2 linearVelocity2 = component2.linearVelocity;
				if (linearVelocity2.x > 0f)
				{
					linearVelocity2.x *= deceleration.Value;
					component2.linearVelocity = linearVelocity2;
				}
			}
		}
	}
}

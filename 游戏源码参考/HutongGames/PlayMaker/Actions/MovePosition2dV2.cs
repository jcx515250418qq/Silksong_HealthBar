using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Moves a Game Object's RigidBody2D to a new position. Unlike SetPosition this will respect physics collisions.")]
	public class MovePosition2dV2 : ComponentAction<Rigidbody2D>
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[Tooltip("The GameObject to move.")]
		public FsmOwnerDefault gameObject;

		public FsmGameObject targetPosition;

		[UIHint(UIHint.Variable)]
		[Tooltip("Movement vector.")]
		public FsmVector2 vector;

		[Tooltip("Movement in x axis.")]
		public FsmFloat x;

		[Tooltip("Movement in y axis.")]
		public FsmFloat y;

		[Tooltip("Coordinate space to move in.")]
		public Space space;

		[Tooltip("Keep running every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			gameObject = null;
			targetPosition = new FsmGameObject
			{
				UseVariable = true
			};
			vector = null;
			x = new FsmFloat
			{
				UseVariable = true
			};
			y = new FsmFloat
			{
				UseVariable = true
			};
			space = Space.World;
			everyFrame = false;
		}

		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		public override void OnFixedUpdate()
		{
			if (!DoMovePosition() || !everyFrame)
			{
				Finish();
			}
		}

		private bool DoMovePosition()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!UpdateCache(ownerDefaultTarget))
			{
				return false;
			}
			Vector3 vector = ((!targetPosition.IsNone && targetPosition.Value != null) ? ((space != 0) ? targetPosition.Value.transform.localPosition : targetPosition.Value.transform.position) : ((!this.vector.IsNone) ? ((Vector3)this.vector.Value) : ((space == Space.World) ? new Vector3(base.rigidbody2d.position.x, base.rigidbody2d.position.y, 0f) : base.cachedTransform.TransformPoint(base.rigidbody.position))));
			if (!x.IsNone)
			{
				vector.x = x.Value;
			}
			if (!y.IsNone)
			{
				vector.y = y.Value;
			}
			base.rigidbody2d.MovePosition((space == Space.World) ? vector : ownerDefaultTarget.transform.InverseTransformPoint(vector));
			return true;
		}
	}
}

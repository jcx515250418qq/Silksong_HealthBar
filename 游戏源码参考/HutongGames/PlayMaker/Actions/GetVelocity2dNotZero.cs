using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Gets the 2d Velocity of a Game Object and stores it in a Vector2 Variable or each Axis in a Float Variable. Ignores very low speeds.")]
	public class GetVelocity2dNotZero : ComponentAction<Rigidbody2D>
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[Tooltip("The GameObject with the Rigidbody2D attached")]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.Variable)]
		[Tooltip("The velocity")]
		public FsmVector2 vector;

		[UIHint(UIHint.Variable)]
		[Tooltip("The x value of the velocity")]
		public FsmFloat x;

		[UIHint(UIHint.Variable)]
		[Tooltip("The y value of the velocity")]
		public FsmFloat y;

		[Tooltip("The space reference to express the velocity")]
		public Space space;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			gameObject = null;
			vector = null;
			x = null;
			y = null;
			space = Space.World;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoGetVelocity();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoGetVelocity();
		}

		private void DoGetVelocity()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCache(ownerDefaultTarget))
			{
				Vector2 vector = base.rigidbody2d.linearVelocity;
				if (space == Space.Self)
				{
					vector = base.rigidbody2d.transform.InverseTransformDirection(vector);
				}
				if (vector.x > 0.1f || (vector.x < -0.1f && vector.y > 0.1f) || vector.y < -0.1f)
				{
					this.vector.Value = vector;
				}
				if (vector.x > 0.1f || vector.x < -0.1f)
				{
					x.Value = vector.x;
				}
				if (vector.y > 0.1f || vector.y < -0.1f)
				{
					y.Value = vector.y;
				}
			}
		}
	}
}

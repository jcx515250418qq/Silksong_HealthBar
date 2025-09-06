using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics)]
	[Tooltip("Moves a Game Object's Rigid Body to a new position. Unlike Set Position this will respect physics collisions.")]
	public class MovePosition : ComponentAction<Rigidbody>
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody))]
		[Tooltip("The GameObject to move.")]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.Variable)]
		[Tooltip("Movement vector.")]
		public FsmVector3 vector;

		[Tooltip("Movement in x axis.")]
		public FsmFloat x;

		[Tooltip("Movement in y axis.")]
		public FsmFloat y;

		[Tooltip("Movement in z axis.")]
		public FsmFloat z;

		[Tooltip("Coordinate space to move in.")]
		public Space space;

		[Tooltip("Keep running every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			gameObject = null;
			vector = null;
			x = new FsmFloat
			{
				UseVariable = true
			};
			y = new FsmFloat
			{
				UseVariable = true
			};
			z = new FsmFloat
			{
				UseVariable = true
			};
			space = Space.Self;
			everyFrame = false;
		}

		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		public override void OnFixedUpdate()
		{
			DoMovePosition();
			if (!everyFrame)
			{
				Finish();
			}
		}

		private void DoMovePosition()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCache(ownerDefaultTarget))
			{
				Vector3 vector = ((!this.vector.IsNone) ? this.vector.Value : ((space == Space.World) ? base.rigidbody.position : ownerDefaultTarget.transform.TransformPoint(base.rigidbody.position)));
				if (!x.IsNone)
				{
					vector.x = x.Value;
				}
				if (!y.IsNone)
				{
					vector.y = y.Value;
				}
				if (!z.IsNone)
				{
					vector.z = z.Value;
				}
				base.rigidbody.MovePosition((space == Space.World) ? vector : ownerDefaultTarget.transform.InverseTransformPoint(vector));
			}
		}
	}
}

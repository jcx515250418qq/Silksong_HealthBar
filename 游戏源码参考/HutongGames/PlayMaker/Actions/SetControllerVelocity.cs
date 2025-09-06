using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Character)]
	[Tooltip("Sets the velocity of a CharacterController on a GameObject. To leave any axis unchanged, set variable to 'None'.")]
	public class SetControllerVelocity : ComponentAction<CharacterController>
	{
		[RequiredField]
		[CheckForComponent(typeof(CharacterController))]
		[Tooltip("The GameObject with the Character Controller component.")]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.Variable)]
		[Tooltip("Set velocity using Vector3 variable and/or individual channels below.")]
		public FsmVector3 vector;

		[Tooltip("Velocity in X axis.")]
		public FsmFloat x;

		[Tooltip("Velocity in Y axis.")]
		public FsmFloat y;

		[Tooltip("Velocity in Z axis.")]
		public FsmFloat z;

		[Tooltip("You can set velocity in world or local space.")]
		public Space space;

		[Tooltip("Set the velocity every frame.")]
		public bool everyFrame;

		private CharacterController controller => cachedComponent;

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

		public override void OnEnter()
		{
			DoSetVelocity();
			if (!everyFrame)
			{
				Finish();
			}
		}

		private void DoSetVelocity()
		{
			if (UpdateCacheAndTransform(base.Fsm.GetOwnerDefaultTarget(gameObject)))
			{
				Vector3 vector = ((!this.vector.IsNone) ? this.vector.Value : ((space == Space.World) ? controller.velocity : base.cachedTransform.InverseTransformDirection(controller.velocity)));
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
				if (space == Space.Self)
				{
					vector = base.cachedTransform.TransformDirection(vector);
				}
				controller.Move(vector * Time.deltaTime);
			}
		}
	}
}

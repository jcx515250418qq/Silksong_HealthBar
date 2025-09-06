using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Character)]
	[Tooltip("Moves a Game Object with a Character Controller. Velocity along the y-axis is ignored. Speed is in meters/s. Gravity is automatically applied.")]
	public class ControllerSimpleMove : ComponentAction<CharacterController>
	{
		[RequiredField]
		[CheckForComponent(typeof(CharacterController))]
		[Tooltip("A Game Object with a Character Controller.")]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[Tooltip("The movement vector.")]
		public FsmVector3 moveVector;

		[Tooltip("Multiply the Move Vector by a speed factor.")]
		public FsmFloat speed;

		[Tooltip("Move in local or world space.")]
		public Space space;

		[Tooltip("Event sent if the Character Controller starts falling.")]
		public FsmEvent fallingEvent;

		private CharacterController controller => cachedComponent;

		public override void Reset()
		{
			gameObject = null;
			moveVector = new FsmVector3
			{
				UseVariable = true
			};
			speed = new FsmFloat
			{
				Value = 1f
			};
			space = Space.World;
			fallingEvent = null;
		}

		public override void OnUpdate()
		{
			if (UpdateCacheAndTransform(base.Fsm.GetOwnerDefaultTarget(gameObject)))
			{
				Vector3 vector = ((space == Space.World) ? moveVector.Value : base.cachedTransform.TransformDirection(moveVector.Value));
				controller.SimpleMove(vector * speed.Value);
				if (!controller.isGrounded && !Physics.Raycast(base.cachedTransform.position, Vector3.down, controller.stepOffset))
				{
					base.Fsm.Event(fallingEvent);
				}
			}
		}
	}
}

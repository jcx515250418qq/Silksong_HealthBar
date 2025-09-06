using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Character)]
	[Tooltip("Handles CharacterController while in the air, e.g., after jumping.")]
	public class ControllerMoveInAir : ComponentAction<CharacterController>
	{
		[RequiredField]
		[CheckForComponent(typeof(CharacterController))]
		[Tooltip("The GameObject that owns the CharacterController component.")]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.Variable)]
		[Tooltip("Movement vector applied while in the air. Usually to allow the player to influence the jump.")]
		public FsmVector3 moveVector;

		[Tooltip("Clamp horizontal speed while jumping. Set to None for no clamping.")]
		public FsmFloat maxMoveSpeed;

		[Tooltip("Multiply the gravity set in the Physics system.")]
		public FsmFloat gravityMultiplier;

		[Tooltip("Extra gravity multiplier when falling. Note: This is on top of the gravity multiplier above. This can be used to make jumps less 'floaty.'")]
		public FsmFloat fallMultiplier;

		[Tooltip("Move in local or word space.")]
		public Space space;

		[Tooltip("Event to send when landed.")]
		public FsmEvent landedEvent;

		private CharacterController controller => cachedComponent;

		public override void Reset()
		{
			gameObject = null;
			moveVector = null;
			maxMoveSpeed = null;
			gravityMultiplier = new FsmFloat
			{
				Value = 1f
			};
			fallMultiplier = new FsmFloat
			{
				Value = 1f
			};
			space = Space.World;
			landedEvent = null;
		}

		public override void OnUpdate()
		{
			if (UpdateCacheAndTransform(base.Fsm.GetOwnerDefaultTarget(gameObject)))
			{
				Vector3 vector = controller.velocity;
				if (!moveVector.IsNone)
				{
					vector += moveVector.Value;
				}
				float num = Physics.gravity.y * gravityMultiplier.Value * ((vector.y < 0f) ? fallMultiplier.Value : 1f);
				vector.y += num * Time.deltaTime;
				if (!maxMoveSpeed.IsNone)
				{
					Vector2 vector2 = Vector2.ClampMagnitude(new Vector2(vector.x, vector.z), maxMoveSpeed.Value);
					vector.Set(vector2.x, vector.y, vector2.y);
				}
				if (space == Space.Self)
				{
					vector = base.cachedTransform.TransformDirection(vector);
				}
				controller.Move(vector * Time.deltaTime);
				if (controller.isGrounded && controller.velocity.y < 0.1f)
				{
					controller.Move(Vector3.zero);
					Fsm.EventData.FloatData = vector.magnitude;
					Fsm.EventData.Vector2Data = vector;
					base.Fsm.Event(landedEvent);
				}
			}
		}
	}
}
